using UnityEngine;

[RequireComponent(typeof(Camera))]
public class OrbitCamera : MonoBehaviour
{
    [SerializeField, Tooltip("What the camera will focus on")]
    private Transform focus = default;

    [SerializeField, Range(1f, 20f), Tooltip("Orbiting distance of the camera")]
    private float distance = 5f;

    [SerializeField, Min(0f), Tooltip("Only move the camera when its focus point differs from ideal focus by this amount (deadzone)")]
    private float focusRadius = 1f;

    [SerializeField, Range(0f, 1f), Tooltip("Factor at which we center the camera on the focus when its not already centered.")]
    private float focusCentering = 0.5f;

    Vector3 focusPoint;

    void Awake()
    {
        focusPoint = focus.position;
    }

    void LateUpdate()
    {
        UpdateFocusPoint();
        var lookDirection = transform.forward;
        transform.localPosition = focusPoint - lookDirection * distance;
    }

    private void UpdateFocusPoint()
    {
        Vector3 targetPoint = focus.position;
        // deadzone check - measure distance to deadzone and lerp when we're nearing the edge
        if (focusRadius > 0f)
        {
            var distance = Vector3.Distance(targetPoint, focusPoint);
            var t = 1f;
            // If we're not centered, set up t to gradually center
            if (distance > 0.01f && focusCentering > 0f)
            {
                t = Mathf.Pow(1f - focusCentering, Time.deltaTime);
            }
            // Move us towards the focus radius gradually
            if (distance > focusRadius)
            {
                t = Mathf.Min(t, focusRadius / distance);
            }
            focusPoint = Vector3.Lerp(targetPoint, focusPoint, t);
        }
        else
        {
            focusPoint = targetPoint;
        }
    }
}
