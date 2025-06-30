/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Physics;

using OpenTK.Mathematics;

/// <summary>
/// Represents a collision plane aligned with the XZ axes at a specific Y level.
/// If you put it in above you (plane's YPosition is greater than your position), you are inside.
/// </summary>
public class TopDownAxisAlignedCollisionPlane : ICollisionObject
{
    public Vector3 Position { get; set; }

    /// <summary>
    /// The Y position of the plane.
    /// </summary>
    public float YPosition
    {
        get => Position.Y;
        set => Position = new Vector3(0, value, 0);
    }

    /// <summary>
    /// The normal vector of the plane (always aligned with the Y axis).
    /// </summary>
    public Vector3 Normal => -Vector3.UnitY;

    /// <summary>
    /// Constructs a new instance of the TopDownAxisAlignedCollisionPlane class.
    /// </summary>
    /// <param name="yPosition">The Y position of the plane.</param>
    private TopDownAxisAlignedCollisionPlane(float yPosition)
    {
        YPosition = yPosition;
    }
    
    
    /// <summary>
    /// Creates a new instance of the TopDownAxisAlignedCollisionPlane at the specified Y position.
    /// </summary>
    /// <param name="yPosition">The Y position of the plane.</param>
    /// <returns>A new instance of TopDownAxisAlignedCollisionPlane.</returns>
    public static TopDownAxisAlignedCollisionPlane CreateAtY(float yPosition)
    {
        return new TopDownAxisAlignedCollisionPlane(yPosition);
    }

    
    public bool CheckCollision(ICollisionObject other)
    {
        return other switch
        {
            CollisionSphere sphere => sphere.Position.Y > YPosition - sphere.Radius,
            AxisAlignedCollisionBox box => box.Max.Y > YPosition,

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
        => point.Y <= YPosition;
}
