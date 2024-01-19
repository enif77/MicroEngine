/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Shaders;

using MicroEngine.Core;

public class LampShader : IShader
{
    private readonly Shader _shader;

    public string Name => "lamp";
    
    
    public LampShader()
    {
        _shader = new Shader(
            File.ReadAllText("Resources/Shaders/shader.vert"),
            File.ReadAllText("Resources/Shaders/lamp.frag"));
    }

    
    public int GetAttributeLocation(string name)
    {
        return _shader.GetAttribLocation(name);
    }
    
    
    public void Use(Scene scene, ISceneObject sceneObject)
    {
        var camera = scene.Camera;
        
        _shader.Use();
        
        _shader.SetVector3("color", sceneObject.Material.Color);
        _shader.SetMatrix4("view", camera.GetViewMatrix());
        _shader.SetMatrix4("projection", camera.GetProjectionMatrix());
        _shader.SetMatrix4("model", sceneObject.ModelMatrix);
    }
}
