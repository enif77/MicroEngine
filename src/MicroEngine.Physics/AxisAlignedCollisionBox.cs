/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Physics;

using OpenTK.Mathematics;

/// <summary>
/// Defines an axis-aligned bounding box (AABB) for collision detection.
/// </summary>
public class AxisAlignedCollisionBox : ICollisionObject
{
    private Vector3 _min;

    /// <summary>
    /// The minimum point of the bounding box.
    /// </summary>
    public Vector3 Min
    {
        get => _min;
        set
        {
            _min = value;
            
            UpdateCornersAndPosition();
        }
    }
    
    private Vector3 _max;

    /// <summary>
    /// The maximum point of the bounding box.
    /// </summary>
    public Vector3 Max
    {
        get => _max;
        set
        {
            _max = value;
            
            UpdateCornersAndPosition();
        }
    }

    private Vector3 _position = Vector3.Zero;

    /// <summary>
    /// The center position of the bounding box.
    /// Updates automatically when Min or Max is set.
    /// Updates the Min and Max based on the new position.
    /// </summary>
    public Vector3 Position
    {
        get => _position;
        set
        {
            // Position will be calculated as the average of Min and Max. See the UpdateCornersAndPosition() method.
            
            // Update Min and Max based on the new position
            var min = new Vector3(value.X - (_max.X - _min.X) / 2, value.Y - (_max.Y - _min.Y) / 2, value.Z - (_max.Z - _min.Z) / 2);
            var max = new Vector3(value.X + (_max.X - _min.X) / 2, value.Y + (_max.Y - _min.Y) / 2, value.Z + (_max.Z - _min.Z) / 2);
            
            _min = min;
            _max = max;
            
            UpdateCornersAndPosition();
        }
    }
    
    /// <summary>
    /// Constructs a new instance of the CollisionAxisAlignedBox with specified minimum and maximum points.
    /// </summary>
    /// <param name="min">The minimum point of the bounding box.</param>
    /// <param name="max">The maximum point of the bounding box.</param>
    public AxisAlignedCollisionBox(Vector3 min, Vector3 max)
    {
        _min = min;
        _max = max;

        UpdateCornersAndPosition();
    }
    

    public bool CheckCollision(ICollisionObject other)
    {
        return other switch
        {
            CollisionSphere sphere => sphere.CheckCollision(this),
            AxisAlignedCollisionBox box => CheckCollisionWithBox(box),
            XyAxisAlignedCollisionPlane xyPlane => xyPlane.CheckCollision(this),
            XzAxisAlignedCollisionPlane xzPlane => xzPlane.CheckCollision(this),
            YzAxisAlignedCollisionPlane yzPlane => yzPlane.CheckCollision(this),
            CollisionPlane plane => CheckCollisionWithPlane(plane),
            _ => false
        };
    }

    
    private bool CheckCollisionWithPlane(CollisionPlane plane)
    {
        // Check, if any corner of the box is on the positive side of the plane.
        foreach (var corner in _corners)
        {
            if (plane.GetDistanceToPoint(corner) > 0)
            {
                return true; // At least one corner is on the positive side of the plane
            }
        }
        
        // If all corners are on the negative side, there is no collision
        return false;
    }


    private readonly Vector3[] _corners = new Vector3[8];
    
    
    private void UpdateCornersAndPosition()
    {
        _position = (Min + Max) / 2;
        
        _corners[0] = new Vector3(Min.X, Min.Y, Min.Z);
        _corners[1] = new Vector3(Min.X, Min.Y, Max.Z);
        _corners[2] = new Vector3(Min.X, Max.Y, Min.Z);
        _corners[3] = new Vector3(Min.X, Max.Y, Max.Z);
        _corners[4] = new Vector3(Max.X, Min.Y, Min.Z);
        _corners[5] = new Vector3(Max.X, Min.Y, Max.Z);
        _corners[6] = new Vector3(Max.X, Max.Y, Min.Z);
        _corners[7] = new Vector3(Max.X, Max.Y, Max.Z);
    }
    
    
    private bool CheckCollisionWithBox(AxisAlignedCollisionBox box)
    {
        return Min.X <= box.Max.X && Max.X >= box.Min.X &&
               Min.Y <= box.Max.Y && Max.Y >= box.Min.Y &&
               Min.Z <= box.Max.Z && Max.Z >= box.Min.Z;
    }
}
