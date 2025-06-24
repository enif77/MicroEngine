/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Materials;

using OpenTK.Mathematics;

using MicroEngine.Core;
using MicroEngine.Shaders;

/// <summary>
/// A material that uses multiple textures with lighting. 
/// </summary>
public class Material : IMaterial
{
    public Vector3 Color { get; private init; } = new(1);
    public ITexture DiffuseMap { get; private init; } = new NullTexture();
    public ITexture SpecularMap { get; private init; } = new NullTexture();
    public Vector3 Specular { get; set; } = Vector3.Zero;
    public float Shininess { get; set; }
    public int OpacityLevel { get; set; }
    public int OpacityBias { get; set; }

    public IShader Shader { get; private init; } = new NullShader();


    private Material()
    {
    }

    
    /// <summary>
    /// Creates a new instance of <see cref="Material"/> with default values.
    /// </summary>
    /// <returns>A new instance of <see cref="IMaterial"/>.</returns>
    public static IMaterial Create()
    {
        return new Material();
    }

    /// <summary>
    /// Create a new material with a diffuse map and a shader.
    /// </summary>
    /// <param name="diffuseMap">A texture to be used by this material as a diffuse map.</param>
    /// <param name="shader">The shader to use for rendering this material.</param>
    /// <returns>A new instance of <see cref="IMaterial"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="diffuseMap"/> or <paramref name="shader"/> is null.</exception>
    public static IMaterial Create(ITexture diffuseMap, IShader shader)
    {
        return new Material
        {
            DiffuseMap = diffuseMap ?? throw new ArgumentNullException(nameof(diffuseMap)),
            Shader = shader ?? throw new ArgumentNullException(nameof(shader)),
        };
    }

    /// <summary>
    /// Create a new material with diffuse and specular maps, and a shader.
    /// </summary>
    /// <param name="diffuseMap">The diffuse texture map. Defines the base color of the material.</param>
    /// <param name="specularMap">The specular texture map. Defines the shininess and specular highlights of the material.</param>
    /// <param name="shader">The shader to use for rendering this material.</param>
    /// <returns>A new instance of <see cref="IMaterial"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="diffuseMap"/>, <paramref name="specularMap"/>, or <paramref name="shader"/> is null.</exception>
    public static IMaterial Create(ITexture diffuseMap, ITexture specularMap, IShader shader)
    {
        return new Material
        {
            DiffuseMap = diffuseMap ?? throw new ArgumentNullException(nameof(diffuseMap)),
            SpecularMap = specularMap ?? throw new ArgumentNullException(nameof(specularMap)),
            Specular = new Vector3(0.5f, 0.5f, 0.5f),
            Shininess = 32.0f,
            OpacityLevel = 0,
            OpacityBias = 0,
            Shader = shader ?? throw new ArgumentNullException(nameof(shader)),
        };
    }

    /// <summary>
    /// Create a new material with a single color and a shader.
    /// </summary>
    /// <param name="color">A color used by this material.</param>
    /// <param name="shader">The shader to use for rendering this material.</param>
    /// <returns>A new instance of <see cref="IMaterial"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="shader"/> is null.</exception>
    public static IMaterial Create(Vector3 color, IShader shader)
    {
        return new Material
        {
            Color = color,
            Shader = shader ?? throw new ArgumentNullException(nameof(shader)),
        };
    }
}
