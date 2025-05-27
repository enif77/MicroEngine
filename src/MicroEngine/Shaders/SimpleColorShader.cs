/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Shaders;

using MicroEngine.OGL;

public class SimpleColorShader : IShader
{
    private readonly GlslShader _glslShader = new();
    private bool _wasBuilt;
    
    
    public bool SupportsOpenGLES => true;
    
    
    public void Build()
    {
        if (_wasBuilt)
        {
            return; // Already built, no need to build again.
        }
        
        var vertexShaderSource = VertexShaderSource;
        var fragmentShaderSource = FragmentShaderSource;
        
        // Patch the shader to work with OpenGL ES 3.1 and higher.
        if (GlContext.IsGLES)
        {
            vertexShaderSource = vertexShaderSource.Replace(
                "#version 330 core",
                /*language=glsl*/
                """
                #version 310 es
                precision highp float;
                precision highp int; 
                precision highp sampler2D;
                """);
            
            fragmentShaderSource = fragmentShaderSource.Replace(
                "#version 330 core",
                /*language=glsl*/
                """
                #version 310 es
                precision highp float;
                precision highp int; 
                precision highp sampler2D;
                """);
        }

        _glslShader.Build(vertexShaderSource, fragmentShaderSource);
        
        _wasBuilt = true;
    }

    
    public int GetAttributeLocation(string name)
    {
        return _glslShader.GetAttribLocation(name);
    }
    
    
    public void Use(Scene scene, ISceneObject sceneObject)
    {
        var camera = scene.Camera;
        
        _glslShader.Use();
        
        _glslShader.SetVector3("color", sceneObject.Material.Color);
        _glslShader.SetMatrix4("view", camera.GetViewMatrix());
        _glslShader.SetMatrix4("projection", camera.GetProjectionMatrix());
        _glslShader.SetMatrix4("model", sceneObject.ModelMatrix);
    }

    private const string VertexShaderSource =
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
        """;
    
    private const string FragmentShaderSource =
        /*language=glsl*/
        """
        #version 330 core
        
        uniform vec3 color;
        
        out vec4 FragColor;
        
        void main()
        {
            FragColor = vec4(color, 1.0);
        }    
        """;
}
