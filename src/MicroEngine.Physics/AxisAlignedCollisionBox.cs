/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Physics;

using OpenTK.Mathematics;

/// <summary>
/// Defines an axis-aligned bounding box (AABB) for collision detection.
/// </summary>
public class AxisAlignedCollisionBox : ICollisionObject
{
    /// <summary>
    /// Gets the calculated size of the bounding box.
    /// </summary>
    public Vector3 Size => Max - Min;
    
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
            // We assume that the box is centered around the new position.
            var halfSizeX = (_max.X - _min.X) / 2.0f;
            var halfSizeY = (_max.Y - _min.Y) / 2.0f;
            var halfSizeZ = (_max.Z - _min.Z) / 2.0f;
            var min = new Vector3(value.X - halfSizeX, value.Y - halfSizeY, value.Z - halfSizeZ);
            var max = new Vector3(value.X + halfSizeX, value.Y + halfSizeY, value.Z + halfSizeZ);
            
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
    private AxisAlignedCollisionBox(Vector3 min, Vector3 max)
    {
        _min = min;
        _max = max;

        UpdateCornersAndPosition();
    }

    
    /// <summary>
    /// Creates a new AxisAlignedCollisionBox from minimum and maximum points.
    /// </summary>
    /// <param name="min">A point that represents the minimum corner of the box.</param>
    /// <param name="max">A point that represents the maximum corner of the box.</param>
    /// <returns>A new instance of AxisAlignedCollisionBox.</returns>
    /// <exception cref="ArgumentException">Thrown when Min is not less than Max in any dimension.</exception>
    public static AxisAlignedCollisionBox Create(Vector3 min, Vector3 max)
    {
        if (min.X >= max.X || min.Y >= max.Y || min.Z >= max.Z)
        {
            throw new ArgumentException("Min must be less than Max in all dimensions.", nameof(min));
        }
        
        return new AxisAlignedCollisionBox(min, max);
    }

    /// <summary>
    /// Creates a new AxisAlignedCollisionBox from a position and size.
    /// </summary>
    /// <param name="position">A position that represents the center of the box.</param>
    /// <param name="size">A size that represents the dimensions of the box.</param>
    /// <returns>A new instance of AxisAlignedCollisionBox.</returns>
    /// <exception cref="ArgumentException">Thrown when size is not greater than zero in any dimension.</exception>
    public static AxisAlignedCollisionBox CreateFromPositionAndSize(Vector3 position, Vector3 size)
    {
        if (size.X <= 0 || size.Y <= 0 || size.Z <= 0)
        {
            throw new ArgumentException("Size must be greater than zero in all dimensions.", nameof(size));
        }
        
        return new AxisAlignedCollisionBox(
            position - size / 2.0f, // Calculate Min
            position + size / 2.0f  // Calculate Max
        );
    }

