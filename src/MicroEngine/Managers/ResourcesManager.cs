/* Copyright (C) Premysl Fara and Contributors */

using System.Runtime.InteropServices;

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
    
    
    public string RootPath { get; set; } = string.Empty;
    
    
    #region general files
    
    public bool FileExists(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("A path to a file expected.");
        }

        return File.Exists(GetFullPath(path));
    }
    
    
    public string LoadTextFile(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("A path to a text file expected.");
        }

        return File.ReadAllText(GetFullPath(path));
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

        var texture = Texture.LoadFromFile(GetFullPath(path), wrapMode);
        _textures.Add(path, texture);
        
        return texture;
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
    
    
    #region helpers
    
    private string GetFullPath(string path)
    {
        // An absolute path or a path relative to the executable.
        if (File.Exists(path))
        {
            return path;
        }

        // A path relative to the root path.
        if (string.IsNullOrWhiteSpace(RootPath) == false)
        {
            var pathWithRoot = Path.Combine(RootPath, path);
            if (File.Exists(pathWithRoot))
            {
                return OsSafePath(pathWithRoot);
            }
        }

        // A path relative to the executable.
        var pathWithBaseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
        if (File.Exists(pathWithBaseDirectory))
        {
            return OsSafePath(pathWithBaseDirectory);
        }

        throw new FileNotFoundException($"A file '{path}' not found.");
    }
    
    
    private string OsSafePath(string path)
    {
        // We are fixing a path to a resource in a MacOS app bundle.
        // The path to a resource in a MacOS app bundle is different (in the parent dir of the executable)
        // from the path to the same resource in a Windows or Linux app.
        return RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
            ? path.Replace("Contents/MacOS/", "Contents/")
            : path;
    }
    
    #endregion
}