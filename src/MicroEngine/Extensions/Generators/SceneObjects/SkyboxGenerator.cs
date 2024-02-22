/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Extensions.Generators.SceneObjects;

using MicroEngine.Geometries;
using MicroEngine.SceneObjects;

/// <summary>
/// Generator of a skybox that uses 6 textures.
/// </summary>
public static class SkyboxGenerator
{
    /// <summary>
    /// Generates a skybox.
    /// </summary>
    /// <param name="material">A material with 6 textures, that will be used by the generated skybox.</param>
    /// <returns>A skybox.</returns>
    /// <exception cref="ArgumentNullException">Thrown, if the material argument is null.</exception>
    public static ISceneObject Generate(IMaterial material)
    {
        return new GenericSkybox(new MultiTextureIndexedGeometry(
            [
                // Positions          Texture ID and coords
                -0.5f, -0.5f, -0.5f,  0,  0.0f, 0.0f,  // Front face.
                0.5f, -0.5f, -0.5f,  0,  1.0f, 0.0f,
                0.5f,  0.5f, -0.5f,  0,  1.0f, 1.0f,
                -0.5f,  0.5f, -0.5f,  0,  0.0f, 1.0f,
        
                0.5f, -0.5f, -0.5f,  1,  0.0f, 0.0f,  // Right face.
                0.5f, -0.5f,  0.5f,  1,  1.0f, 0.0f,
                0.5f,  0.5f,  0.5f,  1,  1.0f, 1.0f,
                0.5f,  0.5f, -0.5f,  1,  0.0f, 1.0f,
            
                0.5f, -0.5f,  0.5f,  2,  0.0f, 0.0f,  // Back face.
                -0.5f, -0.5f,  0.5f,  2,  1.0f, 0.0f,
                -0.5f,  0.5f,  0.5f,  2,  1.0f, 1.0f,
                0.5f,  0.5f,  0.5f,  2,  0.0f, 1.0f,
        
                -0.5f, -0.5f,  0.5f,  3,  0.0f, 0.0f,  // Left face.
                -0.5f, -0.5f, -0.5f,  3,  1.0f, 0.0f,
                -0.5f,  0.5f, -0.5f,  3,  1.0f, 1.0f,
                -0.5f,  0.5f,  0.5f,  3,  0.0f, 1.0f,
        
                -0.5f,  0.5f, -0.5f,  4,  0.0f, 0.0f,  // Top face.
                0.5f,  0.5f, -0.5f,  4,  1.0f, 0.0f,
                0.5f,  0.5f,  0.5f,  4,  1.0f, 1.0f,
                -0.5f,  0.5f,  0.5f,  4,  0.0f, 1.0f,
            
                -0.5f, -0.5f,  0.5f,  5,  0.0f, 0.0f,  // Bottom face.
                0.5f, -0.5f,  0.5f,  5,  1.0f, 0.0f,
                0.5f, -0.5f, -0.5f,  5,  1.0f, 1.0f,
                -0.5f, -0.5f, -0.5f,  5,  0.0f, 1.0f,
            ],
            [
                // Each side has 2 triangles, each triangle has 3 vertices.
                0,  1,  2,   2,  3,  0, // Front
                4,  5,  6,   6,  7,  4, // Right
                8,  9, 10,  10, 11,  8, // Back
                12, 13, 14,  14, 15, 12, // Left
                16, 17, 18,  18, 19, 16, // Top
                20, 21, 22,  22, 23, 20  // Bottom
            ]))
        {
            Material = material ?? throw new ArgumentNullException(nameof(material))
        };
    }
}