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
    }

    public override void OnStateExit(StateMachine<CombatState> sm)
    {
    }

    public override void OnStateUpdate(StateMachine<CombatState> sm, float deltaTime)
    {
    }
}