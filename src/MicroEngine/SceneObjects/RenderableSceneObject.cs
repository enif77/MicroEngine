/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.SceneObjects;

/// <summary>
/// A renderable scene object.
/// </summary>
public class RenderableSceneObject : SceneObjectBase
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="geometry">A geometry representing this scene object.</param>
    /// <exception cref="ArgumentNullException">If the geometry parameter is null.</exception>
    public RenderableSceneObject(IGeometry geometry)
    {
        Geometry = geometry ?? throw new ArgumentNullException(nameof(geometry));
    }
    
    
    private Scene? _scene;

    public override void Render()
    {
        if (IsVisible == false)
        {
            return;
        }
        
        _scene ??= GetScene();
        
        Material.Shader.Use(_scene, this);
        Geometry.Render();
        
        base.Render();
    }
}
