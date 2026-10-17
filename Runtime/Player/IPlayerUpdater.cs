namespace HerosCode.Toolkit.Player
{
    public interface IPlayerUpdater
    {
        public abstract void Tick(PlayerContext context);
        public virtual void FixedTick(PlayerContext context) {}
        public virtual void LateTick(PlayerContext context) {}
    }
}