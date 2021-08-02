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
    private Rigidbody _rigidBody;
    private CapsuleCollider _collider;
    private CombatCamera combatCamera;
    public Transform shoulderFollowTarget;

    public Animator animator { get; private set; }
    #endregion

    #region Properties / Status
    [SyncVar] public Vector2 moveDirection;
    [SyncVar] public Vector3 lookDirection;
    [SyncVar] public bool isJumping;
    [SyncVar] public bool isGrounded;
    [SyncVar] public bool isLightAttacking;
    [SyncVar] public bool isHeavyAttacking;
    [SyncVar] public bool isBlocking;

    public HitInfo lastHitInfo { get; private set; }
    #endregion

    // states
    private Dictionary<CombatState.ECombatStateType, CombatState> _combatStateDict;

    private void Awake()
    {
        _combatSM = new StateMachine<CombatState>();
        animator = this.GetComponent<Animator>();
        _rigidBody = this.GetComponent<Rigidbody>();
        _collider = this.GetComponent<CapsuleCollider>();

        // create states
        _combatStateDict = new Dictionary<CombatState.ECombatStateType, CombatState>();

        var idle_state = new CState_Grounded_Idle(this);
        var walk_state = new CState_Grounded_Walking(this);

        _combatStateDict.Add(idle_state.CombatStateType, idle_state);
        _combatStateDict.Add(walk_state.CombatStateType, walk_state);
        SetCombatState(CombatState.ECombatStateType.Idle);
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
        base.OnStartClient();
    }

    public void Move(Vector2 inputDir)
    {
        moveDirection = inputDir;
    }

    public void Look(Vector3 dir)
    {
        Vector3 newDir = lookDirection + dir;
        newDir.x = Mathf.Clamp(newDir.x, MIN_LOOK_Y, MAX_LOOK_Y);
        newDir.y %= 360;
        lookDirection = newDir;
    }

    public void Jump(Vector2 inputDir)
    {
        isJumping = true;
    }

    public void Hit(HitInfo hitInfo)
    {
        lastHitInfo = hitInfo;
    }

    public void TurnTowardsCameraDirection(float deltaTime, float speed = 1.0f)
    {
    }

    public void SetCombatState(CombatState.ECombatStateType stateType)
    {
        if (_combatStateDict.ContainsKey(stateType))
        {
            _combatSM.GoToState(_combatStateDict[stateType]);
        }
    }

    private void Update()
    {
        _combatSM.TickStateMachine(Time.deltaTime);

        if (isJumping) isJumping = false;
        isGrounded = IsGrounded();
    }

    private bool IsGrounded()
    {
        return false;
    }
}