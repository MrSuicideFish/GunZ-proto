using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/// <summary>
/// The player controller should be responsible for firing
/// initial animation commands to the animation controller (script).
/// </summary>
[RequireComponent(typeof(Animator))]
public class PlayerController : NetworkBehaviour
{
    private const float MAX_LOOK_Y = 80;
    private const float MIN_LOOK_Y = -70;
    private const float MAX_GROUNDED_TIME = 0.10f;

    #region Internal Components
    private StateMachine<CombatState> _combatSM;
    private CombatCamera combatCamera;
    public Transform shoulderFollowTarget;

    public Rigidbody rigidBody { get; private set; }
    public CapsuleCollider collider { get; private set; }
    public Animator animator { get; private set; }
    #endregion

    #region Properties / Status
    public Vector3 InputLookDelta { get; private set; }
    public Vector3 InputMoveDelta { get; private set; }
    public Vector3 WorldInputMoveDelta { get; private set; }

    public bool isJumping { get; private set; }
    public bool isGrounded { get; private set; }
    public bool isLightAttacking { get; private set; }
    public bool isHeavyAttacking { get; private set; }
    public bool isBlocking { get; private set; }
    public bool hasDoubleJumped { get; private set; }
    public float groundedTime { get; private set; }
    public bool groundedTimerExpired
    {
        get
        {
            return groundedTime >= MAX_GROUNDED_TIME;
        }
    }

    private float _playerHeight = -1.0f;
    public float playerHeight {
        get
        {
            if(_playerHeight <= 0.0)
            {
                _playerHeight = collider.height;
            }
            return _playerHeight;
        } 
    }

    public float walkSpeed = 10;
    public float airSpeed = 10;
    public float jumpStrength = 100;

    [SyncVar] public HitInfo lastHitInfo;
    #endregion

    private Vector3 _bodyRotation;
    private Vector3 _lookRotation;

    // states
    private Dictionary<CombatState.ECombatStateType, CombatState> _combatStateDict;

    private void Awake()
    {
        _combatSM = new StateMachine<CombatState>();
        animator = this.GetComponent<Animator>();
        rigidBody = this.GetComponent<Rigidbody>();
        collider = this.GetComponent<CapsuleCollider>();

        // create states
        _combatStateDict = new Dictionary<CombatState.ECombatStateType, CombatState>();

        var idle_state = new CState_Grounded_Idle(this);
        var walk_state = new CState_Grounded_Walking(this);
        var jump_state = new CState_Jump(this);
        var fall_state = new CState_Falling(this);
        var doubleJump_state = new CState_DoubleJump(this);
        var landing_state = new CState_Landing(this);

        _combatStateDict.Add(idle_state.CombatStateType, idle_state);
        _combatStateDict.Add(walk_state.CombatStateType, walk_state);
        _combatStateDict.Add(jump_state.CombatStateType, jump_state);
        _combatStateDict.Add(fall_state.CombatStateType, fall_state);
        _combatStateDict.Add(doubleJump_state.CombatStateType, doubleJump_state);
        _combatStateDict.Add(landing_state.CombatStateType, landing_state);
    }

    public override void OnStartClient()
    {
        if (combatCamera != null)
        {
            GameObject.Destroy(combatCamera.gameObject);
        }
        combatCamera = CameraService.CreateCombatCamera() as CombatCamera;
        combatCamera.SetFollowTarget(shoulderFollowTarget);
        combatCamera.Toggle(this.isLocalPlayer);
        TryGoToState(CombatState.ECombatStateType.Idle);

        if (this.isLocalPlayer)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        base.OnStartClient();
    }

    public void FlagDoubleJump()
    {
        hasDoubleJumped = true;
    }

    public void ResetDoubleJump()
    {
        hasDoubleJumped = false;
    }

    [Command]
    public void Hit(HitInfo hitInfo)
    {
        lastHitInfo = hitInfo;
    }

    public void TryGoToState(CombatState.ECombatStateType state)
    {
        if (isLocalPlayer)
        {
            GoToState(state);
        }
    }

    [Command]
    private void GoToState(CombatState.ECombatStateType state)
    {
        SetCombatState(state);
    }

    [ClientRpc]
    private void SetCombatState(CombatState.ECombatStateType stateType)
    {
        if (_combatStateDict.ContainsKey(stateType))
        {
            _combatSM.GoToState(_combatStateDict[stateType]);
        }
    }

    private void Update()
    {
        if (this.isLocalPlayer)
        {
            ProcessInput();

            isGrounded = IsGrounded();
            if (isGrounded)
            {
                ResetDoubleJump();
                groundedTime = Mathf.Clamp(groundedTime + Time.deltaTime, 0, MAX_GROUNDED_TIME);
            }
            else
            {
                groundedTime = 0.0f;
            }

            WorldInputMoveDelta = transform.TransformDirection(InputMoveDelta).normalized;
        }

        _combatSM.StateMachineUpdate(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (this.isLocalPlayer)
        {
            Rotate(InputLookDelta.y);
            Look(InputLookDelta.x);
        }

        _combatSM.StateMachineFixedUpdate(Time.deltaTime);
    }

    private void LateUpdate()
    {
        _combatSM.StateMachineLateUpdate(Time.deltaTime);
    }

    private void OnGUI()
    {
        if (this.isLocalPlayer)
        {
            if(_combatSM.CurrentState != null)
            {
                GUI.Label(new Rect(0, 0, 200, 200), _combatSM.CurrentState.CombatStateType.ToString());
            }
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 feetPoint = collider.bounds.center - new Vector3(0, collider.bounds.extents.y, 0);
        Gizmos.DrawWireSphere(feetPoint, 0.3f);
    }

    private void Look(float amount)
    {
        _lookRotation.x = shoulderFollowTarget.localEulerAngles.x + amount;
        _lookRotation.y = 0;
        _lookRotation.z = 0;

        //if (_lookRotation.x < MIN_LOOK_Y) _lookRotation.x = MIN_LOOK_Y;
        //if (_lookRotation.x > MAX_LOOK_Y) _lookRotation.x = MAX_LOOK_Y;

        shoulderFollowTarget.localEulerAngles = Quaternion.Euler(_lookRotation).eulerAngles;
    }

    private void Rotate(float amount)
    {
        _bodyRotation.y += amount;
        transform.eulerAngles = _bodyRotation;
    }

    private void ProcessInput()
    {
        InputLookDelta = new Vector3()
        {
            x = -Input.GetAxis("Mouse Y"),
            y = Input.GetAxis("Mouse X"),
            z = 0
        };

        InputMoveDelta = new Vector3()
        {
            x = Input.GetAxis("Horizontal"),
            y = 0,
            z = Input.GetAxis("Vertical")
        }.normalized;

        isJumping = Input.GetKeyDown(KeyCode.Space);
        isBlocking = Input.GetKey(KeyCode.LeftControl);
        isLightAttacking = Input.GetMouseButtonDown(0);
        isHeavyAttacking = Input.GetMouseButtonDown(1);
    }

    private bool IsGrounded()
    {
        bool grounded = false;
        Vector3 feetPoint = collider.bounds.center - new Vector3(0, collider.bounds.extents.y, 0);
        const float collisionRadius = 0.2f;
        var overlapping = Physics.OverlapSphere(feetPoint, collisionRadius);
        foreach(Collider col in overlapping)
        {
            if(col.gameObject.layer == 0 
                || col.gameObject.layer == LayerMask.NameToLayer("Walkable"))
            {
                grounded = true;
            }
        }
        return grounded;
    }
}