/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Shaders;

using OpenTK.Graphics.OpenGL4;

using MicroEngine.Core;

public class DefaultShader : IShader
{
    private readonly Shader _shader;

    public string Name => "cube";
    

    public DefaultShader()
    {
        _shader = new Shader(
            File.ReadAllText("Resources/Shaders/shader.vert"),
            File.ReadAllText("Resources/Shaders/default.frag"));
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
        material.SpecularMap.Use(TextureUnit.Texture1);
        
        _shader.Use();
        
        _shader.SetInt("material.diffuse", 0);
        _shader.SetInt("material.specular", 1);
        _shader.SetVector3("material.specular", material.Specular);
        _shader.SetFloat("material.shininess", material.Shininess);
        
        _shader.SetMatrix4("view", camera.GetViewMatrix());
        _shader.SetMatrix4("projection", camera.GetProjectionMatrix());
        _shader.SetVector3("viewPos", camera.Position);
        _shader.SetMatrix4("model", sceneObject.ModelMatrix);
        
        /*
           Here we set all the uniforms for the 5/6 types of lights we have. We have to set them manually and index
           the proper PointLight struct in the array to set each uniform variable. This can be done more code-friendly
           by defining light types as classes and set their values in there, or by using a more efficient uniform approach
           by using 'Uniform buffer objects', but that is something we'll discuss in the 'Advanced GLSL' tutorial.
        */
        
        _shader.SetInt("numLights", scene.Lights.Count);
        
        foreach (var light in scene.Lights)
        {
            light.SetUniforms(_shader);
        }
    }
}
