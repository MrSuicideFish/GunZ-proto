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