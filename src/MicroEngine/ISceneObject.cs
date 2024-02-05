/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine;

using OpenTK.Mathematics;

/// <summary>
/// Generic interface for all scene objects.
/// </summary>
public interface ISceneObject : IUpdatable, IRenderable
{
    /// <summary>
    /// This scene object's parent.
    /// </summary>
    ISceneObject? Parent { get; set; }
    
    /// <summary>
    /// This scene object's children.
    /// </summary>
    IList<ISceneObject> Children { get; }
    
    /// <summary>
    /// A geometry defining this scene object.
    /// </summary>
    IGeometry Geometry { get; }
    
    /// <summary>
    /// A material used by this scene object.
    /// </summary>
    IMaterial Material { get; }
    
    /// <summary>
    /// Scale of this scene object relative to the parent.
    /// </summary>
    float Scale { get; set; }
    
    /// <summary>
    /// Position of this scene object relative to the parent.
    /// </summary>
    Vector3 Position { get; set; }
    
    /// <summary>
    /// Rotation of this scene object in counter-clockwise angles in radians.
    /// </summary>
    Vector3 Rotation { get; set; }
    
    /// <summary>
    /// True, if this object needs model matrix update.
    /// </summary>
    bool NeedsModelMatrixUpdate { get; set; }
    
    /// <summary>
    /// Model matrix of tis scene object.
    /// Should be updated before rendering.
    /// </summary>
    Matrix4 ModelMatrix { get; set; }
}
