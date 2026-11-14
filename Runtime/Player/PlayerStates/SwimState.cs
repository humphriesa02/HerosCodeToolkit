namespace HerosCode.Toolkit.Player
{
    public class SwimState : PlayerLocomotionState
    {
        public override bool CanEnter(PlayerContext context) => context.isSwimming;

        public SwimState(PlayerValues _values, PlayerContext _context) : base(_values, _context) { }

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
            PlayerStateUtil.ApplyWaterDrag(context, values.waterDrag);
            if (values.waterMovement == PlayerValues.WaterMovement.Surface)
            {
                PlayerStateUtil.AdjustVelocity(context, values.maxSwimAcceleration);
                PlayerStateUtil.FaceMoveDirection(context);
                PlayerStateUtil.ApplyWaterGravity(context, values.buoyancy);
            }
            else if (values.waterMovement == PlayerValues.WaterMovement.Free)
            {
                // TODO - mario style movement
            }
        }
        
        public override void Exit()
        {
            base.Exit();
        }
    }
}