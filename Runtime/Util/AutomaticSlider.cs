using UnityEngine;
using UnityEngine.Events;

namespace HerosCode.Toolkit.Util
{
    [System.Serializable]
    public class OnValueChangedEvent : UnityEvent<float> { }
    /// <summary>
    /// A "slider" moving a value automatically from 0 to 1
    /// for as long as "duration"
    /// </summary>
    public class AutomaticSlider : MonoBehaviour
    {
        [SerializeField, Min(0.01f), Tooltip("Duration of the slide from 0 to 1")]
        private float duration = 1f;
        [SerializeField, Tooltip("If true, will begin on awake")]
        private bool startOnAwake = false;
        [SerializeField, Tooltip("If true, ping pong the slider back and forth")]
        private bool autoReverse = false;
        [SerializeField, Tooltip("If true, eases in and out slower, making things smooth")]
        private bool smoothStep = false;
        [SerializeField, Tooltip("This action will send out the current value of the slider")]
        private OnValueChangedEvent onValueChanged = default;

        private float value;
        private bool started = false;
        // Can be set externally
        public bool Reversed { get; set; }
        public bool AutoReverse { get => autoReverse; set => autoReverse = value; }
        private float SmoothedValue => 3f * value * value - 2f * value * value * value;

        private void Awake()
        {
            started = startOnAwake;
        }

        private void FixedUpdate()
        {
            if (!started) return;
            var delta = Time.deltaTime / duration;
            if (Reversed)
            {
                value -= delta;
                if (value <= 0f)
                {
                    if (autoReverse)
                    {
                        value = Mathf.Min(1f, -value);
                        Reversed = false;
                    }
                    else
                    {
                        value = 0f;
                        enabled = false;
                    }
                }
            }
            else
            {
                value += delta;
                if (value >= 1f)
                {
                    if (autoReverse)
                    {
                        value = Mathf.Max(0f, 2f - value);
                        Reversed = true;
                    }
                    else
                    {
                        value = 1f;
                        enabled = false;
                    }
                }
            }
            onValueChanged.Invoke(smoothStep ? SmoothedValue : value);
        }

        public void Activate()
        {
            started = true;
        }
    }
}
