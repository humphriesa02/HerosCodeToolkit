/// <summary>
/// Base StateMachine State
/// </summary>
public class State
{
    /// <summary>
    /// Called upon entering a state, once.
    /// </summary>
    public virtual void Enter() { }

    /// <summary>
    /// Called every frame. Equivalent technically
    /// to a logic update, but dedicated to polling input
    /// from the input actions
    /// </summary>
    public virtual void HandleInput() { }

    /// <summary>
    /// Equivalent to a single Update() call.
    /// Called once every frame. Should not handle physics logic
    /// </summary>
    public virtual void LogicUpdate() { }

    /// <summary>
    /// Equivalent to a single LateUpdate() call.
    /// Called once every frame, at the end.
    /// </summary>
    public virtual void LateUpdate() { }

    /// <summary>
    /// Equivalent to a FixedUpdate.
    /// Called once every frame, should handle physics related things.
    /// </summary>
    public virtual void PhysicsUpdate() { }

    /// <summary>
    /// Called upon leaving a state, once.
    /// </summary>
    public virtual void Exit() { }
}
