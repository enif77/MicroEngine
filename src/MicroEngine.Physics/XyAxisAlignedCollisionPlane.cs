/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Physics;

using OpenTK.Mathematics;

/// <summary>
/// Represents a collision plane aligned with the XY axes at a specific Z level.
/// </summary>
public class XyAxisAlignedCollisionPlane : ICollisionObject
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
    public Vector3 Normal { get; }

    /// <summary>
    /// Constructs a new instance of the XyAxisAlignedCollisionPlane class.
    /// </summary>
    /// <param name="zPosition">The Z position of the plane.</param>
    /// <param name="normalDirectionForward">If true, the normal points forward; otherwise, it points backward.</param>
    public XyAxisAlignedCollisionPlane(float zPosition, bool normalDirectionForward)
    {
        ZPosition = zPosition;
        Normal = normalDirectionForward ? Vector3.UnitZ : -Vector3.UnitZ;
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
        // Check if the sphere is in front of or behind the plane based on its center and radius.
        var distance = sphere.Position.Z - ZPosition;

        return Normal.Z > 0
            ? distance <= sphere.Radius
            : distance >= -sphere.Radius;
    }

    
    private bool CheckBoxCollision(AxisAlignedCollisionBox box)
    {
        // Check if the box is in front of or behind the plane based on its Min and Max Z values.
        return Normal.Z > 0
            ? box.Min.Z <= ZPosition
            : box.Max.Z >= ZPosition;
    }
}
