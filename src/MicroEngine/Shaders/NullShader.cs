/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Shaders;

public class NullShader : IShader
{
    public string Name => "null";

    
    public int GetAttributeLocation(string name)
    {
        return -1;
    }
    
    
    public void Use(Scene scene, ISceneObject sceneObject)
    {
        // Do nothing.
    }
}
