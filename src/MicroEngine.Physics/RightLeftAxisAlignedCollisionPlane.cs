/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Physics;

using OpenTK.Mathematics;

/// <summary>
/// Represents a collision plane aligned with the YZ axes at a specific X level.
/// If you put it to the right of you (plane's XPosition is greater than your position), you are inside.
/// </summary>
public class RightLeftAxisAlignedCollisionPlane : ICollisionObject
{
    public Vector3 Position { get; set; }

    /// <summary>
    /// The X position of the plane.
    /// </summary>
    public float XPosition
    {
        get => Position.X;
        set => Position = new Vector3(value, 0, 0);
    }

    /// <summary>
    /// The normal vector of the plane (always aligned with the X axis).
    /// </summary>
    public Vector3 Normal => -Vector3.UnitX;

    /// <summary>
    /// Constructs a new instance of the RightLeftAxisAlignedCollisionPlane class.
    /// </summary>
    /// <param name="xPosition">The X position of the plane.</param>
    private RightLeftAxisAlignedCollisionPlane(float xPosition)
    {
        XPosition = xPosition;
    }
    
    
    /// <summary>
    /// Creates a new instance of the RightLeftAxisAlignedCollisionPlane at the specified X position.
    /// </summary>
    /// <param name="xPosition">The X position of the plane.</param>
    /// <returns>A new instance of RightLeftAxisAlignedCollisionPlane.</returns>
    public static RightLeftAxisAlignedCollisionPlane CreateAtX(float xPosition)
    {
        return new RightLeftAxisAlignedCollisionPlane(xPosition);
    }

    
    public bool CheckCollision(ICollisionObject other)
    {
        return other switch
        {
            CollisionSphere sphere => sphere.Position.X > XPosition - sphere.Radius,
            AxisAlignedCollisionBox box => box.Max.X > XPosition,

            // Planes do not collide with each other.
            _ => false
        };
    }
    
    /// <summary>
    /// Checks if a point is inside the plane. Inside is defined as being in front of the plane.
    /// </summary>
    /// <param name="point">A point in the physics world to check.</param>
    /// <returns>True if the point is inside the plane, otherwise false.</returns>
    public bool IsPointInside(Vector3 point)
        => point.X <= XPosition;
}
