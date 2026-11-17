using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace HerosCode.Toolkit.Misc
{
    /// <summary>
    /// Has two events, on enter and on exit.
    /// When something enters the trigger collider,
    /// on enter fires. When everything leaves the collider,
    /// onExit fires.
    /// </summary>
    public class DetectionZone : MonoBehaviour
    {
        [SerializeField] private UnityEvent onEnter = default, onExit = default;
        private List<Collider> colliders = new List<Collider>();

        void Awake()
        {
            enabled = false;
        }

        void OnDisable()
        {
#if UNITY_EDITOR
            // hot-reload proof
            if (enabled && gameObject.activeInHierarchy)
            {
                return;
            }
#endif
            if (colliders.Count > 0)
            {
                colliders.Clear();
                onExit.Invoke();
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (colliders.Count == 0)
            {
                onEnter.Invoke();
                enabled = true;
            }
            colliders.Add(other);
        }
        void OnTriggerExit(Collider other)
        {
            if (colliders.Remove(other) && colliders.Count == 0)
            {
                onExit.Invoke();
                enabled = false;
            }
        }

        void FixedUpdate()
        {
            for (var i = 0; i < colliders.Count; i++)
            {
                var collider = colliders[i];
                if (collider == null || !collider.gameObject.activeInHierarchy)
                {
                    colliders.RemoveAt(i--);
                    if (colliders.Count == 0)
                    {
                        onExit.Invoke();
                        enabled = false;
                    }
                }
            }
        }
    }
}