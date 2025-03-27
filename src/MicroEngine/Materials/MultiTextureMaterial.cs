/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Materials;

using OpenTK.Mathematics;

using MicroEngine.Graphics;

/// <summary>
/// A material that uses multiple textures with no lighting. 
/// </summary>
public class MultiTextureMaterial : IMaterial
{
    public Vector3 Color => new(1);
    
    /// <summary>
    /// The texture used by this material.
    /// </summary>
    public ITexture DiffuseMap => Textures[0];

    public ITexture SpecularMap { get; } = new NullTexture();
    public Vector3 Specular { get; set; } = Vector3.Zero;
    public float Shininess { get; set; }
    public int OpacityLevel { get; set; }
    public int OpacityBias { get; set; }
    public IShader Shader { get; }


    public ITexture[] Textures { get; }
    
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="textures">A list of textures to be used by this material.</param>
    /// <param name="shader">A shader to be used to render this material.</param>
    /// <exception cref="ArgumentNullException">Thrown, when the diffuseMap or the shader parameter is null.</exception>
    public MultiTextureMaterial(ITexture[] textures, IShader shader)
    {
        Textures = textures ?? throw new ArgumentNullException(nameof(textures));
        Shader = shader ?? throw new ArgumentNullException(nameof(shader));

        if (Textures[0] == null)
        {
            throw new ArgumentNullException(nameof(textures), "The first texture must not be null.");
        }

        if (Textures.Length > 16)
        {
            throw new ArgumentException("The number of textures must be 16 or less.", nameof(textures));
        }
    }
}