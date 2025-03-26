/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;


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
    
    
    #region Helper methods
    
    /// <summary>
    /// Generates a VBO for a scene object geometry.
    /// </summary>
    protected void GenerateVertexBufferObject()
    {
        VertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
        GL.BufferData(
            BufferTarget.ArrayBuffer,
            VertexData.Length * sizeof(float),
            VertexData,
            IsDynamic
                ? BufferUsageHint.DynamicDraw
                : BufferUsageHint.StaticDraw);
    }
    
    /// <summary>
    /// Updates a VBO data.
    /// </summary>
    protected void UpdateVertexBufferObjectData()
    {
        GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
     
        // For OpenGL 4.3 and higher.
        //GL.InvalidateBufferData(geometry.VertexBufferObject);  
        
        var bufferUsageHint = IsDynamic
            ? BufferUsageHint.DynamicDraw
            : BufferUsageHint.StaticDraw;
        
        // Clear the buffer.
        GL.BufferData(BufferTarget.ArrayBuffer, 0, (float[])null!, bufferUsageHint);
        
        // Fill the buffer with new data.
        GL.BufferData(BufferTarget.ArrayBuffer, VertexData.Length * sizeof(float), VertexData, bufferUsageHint);
    }
    
    /// <summary>
    /// Generates a EBO for a scene object geometry.
    /// </summary>
    protected void GenerateElementBufferObject()
    {
        ElementBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
        GL.BufferData(
            BufferTarget.ElementArrayBuffer,
            Indices.Length * sizeof(uint),
            Indices,
            IsDynamic
                ? BufferUsageHint.DynamicDraw
                : BufferUsageHint.StaticDraw);
    }
    
    /// <summary>
    /// Updates an EBO data.
    /// </summary>
    protected void UpdateElementBufferObject()
    {
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);

        var bufferUsageHint = IsDynamic
            ? BufferUsageHint.DynamicDraw
            : BufferUsageHint.StaticDraw;
        
        GL.BufferData(BufferTarget.ElementArrayBuffer, 0, (uint[])null!, bufferUsageHint);
        GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.Length * sizeof(uint), Indices, bufferUsageHint);
    }
    
    /// <summary>
    /// Generates a vertex attribute pointer for a vertex position.
    /// </summary>
    /// <param name="forShader">A shader for which we are generating the vertex attribute pointer.</param>
    /// <param name="stride">How many values we have per vertex.</param>
    /// <param name="offset">How many values to skip to reach the first value defining the vertex position.</param>
    protected void GenerateVertexAttribPointerForPosition(IShader forShader, int stride, int offset = 0)
    {
        var positionLocation = forShader.GetAttributeLocation("aPos");
        GL.EnableVertexAttribArray(positionLocation);
        GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, stride * sizeof(float), offset);
    }
    
    /// <summary>
    /// Generates a vertex attribute pointer for a vertex normal.
    /// </summary>
    /// <param name="forShader">A shader for which we are generating the vertex attribute pointer.</param>
    /// <param name="stride">How many values we have per vertex.</param>
    /// <param name="offset">How many values to skip to reach the first value defining the vertex normal.</param>
    protected void GenerateVertexAttribPointerForNormals(IShader forShader, int stride, int offset = 0)
    {
        var normalLocation = forShader.GetAttributeLocation("aNormal");
        GL.EnableVertexAttribArray(normalLocation);
        GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, stride * sizeof(float), offset * sizeof(float));
    }
    
    /// <summary>
    /// Generates a vertex attribute pointer for a vertex texture ID.
    /// </summary>
    /// <param name="forShader">A shader for which we are generating the vertex attribute pointer.</param>
    /// <param name="stride">How many values we have per vertex.</param>
    /// <param name="offset">How many values to skip to reach the first value defining the vertex texture position.</param>
    protected void GenerateVertexAttribPointerForTextureId(IShader forShader, int stride, int offset = 0)
    {
        var texId = forShader.GetAttributeLocation("aTexId");
        GL.EnableVertexAttribArray(texId);
        GL.VertexAttribPointer(texId, 1, VertexAttribPointerType.Float, false, stride * sizeof(float), offset * sizeof(float));
    }

    /// <summary>
    /// Generates a vertex attribute pointer for a vertex texture position.
    /// </summary>
    /// <param name="forShader">A shader for which we are generating the vertex attribute pointer.</param>
    /// <param name="stride">How many values we have per vertex.</param>
    /// <param name="offset">How many values to skip to reach the first value defining the vertex texture position.</param>
    protected void GenerateVertexAttribPointerForTextureCoords(IShader forShader, int stride, int offset = 0)
    {
        var texCoordLocation = forShader.GetAttributeLocation("aTexCoords");
        GL.EnableVertexAttribArray(texCoordLocation);
        GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, stride * sizeof(float), offset * sizeof(float));
    }
    
    /// <summary>
    /// Gets the bounding box of a geometry in world space.
    /// </summary>
    /// <param name="worldMatrix">A matrix, that transforms geometry vertices to the world space.</param>
    /// <returns>A tuple containing minimal and maximal X, Y, Z.</returns>
    protected (Vector3 Min, Vector3 Max) GetBoundingBox(Matrix4 worldMatrix)
    {
        // Min/max in world space.
        var minX = float.MaxValue;
        var minY = float.MaxValue;
        var minZ = float.MaxValue;
            
        var maxX = float.MinValue;
        var maxY = float.MinValue;
        var maxZ = float.MinValue;    
            
        // We need to transform all parent's vertices to world space.
        foreach (var vertex in GetVertices())
        {
            var transformedVertex = Vector3.TransformPosition(vertex, worldMatrix);
                
            if (transformedVertex.X < minX)
            {
                minX = transformedVertex.X;
            }
            if (transformedVertex.Y < minY)
            {
                minY = transformedVertex.Y;
            }
            if (transformedVertex.Z < minZ)
            {
                minZ = transformedVertex.Z;
            }
                
            if (transformedVertex.X > maxX)
            {
                maxX = transformedVertex.X;
            }
            if (transformedVertex.Y > maxY)
            {
                maxY = transformedVertex.Y;
            }
            if (transformedVertex.Z > maxZ)
            {
                maxZ = transformedVertex.Z;
            }
        }
            
        // We have the min/max in world space.
        return (new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ));
    }
    
    #endregion
}
