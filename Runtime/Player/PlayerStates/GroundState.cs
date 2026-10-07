using UnityEngine;
using HerosCode.Toolkit.Core;

namespace HerosCode.Toolkit.Player
{
    /// <summary>
    /// Basic grounded movement
    /// </summary>
    public class GroundState : PlayerLocomotionState
    {
        Vector3 currentVelocity;
        Vector3 smoothVelocityRef;

        public override bool CanEnter(PlayerContext context) => context.isGrounded;

        public GroundState(PlayerValues _values, PlayerContext _context) : base(_values, _context) { }

        public override void Enter()
        {
            base.Enter();
            currentVelocity = Vector3.zero;
            smoothVelocityRef = Vector3.zero;
            context.velocity = Vector3.zero;
            context.gravityVelocity.y = 0f;
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            context.velocity = context.moveDirection;
            currentVelocity = Vector3.SmoothDamp(currentVelocity, context.moveDirection, ref smoothVelocityRef, values.velocityDampTime);
            context.desiredVelocity = currentVelocity * values.moveSpeed;

            if (context.moveDirection.sqrMagnitude > 0f)
            {
                context.lookDirection = context.moveDirection;
            }
        }

        public override void Exit()
        {
            base.Exit();

            if (context.velocity.sqrMagnitude > 0f)
            {
                context.lookDirection = context.velocity;
                context.snapLook = true;
            }
        }
    }
}