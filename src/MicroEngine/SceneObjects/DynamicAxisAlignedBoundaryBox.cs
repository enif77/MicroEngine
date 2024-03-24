/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.SceneObjects;

using OpenTK.Mathematics;

using MicroEngine.Extensions;

/// <summary>
/// An axis-aligned boundary box, that is automatically changing its size, when its parent object changes its size or rotation.
/// </summary>
public sealed class DynamicAxisAlignedBoundaryBox : SceneObjectBase, IAxisAlignedBoundaryBox
{
    public Vector3 Min { get; private set; } = new Vector3(-0.5f, -0.5f, -0.5f);
    public Vector3 Max { get; private set; } = new Vector3(0.5f, 0.5f, 0.5f);

    
    public DynamicAxisAlignedBoundaryBox(ISceneObject parent)
    {
        Parent = parent ?? throw new ArgumentNullException(nameof(parent));
        Parent.AddChild(this);
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
            var minX = float.MaxValue;
            var minY = float.MaxValue;
            var minZ = float.MaxValue;
            
            var maxX = float.MinValue;
            var maxY = float.MinValue;
            var maxZ = float.MinValue;    
            
            foreach (var vertex in Geometry.GetVertices())
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
            }
            
            Min = new Vector3(minX, minY, minZ);
            Max = new Vector3(maxX, maxY, maxZ);
            
            NeedsModelMatrixUpdate = false;
        }
        
        foreach (var child in Children)
        {
            child.Update(deltaTime);
        }
    }
    
}
