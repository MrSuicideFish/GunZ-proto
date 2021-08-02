#nullable enable
using System;
using UnityEngine;

public abstract class CombatState : SM_State<CombatState>
{
    public enum ECombatStateType
    {
        None = 0,
        Idle,
        Walk,
        Dash,
        Jump,
        LightAttack,
        HeavyAttack,
        Hit_Heavy,
        Dying,
        Dead
    }

    public ECombatStateType CombatStateType;
    protected PlayerController _controller;
    protected Guid StateId;

    protected Vector3 InputLookDelta;
    protected Vector3 InputMoveDelta;
    protected Vector3 WorldInputMoveDelta;

    public CombatState(PlayerController controller)
    {
        _controller = controller;
        StateId = Guid.NewGuid();
    }

    #region State Machine Stuff
    public abstract void OnStateEnter(StateMachine<CombatState> sm);

    public abstract void OnStateExit(StateMachine<CombatState> sm);

    public virtual void OnStateUpdate(StateMachine<CombatState> sm, float deltaTime)
    {
        if (_controller.isLocalPlayer)
        {
            InputLookDelta = new Vector3()
            {
                x = Input.GetAxis("Mouse Y"),
                y = Input.GetAxis("Mouse X"),
                z = 0
            };

            InputMoveDelta = new Vector3()
            {
                x = Input.GetAxis("Horizontal"),
                y = 0,
                z = Input.GetAxis("Vertical")
            };

            WorldInputMoveDelta = _controller.shoulderFollowTarget
                .TransformDirection(InputMoveDelta);
        }
    }

    public abstract bool CanInterrupt(CombatState? nextState);

    public int CompareTo(CombatState other)
    {
        if (other == null) return 1;
        return this.StateId.CompareTo(other.StateId);
    }
    #endregion
}
