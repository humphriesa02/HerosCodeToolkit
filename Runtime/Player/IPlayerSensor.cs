namespace HerosCode.Toolkit.Player
{
    /// <summary>
    /// Sensors get information outside the player
    /// and populate the context
    /// </summary>
    public interface IPlayerSensor
    {
        abstract void Initialize(PlayerContext context, PlayerValues values);
        void CollectData(PlayerContext context);
        void FixedCollectData(PlayerContext context);
        void ClearData(PlayerContext context);
    }
}
