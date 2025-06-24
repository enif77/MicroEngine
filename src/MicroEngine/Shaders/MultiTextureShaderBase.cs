/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Shaders;

using OpenTK.Graphics.OpenGL4;

using MicroEngine.Materials;
using MicroEngine.OGL;

/// <summary>
/// A base for all shaders, that use multiple textures.
/// </summary>
public abstract class MultiTextureShaderBase : IShader
{
    private bool _wasBuilt;
    
    public bool SupportsOpenGLES => true;

    protected GlslShader GlslShader { get; } = new();
    

    public int GetAttributeLocation(string name)
    {
        return GlslShader.GetAttribLocation(name);
    }


    public void Build()
    {
        if (_wasBuilt)
        {
            return; // Already built, no need to build again.
        }

        BuildImpl();
        
        _wasBuilt = true;
    }
    
    
    protected abstract void BuildImpl();


    public virtual void Use(Scene scene, ISceneObject sceneObject)
    {
        var camera = scene.Camera;
        var material = sceneObject.Material;
        
        GlslShader.Use();
        
        for (var i = 0; i < material.Textures.Count; i++)
        {
            material.Textures[i].Use(TextureUnit.Texture0 + i);
            GlslShader.SetInt(SamplersUniformNames[i], i);
        }
        
        GlslShader.SetMatrix4("view", camera.GetViewMatrix());
        GlslShader.SetMatrix4("projection", camera.GetProjectionMatrix());
        GlslShader.SetMatrix4("model", sceneObject.ModelMatrix);
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
