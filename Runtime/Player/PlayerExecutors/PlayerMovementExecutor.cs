using UnityEngine;

namespace HerosCode.Toolkit.Player
{
    /// <summary>
    /// Applies actual movement to the player
    /// via <see cref="PlayerContext"/>
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovementExecutor : MonoBehaviour, IPlayerExecutor
    {
        private PlayerValues playerValues;
        private Rigidbody rb;

        public void Initialize(PlayerContext context, PlayerValues values)
        {
            playerValues = values;
            rb = GetComponent<Rigidbody>();
        }

        public void Execute(PlayerContext context)
        {
            context.velocity += context.pendingImpulse;
            if (context.pendingImpulse != Vector3.zero) context.stepsSinceImpulse = 0;
            context.pendingImpulse = Vector3.zero;
            rb.linearVelocity = context.velocity;
            ApplyRotation(context, Time.fixedDeltaTime);
        }

        void ApplyRotation(PlayerContext c, float dt)
        {
            Vector3 dir = c.lookDirection;
            dir.y = 0f;
            if (dir.sqrMagnitude < 0.0001f) return;

            Quaternion target = Quaternion.LookRotation(dir.normalized, Vector3.up);
            Quaternion next = c.snapLook
                ? target
                : Quaternion.RotateTowards(rb.rotation, target, playerValues.turnSpeed * dt);

            c.snapLook = false;
            rb.MoveRotation(next);
        }
    }
}