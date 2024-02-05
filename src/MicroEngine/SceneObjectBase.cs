/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine;

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
    
    public Matrix4 ModelMatrix { get; set; } = Matrix4.Identity;
    
    
    
    public void BuildGeometry()
    {
        Geometry.Build(Material.Shader);
    }
    
    
    public void UpdateModelMatrix()
    {
        ModelMatrix = Matrix4.CreateScale(Scale);
        
        ModelMatrix *= Matrix4.CreateRotationZ(Rotation.Z);
        ModelMatrix *= Matrix4.CreateRotationX(Rotation.X);
        ModelMatrix *= Matrix4.CreateRotationY(Rotation.Y);

        ModelMatrix *= Matrix4.CreateTranslation(Position);
        
        if (Parent != null)
        {
            ModelMatrix *= Parent.ModelMatrix;
        }
    }
    
    
    public virtual void Update(float deltaTime)
    {
        if (NeedsModelMatrixUpdate)
        {
            UpdateModelMatrix();

            NeedsModelMatrixUpdate = false;
        }

        foreach (var child in Children)
        {
            child.Update(deltaTime);
        }
    }

    public virtual void Render()
    {
        foreach (var child in Children)
        {
            child.Render();
        }
    }
}
