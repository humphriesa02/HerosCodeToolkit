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

            // TODO - determine if this is what we want.
            // ALL movement will get slowed in water. I believe that makes sense.
            if (context.inWater)
            {
                PlayerStateUtil.ApplyWaterDrag(context, playerValues.waterDrag);
            }
            rb.linearVelocity = context.velocity;
            ApplyRotation(context);
        }

        void ApplyRotation(PlayerContext context)
        {
            Vector3 dir = context.lookDirection;
            dir.y = 0f;
            if (dir.sqrMagnitude < 0.0001f) return;

            Quaternion target = Quaternion.LookRotation(dir.normalized, Vector3.up);
            Quaternion next = context.snapLook ? target : Quaternion.RotateTowards(rb.rotation, target, playerValues.turnSpeed * Time.fixedDeltaTime);

            context.snapLook = false;
            rb.MoveRotation(next);
        }
    }
}