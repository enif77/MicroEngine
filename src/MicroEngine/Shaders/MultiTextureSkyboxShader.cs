/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Shaders;

using MicroEngine.Core;

public class MultiTextureSkyboxShader : MultiTextureShaderBase
{
    public override string Name => "multi-texture-skybox";
    
    public MultiTextureSkyboxShader(IResourcesManager resourcesManager)
        : base(new Shader(
            resourcesManager.LoadTextFile("Resources/Shaders/multi-texture-skybox.vert"),
            resourcesManager.LoadTextFile("Resources/Shaders/multi-texture.frag")))
    {
        ArgumentNullException.ThrowIfNull(resourcesManager);
    }
}
