/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine;

using OpenTK.Mathematics;

using MicroEngine.SceneObjects;
using MicroEngine.SceneObjects.Cameras;

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
    public ICamera Camera { get; private set; }
    
    /// <summary>
    /// An optional skybox used by this scene.
    /// </summary>
    public ISceneObject? Skybox { get; set; }
    
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
    /// <param name="maxLights">A maximum of supported lights. 16 by default.</param>
    public Scene(int maxLights = 16)
    {
        Camera = new NullCamera();
        Camera.Parent = this;
        Children.Add(Camera);
        
        MaxLights = maxLights;
        
        ModelMatrix = Matrix4.Identity;
    }


    /// <summary>
    /// Sets a camera for this scene.
    /// </summary>
    /// <param name="camera">An ICamera instance.</param>
    /// <param name="addToChildren">If true (the default), the camera is added to the scene children too.</param>
    public void SetCamera(ICamera camera, bool addToChildren = true)
    {
        ArgumentNullException.ThrowIfNull(camera);

        Children.Remove(Camera);
        
        Camera = camera;

        if (addToChildren == false)
        {
            return;
        }

        Camera.Parent = this;
        Children.Add(Camera);
    }

    
    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
        
        // Skybox must update here, because it is not a child of the scene.
        // And it requires the camera position to be already updated.
        Skybox?.Update(deltaTime);
    }


    public override void Render()
    {
        base.Render();
        
        // We render the skybox as the last thing.
        // See: https://learnopengl.com/Advanced-OpenGL/Cubemaps
        Skybox?.Render();
    }
}
