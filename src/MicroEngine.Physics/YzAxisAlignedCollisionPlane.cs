/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Physics;

using OpenTK.Mathematics;

/// <summary>
/// Represents a collision plane aligned with the YZ axes at a specific X level.
/// </summary>
public class YzAxisAlignedCollisionPlane : ICollisionObject
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
    public Vector3 Normal { get; }

    /// <summary>
    /// Constructs a new instance of the YzAxisAlignedCollisionPlane class.
    /// </summary>
    /// <param name="xPosition">The X position of the plane.</param>
    /// <param name="normalDirectionRight">If true, the normal points right; otherwise, it points left.</param>
    public YzAxisAlignedCollisionPlane(float xPosition, bool normalDirectionRight)
    {
        XPosition = xPosition;
        Normal = normalDirectionRight ? Vector3.UnitX : -Vector3.UnitX;
    }

    public bool CheckCollision(ICollisionObject other)
    {
        return other switch
        {
            CollisionSphere sphere => CheckSphereCollision(sphere),
            AxisAlignedCollisionBox box => CheckBoxCollision(box),

            // Planes do not collide with each other.
            _ => false
        };
    }

    private bool CheckSphereCollision(CollisionSphere sphere)
    {
        // Check if the sphere is to the left or right of the plane based on its center and radius.
        var distance = sphere.Position.X - XPosition;

        return Normal.X > 0
            ? distance <= sphere.Radius
            : distance >= -sphere.Radius;
    }

    private bool CheckBoxCollision(AxisAlignedCollisionBox box)
    {
        // Check if the box is to the left or right of the plane based on its Min and Max X values.
        return Normal.X > 0
            ? box.Min.X <= XPosition
            : box.Max.X >= XPosition;
    }
}
