namespace HerosCode.Toolkit.Player
{
    /// <summary>
    /// Inair state
    /// </summary>
    public class AirState : PlayerLocomotionState
    {
        public override bool CanEnter(PlayerContext context) => !context.onGround;

        public AirState(PlayerValues _values, PlayerContext _context) : base(_values, _context) { }

        public override void Enter()
        {
            base.Enter();
            PlayerStateUtil.ConvertMoveInputToDesiredVelocity(context, values.maxSpeed);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            PlayerStateUtil.ConvertMoveInputToDesiredVelocity(context, values.maxSpeed);
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            PlayerStateUtil.AdjustVelocityClamped(context, values.maxAirAcceleration);
            PlayerStateUtil.ApplyGravity(context, values);
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}
