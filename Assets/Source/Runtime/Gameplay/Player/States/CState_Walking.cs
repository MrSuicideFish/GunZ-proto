using UnityEngine;

public class CState_Grounded_Walking : CombatState
{
    // cardinal
    private const string anim_walk_fwd = "Jogging_8Way_verB_F";
    private const string anim_walk_left = "Jogging_8Way_verB_L90";
    private const string anim_walk_right = "Jogging_8Way_verB_R90";
    private const string anim_walk_back = "Jogging_8Way_verB_B";

    // inter-cardinal
    private const string anim_walk_fwd_left = "Jogging_8Way_verB_FL45";
    private const string anim_walk_fwd_right = "Jogging_8Way_verB_FR45";
    private const string anim_walk_back_left = "Jogging_8Way_verB_BL45";
    private const string anim_walk_back_right = "Jogging_8Way_verB_BR45";

    private string current_anim = string.Empty;

    public CState_Grounded_Walking(PlayerController controller) : base(controller)
    {
        CombatStateType = ECombatStateType.Walk;
    }

    public override bool CanInterrupt(CombatState nextState = null)
    {
        return true;
    }

    public override void OnStateEnter(StateMachine<CombatState> sm)
    {
        current_anim = string.Empty;
    }

    public override void OnStateExit(StateMachine<CombatState> sm)
    {
        current_anim = string.Empty;
    }

    private void DoWalkAnim(float deltaTime)
    {
        Vector3 delta = _controller.InputMoveDelta;
        string animToPlay = anim_walk_fwd;
        if (delta.z > 0 && delta.x == 0)
        {
            animToPlay = anim_walk_fwd;
        }
        else if (delta.z < 0 && delta.x == 0)
        {
            animToPlay = anim_walk_back;
        }
        else if (delta.x < 0 && delta.z == 0)
        {
            animToPlay = anim_walk_left;
        }
        else if (delta.x > 0 && delta.z == 0)
        {
            animToPlay = anim_walk_right;
        }
        else if (delta.z > 0 && delta.x < 0)
        {
            animToPlay = anim_walk_fwd_left;
        }
        else if (delta.z > 0 && delta.x > 0)
        {
            animToPlay = anim_walk_fwd_right;
        }
        else if (delta.z < 0 && delta.x < 0)
        {
            animToPlay = anim_walk_back_left;
        }
        else if (delta.z < 0 && delta.x > 0)
        {
            animToPlay = anim_walk_back_right;
        }

        if (!current_anim.Equals(animToPlay))
        {
            current_anim = animToPlay;
            _controller.animator.Play(animToPlay);
        }
    }

    public override void OnStateFixedUpdate(StateMachine<CombatState> sm, float deltaTime)
    {
        // do move
        Vector3 delta = _controller.WorldInputMoveDelta;
        Vector3 vel = delta * _controller.walkSpeed * deltaTime;
        vel.y = _controller.rigidBody.velocity.y;
        _controller.rigidBody.velocity = vel;
    }

    public override void OnStateLateUpdate(StateMachine<CombatState> sm, float deltaTime)
    {
        
    }

    public override void OnStateUpdate(StateMachine<CombatState> sm, float deltaTime)
    {
        if (_controller.isJumping)
        {
            _controller.TryGoToState(ECombatStateType.Jump);
            return;
        }

        if (_controller.InputMoveDelta == Vector3.zero)
        {
            _controller.TryGoToState(ECombatStateType.Idle);
            return;
        }

        Debug.Log(_controller.InputMoveDelta);
        DoWalkAnim(deltaTime);
    }
}