/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Core;

using OpenTK.Graphics.OpenGL4;

/// <summary>
/// Null texture implementation.
/// </summary>
public sealed class NullTexture : ITexture
{
    private static readonly Lazy<NullTexture> Singleton = new(() => new NullTexture());

    /// <summary>
    /// Gets the singleton instance of the NullTexture.
    /// </summary>
    public static NullTexture Instance => Singleton.Value;
    
    
    private NullTexture()
    {
        // Private constructor to prevent instantiation.
    }
    
    
    public void Use(TextureUnit unit)
    {
        // No operation for NullTexture.
    }
}
