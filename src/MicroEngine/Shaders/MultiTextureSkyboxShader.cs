/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Shaders;

using MicroEngine.OGL;

public class MultiTextureSkyboxShader : MultiTextureShaderBase
{
    protected override void BuildImpl()
    {
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

        GlslShader.Build(vertexShaderSource, fragmentShaderSource);
    }

    private const string VertexShaderSource =
        /*language=glsl*/
        """
        #version 330 core
        
        layout (location = 0) in vec3 aPos;
        layout (location = 1) in float aTexId;
        layout (location = 2) in vec2 aTexCoords;
        
        uniform mat4 model;
        uniform mat4 view;
        uniform mat4 projection;
        
        flat out int TexId;
        out vec2 TexCoords;
        
        void main()
        {
            vec4 position = vec4(aPos, 1.0) * model * view * projection;
            gl_Position = position.xyww;  // This forces the z value to be 1.0 as it is required to have the skybox "as far as possible".
            TexId = int(aTexId);
            TexCoords = aTexCoords;
        }    
        """;
    
    private const string FragmentShaderSource =
        /*language=glsl*/
        """
        #version 330 core
        
        struct Sampler
        {
            sampler2D tex;
        };
        
        #define NR_SAMPLERS 16
        uniform Sampler samplers[NR_SAMPLERS];
        
        flat in int TexId;
        in vec2 TexCoords;
        
        out vec4 FragColor;
        
        void main()
        {
            // FragColor = texture(samplers[TexId].tex, TexCoords);
            vec4 texColor = texture(samplers[TexId].tex, TexCoords);
            if (texColor.a < 0.1)
            {
                discard;    
            }
            
            FragColor = texColor;
        }    
        """;
}
