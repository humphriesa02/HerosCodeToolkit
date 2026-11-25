using HerosCode.Toolkit.Player;
using UnityEngine;

namespace HerosCode.Toolkit.Misc
{
    /// <summary>
    /// Within the zone, the rigidody that collides
    /// gets a speed boost
    /// </summary>
    public class AccelerationZone : MonoBehaviour
    {
        [SerializeField, Min(0f), Tooltip("The speed to set the colliding body")]
        private float speed = 10f;

        private void OnTriggerEnter(Collider other)
        {
            var rb = other.attachedRigidbody;
            if (rb != null)
            {
                Accelerate(rb);
            }
        }

        private void Accelerate(Rigidbody rb)
        {
            var velocity = transform.InverseTransformDirection(rb.linearVelocity);
            if (velocity.y >= speed) return;

            velocity.y = speed;
            rb.linearVelocity = transform.TransformDirection(velocity);
            if (rb.TryGetComponent(out PlayerWorldSensor sensor))
            {
                sensor.PreventSnapToGround();
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            var localEnd = transform.position + (transform.up * 2f);
            Gizmos.DrawLine(transform.position, localEnd);
        }
    }
}
