namespace HerosCode.Toolkit.Player
{
    /// <summary>
    /// Drivers take information from
    /// the context and drive the physical
    /// player gameobject
    /// </summary>
    public interface IPlayerDriver
    {
        public void Apply(PlayerContext context);
    }
}
