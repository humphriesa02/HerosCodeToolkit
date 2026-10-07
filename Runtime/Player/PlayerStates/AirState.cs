using UnityEngine;
using HerosCode.Toolkit.Core;

namespace HerosCode.Toolkit.Player
{
    /// <summary>
    /// Inair state
    /// </summary>
    public class AirState : PlayerLocomotionState
    {
        public override bool CanEnter(PlayerContext context) => !context.isGrounded;

        public AirState(PlayerValues _values, PlayerContext _context) : base(_values, _context) { }

        public override void Enter()
        {
            base.Enter();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            
            Vector3 blended = context.moveDirection * values.airControl + context.velocity * (1 - values.airControl);
            context.desiredVelocity = blended * values.moveSpeed;
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}