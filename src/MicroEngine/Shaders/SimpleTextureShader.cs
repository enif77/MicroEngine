/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Shaders;

using OpenTK.Graphics.OpenGL4;

using MicroEngine.Core;

public class SimpleTextureShader : IShader
{
    private readonly Shader _shader;
    
    
    public SimpleTextureShader(IResourcesManager resourcesManager)
    {
        ArgumentNullException.ThrowIfNull(resourcesManager);
        
        _shader = new Shader(
            /*language=glsl*/
            """
            #version 330 core
            
            layout (location = 0) in vec3 aPos;
            layout (location = 1) in vec2 aTexCoords;
            
            uniform mat4 model;
            uniform mat4 view;
            uniform mat4 projection;
            
            out vec2 TexCoords;
            
            void main()
            {
                gl_Position = vec4(aPos, 1.0) * model * view * projection;
                TexCoords = aTexCoords;
            }
            """,
            /*language=glsl*/
            """
            #version 330 core
            
            uniform sampler2D texture0;
            
            in vec2 TexCoords;
            
            out vec4 FragColor;
            
            void main()
            {
                //FragColor = texture(texture0, TexCoords);
                vec4 texColor = texture(texture0, TexCoords);
                if (texColor.a < 0.1)
                {
                    discard;
                }
            
                FragColor = texColor;
            }
            """);
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
