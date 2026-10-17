using UnityEngine;
using HerosCode.Toolkit.Core;

namespace HerosCode.Toolkit.Player
{
    /// <summary>
    /// Basic grounded movement
    /// </summary>
    public class DynamicGroundState : PlayerLocomotionState
    {
        Vector3 currentVelocity;
        Vector3 smoothVelocityRef;

        public override bool CanEnter(PlayerContext context) => context.isGrounded;

        public DynamicGroundState(PlayerValues _values, PlayerContext _context) : base(_values, _context) { }

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
            var playerInput = context.moveDirection;
            playerInput = Vector2.ClampMagnitude(playerInput, 1f);

            context.desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * values.maxSpeed;
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            AdjustVelocity();
        }

        void AdjustVelocity () {

            float currentX = Vector3.Dot(context.velocity, context.xAxis);
            float currentZ = Vector3.Dot(context.velocity, context.zAxis);

            float acceleration = values.maxAcceleration;
            float maxSpeedChange = acceleration * Time.deltaTime;

            float newX =
                Mathf.MoveTowards(currentX, context.desiredVelocity.x, maxSpeedChange);
            float newZ =
                Mathf.MoveTowards(currentZ, context.desiredVelocity.z, maxSpeedChange);

            context.velocity += context.xAxis * (newX - currentX) + context.zAxis * (newZ - currentZ);
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}