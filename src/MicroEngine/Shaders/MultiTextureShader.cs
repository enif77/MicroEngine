/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Shaders;

using MicroEngine.Core;

public class MultiTextureShader : MultiTextureShaderBase
{
    public override string Name => "multi-texture";
    

    public MultiTextureShader()
        : base(new Shader(
            File.ReadAllText("Resources/Shaders/multi-texture.vert"),
            File.ReadAllText("Resources/Shaders/multi-texture.frag")))
    {
    }
}
