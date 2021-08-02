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
    private const float MAX_LOOK_Y = 60;
    private const float MIN_LOOK_Y = -30;

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

    [SyncVar] public HitInfo lastHitInfo;
    #endregion

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

        _combatStateDict.Add(idle_state.CombatStateType, idle_state);
        _combatStateDict.Add(walk_state.CombatStateType, walk_state);
        _combatStateDict.Add(jump_state.CombatStateType, jump_state);
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

        base.OnStartClient();
    }

    public void Jump(Vector2 inputDir)
    {
        TryGoToState(CombatState.ECombatStateType.Jump);
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
            InputLookDelta = new Vector3()
            {
                x = Input.GetAxis("Mouse Y"),
                y = Input.GetAxis("Mouse X"),
                z = 0
            };

            InputMoveDelta = new Vector3()
            {
                x = Input.GetAxis("Horizontal"),
                y = 0,
                z = Input.GetAxis("Vertical")
            };

            WorldInputMoveDelta = shoulderFollowTarget
                .TransformDirection(InputMoveDelta);

            isJumping = Input.GetKeyDown(KeyCode.Space);
            isBlocking = Input.GetKeyDown(KeyCode.LeftControl);
            isLightAttacking = Input.GetMouseButtonDown(0);
            isHeavyAttacking = Input.GetMouseButtonDown(1);
            isGrounded = IsGrounded();
        }
    }

    private void FixedUpdate()
    {
        _combatSM.TickStateMachine(Time.deltaTime);
    }

    private bool IsGrounded()
    {
        return false;
    }
}