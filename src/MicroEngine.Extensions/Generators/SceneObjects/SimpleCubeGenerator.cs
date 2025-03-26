/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Extensions.Generators.SceneObjects;

using MicroEngine.Geometries;
using MicroEngine.SceneObjects;

/// <summary>
/// Generator of simple cubes.
/// </summary>
public static class SimpleCubeGenerator
{
    /// <summary>
    /// Generates a cube as 8 vertices connected with indices.
    /// </summary>
    /// <param name="material">A material, that will be used by the generated cube.</param>
    /// <returns>A scene object with geometry representing a simple cube.</returns>
    public static ISceneObject Generate(IMaterial material)
    {
        return new RenderableSceneObject(new SimpleIndexedGeometry(
            [
                -0.5f,  0.5f,  0.5f,
                 0.5f,  0.5f,  0.5f,
                 0.5f, -0.5f,  0.5f,
                -0.5f, -0.5f,  0.5f,
            
                -0.5f,  0.5f, -0.5f,
                 0.5f,  0.5f, -0.5f,
                 0.5f, -0.5f, -0.5f,
                -0.5f, -0.5f, -0.5f
            ],
            [
                // Each side has 2 triangles, each triangle has 3 vertices.
                0, 3, 2,  2, 1, 0, // Front
                1, 2, 6,  6, 5, 1, // Right
                5, 6, 7,  7, 4, 5, // Back
                4, 7, 3,  3, 0, 4, // Left
                0, 1, 5,  5, 4, 0, // Top
                6, 2, 3,  3, 7, 6  // Bottom
            ]
        ))
        {
            Material = material    
        };
    }
}