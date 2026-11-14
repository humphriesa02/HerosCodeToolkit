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

        public override void PhysicsUpdate()
        {
            base.LogicUpdate();
            if (context.inWater)
            {
                PlayerStateUtil.ApplyWaterDrag(context, values.waterDrag); // I.e. falling into water
            }
            PlayerStateUtil.AdjustVelocity(context, values.maxAirAcceleration);
            PlayerStateUtil.ApplyGravity(context, values);
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}