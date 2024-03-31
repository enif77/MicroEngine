/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.SceneObjects;

using OpenTK.Mathematics;

using MicroEngine.Extensions;
using MicroEngine.Geometries;

/// <summary>
/// An axis-aligned boundary box, that is automatically changing its size, when its parent object changes its size or rotation.
/// </summary>
public sealed class DynamicAxisAlignedBoundaryBox : SceneObjectBase, IAxisAlignedBoundaryBox
{
    public Vector3 Min { get; private set; } = new(-0.5f, -0.5f, -0.5f);
    public Vector3 Max { get; private set; } = new(0.5f, 0.5f, 0.5f);

    
    public DynamicAxisAlignedBoundaryBox(ISceneObject parent)
    {
        Parent = parent ?? throw new ArgumentNullException(nameof(parent));
        Geometry = new SimpleIndexedLinesGeometry(
            _vertices,
            [
                0, 1,  // Front
                1, 2,
                2, 3,
                3, 0,
   
                4, 5,  // Back
                5, 6,
                6, 7,
                7, 4,
   
                0, 4,  // Connections
                1, 5,
                2, 6,
                3, 7
            ],
            true
        );
        
        Parent.AddChild(this);
    }
    
    
    // Buffer for the 8 vertices of the box.
    private readonly float[] _vertices =
    [
        -0.5f,  0.5f,  0.5f,
         0.5f,  0.5f,  0.5f,
         0.5f, -0.5f,  0.5f,
        -0.5f, -0.5f,  0.5f,
            
        -0.5f,  0.5f, -0.5f,
         0.5f,  0.5f, -0.5f,
         0.5f, -0.5f, -0.5f,
        -0.5f, -0.5f, -0.5f
    ];
    

    public override void Update(float deltaTime)
    {
        if (NeedsModelMatrixUpdate)
        {
            var parent = Parent!;
            
            // We are updating the geometry using world coordinates,
            // so we do not need any transformations here.
            // Note: This breaks the scene graph hierarchy, but we need to do it this way.
            ModelMatrix = Matrix4.Identity;
            
            // We need this to calculate Min/Max in world space.
            var worldMatrix = parent.ModelMatrix;
            
            // Min/max in world space.
            var minX = float.MaxValue;
            var minY = float.MaxValue;
            var minZ = float.MaxValue;
            
            var maxX = float.MinValue;
            var maxY = float.MinValue;
            var maxZ = float.MinValue;    
            
            // We need to transform all parent's vertices to world space.
            foreach (var vertex in parent.Geometry.GetVertices())
            {
                var transformedVertex = Vector3.TransformPosition(vertex, worldMatrix);
                
                if (transformedVertex.X < minX)
                {
                    minX = transformedVertex.X;
                }
                if (transformedVertex.Y < minY)
                {
                    minY = transformedVertex.Y;
                }
                if (transformedVertex.Z < minZ)
                {
                    minZ = transformedVertex.Z;
                }
                
                if (transformedVertex.X > maxX)
                {
                    maxX = transformedVertex.X;
                }
                if (transformedVertex.Y > maxY)
                {
                    maxY = transformedVertex.Y;
                }
                if (transformedVertex.Z > maxZ)
                {
                    maxZ = transformedVertex.Z;
                }
            }
            
            // We have the min/max in world space.
            Min = new Vector3(minX, minY, minZ);
            Max = new Vector3(maxX, maxY, maxZ);
            
            // Reusing a single array.
            _vertices[0] = minX;
            _vertices[1] = maxY;
            _vertices[2] = maxZ;
            
            _vertices[3] = maxX;
            _vertices[4] = maxY;
            _vertices[5] = maxZ;
            
            _vertices[6] = maxX;
            _vertices[7] = minY;
            _vertices[8] = maxZ;
            
            _vertices[9] = minX;
            _vertices[10] = minY;
            _vertices[11] = maxZ;
            
            _vertices[12] = minX;
            _vertices[13] = maxY;
            _vertices[14] = minZ;
            
            _vertices[15] = maxX;
            _vertices[16] = maxY;
            _vertices[17] = minZ;
            
            _vertices[18] = maxX;
            _vertices[19] = minY;
            _vertices[20] = minZ;
            
            _vertices[21] = minX;
            _vertices[22] = minY;
            _vertices[23] = minZ;
            
            Geometry.UpdateVertices(_vertices);
            
            NeedsModelMatrixUpdate = false;
        }
        
        foreach (var child in Children)
        {
            child.Update(deltaTime);
        }
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
