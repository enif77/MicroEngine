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
        
        Parent.AddChild(this);
    }
    
    
    public override void Update(float deltaTime)
    {
        if (NeedsModelMatrixUpdate)
        {
            var parent = Parent!;
            
            // We need this to calculate Min/Max in world space.
            ModelMatrix = parent.ModelMatrix;
            
            // Min/max in world space.
            var minX = float.MaxValue;
            var minY = float.MaxValue;
            var minZ = float.MaxValue;
            
            var maxX = float.MinValue;
            var maxY = float.MinValue;
            var maxZ = float.MinValue;    
            
            // We need this to calculate min/max in parent's local space.
            var localMatrix = Matrix4.CreateRotationZ(parent.Rotation.Z);  // Just the parent's rotation.
            localMatrix *= Matrix4.CreateRotationX(parent.Rotation.X);
            localMatrix *= Matrix4.CreateRotationY(parent.Rotation.Y);
            
            // Min/max in parent's local space.
            var localMinX = float.MaxValue;
            var localMinY = float.MaxValue;
            var localMinZ = float.MaxValue;
            
            var localMaxX = float.MinValue;
            var localMaxY = float.MinValue;
            var localMaxZ = float.MinValue;
            
            foreach (var vertex in parent.Geometry.GetVertices())
            {
                var transformedVertex = Vector3.TransformPosition(vertex, ModelMatrix);
                
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
                
                var transformedLocalVertex = Vector3.TransformPosition(vertex, localMatrix);
                
                if (transformedLocalVertex.X < localMinX)
                {
                    localMinX = transformedLocalVertex.X;
                }
                if (transformedLocalVertex.Y < localMinY)
                {
                    localMinY = transformedLocalVertex.Y;
                }
                if (transformedLocalVertex.Z < localMinZ)
                {
                    localMinZ = transformedLocalVertex.Z;
                }
                
                if (transformedLocalVertex.X > localMaxX)
                {
                    localMaxX = transformedLocalVertex.X;
                }
                if (transformedLocalVertex.Y > localMaxY)
                {
                    localMaxY = transformedLocalVertex.Y;
                }
                if (transformedLocalVertex.Z > localMaxZ)
                {
                    localMaxZ = transformedLocalVertex.Z;
                }
            }
            
            Min = new Vector3(minX, minY, minZ);
            Max = new Vector3(maxX, maxY, maxZ);
            
            // TODO: Reuse a single array.
            Geometry.UpdateVertices([
                Min.X, Max.Y, Max.Z,
                Max.X, Max.Y, Max.Z,
                Max.X, Min.Y, Max.Z,
                Min.X, Min.Y, Max.Z,
            
                Min.X, Max.Y, Min.Z,
                Max.X, Max.Y, Min.Z,
                Max.X, Min.Y, Min.Z,
                Min.X, Min.Y, Min.Z
            ]);
            
            NeedsModelMatrixUpdate = false;
        }
        
        foreach (var child in Children)
        {
            child.Update(deltaTime);
        }
    }
    
}
