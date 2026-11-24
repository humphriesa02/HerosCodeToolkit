namespace HerosCode.Toolkit.Player
{
    /// <summary>
    /// Updates the logic within the
    /// <see cref="PlayerContext"/>
    /// to help send it down the line.
    /// Things like locomotion control is done
    /// with these
    /// </summary>
    public interface IPlayerUpdater
    {
        abstract void Tick(PlayerContext context);
        virtual void FixedTick(PlayerContext context) { }
        virtual void LateTick(PlayerContext context) { }
    }
}
