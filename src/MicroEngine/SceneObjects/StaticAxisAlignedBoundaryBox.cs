/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.SceneObjects;

using OpenTK.Mathematics;

/// <summary>
/// An axis-aligned boundary box, that is not automatically changing its size.
/// Its similar to the SceneObjectsGroup (hac no geometry etc.), but it is ignoring its rotation and its parent rotation.
/// </summary>
public class StaticAxisAlignedBoundaryBox : SceneObjectBase, IAxisAlignedBoundaryBox
{
    private static readonly Vector3 MinVertex = new Vector3(-0.5f, -0.5f, -0.5f);
    private static readonly Vector3 MaxVertex = new Vector3(0.5f, 0.5f, 0.5f);
    
    public Vector3 Min { get; private set; } = MinVertex;
    public Vector3 Max { get; private set; } = MaxVertex;
    
    
    public override void Update(float deltaTime)
    {
        if (NeedsModelMatrixUpdate)
        {
            // We are updating the geometry using world coordinates,
            // so we do not need any transformations here.
            // Note: This breaks the scene graph hierarchy, but we need to do it this way.
            ModelMatrix = Matrix4.Identity;
            
            // Update the ModelMatrix, ignoring the rotation.
            var worldMatrix = Matrix4.CreateScale(Scale);
            worldMatrix *= Matrix4.CreateTranslation(Position);
        
            if (Parent != null)
            {
                // Combine with the scale and the translation of the parent scene object.
                worldMatrix *= Parent.ModelMatrix.ClearRotation();
            }
            
            // Update the Min and Max values.
            Min = Vector3.TransformPosition(MinVertex, worldMatrix);
            Max = Vector3.TransformPosition(MaxVertex, worldMatrix);
            
            NeedsModelMatrixUpdate = false;
        }
        
        foreach (var child in Children)
        {
            child.Update(deltaTime);
        }
    }
}
