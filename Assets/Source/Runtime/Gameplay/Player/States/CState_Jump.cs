using UnityEngine;

public class CState_Jump : CombatState
{
    public CState_Jump(PlayerController controller) : base(controller)
    {
        CombatStateType = ECombatStateType.Jump;
    }

    public override bool CanInterrupt(CombatState nextState)
    {
        return false;
    }

    public override void OnStateEnter(StateMachine<CombatState> sm)
    {
        _controller.rigidBody.AddForce(Vector3.up * _controller.jumpStrength, ForceMode.Impulse);
    }

    public override void OnStateExit(StateMachine<CombatState> sm)
    {
    }

    public override void OnStateUpdate(StateMachine<CombatState> sm, float deltaTime)
    {
        if (_controller.isGrounded)
        {
            _controller.TryGoToState(ECombatStateType.Walk);
        }
    }

    public override void OnStateFixedUpdate(StateMachine<CombatState> sm, float deltaTime)
    {

    }

    public override void OnStateLateUpdate(StateMachine<CombatState> sm, float deltaTime)
    {

    }
}