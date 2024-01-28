/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Shaders;

using OpenTK.Graphics.OpenGL4;

using MicroEngine.Core;
using MicroEngine.Materials;

public class MultiTextureShader : IShader
{
    private readonly Shader _shader;

    public string Name => "multi-texture";
    

    public MultiTextureShader()
    {
        _shader = new Shader(
            File.ReadAllText("Resources/Shaders/multi-texture.vert"),
            File.ReadAllText("Resources/Shaders/multi-texture.frag"));
    }

    
    public int GetAttributeLocation(string name)
    {
        return _shader.GetAttribLocation(name);
    }
    
    
    public void Use(Scene scene, ISceneObject sceneObject)
    {
        var camera = scene.Camera;
        var material = sceneObject.Material;
        
        _shader.Use();
        
        var m = material as MultiTextureMaterial;
        if (m == null)
        {
            material.DiffuseMap.Use(TextureUnit.Texture0);
        }
        else
        {
            for (var i = 0; i < m.Textures.Length; i++)
            {
                m.Textures[i].Use(TextureUnit.Texture0 + i);
                _shader.SetInt("samplers[" + i + "].tex", i);
            }
        }
        
        _shader.SetMatrix4("view", camera.GetViewMatrix());
        _shader.SetMatrix4("projection", camera.GetProjectionMatrix());
        _shader.SetMatrix4("model", sceneObject.ModelMatrix);
    }
}
