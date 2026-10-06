using UnityEngine;
using HerosCode.Toolkit.Gameplay.SM;

namespace HerosCode.Toolkit.Gameplay.Player
{
    public class LandingState : PlayerLocomotionState
    {
        private float timePassed;
        public LandingState(PlayerController _player, StateMachine _stateMachine,  PlayerValues _values, PlayerContext _context) : base(_player, _stateMachine, _values, _context) { }

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
}