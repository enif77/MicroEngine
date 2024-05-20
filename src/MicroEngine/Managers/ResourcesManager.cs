/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Managers;

using System.Runtime.InteropServices;

using OpenTK.Graphics.OpenGL4;

using MicroEngine.Core;
using MicroEngine.Geometries;
using MicroEngine.Materials;
using MicroEngine.Shaders;
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


    public bool HasTexture(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("A texture name expected.");
        }

        return _textures.ContainsKey(name);
    }
    
    
    public ITexture GetTexture(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("A texture name expected.");
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
    
    
    #region materials
    
    private readonly IMaterial _nullMaterial = new NullMaterial();
    private readonly Dictionary<string, IMaterial> _materials = new();
    
    
    public bool HasMaterial(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("A material name expected.");
        }

        return _materials.ContainsKey(name);
    }
    
    
    public IMaterial GetMaterial(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("A material name expected.");
        }

        return _materials.GetValueOrDefault(name, _nullMaterial);
    }
    
    
    public bool LoadMaterial(string name, IMaterial material)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("A material name expected.");
        }

        ArgumentNullException.ThrowIfNull(material);

        return _materials.TryAdd(name, material);
    }
    
    #endregion
    
    
    #region shaders
    
    private readonly IShader _nullShader = new NullShader();
    private readonly Dictionary<string, IShader> _shaders = new();
    
    
    public bool HasShader(string name)
    {
        CheckShaderName(name);

        return _shaders.ContainsKey(name);
    }
    
    
    public IShader GetShader(string name)
    {
        CheckShaderName(name);

        return _shaders.GetValueOrDefault(name, _nullShader);
    }
    
    
    public bool LoadShader(string name, IShader shader)
    {
        CheckShaderName(name);
        ArgumentNullException.ThrowIfNull(shader);

        return _shaders.TryAdd(name, shader);
    }
    
    
    private static void CheckShaderName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("A shader name expected.");
        }
    }
    
    #endregion
    
    
    #region geometries
    
    private readonly IGeometry _nullGeometry = new NullGeometry();
    private readonly Dictionary<string, IGeometry> _geometries = new();
    
    
    public bool HasGeometry(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("A geometry name expected.");
        }

        return _geometries.ContainsKey(name);
    }
    
    
    public IGeometry GetGeometry(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("A geometry name expected.");
        }

        return _geometries.GetValueOrDefault(name, _nullGeometry);
    }
    
    
    public bool LoadGeometry(string name, IGeometry geometry)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("A geometry name expected.");
        }

        ArgumentNullException.ThrowIfNull(geometry);

        return _geometries.TryAdd(name, geometry);
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
    
    
    private static string OsSafePath(string path)
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