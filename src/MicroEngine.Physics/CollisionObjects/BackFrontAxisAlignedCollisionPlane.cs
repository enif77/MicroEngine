/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Physics.CollisionObjects;

using OpenTK.Mathematics;

/// <summary>
/// Represents a collision plane aligned with the XY axes at a specific Z level.
/// If you put it in behind of you (plane's ZPosition is greater than your position), you are inside.
/// </summary>
public class BackFrontAxisAlignedCollisionPlane : ICollisionObject
{
    public Vector3 Position { get; set; }

    /// <summary>
    /// The Z position of the plane.
    /// </summary>
    public float ZPosition
    {
        get => Position.Z;
        set => Position = new Vector3(0, 0, value);
    }

    /// <summary>
    /// The normal vector of the plane (always aligned with the Z axis).
    /// </summary>
    public Vector3 Normal => -Vector3.UnitZ;

    /// <summary>
    /// Constructs a new instance of the BackFrontAxisAlignedCollisionPlane class.
    /// </summary>
    /// <param name="zPosition">The Z position of the plane.</param>
    private BackFrontAxisAlignedCollisionPlane(float zPosition)
    {
        ZPosition = zPosition;
    }
    
    
    /// <summary>
    /// Creates a new instance of the BackFrontAxisAlignedCollisionPlane at the specified Z position.
    /// </summary>
    /// <param name="zPosition">The Z position of the plane.</param>
    /// <returns>A new instance of BackFrontAxisAlignedCollisionPlane.</returns>
    public static BackFrontAxisAlignedCollisionPlane CreateAtZ(float zPosition)
    {
        return new BackFrontAxisAlignedCollisionPlane(zPosition);
    }

    
    public bool CheckCollision(ICollisionObject other)
    {
        return other switch
        {
            CollisionSphere sphere => sphere.Position.Z > ZPosition - sphere.Radius,
            AxisAlignedCollisionBox box => box.Max.Z > ZPosition,

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
        => point.Z <= ZPosition;
}
