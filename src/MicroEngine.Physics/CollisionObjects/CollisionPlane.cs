/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Physics.CollisionObjects;

using OpenTK.Mathematics;

/// <summary>
/// Describes a plane for collision detection in the physics engine.
/// </summary>
public class CollisionPlane : ICollisionObject
{
    /// <summary>
    /// A position on the plane, used for collision detection.
    /// </summary>
    public Vector3 Position { get; set; }
    
    /// <summary>
    /// A normal vector defining the orientation of the plane.
    /// Make sure this vector is normalized (length of 1) for accurate collision detection.
    /// </summary>
    public Vector3 Normal { get; set; }

    
    /// <summary>
    /// Constructs a new instance of the CollisionPlane class.
    /// </summary>
    /// <param name="position">A point on the plane.</param>
    /// <param name="normal">A vector normal to the plane.</param>
    public CollisionPlane(Vector3 position, Vector3 normal)
    {
        Position = position;
        Normal = normal.Normalized();
    }
    
    
    public bool CheckCollision(ICollisionObject other)
    {
        return other switch
        {
            CollisionSphere sphere => sphere.CheckCollision(this),
            AxisAlignedCollisionBox box => box.CheckCollision(this),
            
            // Planes do not collide with each other.
            _ => false
        };
    }
    
    /// <summary>
    /// Checks if a point is inside the plane. Inside is defined as being above or below the plane based on the normal vector.
    /// </summary>
    /// <param name="point">A point in the physics world to check.</param>
    /// <returns>True if the point is inside the plane, otherwise false.</returns>
    public bool IsPointInside(Vector3 point)
        => Vector3.Dot(Normal, point - Position) >= 0;

    
    public float GetDistanceToPoint(Vector3 corner)
    {
        // Calculate the distance from the point to the plane using the plane equation.
        // The equation of a plane is: Ax + By + Cz + D = 0
        // Where (A, B, C) is the normal vector and D can be calculated as -dot(Normal, Position).
        
        // The distance from a point (x, y, z) to the plane is given by:
        var d = -Vector3.Dot(Normal, Position);
        
        // The distance from the point to the plane is:
        return (Vector3.Dot(Normal, corner) + d) / Normal.Length;
    }
}
