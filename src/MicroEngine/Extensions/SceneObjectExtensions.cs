/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Extensions;

using OpenTK.Mathematics;

/// <summary>
/// Scene object related extensions.
/// </summary>
public static class SceneObjectExtensions
{
    /// <summary>
    /// Sets rotation around X axis.
    /// </summary>
    /// <param name="sceneObject">A scene object.</param>
    /// <param name="angle">An angle in radians.</param>
    public static void SetRotationX(this ISceneObject sceneObject, float angle)
    {
        sceneObject.Rotation = new Vector3(angle, sceneObject.Rotation.Y, sceneObject.Rotation.Z);
    }
    
    /// <summary>
    /// Sets rotation around Y axis.
    /// </summary>
    /// <param name="sceneObject">A scene object.</param>
    /// <param name="angle">An angle in radians.</param>
    public static void SetRotationY(this ISceneObject sceneObject, float angle)
    {
        sceneObject.Rotation = new Vector3(sceneObject.Rotation.X, angle, sceneObject.Rotation.Z);
    }
    
    /// <summary>
    /// Sets rotation around Z axis.
    /// </summary>
    /// <param name="sceneObject">A scene object.</param>
    /// <param name="angle">An angle in radians.</param>
    public static void SetRotationZ(this ISceneObject sceneObject, float angle)
    {
        sceneObject.Rotation = new Vector3(sceneObject.Rotation.X, sceneObject.Rotation.Y, angle);
    }
    
    /// <summary>
    /// Try to get a scene from a scene object.
    /// </summary>
    /// <param name="sceneObject">A scene object.</param>
    /// <returns>A Scene instance a scene object belongs to.</returns>
    /// <exception cref="InvalidOperationException">When no scene object was found as a parent of a game object.</exception>
    public static Scene GetScene(this ISceneObject sceneObject)
    {
        while (true)
        {
            if (sceneObject is Scene scene)
            {
                return scene;
            }

            sceneObject = sceneObject.Parent ?? throw new InvalidOperationException("Scene not found.");
        }
    }

    /// <summary>
    /// Adds a child to a scene object.
    /// </summary>
    /// <param name="sceneObject">A scene object.</param>
    /// <param name="child">A child to be added.</param>
    /// <exception cref="InvalidOperationException">Thrown, when such child already exists in the scene object children.</exception>
    public static void AddChild(this ISceneObject sceneObject, ISceneObject child)
    {
        ArgumentNullException.ThrowIfNull(child);
        if (sceneObject.Children.Contains(child))
        {
            throw new InvalidOperationException("Child already exists in the parent object.");
        }
        
        child.Parent = sceneObject;
        sceneObject.Children.Add(child);
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
    /// <param name="sceneObject"></param>
    public static void BuildGeometry(this ISceneObject sceneObject)
    {
        sceneObject.Geometry.Build(sceneObject.Material.Shader);
    }
}
