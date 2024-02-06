/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Extensions.Generators.SceneObjects;

using MicroEngine.Geometries;
using MicroEngine.SceneObjects;

public static class TexturedPlaneGenerator
{
    public static ISceneObject Generate(IMaterial material, float textureScale = 1.0f)
    {
        return new GenericSceneObject(new SingleTextureIndexedGeometry(
            [
                // Positions          Texture coords
                -0.5f, 0.0f,  0.5f,          0.0f,         0.0f,
                 0.5f, 0.0f,  0.5f,  textureScale,         0.0f,
                 0.5f, 0.0f, -0.5f,  textureScale, textureScale,
                -0.5f, 0.0f, -0.5f,          0.0f, textureScale,
            ],
            [
                // Each side has 2 triangles, each triangle has 3 vertices.
                0, 1, 2,  2, 3, 0
            ]))
        {
            Material = material ?? throw new ArgumentNullException(nameof(material))
        };
    }
}