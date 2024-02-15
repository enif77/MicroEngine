/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.SceneObjects;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using MicroEngine.Extensions;

/// <summary>
/// Generic skybox.
/// </summary>
public class GenericSkybox : SceneObjectBase
{
    public GenericSkybox(IGeometry geometry)
    {
        Geometry = geometry ?? throw new ArgumentNullException(nameof(geometry));
    }
    

    private Scene? _scene;

    public override void Render()
    {
        _scene ??= this.GetScene();

        // Skybox should be rendered at the camera position.
        ModelMatrix = Matrix4.CreateTranslation(_scene.Camera.ModelMatrix.ExtractTranslation()); 
        
        // Sets shader and its properties.
        Material.Shader.Use(_scene, this);
        
        // Render.
        GL.DepthFunc(DepthFunction.Lequal);
        Geometry.Render();
        GL.DepthFunc(DepthFunction.Less);
       
        base.Render();
    }
}
