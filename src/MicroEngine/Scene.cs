/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine;

using OpenTK.Mathematics;

using MicroEngine.SceneObjects;

/// <summary>
/// Scene is the parent for all game objects.
/// </summary>
public class Scene : SceneObjectBase
{
    /// <summary>
    /// Scene cannot have a parent.
    /// </summary>
    /// <exception cref="InvalidOperationException">When a scene parent is going to be set.</exception>
    public override ISceneObject? Parent
    {
        get => null;
        set => throw new InvalidOperationException("Scene cannot have a parent.");
    }

    /// <summary>
    /// A primary camera for the scene.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown, if the value is going to be set to null.</exception>
    public Camera Camera { get; }
    
    /// <summary>
    /// An optional skybox used by this scene.
    /// </summary>
    public Skybox? Skybox { get; set; }
    
    /// <summary>
    /// The maximum number of lights supported by this scene.
    /// </summary>
    public readonly int MaxLights;
    
    /// <summary>
    /// Lights used in this scene.
    /// </summary>
    public readonly IList<ILight> Lights = new List<ILight>();


    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="camera">A primary scene camera.</param>
    /// <param name="maxLights">A maximum of supported lights. 16 by default.</param>
    public Scene(Camera camera, int maxLights = 16)
    {
        Camera = camera ?? throw new ArgumentNullException(nameof(camera));
        Camera.Parent = this;
        Children.Add(Camera);
        
        MaxLights = maxLights;
        
        ModelMatrix = Matrix4.Identity;
    }


    public override void Update(float deltaTime)
    {
        Skybox?.Update(deltaTime);
        base.Update(deltaTime);
    }


    public override void Render()
    {
        base.Render();
        
        // We render the skybox as the last thing.
        // See: https://learnopengl.com/Advanced-OpenGL/Cubemaps
        Skybox?.Render();
    }
}
