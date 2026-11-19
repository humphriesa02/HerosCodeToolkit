
namespace HerosCode.Toolkit.Player
{
    /// <summary>
    /// Rolling on the ground.
    /// </summary>
    public class RollState : PlayerLocomotionState
    {

        public override bool CanEnter(PlayerContext context) => context.isRolling;

        public RollState(PlayerValues _values, PlayerContext _context) : base(_values, _context) { }

        public override void Enter()
        {
            base.Enter();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            PlayerStateUtil.ConvertMoveInputToDesiredVelocity(context, values.maxRollSpeed);
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            var hasInput = context.moveInput.sqrMagnitude > 0.01f;
            var acceleration = hasInput ? values.maxRollAcceleration : values.rollFriction;
            PlayerStateUtil.AdjustVelocityClamped(context, acceleration);
            PlayerStateUtil.ApplyGravity(context, values);
        }
        
        public override void Exit()
        {
            base.Exit();
        }
    }
}