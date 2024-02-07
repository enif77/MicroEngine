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
    /// Loads a texture from the given path.
    /// </summary>
    /// <param name="path">A path to a texture.</param>
    /// <param name="wrapMode">Texture wrap mode. TextureWrapMode.Repeat by default.</param>
    /// <returns>A loaded texture.</returns>
    ITexture LoadTexture(string path, TextureWrapMode wrapMode = TextureWrapMode.Repeat);
    
    #endregion
}