/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.SceneObjects;

using MicroEngine.Extensions;
using MicroEngine.Geometries;

/// <summary>
/// Horizontal plane with a single texture. No lighting is applied.
/// </summary>
public class SingleTexturePlaneWithIndices : SceneObjectBase
{
    public SingleTexturePlaneWithIndices(IMaterial material, float textureScale = 1.0f)
    {
        Geometry = new SingleTextureGeometry(
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
        ]);
        
        Material = material ?? throw new ArgumentNullException(nameof(material));
    }
    

    private Scene? _scene;

    public override void Render()
    {
        _scene ??= this.GetScene();

        Material.Shader.Use(_scene, this);
        
        Geometry.Render();
       
        base.Render();
    }
}
