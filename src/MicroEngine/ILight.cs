/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine;

using MicroEngine.OGL;

/// <summary>
/// Generic interface for all lights.
/// </summary>
public interface ILight : ISceneObject
{
    /// <summary>
    /// A type of the light.
    /// </summary>
    LightType LightType { get; }
    
    /// <summary>
    /// Sets this light uniforms in the shader.
    /// </summary>
    /// <param name="glslShader">A shader, which uniforms should be set.</param>
    void SetUniforms(GlslShader glslShader);
}