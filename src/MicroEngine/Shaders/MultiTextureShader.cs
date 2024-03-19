/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Shaders;

using MicroEngine.Core;

public class MultiTextureShader : MultiTextureShaderBase
{
    public override string Name => "multi-texture";
    

    public MultiTextureShader(IResourcesManager resourcesManager)
        : base(new Shader(
            resourcesManager.LoadTextFile("Resources/Shaders/multi-texture.vert"),
            resourcesManager.LoadTextFile("Resources/Shaders/multi-texture.frag")))
    {
        ArgumentNullException.ThrowIfNull(resourcesManager);
    }
}
