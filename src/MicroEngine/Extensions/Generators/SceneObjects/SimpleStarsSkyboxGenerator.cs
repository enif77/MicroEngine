/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Extensions.Generators.SceneObjects;

using OpenTK.Graphics.OpenGL4;

using MicroEngine.Core;
using MicroEngine.Materials;
using MicroEngine.SceneObjects;
using MicroEngine.Shaders;

/// <summary>
/// A simple stars skybox generator.
/// </summary>
public static class SimpleStarsSkyboxGenerator
{
    public const int DefaultTextureSizePixels = 1024;
    public const int DefaultNumberOfStars = 256;
    public const int DefaultMinStartSize = 3;
    public const int DefaultMaxStartSize = 7;
    
    /// <summary>
    /// Generates a simple stars skybox.
    /// </summary>
    /// <param name="textureSizePixels">The side size of a texture used for this skybox. DefaultTextureSizePixels by default.</param>
    /// <param name="numberOfStars">A number of stars per skybox side to be generated. DefaultNumberOfStars by default.</param>
    /// <param name="minStarSize">A minimal size of a star generated. DefaultMinStartSize by default.</param>
    /// <param name="maxStarSize">A maximal size of a star generated. DefaultMaxStartSize by default.</param>
    /// <returns>A skybox.</returns>
    /// <exception cref="ArgumentOutOfRangeException">If textureSizePixels is les than 1.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If numberOfStars is les than 1.</exception>
    public static ISceneObject Generate(
        int textureSizePixels = DefaultTextureSizePixels,
        int numberOfStars = DefaultNumberOfStars,
        int minStarSize = DefaultMinStartSize,
        int maxStarSize = DefaultMaxStartSize)
    {
        if (textureSizePixels < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(textureSizePixels), "The texture size must be at least 1.");
        }
        
        if (numberOfStars < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(numberOfStars), "The number of stars must be at least 1.");
        }
        
        if (minStarSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(numberOfStars), "The minimal size of a star must be at least 1.");
        }
        
        if (maxStarSize < 1 || maxStarSize < minStarSize)
        {
            throw new ArgumentOutOfRangeException(nameof(numberOfStars), "The maximal size of a star must be at least 1 and greater than the minimal size.");
        }
        
        if (maxStarSize > textureSizePixels / 2)
        {
            throw new ArgumentOutOfRangeException(nameof(numberOfStars), "The maximal size of a star must be at most half of the texture size.");
        }
        
        // RGBA texture buffer
        var texture = new byte[textureSizePixels * textureSizePixels * 4];

        // Clear the texture.
        for (var i = 0; i < texture.Length; i += 4)
        {
            texture[i + 3] = 255;
        }

        //var rand = new Random(5728);
        var rand = new Random();
        var textures = new ITexture[6];
        for (var t = 0; t < textures.Length; t++)
        {
            for (var s = 0; s < numberOfStars; s++)
            {
                // Star size,
                var starSize = rand.Next(minStarSize, maxStarSize);
                
                // A random position, but avoiding stars on the edges.
                var x = rand.Next(starSize, textureSizePixels - starSize - 1);
                var y = rand.Next(starSize, textureSizePixels - starSize - 1);

                // A random star light intensity. Bigger stars can be brighter.
                var c = (starSize == maxStarSize)
                    ? (byte)rand.Next(128, 256)
                    : (byte)rand.Next(32, 200);
                
                // Draw the star as a square
                for (var dx = -starSize / 2; dx < starSize / 2 + 1; dx++)
                {
                    for (var dy = -starSize / 2; dy < starSize / 2 + 1; dy++)
                    {
                        if (dx == 0 && dy == 0)
                        {
                            PutPixel(texture, textureSizePixels, x + dx, y + dy, c, c, c);
                        }
                        else
                        {
                            var abs = (Math.Abs(dx) + Math.Abs(dy)) / 2.0f;
                            var cc = (byte)(c * (1.0f / abs));
                            
                            PutPixel(texture, textureSizePixels, x + dx, y + dy, cc, cc, cc);
                        }
                    }
                }
            }

            textures[t] = Texture.LoadFromRgbaBytes(texture, textureSizePixels, textureSizePixels, TextureWrapMode.ClampToEdge);
            
            for (var i = 0; i < texture.Length; i += 4)
            {
                texture[i] = 0;
                texture[i + 1] = 0;
                texture[i + 2] = 0;
                texture[i + 3] = 255;
            }
        }
        
        var skybox = SkyboxGenerator.Generate(new MultiTextureMaterial(
            textures,
            new MultiTextureSkyboxShader()));
        
        skybox.BuildGeometry();
        
        return skybox;
    }

    
    private static void PutPixel(byte[] texture, int textureSize, int x, int y, byte r, byte g, byte b, byte a = 255)
    {
        var index = (x + y * textureSize) * 4;
        texture[index] = r;
        texture[index + 1] = g;
        texture[index + 2] = b;
        texture[index + 3] = a;
    }
}