/// <summary>
/// Sensors get information outside the player
/// and populate the context
/// </summary>
public interface IPlayerSensor
{
    void CollectData(PlayerContext player);
}
