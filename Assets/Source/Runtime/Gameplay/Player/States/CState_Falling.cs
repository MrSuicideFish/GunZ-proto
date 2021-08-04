using UnityEngine;

public class CState_Falling : CombatState
{
    private const float MAX_FALL_TIME = 1.2f;
    private const string anim_fall_loop = "Jump_Loop";
    private const string anim_action_fall_loop = "Double_Jump_Loop";

    private float _fallTime = 0.0f;

    public CState_Falling(PlayerController controller) : base(controller)
    {
        CombatStateType = ECombatStateType.Fall;
    }

    public override bool CanInterrupt(CombatState nextState)
    {
        return true;
    }

    public override void OnStateEnter(StateMachine<CombatState> sm)
    {
                _fallTime = 0.0f;
        if (_controller.hasDoubleJumped)
        {
            _controller.animator.Play(anim_action_fall_loop);
            _controller.collider.height = _controller.playerHeight;
        }
        else
        {
            _controller.animator.Play(anim_fall_loop);
            _controller.collider.height = _controller.playerHeight / 2;
        }
    }

    public override void OnStateExit(StateMachine<CombatState> sm)
    {
        _controller.collider.height = _controller.playerHeight;
        _fallTime = 0.0f;
    }

    public override void OnStateUpdate(StateMachine<CombatState> sm, float deltaTime)
    {
        Debug.Log(_fallTime);
        if (_controller.isGrounded)
        {
            _controller.TryGoToState(_fallTime >= MAX_FALL_TIME ?
                ECombatStateType.Landing : ECombatStateType.Idle);
        }
        else
        {
            if (_controller.isJumping && !_controller.hasDoubleJumped)
            {
                _controller.FlagDoubleJump();
                _controller.TryGoToState(ECombatStateType.DoubleJump);
            }

            if(_fallTime >= MAX_FALL_TIME 
                && !_controller.animator.GetCurrentAnimatorStateInfo(0).IsName(anim_action_fall_loop))
            {
                _controller.animator.Play(anim_action_fall_loop);
            }

            // do air move
            Vector3 delta = _controller.WorldInputMoveDelta;
            Vector3 vel = delta * _controller.airSpeed * deltaTime;
            vel.y = 0;
            _controller.rigidBody.AddForce(vel, ForceMode.Acceleration);

            _fallTime += Time.deltaTime;
        }
    }

    public override void OnStateFixedUpdate(StateMachine<CombatState> sm, float deltaTime)
    {

    }

    public override void OnStateLateUpdate(StateMachine<CombatState> sm, float deltaTime)
    {

    }
}