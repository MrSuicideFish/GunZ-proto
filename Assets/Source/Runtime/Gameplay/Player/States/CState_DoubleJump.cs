using UnityEngine;

public class CState_DoubleJump : CombatState
{
    private const string anim_double_jump = "Double_Jump_Start";
    private const string anim_jump_start = "Jump_Start";

    private bool bJumpStart;
    private float jumpTimer;

    public CState_DoubleJump(PlayerController controller) : base(controller)
    {
        CombatStateType = ECombatStateType.DoubleJump;
    }

    public override bool CanInterrupt(CombatState nextState)
    {
        return true;
    }

    public override void OnStateEnter(StateMachine<CombatState> sm)
    {
        if(_controller.rigidBody.velocity.y > 0.0f)
        {
            _controller.animator.Play(anim_double_jump);
            _controller.rigidBody.AddForce(Vector3.up * (_controller.jumpStrength / 2), ForceMode.VelocityChange);
        }
        else
        {
            _controller.animator.Play(anim_jump_start);
            _controller.rigidBody.AddForce(Vector3.up * _controller.jumpStrength, ForceMode.VelocityChange);
        }

        _controller.collider.height = _controller.playerHeight / 2;
        bJumpStart = false;
        jumpTimer = 0.0f;
    }

    public override void OnStateExit(StateMachine<CombatState> sm)
    {
        bJumpStart = false;
        jumpTimer = 0.0f;
        _controller.collider.height = _controller.playerHeight;
    }

    public override void OnStateUpdate(StateMachine<CombatState> sm, float deltaTime)
    {
        jumpTimer += deltaTime;
        if (!bJumpStart)
        {
            if (jumpTimer < 0.1f)
            {
                if (!_controller.isGrounded)
                {
                    bJumpStart = true;
                }
            }
            else
            {
                bJumpStart = true;
            }
        }

        if (bJumpStart)
        {
            if (_controller.isGrounded)
            {
                _controller.TryGoToState(ECombatStateType.Walk);
            }
            else
            {
                if (_controller.rigidBody.velocity.y <= 0)
                {
                    _controller.TryGoToState(ECombatStateType.Fall);
                }
            }
        }
    }

    public override void OnStateFixedUpdate(StateMachine<CombatState> sm, float deltaTime)
    {
        if (bJumpStart)
        {
            // do air move
            Vector3 delta = _controller.WorldInputMoveDelta;
            Vector3 vel = delta * _controller.airSpeed * deltaTime;
            vel.y = 0;
            _controller.rigidBody.AddForce(vel, ForceMode.Acceleration);
        }
    }

    public override void OnStateLateUpdate(StateMachine<CombatState> sm, float deltaTime)
    {

    }
}