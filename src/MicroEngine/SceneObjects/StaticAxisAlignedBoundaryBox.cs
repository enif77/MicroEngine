/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.SceneObjects;

using OpenTK.Mathematics;

/// <summary>
/// An axis-aligned boundary box, that is not automatically changing its size.
/// Its similar to the SceneObjectsGroup (hac no geometry etc.), but it is ignoring its rotation and its parent rotation.
/// </summary>
public class StaticAxisAlignedBoundaryBox : SceneObjectBase, IAxisAlignedBoundaryBox
{
    public Vector3 Min { get; private set; } = new Vector3(-0.5f, -0.5f, -0.5f);
    public Vector3 Max { get; private set; } = new Vector3(0.5f, 0.5f, 0.5f);
    
    
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
            Min = Vector3.TransformPosition(new Vector3(-0.5f, -0.5f, -0.5f), ModelMatrix);
            Max = Vector3.TransformPosition(new Vector3(0.5f, 0.5f, 0.5f), ModelMatrix);
            
            NeedsModelMatrixUpdate = false;
        }
        
        foreach (var child in Children)
        {
            child.Update(deltaTime);
        }
    }
}
