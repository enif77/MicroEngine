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
    /// Value above 1 turns on/off transparency effect of this material.
    /// 0 or 1 = fully opaque, 2 = maximally (approx. 50%) transparent. Above = more opaque.
    /// The higher the number, the less transparent the object will be.
    /// The transparency effect is achieved by discarding some fragments based on their screen position and depth.
    /// </summary>
    int OpacityLevel { get; set; }
    
    /// <summary>
    /// Modifies the transparency effect of this material.
    /// </summary>
    int OpacityBias { get; set; }
    
    /// <summary>
    /// The list of textures used by this material.
    /// </summary>
    IReadOnlyList<ITexture> Textures { get; }
    
    /// <summary>
    /// Shader to be used for rendering of this material.
    /// </summary>
    IShader Shader { get; }
}
