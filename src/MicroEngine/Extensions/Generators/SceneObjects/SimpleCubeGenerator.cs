/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Extensions.Generators.SceneObjects;

using MicroEngine.Geometries;
using MicroEngine.SceneObjects;

public static class SimpleCubeGenerator
{
    public static ISceneObject Generate(IMaterial material)
    {
        return new GenericSceneObject(new SimpleIndexedGeometry(
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