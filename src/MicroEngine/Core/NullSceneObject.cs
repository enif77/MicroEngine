/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Core;

using OpenTK.Mathematics;

/// <summary>
/// NullSceneObject is a placeholder implementation of ISceneObject.
/// </summary>
public sealed class NullSceneObject : ISceneObject
{
    private static readonly Lazy<NullSceneObject> Singleton = new(() => new NullSceneObject());

    /// <summary>
    /// Gets the singleton instance of the NullSceneObject.
    /// </summary>
    public static NullSceneObject Instance => Singleton.Value;
    
    
    private NullSceneObject()
    {
        // Private constructor to prevent instantiation.
    }
    

    public ISceneObject? Parent
    {
        get => null;
        set
        {
            // Do nothing, as this is a null scene object
        }
    }
    
    
    public IList<ISceneObject> Children { get; } = new List<ISceneObject>();
    public IGeometry Geometry => NullGeometry.Instance;
    public IMaterial Material => NullMaterial.Instance;
    
    public float Scale
    {
        get => 1.0f;
        set
        {
            // Do nothing, as this is a null scene object
        }
    }
    
    public Vector3 Position
    {
        get => Vector3.Zero;
        set
        {
            // Do nothing, as this is a null scene object
        }
    }
    
    public Vector3 WorldPosition => Position;
    
    public Vector3 Rotation
    {
        get => Vector3.Zero;
        set
        {
            // Do nothing, as this is a null scene object
        }
    }
    
    public bool UseOrientation
    {
        get => false;
        set
        {
            // Do nothing, as this is a null scene object
        }
    }
    
    public Quaternion Orientation
    {
        get => Quaternion.Identity;
        set
        {
            // Do nothing, as this is a null scene object
        }
    }
    
    public bool NeedsModelMatrixUpdate
    {
        get => false;
        set
        {
            // Do nothing, as this is a null scene object
        }
    }

    public bool IsVisible
    {
        get => false;
        set
        {
            // Do nothing, as this is a null scene object
        }
    }
    
    public Matrix4 ModelMatrix => Matrix4.Identity;

    
    public void Update(float deltaTime)
    {
        // No operation for NullSceneObject.
    }
    

    public void Render()
    {
        // No operation for NullSceneObject.
    }
    
    
    public void SetRotationX(float angle)
    {
        // Not implemented for NullSceneObject.
    }

    
    public void SetRotationY(float angle)
    {
        // Not implemented for NullSceneObject.
    }

    
    public void SetRotationZ(float angle)
    {
        // Not implemented for NullSceneObject.
    }

    
    public Scene GetScene()
    {
        throw new InvalidOperationException("NullSceneObject does not belong to any scene.");
    }
}
