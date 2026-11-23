namespace HerosCode.Toolkit.Player
{
    /// <summary>
    /// Drivers take information from
    /// the context and drive the physical
    /// player gameobject
    ///
    /// Exposes all possible Unity tick operations,
    /// but they're driven in order by the controller
    /// </summary>
    public interface IPlayerExecutor
    {
        void Initialize(PlayerContext context, PlayerValues values);
        void Execute(PlayerContext context);
    }
}
