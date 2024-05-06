/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Shaders;

using MicroEngine.Core;

public class SimpleColorShader : IShader
{
    private readonly Shader _shader;
    
    public SimpleColorShader(IResourcesManager resourcesManager)
    {
        ArgumentNullException.ThrowIfNull(resourcesManager);
        
        _shader = new Shader(
            /*language=glsl*/
            """
            #version 330 core
            
            layout (location = 0) in vec3 aPos;
            layout (location = 1) in vec3 aNormal;
            layout (location = 2) in vec2 aTexCoords;
            
            uniform mat4 model;
            uniform mat4 view;
            uniform mat4 projection;
            
            out vec3 Normal;
            out vec3 FragPos;
            out vec2 TexCoords;
            
            void main()
            {
                gl_Position = vec4(aPos, 1.0) * model * view * projection;
                FragPos = vec3(vec4(aPos, 1.0) * model);
                Normal = aNormal * mat3(transpose(inverse(model)));
                TexCoords = aTexCoords;
            } 
            """,
            /*language=glsl*/
            """
            #version 330 core
            
            uniform vec3 color;
            
            out vec4 FragColor;
            
            void main()
            {
                FragColor = vec4(color, 1.0);
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
        
        _shader.Use();
        
        _shader.SetVector3("color", sceneObject.Material.Color);
        _shader.SetMatrix4("view", camera.GetViewMatrix());
        _shader.SetMatrix4("projection", camera.GetProjectionMatrix());
        _shader.SetMatrix4("model", sceneObject.ModelMatrix);
    }
}
