namespace MicroEngine.Extensions;

using MicroEngine.SceneObjects;

/// <summary>
/// Scene related extensions.
/// </summary>
public static class SceneExtensions
{
    /// <summary>
    /// Adds a child to a game object.
    /// </summary>
    /// <param name="scene">A scene.</param>
    /// <param name="skybox">A skybox to be added to a scene.</param>
    /// <exception cref="InvalidOperationException">Thrown, when such child already exists in game object children.</exception>
    public static void AddSkybox(this Scene scene, Skybox skybox)
    {
        if (skybox == null) throw new ArgumentNullException(nameof(skybox));
        
        skybox.Parent = scene;
        scene.Skybox = skybox;
    }

    /// <summary>
    /// Adds a point light to the scene.
    /// </summary>
    /// <param name="scene">A scene.</param>
    /// <param name="light">A point light.</param>
    /// <param name="parent">An optional parent of the light.</param>
    /// <returns>A newly created point light.</returns>
    /// <exception cref="InvalidOperationException">If the MaxPointLights point lights are already in this scene.</exception>
    public static void AddLight(this Scene scene, ILight light, ISceneObject? parent = default)
    {
        ArgumentNullException.ThrowIfNull(light);
        
        if (scene.Lights.Count >= scene.MaxLights)
        {
            throw new InvalidOperationException($"Only {scene.MaxLights} lights are supported.");
        }
        
        scene.Lights.Add(light);
        
        parent ??= scene;
        parent.AddChild(light);
    }
}