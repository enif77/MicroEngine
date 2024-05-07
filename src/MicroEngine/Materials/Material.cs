/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Materials;

using OpenTK.Mathematics;

/// <summary>
/// A material that uses multiple textures with lighting. 
/// </summary>
public class Material : IMaterial
{
    public Vector3 Color => new(1);
    public ITexture DiffuseMap { get; }
    public ITexture SpecularMap { get; }
    public Vector3 Specular { get; set; }
    public float Shininess { get; set; }
    public bool IsTransparent { get; set; }
    public int TransparencyThreshold { get; set; }

    public IShader Shader { get; }


    public Material(ITexture diffuseMap, ITexture specularMap, IShader shader)
    {
        DiffuseMap = diffuseMap ?? throw new ArgumentNullException(nameof(diffuseMap));
        SpecularMap = specularMap ?? throw new ArgumentNullException(nameof(specularMap));
        Specular = new Vector3(0.5f, 0.5f, 0.5f);
        Shininess = 32.0f;
        IsTransparent = false;
        TransparencyThreshold = 2;
        Shader = shader ?? throw new ArgumentNullException(nameof(shader));
    }
}
