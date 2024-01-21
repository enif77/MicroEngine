/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Shaders;

using OpenTK.Graphics.OpenGL4;

using MicroEngine.Core;

public class SimpleTextureShader : IShader
{
    private readonly Shader _shader;

    public string Name => "simple-texture";
    

    public SimpleTextureShader()
    {
        _shader = new Shader(
            File.ReadAllText("Resources/Shaders/skybox.vert"),
            File.ReadAllText("Resources/Shaders/skybox.frag"));
    }

    
    public int GetAttributeLocation(string name)
    {
        return _shader.GetAttribLocation(name);
    }
    
    
    public void Use(Scene scene, ISceneObject sceneObject)
    {
        var camera = scene.Camera;
        var material = sceneObject.Material;
        
        material.DiffuseMap.Use(TextureUnit.Texture0);
        
        _shader.Use();
        
        _shader.SetMatrix4("view", camera.GetViewMatrix());
        _shader.SetMatrix4("projection", camera.GetProjectionMatrix());
        _shader.SetMatrix4("model", sceneObject.ModelMatrix);
    }
}
