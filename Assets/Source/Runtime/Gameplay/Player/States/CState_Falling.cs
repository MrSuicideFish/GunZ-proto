using UnityEngine;

public class CState_Falling : CombatState
{
    private const string anim_jump_loop = "Jump_Loop";

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
        _controller.animator.Play(anim_jump_loop);
        _controller.collider.height = _controller.playerHeight / 2;
    }

    public override void OnStateExit(StateMachine<CombatState> sm)
    {
        _controller.collider.height = _controller.playerHeight;
    }

    public override void OnStateUpdate(StateMachine<CombatState> sm, float deltaTime)
    {
        if (_controller.isGrounded)
        {
            _controller.TryGoToState(ECombatStateType.Idle);
        }
        else
        {
            // do air move
            Vector3 delta = _controller.WorldInputMoveDelta;
            Vector3 vel = delta * _controller.airSpeed * deltaTime;
            vel.y = 0;
            _controller.rigidBody.AddForce(vel, ForceMode.Acceleration);
        }
    }

    public override void OnStateFixedUpdate(StateMachine<CombatState> sm, float deltaTime)
    {

    }

    public override void OnStateLateUpdate(StateMachine<CombatState> sm, float deltaTime)
    {

    }
}