/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Physics.CollisionObjects;

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
    /// Returns the squared radius of the sphere.
    /// </summary>
    public float RadiusSquared => Radius * Radius;

    
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
            AxisAlignedCollisionBox box => CheckCollisionWithBox(box),
            CollisionSphere sphere => Vector3.DistanceSquared(Position, sphere.Position) <= (RadiusSquared + sphere.RadiusSquared),
            BackFrontAxisAlignedCollisionPlane bfPlane => bfPlane.CheckCollision(this),
            FrontBackAxisAlignedCollisionPlane fbPlane => fbPlane.CheckCollision(this),
            LeftRightAxisAlignedCollisionPlane lrPlane => lrPlane.CheckCollision(this),
            RightLeftAxisAlignedCollisionPlane rlPlane => rlPlane.CheckCollision(this),
            TopDownAxisAlignedCollisionPlane tdPlane => tdPlane.CheckCollision(this),
            BottomUpAxisAlignedCollisionPlane buPlane => buPlane.CheckCollision(this),
            CollisionPlane plane => Math.Abs(Vector3.Dot(plane.Normal, Position - plane.Position)) <= Radius,
            
            _ => false
        };
    }
    
    
    public bool IsPointInside(Vector3 point)
    {
        return Vector3.DistanceSquared(Position, point) <= RadiusSquared;
    }
    
    
    private bool CheckCollisionWithBox(AxisAlignedCollisionBox box)
    {
        // Find the closest point on the box to the sphere's center
        var closestPoint = Vector3.Clamp(Position, box.Min, box.Max);

        // Calculate the distance from the closest point to the sphere's center
        var distanceSquared = (closestPoint - Position).LengthSquared;

        // Check if the distance is less than or equal to the sphere's radius squared
        return distanceSquared <= RadiusSquared;
    }
}
