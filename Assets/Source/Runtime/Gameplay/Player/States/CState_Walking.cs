using UnityEngine;

public class CState_Grounded_Walking : CombatState
{
    // cardinal
    private const string anim_walk_fwd = "Idle_ver_B";
    private const string anim_walk_left = "Idle_ver_B";
    private const string anim_walk_right = "Idle_ver_B";
    private const string anim_walk_back = "Idle_ver_B";

    // inter-cardinal
    private const string anim_walk_fwd_left = "Idle_ver_B";
    private const string anim_walk_fwd_right = "Idle_ver_B";
    private const string anim_walk_back_left  = "Idle_ver_B";
    private const string anim_walk_back_right = "Idle_ver_B";

    public CState_Grounded_Walking(PlayerController controller) : base(controller)
    {
        CombatStateType = ECombatStateType.Walk;
    }

    public override bool CanInterrupt(CombatState nextState = null)
    {
        return true;
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