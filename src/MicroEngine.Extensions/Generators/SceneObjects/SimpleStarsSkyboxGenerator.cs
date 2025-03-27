/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Extensions.Generators.SceneObjects;

using OpenTK.Graphics.OpenGL4;

using MicroEngine.Core;
using MicroEngine.Materials;
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
    /// <param name="resourcesManager">A resource manager.</param>
    /// <param name="textureSizePixels">The side size of a texture used for this skybox. DefaultTextureSizePixels by default.</param>
    /// <param name="numberOfStars">A number of stars per skybox side to be generated. DefaultNumberOfStars by default.</param>
    /// <param name="minStarSize">A minimal size of a star generated. DefaultMinStartSize by default.</param>
    /// <param name="maxStarSize">A maximal size of a star generated. DefaultMaxStartSize by default.</param>
    /// <returns>A skybox.</returns>
    /// <exception cref="ArgumentOutOfRangeException">If textureSizePixels is les than 1.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If numberOfStars is les than 1.</exception>
    public static ISceneObject Generate(
        IResourcesManager resourcesManager,
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
        
        // RGBA texture
        var texture = new MicroEngine.Graphics.Texture(textureSizePixels, textureSizePixels);
        
        //var rand = new Random(5728);
        var rand = new Random();
        var textures = new ITexture[6];
        for (var t = 0; t < textures.Length; t++)
        {
            // Clear the texture with opaque black color.
            texture.Fill(0, 0, 0, 255);
            
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
                            texture.PutPixel(x + dx, y + dy, c, c, c);
                        }
                        else
                        {
                            var abs = (Math.Abs(dx) + Math.Abs(dy)) / 2.0f;
                            var cc = (byte)(c * (1.0f / abs));
                            
                            texture.PutPixel(x + dx, y + dy, cc, cc, cc);
                        }
                    }
                }
            }

            textures[t] = GlTexture.LoadFromRgbaBytes(texture.Pixels, textureSizePixels, textureSizePixels, TextureWrapMode.ClampToEdge);
        }
        
        var skybox = SkyboxGenerator.Generate(new MultiTextureMaterial(
            textures,
            new MultiTextureSkyboxShader(resourcesManager)));
        
        skybox.BuildGeometry();
        
        return skybox;
    }
}