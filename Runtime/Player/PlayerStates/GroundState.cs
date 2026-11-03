using UnityEngine;

namespace HerosCode.Toolkit.Player
{
    /// <summary>
    /// Basic grounded movement
    /// </summary>
    public class GroundState : PlayerLocomotionState
    {

        public override bool CanEnter(PlayerContext context) => context.onGround;

        public GroundState(PlayerValues _values, PlayerContext _context) : base(_values, _context) { }

        public override void Enter()
        {
            base.Enter();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            PlayerStateUtil.ConvertMoveInputToDesiredVelocity(context, values.maxSpeed);
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            PlayerStateUtil.ResetJumpPhase(context);
            PlayerStateUtil.ApplyGroundSnap(context);
            PlayerStateUtil.AdjustVelocity(context, values.maxAcceleration);
            PlayerStateUtil.FaceMoveDirection(context);
            PlayerStateUtil.ApplyGravity(context);
        }
        
        public override void Exit()
        {
            base.Exit();
        }
    }
}