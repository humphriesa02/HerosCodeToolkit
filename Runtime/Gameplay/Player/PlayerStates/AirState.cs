using UnityEngine;
using HerosCode.Toolkit.Gameplay.SM;

namespace HerosCode.Toolkit.Gameplay.Player
{
    /// <summary>
    /// Inair state
    /// </summary>
    public class AirState : PlayerLocomotionState
    {
        public AirState(PlayerController _player, StateMachine _stateMachine, PlayerValues _values, PlayerContext _context) : base(_player, _stateMachine, _values, _context) { }

        public override void Enter()
        {
            base.Enter();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            
            Vector3 blended = context.moveDirection * values.airControl + context.velocity * (1 - values.airControl);
            context.desiredVelocity = blended * values.moveSpeed;

            if (context.isGrounded) stateMachine.ChangeState(player.groundState);
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}