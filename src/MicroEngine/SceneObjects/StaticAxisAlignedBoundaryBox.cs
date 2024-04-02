/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.SceneObjects;

using OpenTK.Mathematics;

using MicroEngine.Extensions;
using MicroEngine.Geometries;

/// <summary>
/// An axis-aligned boundary box, that is not automatically changing its size.
/// Its similar to the SceneObjectsGroup (hac no geometry etc.), but it is ignoring its rotation and its parent rotation.
/// </summary>
public class StaticAxisAlignedBoundaryBox : SceneObjectBase, IAxisAlignedBoundaryBox
{
    private static readonly Vector3 MinVertex = new(-0.5f, -0.5f, -0.5f);
    private static readonly Vector3 MaxVertex = new(0.5f, 0.5f, 0.5f);
    
    public Vector3 Min { get; private set; } = MinVertex;
    public Vector3 Max { get; private set; } = MaxVertex;
    
    
    public StaticAxisAlignedBoundaryBox()
    {
        Geometry = new SimpleIndexedLinesGeometry(
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
            ]
        );
    }
    
    
    public override void Update(float deltaTime)
    {
        if (NeedsModelMatrixUpdate)
        {
            // Update the ModelMatrix, ignoring the rotation.
            ModelMatrix = Matrix4.CreateScale(Scale);
            ModelMatrix *= Matrix4.CreateTranslation(Position);
        
            if (Parent != null)
            {
                // Combine with the scale and the translation of the parent scene object.
                ModelMatrix *= Parent.ModelMatrix.ClearRotation();
            }
            
            // Update the Min and Max values.
            Min = Vector3.TransformPosition(MinVertex, ModelMatrix);
            Max = Vector3.TransformPosition(MaxVertex, ModelMatrix);
            
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
