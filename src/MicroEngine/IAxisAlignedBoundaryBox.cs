/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine;

using OpenTK.Mathematics;

/// <summary>
/// Defines an axis-aligned boundary box.
/// </summary>
public interface IAxisAlignedBoundaryBox : ISceneObject
{
    /// <summary>
    /// Minimal point of the box in the world space.
    /// </summary>
    Vector3 Min { get; }
    
    /// <summary>
    /// Maximal point of the box in the world space.
    /// </summary>
    Vector3 Max { get; }
}