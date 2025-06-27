/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Core;

using OpenTK.Mathematics;

/// <summary>
/// This class represents a null material so we can use it in place of a material
/// </summary>
public sealed class NullMaterial : IMaterial
{
    private static readonly Lazy<NullMaterial> Singleton = new(() => new NullMaterial());

    /// <summary>
    /// Gets the singleton instance of the NullMaterial.
    /// </summary>
    public static NullMaterial Instance => Singleton.Value;
    
    public Vector3 Color => Vector3.Zero;
    public ITexture DiffuseMap { get; }
    public ITexture SpecularMap { get; }

    public Vector3 Specular
    {
        get => Vector3.Zero;
        set
        {
            // Do nothing, as this is a null material
        }
    }

    public float Shininess
    {
        get => 0.0f;
        set
        {
            // Do nothing, as this is a null material
        }
    }

    public int OpacityLevel
    {
        get => 0;
        set
        {
            // Do nothing, as this is a null material
        }
    }

    public int OpacityBias
    {
        get => 0;
        set
        {
            // Do nothing, as this is a null material
        }
    }
    
    public IReadOnlyList<ITexture> Textures { get; }
    
    public IShader Shader { get; }
    
    
    /// <summary>
    /// Private constructor to prevent instantiation.
    /// </summary>
    private NullMaterial()
    {
        Shader = NullShader.Instance;
        
        var nullTexture = NullTexture.Instance;
        
        // Initialize DiffuseMap and SpecularMap with NullTexture
        DiffuseMap = nullTexture;
        SpecularMap = nullTexture;
        
        // No textures are used in NullMaterial, so we can set them to NullTexture
        Textures = new List<ITexture> { nullTexture, nullTexture };
    }
}
