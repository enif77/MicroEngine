/* Copyright (C) Premysl Fara and Contributors */

using MicroEngine.Extensions;
using OpenTK.Mathematics;

namespace MicroEngine;

/// <summary>
/// The Base of geometry implementations.
/// </summary>
public abstract class GeometryBase(float[] vertices, uint[] indices, bool isDynamic) : IGeometry
{
    public float[] Vertices { get; private set; } = vertices;
    public uint[] Indices { get; } = indices;
    public int IndicesCount { get; protected set; } = indices.Length;
    public int VertexBufferObject { get; set; } = -1;
    public int ElementBufferObject { get; set; } = -1;
    public int VertexArrayObject { get; set; } = -1;
    public bool IsDynamic { get; } = isDynamic;
    public bool NeedsToBeBuild { get; private set; } = true;


    public void Build(IShader forShader)
    {
        if (NeedsToBeBuild == false)
        {
            throw new InvalidOperationException("The geometry was already built.");
        }
    
        BuildImpl(forShader);
        
        NeedsToBeBuild = false;
    }
    
    protected abstract void BuildImpl(IShader forShader);

    
    public void UpdateVertices(float[] vertices)
    {
        if (IsDynamic == false)
        {
            throw new InvalidOperationException("This geometry is not dynamic.");
        }
        
        if (NeedsToBeBuild)
        {
            throw new InvalidOperationException("The geometry was not built.");
        }
        
        Vertices = vertices ?? throw new ArgumentNullException(nameof(vertices));
        
        this.UpdateVertexBufferObjectData();
    }
    
    
    public void Render()
    {
        if (NeedsToBeBuild)
        {
            throw new InvalidOperationException("The geometry was not built.");
        }
        
        RenderImpl();
    }


    public abstract IEnumerable<Vector3> GetVertices();
    public abstract IEnumerable<int> GetRawVertices();
    
    
    protected abstract void RenderImpl();
}
