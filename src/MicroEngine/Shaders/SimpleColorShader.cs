/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Shaders;

using MicroEngine.Core;

public class SimpleColorShader : IShader
{
    private readonly Shader _shader;

    public string Name => "simple-color";
    
    
    public SimpleColorShader(IResourcesManager resourcesManager)
    {
        ArgumentNullException.ThrowIfNull(resourcesManager);
        
        _shader = new Shader(
            resourcesManager.LoadTextFile("Resources/Shaders/shader.vert"),
            resourcesManager.LoadTextFile("Resources/Shaders/simple-color.frag"));
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
