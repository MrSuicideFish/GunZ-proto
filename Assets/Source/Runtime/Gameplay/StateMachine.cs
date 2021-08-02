#nullable enable
using System;
using System.Collections.Generic;

/*
public enum ECombatState
{
    Idle,
    GroundRun,
    WallRun,
    Jump,
    DoubleJump,
    Dodge,
    LightAttack,
    HeavyAttack,
    Knocked,
    Stunned
}
*/

public interface SM_State<T> : IComparable<T> where T : SM_State<T>
{
    public bool CanInterrupt(T nextState);
    public void OnStateEnter(StateMachine<T> sm);
    public void OnStateExit(StateMachine<T> sm);
    public void OnStateUpdate(StateMachine<T> sm, float deltaTime);
}

public class StateMachine<T> where T : SM_State<T>
{
    private T _currentState;

    public T CurrentState
    {
        get
        {
            return _currentState;
        }

        private set
        {
            _currentState = value;
        }
    }

    public void TickStateMachine(float deltaTime)
    {
        if(CurrentState != null)
        {
            CurrentState.OnStateUpdate(this, deltaTime);
        }
    }

    public bool GoToState(T state)
    {
        if(state != null && 
            (CurrentState == null 
            || (CurrentState?.CompareTo(state) != 0 && CurrentState.CanInterrupt(state))))
        {
            CurrentState?.OnStateExit(this);
            CurrentState = state;
            CurrentState?.OnStateEnter(this);
            return true;
        }

        return false;
    }
}