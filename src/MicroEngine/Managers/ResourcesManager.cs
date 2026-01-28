/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Managers;

using System.Runtime.InteropServices;

using OpenTK.Graphics.OpenGL;

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
    /// Gets an instance of the default resources manager.
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
        return string.IsNullOrWhiteSpace(path)
            ? throw new ArgumentException("A path to a file expected.")
            : File.Exists(GetFullPath(path));
    }
    
    
    public string LoadTextFile(string path)
    {
        return string.IsNullOrWhiteSpace(path)
            ? throw new ArgumentException("A path to a text file expected.")
            : File.ReadAllText(GetFullPath(path));
    }
    
    
    public byte[] LoadBinaryFile(string path)
    {
        return string.IsNullOrWhiteSpace(path)
            ? throw new ArgumentException("A path to a text file expected.")
            : File.ReadAllBytes(GetFullPath(path));
    }


    public Image LoadBmpImage(string path)
    {
        return string.IsNullOrWhiteSpace(path)
            ? throw new ArgumentException("A path to a BMP file expected.")
            : LoadImageFromBmp(GetFullPath(path));
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
                // BMP file header (BITMAPFILEHEADER)
                if (reader.ReadUInt16() != 0x4D42) // 'BM'
                {
                    throw new InvalidOperationException("Invalid BMP signature.");
                }

                reader.ReadUInt32();                // bfSize (unused)
                reader.ReadUInt16();                // bfReserved1
                reader.ReadUInt16();                // bfReserved2
                var pixelDataOffset = reader.ReadUInt32(); // bfOffBits

                // DIB header (assume at least BITMAPINFOHEADER layout)
                var dibHeaderSize = reader.ReadUInt32();
                if (dibHeaderSize < 40)
                {
                    throw new InvalidOperationException($"Unsupported BMP DIB header size: {dibHeaderSize}.");
                }

                var width = reader.ReadInt32();
                var heightSigned = reader.ReadInt32();
                var planes = reader.ReadUInt16();
                var bitsPerPixel = reader.ReadUInt16();
                var compression = reader.ReadUInt32();

                if (planes != 1)
                {
                    throw new InvalidOperationException($"Unsupported BMP planes value: {planes} (expected 1).");
                }

                // We only support uncompressed BI_RGB for 24/32bpp.
                const uint BI_RGB = 0;
                const uint BI_RGBA = 3;
                if (compression != BI_RGB && compression != BI_RGBA)
                {
                    throw new InvalidOperationException($"Unsupported BMP compression: {compression} (only BI_RGB and BI_RGBA supported).");
                }

                if (width <= 0 || heightSigned == 0)
                {
                    throw new InvalidOperationException($"Invalid BMP dimensions: {width}x{heightSigned}.");
                }

                var topDown = heightSigned < 0;
                var height = Math.Abs(heightSigned);

                if (bitsPerPixel != 24 && bitsPerPixel != 32)
                {
                    throw new InvalidOperationException("Only 24-bit or 32-bit BMP files are supported.");
                }

                var bytesPerPixelSrc = bitsPerPixel / 8;

                int stride;
                int dstSize;
                checked
                {
                    var rowBytes = width * bytesPerPixelSrc;
                    stride = ((rowBytes + 3) / 4) * 4;     // BMP rows are aligned to 4 bytes
                    dstSize = width * height * 4;          // RGBA
                }

                if (pixelDataOffset >= (ulong)reader.BaseStream.Length)
                {
                    throw new InvalidOperationException("BMP pixel data offset is outside of the file.");
                }

                long requiredBytes;
                checked
                {
                    requiredBytes = (long)pixelDataOffset + (long)stride * height;
                }

                if (requiredBytes > reader.BaseStream.Length)
                {
                    throw new InvalidOperationException("BMP file is truncated: not enough data for declared dimensions.");
                }

                reader.BaseStream.Seek((long)pixelDataOffset, SeekOrigin.Begin);

                var imageData = new byte[dstSize];
                var row = new byte[stride];

                for (var rowIndex = 0; rowIndex < height; rowIndex++)
                {
                    // Source row order depends on the sign of height.
                    var dstY = !topDown ? rowIndex : (height - 1 - rowIndex);

                    var read = reader.Read(row, 0, stride);
                    if (read != stride)
                    {
                        throw new EndOfStreamException("Unexpected end of BMP while reading pixel data.");
                    }

                    var src = 0;
                    for (var x = 0; x < width; x++)
                    {
                        var dstIndex = (dstY * width + x) * 4;

                        // BMP stores BGR(A)
                        var b = row[src + 0];
                        var g = row[src + 1];
                        var r = row[src + 2];
                        var a = (bytesPerPixelSrc == 4) ? row[src + 3] : (byte)255;

                        imageData[dstIndex + 0] = r;
                        imageData[dstIndex + 1] = g;
                        imageData[dstIndex + 2] = b;
                        imageData[dstIndex + 3] = a;

                        src += bytesPerPixelSrc;
                    }
                }

                return new Image(width, height, imageData);
            }
        }
    }
    
    #endregion
}