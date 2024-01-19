namespace MicroEngine;

/// <summary>
/// Defines a shader used for rendering of an object.
/// </summary>
public interface IShader
{
    /// <summary>
    /// An unique name of the shader.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Gets an attribute location by its name.
    /// </summary>
    /// <param name="name">An attribute name.</param>
    /// <returns>An attribute location.</returns>
    int GetAttributeLocation(string name);
    
    /// <summary>
    /// Activates sets up this shader for further rendering..
    /// </summary>
    /// <param name="scene">A scene to be used for retrieving values required by this shader.
    /// A camera position and direction for example.</param>
    /// <param name="sceneObject">A scene object, that will be rendered.</param>
    void Use(Scene scene, ISceneObject sceneObject);
}
