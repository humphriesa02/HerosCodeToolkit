namespace HerosCode.Toolkit.Player
{
    /// <summary>
    /// Maps cleanly to a controller/keyboard binding
    /// and can be hotswapped at runtime.
    /// <see cref="PlayerContext"/> tells it whether
    /// it can run (including if the button to activate is pressed)
    /// And then <see cref="PlayerActionDriver"/> runs it
    /// </summary>
    public interface IPlayerAction
    {
        bool CanStart(PlayerContext context);
        void Start(PlayerContext context);
    }
}
