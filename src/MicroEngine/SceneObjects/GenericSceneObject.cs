/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.SceneObjects;

using MicroEngine.Extensions;

/// <summary>
/// A generic scene object.
/// </summary>
public class GenericSceneObject : SceneObjectBase
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="geometry">A geometry representing this scene object.</param>
    /// <exception cref="ArgumentNullException">If the geometry parameter is null.</exception>
    public GenericSceneObject(IGeometry geometry)
    {
        Geometry = geometry ?? throw new ArgumentNullException(nameof(geometry));
    }
    
    
    private Scene? _scene;

    public override void Render()
    {
        _scene ??= this.GetScene();
        
        Material.Shader.Use(_scene, this);
        Geometry.Render();
        
        base.Render();
    }
}
