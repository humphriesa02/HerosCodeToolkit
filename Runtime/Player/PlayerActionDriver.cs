using System.Collections.Generic;

namespace HerosCode.Toolkit.Player
{
    /// <summary>
    /// Keybindings that map to
    /// player input
    /// </summary>
    public enum Keybind
    {
        Primary,
        Secondary
    }

    public class PlayerActionDriver : IPlayerDriver
    {
        private readonly Dictionary<Keybind, IPlayerAction> bindings = new();

        public void Bind (Keybind keybind, IPlayerAction action) => bindings.Add(keybind, action);

        public void Apply(PlayerContext context)
        {
            foreach (var (keybind, action) in bindings)
            {
                if (context.WasPressed(keybind) && action.CanStart(context))
                {
                    action.Start(context);
                }
            }
        }
    }
}
