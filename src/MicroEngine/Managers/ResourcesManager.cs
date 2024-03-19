/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Managers;

using OpenTK.Graphics.OpenGL4;

using MicroEngine.Core;
using MicroEngine.Textures;

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
    
    
    #region general files
    
    public string LoadTextFile(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("A path to a text file expected.");
        }

        if (File.Exists(path) == false)
        {
            path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path)
                .Replace("Contents/MacOS/", "Contents/");
        }
        
        if (File.Exists(path))
        {
            return File.ReadAllText(path);
        }
        
        throw new FileNotFoundException($"A file '{path}' not found.");
    }
    
    #endregion
    
    
    #region textures
    
    private readonly ITexture _nullTexture = new NullTexture();
    private readonly Dictionary<string, ITexture> _textures = new();


    public ITexture GetTexture(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("A texture name is null or empty.");
        }

        return _textures.GetValueOrDefault(name, _nullTexture);
    }


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

        if (File.Exists(path) == false)
        {
            path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path)
                .Replace("Contents/MacOS/", "Contents/");
        }
        
        if (File.Exists(path))
        {
            var texture = Texture.LoadFromFile(path, wrapMode);
            _textures.Add(path, texture);
        
            return texture;
        }
        
        throw new FileNotFoundException($"A texture '{path}' not found.");
    }
    
    
    public ITexture LoadTextureFromRgbaBytes(string name, byte[] pixels, int width, int height, TextureWrapMode wrapMode = TextureWrapMode.Repeat)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("A texture name is null or empty.");
        }

        ArgumentNullException.ThrowIfNull(pixels);

        if (_textures.TryGetValue(name, out var value))
        {
            return value;
        }

        var texture = Texture.LoadFromRgbaBytes(pixels, width, height, wrapMode);
        _textures.Add(name, texture);
        
        return texture;
    }
    
    #endregion
}