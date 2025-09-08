/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.PhysicsNg;

using OpenTK.Mathematics;

/// <summary>
/// Defines an interface for collision objects in the physics engine.
/// </summary>
public interface ICollisionObject
{
    /// <summary>
    /// A position of the collision object in the physics world.
    /// </summary>
    Vector3 Position { get; set; }
    
    /// <summary>
    /// Checks if this collision object collides with another collision object.
    /// </summary>
    /// <param name="other">A collision object to check for collision with.</param>
    /// <returns>True if a collision occurs, otherwise false.</returns>
    bool CheckCollision(ICollisionObject other);

    /// <summary>
    /// Checks if a point is inside this collision object.
    /// </summary>
    /// <param name="point">A point in the physics world to check.</param>
    /// <returns>True if the point is inside the collision object, otherwise false.</returns>
    bool IsPointInside(Vector3 point);
}
