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

        if (_controller.isJumping && _controller.groundedTimerExpired)
        {
            _controller.TryGoToState(ECombatStateType.Jump);
            return;
        }

        if (!_controller.isGrounded)
        {
            _controller.TryGoToState(ECombatStateType.Fall);
            return;
        }

        idleTime += deltaTime;

        Vector3 vel = Vector3.zero;
        vel.y = _controller.rigidBody.velocity.y;
        _controller.rigidBody.velocity = vel;

        if (_controller.InputMoveDelta != Vector3.zero)
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