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
            Vector2 input = Vector2.ClampMagnitude(context.moveInput, 1f);
            context.desiredVelocity = new Vector3(input.x, 0f, input.y) * values.maxSpeed;
        }

        public override void PhysicsUpdate()
        {
            base.LogicUpdate();
            PlayerStateUtil.AdjustVelocity(context, values.maxAirAcceleration);
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}