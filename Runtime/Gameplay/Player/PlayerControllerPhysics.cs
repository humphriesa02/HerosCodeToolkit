using UnityEngine;

namespace HerosCode.Toolkit.Gameplay.Player
{
    /// <summary>
    /// Add to the PlayerController (driven by CharacterController)
    /// if you want it to interact with Physics Objects
    /// </summary>
    public class PlayerControllerPhysics : MonoBehaviour
    {
        [SerializeField] private float pushPower = 2.0f;
        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;

            // no rigidbody
            if (body == null || body.isKinematic)
                return;

            // We dont want to push objects below us
            if (hit.moveDirection.y < -0.3f)
                return;

            // Calculate push direction from move direction,
            // we only push objects to the sides never up and down
            Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

            // If you know how fast your character is trying to move,
            // then you can also multiply the push velocity by that.

            // Apply the push
            body.linearVelocity = pushDir * pushPower;
        }
    }
}