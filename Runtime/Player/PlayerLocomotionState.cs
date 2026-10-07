using HerosCode.Toolkit.Core;

namespace HerosCode.Toolkit.Player
{
    /// <summary>
    /// Locomotion states set these,
    /// and are read by actions to determine
    /// which can be done when
    /// </summary>
    public enum PlayerLocomotionTags
    {
        Grounded,
        Airborne,
        Submerged
    }

    /// <summary>
    /// A state purely represents locomotion available
    /// the player. Things like basic land movement,
    /// swimming, falling, etc.
    /// 
    /// Actions are triggers for locomotion changes,
    /// or, simply do things within a locomotion state.
    /// (Ground combat vs air combat)
    /// </summary>
    public abstract class PlayerLocomotionState : State
    {
        protected PlayerValues values;
        protected PlayerContext context;

        public PlayerLocomotionState(PlayerValues _values, PlayerContext _context)
        {
            values = _values;
            context = _context;
        }

        /// Consumed by <see cref="PlayerLocomotionStateDriver"/>
        /// To determine current locomotion state
        public abstract bool CanEnter(PlayerContext context);
        public virtual bool CanStay(PlayerContext context) => CanEnter(context);
    }
}