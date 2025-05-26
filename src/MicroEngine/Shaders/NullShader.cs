/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Shaders;

public class NullShader : IShader
{
    public bool SupportsOpenGLES => true;
    
    
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
