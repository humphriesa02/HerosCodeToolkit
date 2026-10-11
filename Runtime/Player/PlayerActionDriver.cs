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

        private PlayerValues values;
        public PlayerActionDriver(PlayerValues _values)
        {
            values = _values;
        }

        public void Bind (Keybind keybind, IPlayerAction action) => bindings.Add(keybind, action);

        public void Apply(PlayerContext context)
        {
            foreach (var (keybind, action) in bindings)
            {
                if (context.WasPressed(keybind, values.inputBuffer) && action.CanStart(context))
                {
                    action.Start(context);
                    context.ConsumePress(keybind);
                }
            }
        }
    }
}
