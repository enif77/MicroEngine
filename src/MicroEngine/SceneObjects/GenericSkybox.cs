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

        // TODO: We should get the world camera position. A position, that is combination of the camera and its parents positions.
        
        // Skybox should be rendered at the camera position.
        //ModelMatrix = Matrix4.CreateTranslation(_scene.Camera.Position);
        ModelMatrix = Matrix4.CreateTranslation(new Vector3(_scene.Camera.ModelMatrix.M41, _scene.Camera.ModelMatrix.M42, _scene.Camera.ModelMatrix.M43)); 
        
        // Sets shader and its properties.
        Material.Shader.Use(_scene, this);
        
        // Render.
        GL.DepthFunc(DepthFunction.Lequal);
        Geometry.Render();
        GL.DepthFunc(DepthFunction.Less);
       
        base.Render();
    }
}
