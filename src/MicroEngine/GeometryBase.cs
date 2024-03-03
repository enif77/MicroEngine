/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine;

/// <summary>
/// The Base of geometry implementations.
/// </summary>
public abstract class GeometryBase(float[] vertices, uint[] indices) : IGeometry
{
    public float[] Vertices { get; } = vertices;
    public uint[] Indices { get; } = indices;
    public int IndicesCount { get; protected set; } = indices.Length;
    public int VertexBufferObject { get; set; } = -1;
    public int ElementBufferObject { get; set; } = -1;
    public int VertexArrayObject { get; set; } = -1;
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
