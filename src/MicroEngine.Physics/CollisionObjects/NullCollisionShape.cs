/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Physics.CollisionObjects;

using OpenTK.Mathematics;

/// <summary>
/// A collision object that never collides with anything.
/// This is used to represent a null or empty collision shape.
/// It can be used as a placeholder or when no collision detection is needed.
/// It has no physical presence in the physics world and does not affect any other objects.
/// It is useful for cases where a collision object is required, but no actual collision shape is needed.
/// </summary>
public class NullCollisionShape : ICollisionObject
{
    /// <summary>
    /// Position in the physics world.
    /// </summary>
    public Vector3 Position { get; set; }

    
    /// <summary>
    /// Constructs a new instance of the <see cref="NullCollisionShape"/> class.
    /// </summary>
    /// <param name="position"></param>
    public NullCollisionShape(Vector3 position)
    {
        Position = position;
    }
    
    
    /// <summary>
    /// Returns false, as this collision shape does not collide with anything.
    /// </summary>
    /// <param name="other">The other collision object to check for collision.</param>
    /// <returns>False, as this collision shape does not collide with anything.</returns>
    public bool CheckCollision(ICollisionObject other)
    {
        return false;
    }
    
    /// <summary>
    /// Returns false, as this collision shape does not collide with anything.
    /// </summary>
    /// <param name="point">A point in the physics world to check if it is inside this collision shape.</param>
    /// <returns>False, as this collision shape does not contain any points.</returns>
    public bool IsPointInside(Vector3 point)
    {
        return false;
    }
}
