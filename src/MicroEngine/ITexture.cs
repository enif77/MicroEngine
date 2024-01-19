namespace MicroEngine;

using OpenTK.Graphics.OpenGL4;

/// <summary>
/// Represents a texture.
/// </summary>
public interface ITexture
{
    /// <summary>
    /// Activates this texture for further rendering.
    /// </summary>
    /// <param name="unit">Which texture unit should be used.</param>
    void Use(TextureUnit unit);
}