    /// <summary>
    /// Creates a new AxisAlignedCollisionBox from a position and a uniform size.
    /// </summary>
    /// <param name="position">A position that represents the center of the box.</param>
    /// <param name="size">A uniform size that represents the dimensions of the box.</param>
    /// <returns>A new instance of AxisAlignedCollisionBox.</returns>
    /// <exception cref="ArgumentException">Thrown when size is not greater than zero.</exception>
    public static AxisAlignedCollisionBox CreateFromPositionAndSize(Vector3 position, float size)
    {
        if (size <= 0)
        {
            throw new ArgumentException("Size must be greater than zero.", nameof(size));
        }
        
        return CreateFromPositionAndSize(position, new Vector3(size, size, size));
    }
    
    
    public bool CheckCollision(ICollisionObject other)
    {
        return other switch
        {
            AxisAlignedCollisionBox box => CheckCollisionWithBox(box),
            CollisionSphere sphere => sphere.CheckCollision(this),
            BackFrontAxisAlignedCollisionPlane bfPlane => bfPlane.CheckCollision(this),
            FrontBackAxisAlignedCollisionPlane fbPlane => fbPlane.CheckCollision(this),
            LeftRightAxisAlignedCollisionPlane lrPlane => lrPlane.CheckCollision(this),
            RightLeftAxisAlignedCollisionPlane rlPlane => rlPlane.CheckCollision(this),
            TopDownAxisAlignedCollisionPlane tdPlane => tdPlane.CheckCollision(this),
            BottomUpAxisAlignedCollisionPlane buPlane => buPlane.CheckCollision(this),
            CollisionPlane plane => CheckCollisionWithPlane(plane),
            
            _ => false
        };
    }
    
    
    public bool IsPointInside(Vector3 point)
    {
        return point.X >= Min.X && point.X <= Max.X &&
               point.Y >= Min.Y && point.Y <= Max.Y &&
               point.Z >= Min.Z && point.Z <= Max.Z;
    }
    
    
    // public bool IsInside(ICollisionObject other)
    // {
    //     return other switch
    //     {
    //         AxisAlignedCollisionBox box => IsPointInside(box.Min) && IsPointInside(box.Max),
    //         CollisionSphere sphere => 
    //             Vector3.DistanceSquared(Position, sphere.Position) <= sphere.RadiusSquared &&
    //             IsPointInside(sphere.Position + new Vector3(sphere.Radius, sphere.Radius, sphere.Radius)) &&
    //             IsPointInside(sphere.Position - new Vector3(sphere.Radius, sphere.Radius, sphere.Radius)),
    //         
    //         // We cannot check if this box is inside a plane, as planes are infinite in one direction.
    //         _ => false
    //     };
    // }

    
    private bool CheckCollisionWithPlane(CollisionPlane plane)
    {
        UpdateCorners();
        
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
        _needsCornersUpdate = true;
    }
    
    
    private bool _needsCornersUpdate = true;
    
    private void UpdateCorners()
    {
        if (!_needsCornersUpdate)
        {
            return;
        }

        _corners[0] = new Vector3(Min.X, Min.Y, Min.Z);
        _corners[1] = new Vector3(Min.X, Min.Y, Max.Z);
        _corners[2] = new Vector3(Min.X, Max.Y, Min.Z);
        _corners[3] = new Vector3(Min.X, Max.Y, Max.Z);
        _corners[4] = new Vector3(Max.X, Min.Y, Min.Z);
        _corners[5] = new Vector3(Max.X, Min.Y, Max.Z);
        _corners[6] = new Vector3(Max.X, Max.Y, Min.Z);
        _corners[7] = new Vector3(Max.X, Max.Y, Max.Z);
        
        _needsCornersUpdate = false;
    }
    
    
    private bool CheckCollisionWithBox(AxisAlignedCollisionBox box)
    {
        return Min.X <= box.Max.X && Max.X >= box.Min.X &&
               Min.Y <= box.Max.Y && Max.Y >= box.Min.Y &&
               Min.Z <= box.Max.Z && Max.Z >= box.Min.Z;
    }


    
    // public void Translate(Vector3 translation)
    // {
    //     Min += translation;
    //     Max += translation;
    //     _position += translation;
    //
    //     // Update corners after translation
    //     UpdateCornersAndPosition();
    // }
    
    
    // public void Scale(Vector3 scale)
    // {
    //     if (scale.X <= 0 || scale.Y <= 0 || scale.Z <= 0)
    //     {
    //         throw new ArgumentException("Scale must be greater than zero in all dimensions.", nameof(scale));
    //     }
    //
    //     var center = Position;
    //     var halfSize = Size / 2.0f;
    //
    //     Min = center - halfSize * scale;
    //     Max = center + halfSize * scale;
    //
    //     // Update corners after scaling
    //     UpdateCornersAndPosition();
    // }
    
    
    // public void Scale(float scale)
    // {
    //     if (scale <= 0)
    //     {
    //         throw new ArgumentException("Scale must be greater than zero.", nameof(scale));
    //     }
    //
    //     Scale(new Vector3(scale, scale, scale));
    // }
}
