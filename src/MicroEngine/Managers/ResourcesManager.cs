/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Managers;

using System.Runtime.InteropServices;

using OpenTK.Graphics.OpenGL4;

using MicroEngine.Core;
using MicroEngine.Graphics;
using MicroEngine.Materials;
using MicroEngine.OGL;

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
    
    
    public byte[] LoadBinaryFile(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("A path to a text file expected.");
        }

        return File.ReadAllBytes(GetFullPath(path));
    }
    
    #endregion
    
    
    #region textures
    
    private readonly ITexture _nullTexture = NullTexture.Instance;
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


    public ITexture LoadTexture(string name, string path, TextureWrapMode wrapMode = TextureWrapMode.Repeat)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("A texture name is null or empty.");
        }
        
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("A path to a texture is null or empty.");
        }
        
        return _textures.TryGetValue(name, out var value)
            ? value
            : LoadTextureInternal(name, LoadImageFromBmp(GetFullPath(path)), wrapMode);
    }
    
    
    public ITexture LoadTexture(string name, Image image, TextureWrapMode wrapMode = TextureWrapMode.Repeat)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("A texture name is null or empty.");
        }
        
        ArgumentNullException.ThrowIfNull(image, nameof(image));
        
        return _textures.TryGetValue(name, out var value)
            ? value
            : LoadTextureInternal(name, image, wrapMode);
    }
    
    
    private ITexture LoadTextureInternal(string name, Image image, TextureWrapMode wrapMode)
    {
        var texture = GlTexture.LoadFromRgbaBytes(image.Pixels, image.Width, image.Height, wrapMode);
        _textures.Add(name, texture);
        
        return texture;
    }
    
    #endregion
    
    
    #region materials
    
    private readonly IMaterial _nullMaterial = Material.Create();
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
    
    private readonly IShader _nullShader = NullShader.Instance;
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

        shader.Build();
        
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
    
    private readonly IGeometry _nullGeometry = NullGeometry.Instance;
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
    
    
    private static Image LoadImageFromBmp(string filePath)
    {
        using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            using (var reader = new BinaryReader(fs))
            {
                // BMP Header
                if (reader.ReadUInt16() != 0x4D42) // 'BM' signature
                {
                    throw new InvalidOperationException("Invalid file format.");
                }

                reader.BaseStream.Seek(18, SeekOrigin.Begin); // Move to width/height in the header
                var width = reader.ReadInt32(); // Image width
                var height = reader.ReadInt32(); // Image height

                reader.BaseStream.Seek(28, SeekOrigin.Begin); // Move to bits per pixel

                var bitsPerPixel = (int)reader.ReadUInt16();
                
                // BGR
                if (bitsPerPixel == 24) 
                {
                    reader.BaseStream.Seek(54, SeekOrigin.Begin); // Move to pixel data

                    var rowSize = width * 3; // Size of one row in bytes (RGB = 3 bytes per pixel)
                    var paddingSize = (4 - (rowSize % 4)) % 4; // Padding to make row size divisible by 4

                    var imageData = new byte[width * height * 4]; // RGBA format (4 bytes per pixel)

                    // BMP stores pixel data bottom-to-top
                    var ty = 0;
                    for (var y = height - 1; y >= 0; y--)
                    {
                        for (var x = 0; x < width; x++)
                        {
                            var pixelIndex = (ty * width + x) * 4;

                            imageData[pixelIndex + 2] = reader.ReadByte(); // Blue component
                            imageData[pixelIndex + 1] = reader.ReadByte(); // Green component
                            imageData[pixelIndex + 0] = reader.ReadByte(); // Red component
                            imageData[pixelIndex + 3] = 255;               // Alpha component
                        }

                        ty++;
                        
                        // Skip padding bytes
                        reader.BaseStream.Seek(paddingSize, SeekOrigin.Current);
                    }

                    return new Image(width, height, imageData);
                }
                
                // BGRA
                if (bitsPerPixel == 32)
                {
                    reader.BaseStream.Seek(54, SeekOrigin.Begin);
                    
                    var imageData = new byte[width * height * 4]; 
                    
                    var ty = 0;
                    for (var y = height - 1; y >= 0; y--)
                    {
                        for (var x = 0; x < width; x++)
                        {
                            var pixelIndex = (ty * width + x) * 4;

                            imageData[pixelIndex + 2] = reader.ReadByte(); // Blue component
                            imageData[pixelIndex + 1] = reader.ReadByte(); // Green component
                            imageData[pixelIndex + 0] = reader.ReadByte(); // Red component
                            imageData[pixelIndex + 3] = reader.ReadByte(); // Alpha component
                        }
                        
                        ty++;
                    }

                    return new Image(width, height, imageData);
                }
                
                throw new InvalidOperationException("Only 24-bit or 32-bit BMP files are supported.");
            }
        }
    }
    
    #endregion
}