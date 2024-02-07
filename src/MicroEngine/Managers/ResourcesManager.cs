/* Copyright (C) Premysl Fara and Contributors */

using OpenTK.Graphics.OpenGL4;

namespace MicroEngine.Managers;

using MicroEngine.Core;

/// <summary>
/// Manages resources.
/// </summary>
public class ResourcesManager : IResourcesManager
{
    #region singleton
    
    /// <summary>
    /// Gets instance of the default resources manager.
    /// </summary>
    public static ResourcesManager Instance { get; } = new ResourcesManager();
    
    
    /// <summary>
    /// This resources manager is a singleton.
    /// </summary>
    private ResourcesManager()
    {
    }
    
    #endregion
    
    
    #region textures
    
    private readonly Dictionary<string, ITexture> _textures = new();
    
    
    public ITexture LoadTexture(string path, TextureWrapMode wrapMode = TextureWrapMode.Repeat)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("A path to a texture is null or empty.");
        }
        
        if (_textures.TryGetValue(path, out var value))
        {
            return value;
        }

        var texture = Texture.LoadFromFile(path, wrapMode);
        _textures.Add(path, texture);
        
        return texture;
    }
    
    #endregion
}