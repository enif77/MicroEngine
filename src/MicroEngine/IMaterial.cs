/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine;

using OpenTK.Mathematics;

/// <summary>
/// Defines a material used for rendering of an object.
/// </summary>
public interface IMaterial
{
    /// <summary>
    /// A color used by this material.
    /// </summary>
    Vector3 Color { get; }
    
    /// <summary>
    /// The texture used by this material.
    /// </summary>
    ITexture DiffuseMap { get; }
    
    /// <summary>
    /// A specular map used by this material.
    /// </summary>
    ITexture SpecularMap { get; }
    
    /// <summary>
    /// Specular color of this material.
    /// </summary>
    Vector3 Specular { get; set; }
    
    /// <summary>
    /// Shininess of this material.
    /// </summary>
    float Shininess { get; set; }
    
    /// <summary>
    /// Shader to be used for rendering of this material.
    /// </summary>
    IShader Shader { get; }
}
