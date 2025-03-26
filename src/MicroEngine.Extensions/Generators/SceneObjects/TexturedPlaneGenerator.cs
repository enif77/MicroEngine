/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Extensions.Generators.SceneObjects;

using MicroEngine.Geometries;
using MicroEngine.SceneObjects;

/// <summary>
/// Generator of textured planes.
/// </summary>
public static class TexturedPlaneGenerator
{
    /// <summary>
    /// Generates a textured plane with normals for lighting.
    /// </summary>
    /// <param name="material">A material that will be used by the generated plane.</param>
    /// <param name="textureScale">How many times will be texture repeated on the plane. 1.0 means that the texture will cover the whole plane just once.</param>
    /// <returns>A scene object with geometry representing a textured plane.</returns>
    /// <exception cref="ArgumentNullException">Thrown, if the material argument is null.</exception>
    public static ISceneObject Generate(IMaterial material, float textureScale = 1.0f)
    {
        return new RenderableSceneObject(new DefaultIndexedGeometry(
            [
                // Positions         Normals              Texture coords
                -0.5f, 0.0f,  0.5f,  0.0f,  1.0f,  0.0f,          0.0f,         0.0f,
                 0.5f, 0.0f,  0.5f,  0.0f,  1.0f,  0.0f,  textureScale,         0.0f,
                 0.5f, 0.0f, -0.5f,  0.0f,  1.0f,  0.0f,  textureScale, textureScale,
                -0.5f, 0.0f, -0.5f,  0.0f,  1.0f,  0.0f,          0.0f, textureScale,
            ],
            [
                // Each side has 2 triangles, each triangle has 3 vertices.
                0, 1, 2,  2, 3, 0
            ]))
        {
            Material = material ?? throw new ArgumentNullException(nameof(material))
        };
    }
    
    /// <summary>
    /// Generates a textured plane without normals for lighting.
    /// </summary>
    /// <param name="material">A material that will be used by the generated plane.</param>
    /// <param name="textureScale">How many times will be texture repeated on the plane. 1.0 means that the texture will cover the whole plane just once.</param>
    /// <returns>A scene object with geometry representing a textured plane.</returns>
    /// <exception cref="ArgumentNullException">Thrown, if the material argument is null.</exception>
    public static ISceneObject GenerateWithSimpleTextureGeometry(IMaterial material, float textureScale = 1.0f)
    {
        return new RenderableSceneObject(new SingleTextureIndexedGeometry(
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