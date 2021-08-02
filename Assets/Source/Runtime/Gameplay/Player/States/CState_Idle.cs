using UnityEngine;

public class CState_Grounded_Idle : CombatState
{
    private const string IdleAnimName = "Idle_ver_B";
    private float idleTime = 0.0f;

    public CState_Grounded_Idle(PlayerController controller) : base(controller)
    {
        CombatStateType = ECombatStateType.Idle;
    }

    public override bool CanInterrupt(CombatState nextState = null)
    {
        return true;
    }

    public override void OnStateEnter(StateMachine<CombatState> sm)
    {
        ResetIdleTime();
        PlayIdleAnim();
    }

    public override void OnStateExit(StateMachine<CombatState> sm)
    {
        ResetIdleTime();
    }

    public override void OnStateUpdate(StateMachine<CombatState> sm, float deltaTime)
    {
        if (!PlayingIdleAnim())
        {
            PlayIdleAnim();
        }

        idleTime += deltaTime;
        if(InputMoveDelta != Vector3.zero)
        {
            _controller.SetCombatState(ECombatStateType.Walk);
        }
    }

    private void ResetIdleTime()
    {
        idleTime = 0.0f;
    }

    private bool PlayingIdleAnim()
    {
        return _controller.animator.GetCurrentAnimatorStateInfo(0).IsName(IdleAnimName);
    }

    private void PlayIdleAnim()
    {
        _controller.animator.Play(IdleAnimName);
    }
}