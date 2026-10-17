using UnityEngine;

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
    public abstract class PlayerExecutor : MonoBehaviour
    {
        public abstract void Initialize(PlayerContext context, PlayerValues values);
        public abstract void Execute(PlayerContext context);
    }
}
