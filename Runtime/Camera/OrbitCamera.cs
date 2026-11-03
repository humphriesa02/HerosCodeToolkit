using System;
using UnityEngine;

namespace HerosCode.Toolkit.Cameras
{
    /// <summary>
    /// Based off of CatLikeCoding's OrbitCamera
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class OrbitCamera : MonoBehaviour
    {
        [Header("Camera Settings")]
        [SerializeField, Tooltip("What the camera will focus on")]
        private Transform focus = default;

        [SerializeField, Range(1f, 20f), Tooltip("Orbiting distance of the camera")]
        private float distance = 5f;

        [SerializeField, Min(0f), Tooltip("Only move the camera when its focus point differs from ideal focus by this amount (deadzone)")]
        private float focusRadius = 1f;

        [SerializeField, Range(0f, 1f), Tooltip("Factor at which we center the camera on the focus when its not already centered.")]
        private float focusCentering = 0.5f;

        [SerializeField, Range(-89f, 89f), Tooltip("The min and max that our camera's vertical angles must stay within")]
        private float minVerticalAngle = -30f, maxVerticalAngle = 60f;

        [SerializeField, Min(0f), Tooltip("Time needed to kick in automated alignment. Set very high for no automated alignment")]
        private float alignDelay = 5f;

        [SerializeField, Range(1f, 720f), Tooltip("Automatic alignment turn rate in degrees per second")]
        private float autoAlignSpeed = 180f;
 
        [SerializeField, Range(0f, 90f), Tooltip("Within this many degrees of aligned (or exactly opposite), the turn rate eases off")]
        private float alignSmoothRange = 45f;
 
        [SerializeField, Min(0f), Tooltip("Focus speed (units/sec) below which automatic alignment does nothing")]
        private float minAlignSpeed = 0.5f;
 
        [SerializeField, Min(0.1f), Tooltip("Focus speed (units/sec) at which automatic alignment reaches its full turn rate")]
        private float alignFullSpeed = 3f;
 
        [SerializeField, Min(0.01f), Tooltip("Smoothing time for the measured focus velocity (seconds). Removes noise at very high frame rates")]
        private float velocitySmoothTime = 0.1f;

        [SerializeField, Tooltip("If set in the mask, we will count the layer as an obstruction")]
        private LayerMask obstructionMask = -1;

        [Header("Input")]
        [SerializeField, Range(1f, 360f), Tooltip("Rotation speed when using joystick in degrees per second")]
        private float joyStickRotationSpeed = 90f;

        [SerializeField, Range(0f, 1f), Tooltip("Mouse sensitivity when using mouse")]
        private float mouseSensitivity = 0.2f;
        [SerializeField, Min(1f), Tooltip("Largest mouse delta (pixels) accepted in one frame. Guards against a lag spike flinging the camera")]
        private float maxMouseDelta = 200f;

        [Header("Frame Time")]
        [SerializeField, Range(0.01f, 0.1f), Tooltip("Longest frame time the camera simulates. Longer frames are treated as this long")]
        private float maxStepTime = 0.05f;

        private const float ROTATION_EPSILON = 0.001f;
        private const float MIN_DT = 0.0001f;
        private Vector3 focusPoint, previousFocusPoint;
        private Vector2 smoothedFocusVelocity;
        private Vector2 orbitAngles = new Vector2(45f, 0f); // Vertical and horizontal orientation of the camera

        private float lastManualRotationTime;

        private Camera regularCamera;
        Vector3 CameraHalfExtends 
        {
            get 
            {
                Vector3 halfExtends;
                halfExtends.y = regularCamera.nearClipPlane * Mathf.Tan(0.5f * Mathf.Deg2Rad * regularCamera.fieldOfView);
                halfExtends.x = halfExtends.y * regularCamera.aspect;
                halfExtends.z = 0f;
                return halfExtends;
            }
        }

        void OnValidate()
        {
            if (maxVerticalAngle < minVerticalAngle) {
			    maxVerticalAngle = minVerticalAngle;
		    }
        }

        void Awake()
        {
            regularCamera = GetComponent<Camera>();
            focusPoint = focus.position;
            transform.localRotation = Quaternion.Euler(orbitAngles);
        }

        public void UpdateCamera(Vector2 input, bool mouseBased)
        {
            // Measured quantities use the real frame time, applied steps use the clamped one
            float rawDt = Time.unscaledDeltaTime;
            float dt = Mathf.Min(rawDt, maxStepTime);

            UpdateFocusPoint(rawDt, dt);

            Quaternion lookRotation;
            if (ManualRotation(input, mouseBased, dt) || AutomaticRotation(dt))
            {
                ConstrainAngles();
                lookRotation = Quaternion.Euler(orbitAngles);
            }
            else
            {
                lookRotation = transform.localRotation;
            }
            var lookDirection = lookRotation * Vector3.forward;
            var lookPosition = focusPoint - lookDirection * distance;

            var rectOffset = lookDirection * regularCamera.nearClipPlane;
            var rectPosition = lookPosition + rectOffset;
            var castFrom = focus.position;
            var castLine = rectPosition - castFrom;
            var castDistance = castLine.magnitude;
            var castDirection = castLine / castDistance;
            if (Physics.BoxCast(castFrom, CameraHalfExtends, castDirection, out RaycastHit hit, lookRotation, castDistance, obstructionMask))
            {
                rectPosition = castFrom + castDirection * hit.distance;
			    lookPosition = rectPosition - rectOffset;
            }
            transform.SetPositionAndRotation(lookPosition, lookRotation);
        }

        private void UpdateFocusPoint(float rawDt, float dt)
        {
            previousFocusPoint = focusPoint;
            Vector3 targetPoint = focus.position;

            // deadzone check - measure distance to deadzone and lerp when we're nearing the edge
            if (focusRadius > 0f)
            {
                var focusDistance = Vector3.Distance(targetPoint, focusPoint);
                var t = 1f;
                // If we're not centered, set up t to gradually center
                if (focusDistance > 0.01f && focusCentering > 0f)
                {
                    t = Mathf.Pow(1f - focusCentering, dt);
                }
                // Move us towards the focus radius gradually
                if (focusDistance > focusRadius)
                {
                    t = Mathf.Min(t, focusRadius / focusDistance);
                }
                focusPoint = Vector3.Lerp(targetPoint, focusPoint, t);
            }
            else
            {
                focusPoint = targetPoint;
            }
            // Measure how fast the focus is moving, in units per second.
            // Smoothed with a frame-rate independent filter (1 - e^(-dt/tau)),
            // because dividing a tiny per-frame movement by a tiny dt amplifies noise.
            Vector2 frameMovement = new Vector2(focusPoint.x - previousFocusPoint.x, focusPoint.z - previousFocusPoint.z);
            Vector2 instantVelocity = frameMovement / Mathf.Max(rawDt, MIN_DT);
            float alpha = 1f - Mathf.Exp(-rawDt / velocitySmoothTime);
            smoothedFocusVelocity = Vector2.Lerp(smoothedFocusVelocity, instantVelocity, alpha);
        }

        /// <summary>
        /// Called externally. Typically via the PlayerController,
        /// but can be anywhere
        /// 
        /// returns true if we rotated this frame
        /// </summary>
        /// <param name="input"></param>
        private bool ManualRotation(Vector2 input, bool mouseBased, float dt)
        {
            if (input.x < -ROTATION_EPSILON  ||
                input.x > ROTATION_EPSILON  ||
                input.y < -ROTATION_EPSILON ||
                input.y > ROTATION_EPSILON)
            {
                
                orbitAngles += GetAngleFromInput(input, mouseBased, dt);
                lastManualRotationTime = Time.unscaledTime;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Keeps orbit angles between a vertical min and max
        /// </summary>
        private void ConstrainAngles()
        {
            orbitAngles.x = Mathf.Clamp(orbitAngles.x, minVerticalAngle, maxVerticalAngle);

            if (orbitAngles.y < 0f)
            {
                orbitAngles.y += 360f;
            }
            else if (orbitAngles.y >= 360f)
            {
                orbitAngles.y -= 360f;
            }
        }

        /// <summary>
        /// Applies automatic rotation of the camera behind the focus
        /// after a time
        /// </summary>
        /// <returns></returns>
        private bool AutomaticRotation(float dt)
        {
            if (Time.unscaledTime - lastManualRotationTime < alignDelay)
            {
                return false;
            }

            var focusSpeed = smoothedFocusVelocity.magnitude;
            if (focusSpeed < minAlignSpeed)
            {
                return false;
            }
 
            var headingAngle = GetAngle(smoothedFocusVelocity / focusSpeed);
            var deltaAbs = Mathf.Abs(Mathf.DeltaAngle(orbitAngles.y, headingAngle));
 
            // Degrees per second * seconds, scaled up to full rate as the focus speeds up
            var rotationChange = autoAlignSpeed * dt * Mathf.Clamp01(focusSpeed / alignFullSpeed);
 
            // Ease off when nearly aligned, or nearly opposite (avoids abrupt starts and stops)
            if (deltaAbs < alignSmoothRange)
            {
                rotationChange *= deltaAbs / alignSmoothRange;
            }
            else if (180f - deltaAbs < alignSmoothRange)
            {
                rotationChange *= (180f - deltaAbs) / alignSmoothRange;
            }
 
            orbitAngles.y = Mathf.MoveTowardsAngle(orbitAngles.y, headingAngle, rotationChange);
            return true;
        }

        /// <summary>
        /// Find the horizontal angle matching the given direction
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        private static float GetAngle(Vector2 direction)
        {
            var angle = Mathf.Acos(Mathf.Clamp(direction.y, -1f, 1f)) * Mathf.Rad2Deg;
            return direction.x < 0f ? 360 - angle : angle;
        }

        private Vector2 GetAngleFromInput(Vector2 input, bool mouseBased,  float dt)
        {
            if (mouseBased)
            {
                input = Vector2.ClampMagnitude(input, maxMouseDelta);
            }
 
            Vector2 correctedInput = new Vector2(-input.y, input.x);
            return mouseBased ? correctedInput * mouseSensitivity : correctedInput * (joyStickRotationSpeed * dt);
        }

        public void SnapToFocus()
        {
            
        }
    }
}