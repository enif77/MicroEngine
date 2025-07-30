/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Physics.CollisionObjects;

using OpenTK.Mathematics;

/// <summary>
/// Represents a collision plane aligned with the XY axes at a specific Z level.
/// If you put it in front of you (plane's ZPosition is less than your position), you are inside.
/// </summary>
public class FrontBackAxisAlignedCollisionPlane : ICollisionObject
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
    public Vector3 Normal => Vector3.UnitZ;

    /// <summary>
    /// Constructs a new instance of the FrontBackAxisAlignedCollisionPlane class.
    /// </summary>
    /// <param name="zPosition">The Z position of the plane.</param>
    private FrontBackAxisAlignedCollisionPlane(float zPosition)
    {
        ZPosition = zPosition;
    }
    
    
    /// <summary>
    /// Creates a new instance of the FrontBackAxisAlignedCollisionPlane at the specified Z position.
    /// </summary>
    /// <param name="zPosition">The Z position of the plane.</param>
    /// <returns>A new instance of FrontBackAxisAlignedCollisionPlane.</returns>
    public static FrontBackAxisAlignedCollisionPlane CreateAtZ(float zPosition)
    {
        return new FrontBackAxisAlignedCollisionPlane(zPosition);
    }

    
    public bool CheckCollision(ICollisionObject other)
    {
        return other switch
        {
            CollisionSphere sphere => sphere.Position.Z < ZPosition + sphere.Radius,
            AxisAlignedCollisionBox box => box.Min.Z < ZPosition,

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
        => point.Z >= ZPosition;
}
