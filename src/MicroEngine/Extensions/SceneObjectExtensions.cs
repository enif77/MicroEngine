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
    /// Returns the world position of the center of a scene object.
    /// Forces the model matrix update if needed.
    /// </summary>
    /// <param name="sceneObject">A scene object.</param>
    /// <returns>The world position of the center of a scene object.</returns>
    public static Vector3 WorldPosition(this ISceneObject sceneObject)
    {
        if (sceneObject.NeedsModelMatrixUpdate)
        {
            sceneObject.UpdateModelMatrix();
        }

        // Translated vector is the vector at the center of the scene object geometry transformed by the model matrix.
        var translatedVector = sceneObject.ModelMatrix * Vector4.Zero;
        
        return new Vector3(translatedVector.X, translatedVector.Y, translatedVector.Z);
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
    /// <exception cref="InvalidOperationException">Thrown, when such child already exists in scene object children.</exception>
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
    /// Generates OpenGL buffers for this objects geometry. 
    /// </summary>
    /// <param name="sceneObject"></param>
    public static void BuildGeometry(this ISceneObject sceneObject)
    {
        sceneObject.Geometry.Build(sceneObject.Material.Shader);
    }
}
