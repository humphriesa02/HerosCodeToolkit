using UnityEngine;

namespace HerosCode.Toolkit.Gameplay.Player
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
        public bool CanStart(PlayerContext context);
        public void Start(PlayerContext context);
    }
}