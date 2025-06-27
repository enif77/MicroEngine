/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Physics;

using OpenTK.Mathematics;

/// <summary>
/// Represents a collision plane aligned with the XZ axes at a specific Y level.
/// </summary>
public class XzAxisAlignedCollisionPlane : ICollisionObject
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
    public Vector3 Normal { get; }

    /// <summary>
    /// Constructs a new instance of the XZAxisAlignedCollisionPlane class.
    /// </summary>
    /// <param name="yPosition">The Y position of the plane.</param>
    /// <param name="normalDirectionUp">If true, the normal points up; otherwise, it points down.</param>
    public XzAxisAlignedCollisionPlane(float yPosition, bool normalDirectionUp)
    {
        YPosition = yPosition;
        Normal = normalDirectionUp ? Vector3.UnitY : -Vector3.UnitY;
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
        // Check if the sphere is above or below the plane based on its center and radius.
        var distance = sphere.Position.Y - YPosition;
        
        return Normal.Y > 0
            ? distance <= sphere.Radius
            : distance >= -sphere.Radius;
    }

    
    private bool CheckBoxCollision(AxisAlignedCollisionBox box)
    {
        // Check if the box is above or below the plane based on its Min and Max Y values.
        return Normal.Y > 0 
            ? box.Min.Y <= YPosition
            : box.Max.Y >= YPosition;
    }
}
