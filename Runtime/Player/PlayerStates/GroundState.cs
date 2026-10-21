using UnityEngine;
using HerosCode.Toolkit.Core;

namespace HerosCode.Toolkit.Player
{
    /// <summary>
    /// Basic grounded movement
    /// </summary>
    public class GroundState : PlayerLocomotionState
    {

        public override bool CanEnter(PlayerContext context) => context.isGrounded;

        public GroundState(PlayerValues _values, PlayerContext _context) : base(_values, _context) { }

        public override void Enter()
        {
            base.Enter();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            Vector2 input = Vector2.ClampMagnitude(context.moveInput, 1f);
            context.desiredVelocity = new Vector3(input.x, 0f, input.y) * values.maxSpeed;
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            PlayerStateUtil.AdjustVelocity(context, values.maxAcceleration);
        }
        
        public override void Exit()
        {
            base.Exit();
        }
    }
}