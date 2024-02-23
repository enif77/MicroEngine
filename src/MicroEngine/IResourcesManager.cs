/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine;

using OpenTK.Graphics.OpenGL4;

/// <summary>
/// Manages resources.
/// </summary>
public interface IResourcesManager
{
    #region textures

    /// <summary>
    /// Gets a texture by its name.
    /// </summary>
    /// <param name="name">A texture name.</param>
    /// <returns>A texture with a given name or the NullTexture instance.</returns>
    ITexture GetTexture(string name);
    
    /// <summary>
    /// Loads a texture from the given path.
    /// </summary>
    /// <param name="path">A path to a texture. Will be used as an unique name for this texture.</param>
    /// <param name="wrapMode">Texture wrap mode. TextureWrapMode.Repeat by default.</param>
    /// <returns>A loaded texture.</returns>
    ITexture LoadTexture(string path, TextureWrapMode wrapMode = TextureWrapMode.Repeat);

    /// <summary>
    /// Loads a texture from the given RGBA bytes.
    /// </summary>
    /// <param name="name">An unique name of this texture.</param>
    /// <param name="pixels">An array of RGBA pixels.</param>
    /// <param name="width">A width of this texture.</param>
    /// <param name="height">A height of this texture.</param>
    /// <param name="wrapMode">Texture wrap mode. TextureWrapMode.Repeat by default.</param>
    /// <returns>A loaded texture.</returns>
    ITexture LoadTextureFromRgbaBytes(
        string name, byte[] pixels, int width, int height,
        TextureWrapMode wrapMode = TextureWrapMode.Repeat);

    #endregion
}