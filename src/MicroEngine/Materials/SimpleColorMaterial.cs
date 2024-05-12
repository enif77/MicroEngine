/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Materials;

using OpenTK.Mathematics;

using MicroEngine.Textures;

/// <summary>
/// A material that uses a single color with no lighting. 
/// </summary>
/// <param name="color">A color used by this material.</param>
public class SimpleColorMaterial(Vector3 color, IShader shader) : IMaterial
{
    public Vector3 Color { get; } = color;
    public ITexture DiffuseMap { get; } = new NullTexture();
    public ITexture SpecularMap { get; } = new NullTexture();
    public Vector3 Specular { get; set; } = Vector3.Zero;
    public float Shininess { get; set; }
    public int OpacityLevel { get; set; }
    public int OpacityBias { get; set; }
    public IShader Shader { get; } = shader ?? throw new ArgumentNullException(nameof(shader));
}
