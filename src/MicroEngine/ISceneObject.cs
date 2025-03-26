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
    /// If true, this scene object and its children is visible and will be rendered.
    /// </summary>
    bool IsVisible { get; set; }
    
    /// <summary>
    /// Model matrix of its scene object.
    /// Should be updated before rendering.
    /// </summary>
    Matrix4 ModelMatrix { get; }


    /// <summary>
    /// Sets rotation around X axis.
    /// </summary>
    /// <param name="angle">An angle in radians.</param>
    void SetRotationX(float angle);

    /// <summary>
    /// Sets rotation around Y axis.
    /// </summary>
    /// <param name="angle">An angle in radians.</param>
    void SetRotationY(float angle);

    /// <summary>
    /// Sets rotation around Z axis.
    /// </summary>
    /// <param name="angle">An angle in radians.</param>
    void SetRotationZ(float angle);

    /// <summary>
    /// Try to get a scene from a scene object.
    /// </summary>
    /// <returns>A Scene instance a scene object belongs to.</returns>
    /// <exception cref="InvalidOperationException">When no scene object was found as a parent of a game object.</exception>
    Scene GetScene();
}
