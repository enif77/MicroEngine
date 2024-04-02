/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Extensions;

using OpenTK.Mathematics;

/// <summary>
/// Extensions for the IAxisAlignedBoundaryBox interface.
/// </summary>
public static class AxisAlignedBoundaryBoxExtensions
{
    /// <summary>
    /// Returns true, if the given point is inside of this box.
    /// </summary>
    /// <param name="box">An AABB box.</param>
    /// <param name="point">A 3D point.</param>
    /// <returns>True, if the given point is inside this box.</returns>
    public static bool IsPointInside(this IAxisAlignedBoundaryBox box, Vector3 point) =>
        point.X >= box.Min.X &&
        point.X <= box.Max.X &&
        point.Y >= box.Min.Y &&
        point.Y <= box.Max.Y &&
        point.Z >= box.Min.Z &&
        point.Z <= box.Max.Z;
    
    /// <summary>
    /// Returns true, if the given box is inside of this box.
    /// </summary>
    /// <param name="box">An AABB box.</param>
    /// <param name="otherBox">Another AABB box.</param>
    /// <returns>True, if the given box is inside of this box.</returns>
    public static bool IsBoxInside(this IAxisAlignedBoundaryBox box, IAxisAlignedBoundaryBox otherBox) =>
        otherBox.Min.X >= box.Min.X &&
        otherBox.Max.X <= box.Max.X &&
        otherBox.Min.Y >= box.Min.Y &&
        otherBox.Max.Y <= box.Max.Y &&
        otherBox.Min.Z >= box.Min.Z &&
        otherBox.Max.Z <= box.Max.Z;

    /// <summary>
    /// Returns true, if the given box intersects with this box.
    /// </summary>
    /// <param name="a">An AABB box.</param>
    /// <param name="b">The other AABB box.</param>
    /// <returns>True, if the given box intersects with this box.</returns>
    public static bool Intersect(this IAxisAlignedBoundaryBox a, IAxisAlignedBoundaryBox b) =>
        b.Min.X <= a.Max.X &&
        b.Max.X >= a.Min.X &&
        b.Min.Y <= a.Max.Y &&
        b.Max.Y >= a.Min.Y &&
        b.Min.Z <= a.Max.Z &&
        b.Max.Z >= a.Min.Z;
}