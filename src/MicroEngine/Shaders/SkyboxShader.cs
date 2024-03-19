/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Shaders;

using OpenTK.Graphics.OpenGL4;

using MicroEngine.Core;

public class SkyboxShader : IShader
{
    private readonly Shader _shader;

    public string Name => "skybox";
    
    
    public SkyboxShader(IResourcesManager resourcesManager)
    {
        ArgumentNullException.ThrowIfNull(resourcesManager);
        
        _shader = new Shader(
            resourcesManager.LoadTextFile("Resources/Shaders/skybox.vert"),
            resourcesManager.LoadTextFile("Resources/Shaders/skybox.frag"));
    }

    
    public int GetAttributeLocation(string name)
    {
        return _shader.GetAttribLocation(name);
    }
    
    
    public void Use(Scene scene, ISceneObject sceneObject)
    {
        var camera = scene.Camera;
        
        sceneObject.Material.DiffuseMap.Use(TextureUnit.Texture0);
        
        _shader.Use();
        
        _shader.SetInt("texture0", 0);
        
        _shader.SetMatrix4("view", camera.GetViewMatrix());
        _shader.SetMatrix4("projection", camera.GetProjectionMatrix());
        _shader.SetMatrix4("model", sceneObject.ModelMatrix);
    }
}
