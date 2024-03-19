/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Shaders;

using MicroEngine.Core;

public class MultiTextureSkyboxShader : MultiTextureShaderBase
{
    public override string Name => "multi-texture-skybox";
    
    public MultiTextureSkyboxShader(IResourcesManager resourcesManager)
        : base(new Shader(
            resourcesManager.LoadTextFile("Shaders/multi-texture-skybox.vert"),
            resourcesManager.LoadTextFile("Shaders/multi-texture.frag")))
    {
        ArgumentNullException.ThrowIfNull(resourcesManager);
    }
}
