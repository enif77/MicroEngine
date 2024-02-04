/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Shaders;

using MicroEngine.Core;

public class MultiTextureSkyboxShader : MultiTextureShader
{
    public override string Name => "multi-texture-skybox";
    

    public MultiTextureSkyboxShader()
    {
        Shader = new Shader(
            File.ReadAllText("Resources/Shaders/multi-texture-skybox.vert"),
            File.ReadAllText("Resources/Shaders/multi-texture.frag"));
    }
}
