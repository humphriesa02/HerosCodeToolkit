using UnityEngine;

namespace HerosCode.Toolkit.Player
{
    /// <summary>
    /// // Visual only: shows a ball and rolls it so its surface matches the player's motion.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerRollingVisualExecutor : MonoBehaviour, IPlayerExecutor
    {
        [SerializeField, Tooltip("The ball child: a mesh with no collider. Rotated so its poles point sideways")]
        private Transform ball = default;

        [SerializeField, Tooltip("Optional. The normal character model, hidden while the ball is shown")]
        private GameObject characterModel = default;

        private PlayerValues values;
        private Rigidbody rb;

        public void Initialize(PlayerContext context, PlayerValues _values)
        {
            values = _values;
            rb = GetComponent<Rigidbody>();
        }

        public void Execute(PlayerContext context)
        {
            if (ball.gameObject.activeSelf != context.isRolling) ball.gameObject.SetActive(context.isRolling);
            if (characterModel != null && characterModel.activeSelf == context.isRolling) characterModel.SetActive(!context.isRolling);

            if (context.isRolling) RollBall(context);
        }

        
        private void RollBall(PlayerContext context)
        {
            var rotationPlaneNormal = context.contactNormal;
            var rotationFactor = 1f;

            if (context.isSwimming)
            {
                rotationFactor = values.visualSwimRotation;
            }
            else if (!context.onGround)
            {
                if (context.onSteep)
                {
                    rotationPlaneNormal = context.steepNormal;
                }
                else
                {
                    rotationFactor = values.visualAirRotation;
                }
            }

            var movement = (rb.linearVelocity - context.connectionVelocity) * Time.deltaTime;
            movement -= rotationPlaneNormal * Vector3.Dot(movement, rotationPlaneNormal);

            var distance = movement.magnitude;

            // Work entirely in world space.
            var rotation = ball.rotation;

            rotation = Quaternion.Euler(
                context.connectionAngularVelocity * (Mathf.Rad2Deg * Time.deltaTime)) * rotation;

            if (distance < 0.001f)
            {
                ball.rotation = rotation;
                return;
            }

            var angle = distance * rotationFactor *
                        (180f / Mathf.PI) / values.rollVisualRadius;

            var rotationAxis = Vector3.Cross(rotationPlaneNormal, movement).normalized;

            rotation = Quaternion.AngleAxis(angle, rotationAxis) * rotation;

            if (values.visualAlignSpeed > 0f)
            {
                rotation = AlignBallRotation(rotationAxis, rotation, distance);
            }

            ball.rotation = rotation;
        }

        private Quaternion AlignBallRotation(
            Vector3 rotationAxis,
            Quaternion rotation,
            float traveledDistance)
        {
            var ballAxis = rotation * Vector3.up;

            var dot = Mathf.Clamp(Vector3.Dot(ballAxis, rotationAxis), -1f, 1f);
            var angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
            var maxAngle = values.visualAlignSpeed * traveledDistance;

            var targetRotation =
                Quaternion.FromToRotation(ballAxis, rotationAxis) * rotation;

            if (angle <= maxAngle)
            {
                return targetRotation;
            }

            return Quaternion.SlerpUnclamped(
                rotation,
                targetRotation,
                maxAngle / angle
            );
        }

    }
}