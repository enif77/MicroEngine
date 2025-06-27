/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Physics;

using OpenTK.Mathematics;

/// <summary>
/// Defines a sphere for collision detection in the physics engine.
/// </summary>
public class CollisionSphere : ICollisionObject
{
    /// <summary>
    /// Position of the sphere in the physics world.
    /// </summary>
    public Vector3 Position { get; set; }
    
    /// <summary>
    /// Radius of the sphere. Should be a positive value.
    /// </summary>
    public float Radius { get; set; }

    
    /// <summary>
    /// Constructs a new instance of the <see cref="CollisionSphere"/> class.
    /// </summary>
    /// <param name="position">A position of the sphere in the physics world.</param>
    /// <param name="radius">A radius of the sphere.</param>
    public CollisionSphere(Vector3 position, float radius)
    {
        Position = position;
        Radius = radius;
    }
    
    
    public bool CheckCollision(ICollisionObject other)
    {
        return other switch
        {
            CollisionSphere sphere => Vector3.Distance(Position, sphere.Position) <= (Radius + sphere.Radius),
            AxisAlignedCollisionBox box => CheckCollisionWithBox(box),
            XzAxisAlignedCollisionPlane xzPlane => xzPlane.CheckCollision(this),
            YzAxisAlignedCollisionPlane xzPlane => xzPlane.CheckCollision(this),
            CollisionPlane plane => Math.Abs(Vector3.Dot(plane.Normal, Position - plane.Position)) <= Radius,
            
            // Planes do not collide with each other.
            _ => false
        };
    }

    
    private bool CheckCollisionWithBox(AxisAlignedCollisionBox box)
    {
        var closestPoint = Vector3.Clamp(Position, box.Min, box.Max);
        var distance = Vector3.Distance(Position, closestPoint);
        
        return distance <= Radius;
    }
}
