namespace HerosCode.Toolkit.Player
{
    /// <summary>
    /// Sensors get information outside the player
    /// and populate the context
    /// </summary>
    public interface IPlayerSensor
    {
        public abstract void Initialize(PlayerContext context, PlayerValues values);
        public void CollectData(PlayerContext context);
        public void FixedCollectData(PlayerContext context);
        public void ClearData(PlayerContext context);
    }
}
