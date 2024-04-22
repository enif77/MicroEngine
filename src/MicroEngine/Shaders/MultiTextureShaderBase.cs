/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Shaders;

using OpenTK.Graphics.OpenGL4;

using MicroEngine.Core;
using MicroEngine.Materials;

/// <summary>
/// A base for all shaders, that use multiple textures.
/// </summary>
public abstract class MultiTextureShaderBase : IShader
{
    protected Shader Shader { get; }
    
    
    protected MultiTextureShaderBase(Shader shader)
    {
        Shader = shader ?? throw new ArgumentNullException(nameof(shader));
    }
    

    public int GetAttributeLocation(string name)
    {
        return Shader.GetAttribLocation(name);
    }
    
    
    public virtual void Use(Scene scene, ISceneObject sceneObject)
    {
        var camera = scene.Camera;
        var material = sceneObject.Material;
        
        Shader.Use();
        
        // TODO: The base material should support for multiple textures.
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
                Shader.SetInt(SamplersUniformNames[i], i);
            }
        }
        
        Shader.SetMatrix4("view", camera.GetViewMatrix());
        Shader.SetMatrix4("projection", camera.GetProjectionMatrix());
        Shader.SetMatrix4("model", sceneObject.ModelMatrix);
    }
    
    
    private static readonly string[] SamplersUniformNames =
    [
        "samplers[0].tex",
        "samplers[1].tex",
        "samplers[2].tex",
        "samplers[3].tex",
        "samplers[4].tex",
        "samplers[5].tex",
        "samplers[6].tex",
        "samplers[7].tex",
        "samplers[8].tex",
        "samplers[9].tex",
        "samplers[10].tex",
        "samplers[11].tex",
        "samplers[12].tex",
        "samplers[13].tex",
        "samplers[14].tex",
        "samplers[15].tex"
    ]; 
}
