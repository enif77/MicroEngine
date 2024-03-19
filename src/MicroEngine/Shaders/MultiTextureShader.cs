/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Shaders;

using MicroEngine.Core;

public class MultiTextureShader : MultiTextureShaderBase
{
    public override string Name => "multi-texture";
    

    public MultiTextureShader(IResourcesManager resourcesManager)
        : base(new Shader(
            resourcesManager.LoadTextFile("Shaders/multi-texture.vert"),
            resourcesManager.LoadTextFile("Shaders/multi-texture.frag")))
    {
        ArgumentNullException.ThrowIfNull(resourcesManager);
    }
}
