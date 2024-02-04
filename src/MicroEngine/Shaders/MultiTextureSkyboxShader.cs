/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Shaders;

using MicroEngine.Core;

public class MultiTextureSkyboxShader : MultiTextureShaderBase
{
    public override string Name => "multi-texture-skybox";
    
    public MultiTextureSkyboxShader()
        : base(new Shader(
            File.ReadAllText("Resources/Shaders/multi-texture-skybox.vert"),
            File.ReadAllText("Resources/Shaders/multi-texture.frag")))
    {
    }
}
