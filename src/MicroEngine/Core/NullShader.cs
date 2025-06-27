/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Core;

/// <summary>
/// Null shader implementation that does nothing.
/// </summary>
public sealed class NullShader : IShader
{
    private static readonly Lazy<NullShader> Singleton = new(() => new NullShader());

    /// <summary>
    /// Gets the singleton instance of the NullShader.
    /// </summary>
    public static NullShader Instance => Singleton.Value;
    
    public bool SupportsOpenGLES => true;
    
    
    private NullShader()   
    {
        // Private constructor to prevent instantiation.
    }
    
    
    public int GetAttributeLocation(string name)
    {
        return -1;
    }
    
    
    public void Build()
    {
        // Do nothing.
    }
    
    
    public void Use(Scene scene, ISceneObject sceneObject)
    {
        // Do nothing.
    }
}
