/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.SceneObjects;

using OpenTK.Mathematics;

using MicroEngine.Geometries;
using MicroEngine.Materials;

public abstract class SceneObjectBase : ISceneObject
{
    public virtual ISceneObject? Parent { get; set; }
    public IList<ISceneObject> Children { get; } = new List<ISceneObject>();

    public IGeometry Geometry { get; protected set; } = new NullGeometry();
    public IMaterial Material { get; init; } = new NullMaterial();

    private float _scale = 1.0f;
    public float Scale
    {
        get => _scale;
        
        set
        {
            _scale = value;
            NeedsModelMatrixUpdate = true;
        }
    }
    
    private Vector3 _position = Vector3.Zero;
    public Vector3 Position
    {
        get => _position;

        set
        {
            _position = value;
            NeedsModelMatrixUpdate = true;
        }
    }
    
    private Vector3 _rotation = Vector3.Zero;
    public Vector3 Rotation
    {
        get => _rotation;

        set
        {
            _rotation = value;
            NeedsModelMatrixUpdate = true;
        }
    }
    
    
    private bool _useOrientation;
    public bool UseOrientation
    {
        get => _useOrientation;

        set
        {
            _useOrientation = value;
            NeedsModelMatrixUpdate = true;
        }
    }
    
    private Quaternion _orientation = Quaternion.Identity;
    public Quaternion Orientation
    {
        get => _orientation;

        set
        {
            _orientation = value;
            NeedsModelMatrixUpdate = true;
        }
    }
    
    
    private bool _needsModelMatrixUpdate = true;
    public bool NeedsModelMatrixUpdate
    {
        get => _needsModelMatrixUpdate;

        set
        {
            _needsModelMatrixUpdate = value;
            if (value == false)
            {
                return;
            }

            foreach (var child in Children)
            {
                child.NeedsModelMatrixUpdate = true;
            }
        }
    }
    
    public bool IsVisible { get; set; } = true;
    
    public Matrix4 ModelMatrix { get; set; } = Matrix4.Identity;
    
    
    public virtual void Update(float deltaTime)
    {
        if (NeedsModelMatrixUpdate)
        {
            ModelMatrix = Matrix4.CreateScale(Scale);
        
            if (UseOrientation)
            {
                ModelMatrix *= Matrix4.CreateFromQuaternion(Orientation);
            }
            else
            {
                ModelMatrix *= Matrix4.CreateRotationZ(Rotation.Z);
                ModelMatrix *= Matrix4.CreateRotationX(Rotation.X);
                ModelMatrix *= Matrix4.CreateRotationY(Rotation.Y);
            }

            ModelMatrix *= Matrix4.CreateTranslation(Position);
        
            if (Parent != null)
            {
                ModelMatrix *= Parent.ModelMatrix;
            }

            NeedsModelMatrixUpdate = false;
        }

        foreach (var child in Children)
        {
            child.Update(deltaTime);
        }
    }

    
    public virtual void Render()
    {
        if (IsVisible == false)
        {
            return;
        }
        
        foreach (var child in Children)
        {
            child.Render();
        }
    }
    
    
    #region Helper methods
    
    /// <summary>
    /// Sets rotation around X axis.
    /// </summary>
    /// <param name="angle">An angle in radians.</param>
    public void SetRotationX(float angle)
    {
        Rotation = new Vector3(angle, Rotation.Y, Rotation.Z);
    }
    
    /// <summary>
    /// Sets rotation around Y axis.
    /// </summary>
    /// <param name="angle">An angle in radians.</param>
    public void SetRotationY(float angle)
    {
        Rotation = new Vector3(Rotation.X, angle, Rotation.Z);
    }
    
    /// <summary>
    /// Sets rotation around Z axis.
    /// </summary>
    /// <param name="angle">An angle in radians.</param>
    public void SetRotationZ(float angle)
    {
        Rotation = new Vector3(Rotation.X, Rotation.Y, angle);
    }
    
    /// <summary>
    /// Try to get a scene from a scene object.
    /// </summary>
    /// <returns>A Scene instance a scene object belongs to.</returns>
    /// <exception cref="InvalidOperationException">When no scene object was found as a parent of a game object.</exception>
    public Scene GetScene()
    {
        var sceneObject = (ISceneObject)this;
        
        while (true)
        {
            if (sceneObject is Scene scene)
            {
                return scene;
            }

            sceneObject = sceneObject.Parent ?? throw new InvalidOperationException("Scene not found.");
        }
    }
    
    #endregion
}
