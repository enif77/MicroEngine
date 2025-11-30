/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Shaders;

using OpenTK.Graphics.OpenGL;

using MicroEngine.OGL;

public class SkyboxShader : IShader
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
        
        _wasBuilt = true;
    }

    
    public int GetAttributeLocation(string name)
    {
        return _glslShader.GetAttribLocation(name);
    }
    
    
    public void Use(Scene scene, ISceneObject sceneObject)
    {
        var camera = scene.Camera;
        
        sceneObject.Material.DiffuseMap.Use(TextureUnit.Texture0);
        
        _glslShader.Use();
        
        _glslShader.SetInt("texture0", 0);
        
        _glslShader.SetMatrix4("view", camera.GetViewMatrix());
        _glslShader.SetMatrix4("projection", camera.GetProjectionMatrix());
        _glslShader.SetMatrix4("model", sceneObject.ModelMatrix);
    }

    private const string VertexShaderSource =
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
            vec4 pos = vec4(aPos, 1.0) * model * view * projection;
            gl_Position = pos.xyww;  // This forces the z value to be 1.0.
            TexCoords = aTexCoords;
        }
        """;
    
    private const string FragmentShaderSource =
        /*language=glsl*/
        """
        #version 330 core
        
        uniform sampler2D texture0;
        
        in vec2 TexCoords;
        
        out vec4 FragColor;
        
        void main()
        {
            FragColor = texture(texture0, TexCoords);
        } 
        """;
}
