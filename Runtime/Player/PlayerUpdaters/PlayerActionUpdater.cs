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

    public class PlayerActionUpdater : IPlayerUpdater
    {
        // Actions
        public JumpAction jumpAction;
        public TripleJumpAction tripleJumpAction;
        public DiveAction diveAction;
        public PunchAction punchAction;
        private readonly Dictionary<Keybind, IPlayerAction> bindings = new();
        private PlayerValues values;

        public void Bind (Keybind keybind, IPlayerAction action) => bindings.Add(keybind, action); 
        public PlayerActionUpdater(PlayerContext context, PlayerValues _values)
        {
            values = _values;
            jumpAction = new(values);
            tripleJumpAction = new(values);
            diveAction = new(values);
            punchAction = new(values);

            Bind(Keybind.Primary, tripleJumpAction);
            Bind(Keybind.Secondary, diveAction);
        }

        public void Tick(PlayerContext context)
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
