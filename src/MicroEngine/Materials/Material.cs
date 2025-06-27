/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Materials;

using OpenTK.Mathematics;

using MicroEngine.Core;

/// <summary>
/// A material that uses multiple textures with lighting. 
/// </summary>
public class Material : IMaterial
{
    public Vector3 Color { get; private init; } = new(1);
    public ITexture DiffuseMap => Textures[0];
    public ITexture SpecularMap => Textures[1];
    public Vector3 Specular { get; set; } = Vector3.Zero;
    public float Shininess { get; set; }
    public int OpacityLevel { get; set; }
    public int OpacityBias { get; set; }
    
    public IReadOnlyList<ITexture> Textures { get; } = new List<ITexture>
    {
        NullTexture.Instance,  // Placeholder for the diffuse map
        NullTexture.Instance   // Placeholder for the specular map
    };

    public IShader Shader { get; private init; } = NullShader.Instance;


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
        var material = new Material()
        {
            Shader = shader ?? throw new ArgumentNullException(nameof(shader))
        };

        ((List<ITexture>)material.Textures)[0] = diffuseMap ?? throw new ArgumentNullException(nameof(diffuseMap));
        
        return material;
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
        var material = new Material
        {
            Specular = new Vector3(0.5f, 0.5f, 0.5f),
            Shininess = 32.0f,
            OpacityLevel = 0,
            OpacityBias = 0,
            Shader = shader ?? throw new ArgumentNullException(nameof(shader)),
        };
        
        ((List<ITexture>)material.Textures)[0] = diffuseMap ?? throw new ArgumentNullException(nameof(diffuseMap));
        ((List<ITexture>)material.Textures)[1] = specularMap ?? throw new ArgumentNullException(nameof(specularMap));
        
        return material;
    }
    
    /// <summary>
    /// Create a new material with multiple textures and a shader.
    /// </summary>
    /// <param name="textures">A list of textures to be used by this material. Must contain at least one texture and can contain up to 16 textures.</param>
    /// <param name="shader">The shader to use for rendering this material.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="textures"/> or <paramref name="shader"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="textures"/> is empty or contains more than 16 textures.</exception>
    public static IMaterial Create(ITexture[] textures, IShader shader)
    {
        if (textures == null)
        {
            throw new ArgumentNullException(nameof(textures), "Textures array must not be null or empty.");
        }
        
        if (textures.Length == 0)
        {
            throw new ArgumentException("Textures array must not be empty.", nameof(textures));
        }
        
        if (textures.Length > 16)
        {
            throw new ArgumentException("The number of textures must be 16 or less.", nameof(textures));
        }
        
        if (textures[0] == null)
        {
            throw new ArgumentNullException(nameof(textures), "The first texture must not be null.");
        }
        
        var material = new Material
        {
            Shader = shader ?? throw new ArgumentNullException(nameof(shader)),
        };

        var texturesList = ((List<ITexture>)material.Textures);
        
        texturesList.Clear();
        texturesList.AddRange(textures);
        if (texturesList.Count < 2)
        {
            // Ensure there's always a specular map.
            texturesList.Add(NullTexture.Instance); 
        }
        
        return material;
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
