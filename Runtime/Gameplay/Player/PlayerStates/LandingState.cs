using UnityEngine;

public class LandingState : PlayerLocomotionState
{
    private float timePassed;
    public LandingState(PlayerController _player, StateMachine _stateMachine,  PlayerValues _values) : base(_player, _stateMachine, _values) { }

    public override void Enter()
    {
        base.Enter();
        timePassed = 0f;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (timePassed > values.landingTime)
        {
            player.animator.SetTrigger("grounded");
            stateMachine.ChangeState(player.groundState);   
        }
        timePassed += Time.deltaTime;
    }
}
