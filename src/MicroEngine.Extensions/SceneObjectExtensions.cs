/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Extensions;

using OpenTK.Mathematics;

/// <summary>
/// Scene object related extensions.
/// </summary>
public static class SceneObjectExtensions
{
    /// <summary>
    /// Adds a child to a scene object.
    /// </summary>
    /// <param name="sceneObject">A scene object.</param>
    /// <param name="child">A child to be added.</param>
    /// <returns>The added child.</returns>
    /// <exception cref="InvalidOperationException">Thrown, when such child already exists in the scene object children.</exception>
    public static ISceneObject AddChild(this ISceneObject sceneObject, ISceneObject child)
    {
        ArgumentNullException.ThrowIfNull(child);
        if (sceneObject.Children.Contains(child))
        {
            throw new InvalidOperationException("Child already exists in the parent object.");
        }
        
        child.Parent = sceneObject;
        sceneObject.Children.Add(child);
        
        return child;
    }
    
    /// <summary>
    /// Removes a child from a scene object.
    /// </summary>
    /// <param name="sceneObject">A scene object.</param>
    /// <param name="child">A child to be removed.</param>
    /// <exception cref="InvalidOperationException">Thrown, when such child does not exist in the scene object children.</exception>
    public static void RemoveChild(this ISceneObject sceneObject, ISceneObject child)
    {
        ArgumentNullException.ThrowIfNull(child);
        if (sceneObject.Children.Contains(child) == false)
        {
            throw new InvalidOperationException("Child does not exist in the parent object.");
        }
        
        child.Parent = null;
        sceneObject.Children.Remove(child);
    }
    
    /// <summary>
    /// Generates OpenGL buffers for this objects geometry. 
    /// </summary>
    /// <param name="sceneObject">A scene object.</param>
    public static void BuildGeometry(this ISceneObject sceneObject)
    {
        sceneObject.Geometry.Build(sceneObject.Material.Shader);
    }

    /// <summary>
    /// Calculates the distance to another scene object.
    /// </summary>
    /// <param name="sceneObject">A scene object.</param>
    /// <param name="otherSceneObject">Another scene object.</param>
    /// <returns></returns>
    public static float DistanceTo(this ISceneObject sceneObject, ISceneObject otherSceneObject)
    {
        if (sceneObject.NeedsModelMatrixUpdate || otherSceneObject.NeedsModelMatrixUpdate)
        {
            throw new InvalidOperationException("Scene object model matrix needs to be updated.");
        }
       
        return Vector3.Distance(
            sceneObject.ModelMatrix.ExtractTranslation(),
            otherSceneObject.ModelMatrix.ExtractTranslation());
    }
}
