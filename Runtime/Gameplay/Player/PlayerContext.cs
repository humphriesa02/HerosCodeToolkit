using UnityEngine;

public class PlayerContext
{
    /// Input
    public Vector2 moveInput;
    public bool isPrimaryPressed;
    public bool isSecondaryPressed;

    public Vector3 gravityVelocity;
    public Vector3 velocity;

    /// World
    public bool nearbyWall;
    public bool insideWater;
    public float waterDepth;
}
