/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Shaders;

public class MultiTextureShader : MultiTextureShaderBase
{
    protected override void BuildImpl()
    {
        GlslShader.Build(
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
                gl_Position = vec4(aPos, 1.0) * model * view * projection;
                TexId = int(aTexId);
                TexCoords = aTexCoords;
            }
            """,
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
            """);
    }
}
