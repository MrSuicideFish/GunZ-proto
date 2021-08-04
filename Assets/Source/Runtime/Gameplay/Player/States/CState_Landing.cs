using UnityEngine;

public class CState_Landing : CombatState
{
    private const string anim_landing = "Double_Jump_End";

    public CState_Landing(PlayerController controller) : base(controller)
    {
        CombatStateType = ECombatStateType.Landing;
    }

    public override bool CanInterrupt(CombatState nextState = null)
    {
        if(nextState != null)
        {
            if(nextState.CombatStateType == ECombatStateType.Walk
                || nextState.CombatStateType == ECombatStateType.Jump)
            {
                return false;
            }
        }
        return true;
    }

    public override void OnStateEnter(StateMachine<CombatState> sm)
    {
        _controller.animator.Play(anim_landing);
    }

    public override void OnStateExit(StateMachine<CombatState> sm)
    {
    }

    public override void OnStateUpdate(StateMachine<CombatState> sm, float deltaTime)
    {
        Vector3 vel = Vector3.zero;
        vel.y = _controller.rigidBody.velocity.y;
        _controller.rigidBody.velocity = vel;

        AnimatorStateInfo animState = _controller.animator.GetCurrentAnimatorStateInfo(0);
        if (animState.IsName(anim_landing)
            && animState.normalizedTime >= 1.0f)
        {
            _controller.TryGoToState(ECombatStateType.Idle);
        }
    }

    public override void OnStateFixedUpdate(StateMachine<CombatState> sm, float deltaTime)
    {

    }

    public override void OnStateLateUpdate(StateMachine<CombatState> sm, float deltaTime)
    {

    }
}