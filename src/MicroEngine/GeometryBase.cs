/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine;

using OpenTK.Mathematics;

using MicroEngine.Extensions;

/// <summary>
/// The Base of geometry implementations.
/// </summary>
public abstract class GeometryBase : IGeometry
{
    /// <summary>
    /// The Base of geometry implementations.
    /// </summary>
    protected GeometryBase(float[] vertexData, uint[] indices, bool isDynamic)
    {
        VertexData = vertexData ?? throw new ArgumentNullException(nameof(vertexData));
        Indices = indices ?? throw new ArgumentNullException(nameof(indices));
        IndicesCount = indices.Length;
        IsDynamic = isDynamic;
    }

    public float[] VertexData { get; private set; }
    public abstract int VertexDataStride { get; }
    public uint[] Indices { get; }
    public int IndicesCount { get; protected set; }
    public int VertexBufferObject { get; set; } = -1;
    public int ElementBufferObject { get; set; } = -1;
    public int VertexArrayObject { get; set; } = -1;
    public bool IsDynamic { get; }
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

    
    public virtual IEnumerable<Vector3> GetVertices()
    {
        for (var i = 0; i < VertexData.Length; i += VertexDataStride)
        {
            yield return new Vector3(VertexData[i], VertexData[i + 1], VertexData[i + 2]);
        }
    }
    
    
    public virtual IEnumerable<int> GetVertexData()
    {
        for (var i = 0; i < VertexData.Length; i += VertexDataStride)
        {
            yield return i;
        }
    }
    
    
    public void UpdateVertices(float[] vertexData)
    {
        if (IsDynamic == false)
        {
            throw new InvalidOperationException("This geometry is not dynamic.");
        }
        
        if (NeedsToBeBuild)
        {
            throw new InvalidOperationException("The geometry was not built.");
        }
        
        VertexData = vertexData ?? throw new ArgumentNullException(nameof(vertexData));
        
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
    
    
    protected abstract void RenderImpl();
}
