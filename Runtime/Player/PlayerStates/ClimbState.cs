using UnityEngine;

namespace HerosCode.Toolkit.Player
{
    /// <summary>
    /// Basic grounded movement
    /// </summary>
    public class ClimbState : PlayerLocomotionState
    {

        public override bool CanEnter(PlayerContext context) => context.isClimbing;

        public ClimbState(PlayerValues _values, PlayerContext _context) : base(_values, _context) { }

        public override void Enter()
        {
            base.Enter();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            // contex isclimbing changes this automatically
            PlayerStateUtil.ConvertMoveInputToDesiredVelocity(context, values.maxSpeed);
            FaceWall(false);
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            var normal = context.climbNormal;

            // The wall can disappear between Update (which chose this state) and this fixed step
            if (normal == Vector3.zero)
            {
                PlayerStateUtil.ApplyGravity(context);
                return;
            }

            PlayerStateUtil.ResetJumpPhase(context);
            PlayerStateUtil.AdjustVelocity(context, values.maxClimbAcceleration);

            // No gravity. Press into the wall so contact never drops
            context.velocity -= normal * (values.maxClimbAcceleration * 0.9f * Time.fixedDeltaTime);
        }
        
        public override void Exit()
        {
            base.Exit();
        }

        /// <summary>
        /// Makes the player look at the wall we're climbing
        /// If snap is true, snaps the player instead of lerping
        /// </summary>
        /// <param name="snap"></param>
        private void FaceWall(bool snap)
        {
            var toWall = -context.climbNormal;
            toWall.y = 0f;
            if (toWall.sqrMagnitude < 0.0001f) return;

            context.lookDirection = toWall;
            if (snap) context.snapLook = true;
        }
    }
}