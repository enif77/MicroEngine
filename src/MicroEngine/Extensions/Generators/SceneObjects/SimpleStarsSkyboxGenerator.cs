/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Extensions.Generators.SceneObjects;

using OpenTK.Graphics.OpenGL4;

using MicroEngine.Core;
using MicroEngine.Materials;
using MicroEngine.SceneObjects;
using MicroEngine.Shaders;


public static class SimpleStarsSkyboxGenerator
{
    public static ISceneObject Generate()
    {
        const int textureSize = 1024;
        const int nStars = 256;
        
        // RGBA texture buffer
        var texture = new byte[textureSize * textureSize * 4];

        // Clear the texture.
        for (var i = 0; i < texture.Length; i++)
        {
            texture[i] = 0;
        }

        //var rand = new Random(5728);
        var rand = new Random();
        var textures = new ITexture[6];
        for (var t = 0; t < textures.Length; t++)
        {
            for (var s = 0; s < nStars; s++)
            {
                // 4 and textureSize - 5 to avoid stars on the edges.
                var x = rand.Next(4, textureSize - 5);
                var y = rand.Next(4, textureSize - 5);
                var starSize = rand.Next(3, 7);
                var c = (byte)rand.Next(64, 256);
                // var r = (byte)rand.Next(64, 256);
                // var g = (byte)rand.Next(64, 256);
                // var b = (byte)rand.Next(64, 256);
                
                // Draw the star as a square
                for (var dx = -starSize / 2; dx < starSize / 2 + 1; dx++)
                {
                    for (var dy = -starSize / 2; dy < starSize / 2 + 1; dy++)
                    {
                        // Check if the coordinates are within the image bounds
                        if (0 <= x + dx && x + dx < textureSize && 0 <= y + dy && y + dy < textureSize)
                        {
                            var abs = (Math.Abs(dx) + Math.Abs(dy)) / 2.0f;
                            var cc = (abs == 0) 
                                ? c
                                : (byte)(c * (1.0f / abs));
                            
                            PutPixel(texture, textureSize, x + dx, y + dy, cc, cc, cc);
                        }
                    }
                }
            }

            textures[t] = Texture.LoadFromRgbaBytes(texture, textureSize, textureSize, TextureWrapMode.ClampToEdge);
            
            for (var i = 0; i < texture.Length; i++)
            {
                texture[i] = 0;
            }
        }
        
        var skybox = new MultiTextureSkyboxWithIndices(new MultiTextureMaterial(
            textures,
            new MultiTextureShader()));
        
        skybox.GenerateGeometry();
        
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