/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.SceneObjects;

using OpenTK.Mathematics;

/// <summary>
/// Null camera represents an initial scene camera that forces a real camera to be set.
/// </summary>
public class NullCamera : SceneObjectBase, ICamera
{
    public float Fov
    {
        get => 60.0f;
        set => throw new InvalidOperationException("Null camera's Fov cannot be set.");
    }

    public float AspectRatio
    {
        get => 16.0f / 9.0f;
        set => throw new InvalidOperationException("Null camera's AspectRatio cannot be set.");
    }
    
    public Vector3 Direction => Vector3.Zero;
    
    
    public Matrix4 GetViewMatrix()
    {
        throw new NotImplementedException();
    }
    
    
    public Matrix4 GetProjectionMatrix()
    {
        throw new NotImplementedException();
    }
}
