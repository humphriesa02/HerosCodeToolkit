using UnityEngine;

/// <summary>
/// Values to be loaded into <see cref="PlayerController"/>
/// 
/// Dictates both how the Player controls, and the values
/// that go with it.
/// </summary>
[CreateAssetMenu(fileName = "PlayerValue", menuName = "PlayerValues/CreateNewPlayerValues", order = 1)]
public class PlayerValues : ScriptableObject
{
    [Tooltip("Base max move speed of the player")]
    public float moveSpeed = 6.0f;
    [Tooltip("Base max jump height of the player")]
    public float jumpHeight = 0.8f;
    [Tooltip("Base max roll speed of the player")]
    public float rollSpeed = 10.0f;
    [Tooltip("Gravity multiplier, increase or decrease to affect gravity")]
    public float gravityMultiplier = 1f;
    [Range(0, 1), Tooltip("Animation speed damp time.")]
    public float speedDampTime = 0.1f;
    [Range(0, 1), Tooltip("The rate at which our velocity falls off. Increase for slidey movement.")]
    public float velocityDampTime = 0.9f;
    [Range(0, 1), Tooltip("The rate at which we fully rotate the player.")]
    public float rotationDampTime = 0.2f;
    [Range(0, 1), Tooltip("The amount of control we have over the player in the air.")]
    public float airControl = 0.5f;
    [Tooltip("The amount of time we stay 'landed'. After this time ends we can move again")]
    public float landingTime = 0.5f;
}
