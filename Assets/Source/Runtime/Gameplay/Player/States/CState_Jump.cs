﻿using UnityEngine;

public class CState_Jump : CombatState
{
    private const string anim_jump_start = "Jump_Start";

    public CState_Jump(PlayerController controller) : base(controller)
    {
        CombatStateType = ECombatStateType.Jump;
    }

    public override bool CanInterrupt(CombatState nextState)
    {
        return true;
    }

    public override void OnStateEnter(StateMachine<CombatState> sm)
    {
        _controller.rigidBody.AddForce(Vector3.up * _controller.jumpStrength, ForceMode.Force);
        _controller.animator.Play(anim_jump_start);
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
        else
        {
            if(_controller.rigidBody.velocity.y <= 0)
            {
                _controller.TryGoToState(ECombatStateType.Fall);
            }
        }
    }

    public override void OnStateFixedUpdate(StateMachine<CombatState> sm, float deltaTime)
    {

    }

    public override void OnStateLateUpdate(StateMachine<CombatState> sm, float deltaTime)
    {

    }
}