/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Extensions;

using OpenTK.Graphics.OpenGL4;

/// <summary>
/// Geometry related extensions.
/// </summary>
public static class GeometryExtensions
{
    /// <summary>
    /// Generates a VBO for a scene object geometry.
    /// </summary>
    /// <param name="geometry">A scene object geometry instance.</param>
    public static void GenerateVertexBufferObject(this IGeometry geometry)
    {
        geometry.VertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, geometry.VertexBufferObject);
        GL.BufferData(
            BufferTarget.ArrayBuffer,
            geometry.Vertices.Length * sizeof(float),
            geometry.Vertices,
            geometry.IsDynamic
                ? BufferUsageHint.DynamicDraw
                : BufferUsageHint.StaticDraw);
    }
    
    /// <summary>
    /// Updates a VBO data.
    /// </summary>
    /// <param name="geometry">A scene object geometry instance.</param>
    public static void UpdateVertexBufferObjectData(this IGeometry geometry)
    {
        GL.BindBuffer(BufferTarget.ArrayBuffer, geometry.VertexBufferObject);
     
        // For OpenGL 4.3 and higher.
        //GL.InvalidateBufferData(geometry.VertexBufferObject);  
        
        var bufferUsageHint = geometry.IsDynamic
            ? BufferUsageHint.DynamicDraw
            : BufferUsageHint.StaticDraw;
        
        // Clear the buffer.
        GL.BufferData(BufferTarget.ArrayBuffer, 0, (float[])null!, bufferUsageHint);
        
        // Fill the buffer with new data.
        GL.BufferData(BufferTarget.ArrayBuffer, geometry.Vertices.Length * sizeof(float), geometry.Vertices, bufferUsageHint);
    }
    
    /// <summary>
    /// Generates a EBO for a scene object geometry.
    /// </summary>
    /// <param name="geometry">A scene object instance.</param>
    public static void GenerateElementBufferObject(this IGeometry geometry)
    {
        geometry.ElementBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, geometry.ElementBufferObject);
        GL.BufferData(
            BufferTarget.ElementArrayBuffer,
            geometry.Indices.Length * sizeof(uint),
            geometry.Indices,
            geometry.IsDynamic
                ? BufferUsageHint.DynamicDraw
                : BufferUsageHint.StaticDraw);
    }
    
    /// <summary>
    /// Updates an EBO data.
    /// </summary>
    /// <param name="geometry">A scene object instance.</param>
    public static void UpdateElementBufferObject(this IGeometry geometry)
    {
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, geometry.ElementBufferObject);

        var bufferUsageHint = geometry.IsDynamic
            ? BufferUsageHint.DynamicDraw
            : BufferUsageHint.StaticDraw;
        
        GL.BufferData(BufferTarget.ElementArrayBuffer, 0, (uint[])null!, bufferUsageHint);
        GL.BufferData(BufferTarget.ElementArrayBuffer, geometry.Indices.Length * sizeof(uint), geometry.Indices, bufferUsageHint);
    }
    
    /// <summary>
    /// Generates a vertex attribute pointer for a vertex position.
    /// </summary>
    /// <param name="geometry">A scene object geometry instance.</param>
    /// <param name="forShader">A shader for which we are generating the vertex attribute pointer.</param>
    /// <param name="stride">How many values we have per vertex.</param>
    /// <param name="offset">How many values to skip to reach the first value defining the vertex position.</param>
    public static void GenerateVertexAttribPointerForPosition(this IGeometry geometry, IShader forShader, int stride, int offset = 0)
    {
        var positionLocation = forShader.GetAttributeLocation("aPos");
        GL.EnableVertexAttribArray(positionLocation);
        GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, stride * sizeof(float), offset);
    }
    
    /// <summary>
    /// Generates a vertex attribute pointer for a vertex normal.
    /// </summary>
    /// <param name="geometry">A scene object geometry instance.</param>
    /// <param name="forShader">A shader for which we are generating the vertex attribute pointer.</param>
    /// <param name="stride">How many values we have per vertex.</param>
    /// <param name="offset">How many values to skip to reach the first value defining the vertex normal.</param>
    public static void GenerateVertexAttribPointerForNormals(this IGeometry geometry, IShader forShader, int stride, int offset = 0)
    {
        var normalLocation = forShader.GetAttributeLocation("aNormal");
        GL.EnableVertexAttribArray(normalLocation);
        GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, stride * sizeof(float), offset * sizeof(float));
    }
    
    /// <summary>
    /// Generates a vertex attribute pointer for a vertex texture ID.
    /// </summary>
    /// <param name="geometry">A scene object geometry instance.</param>
    /// <param name="forShader">A shader for which we are generating the vertex attribute pointer.</param>
    /// <param name="stride">How many values we have per vertex.</param>
    /// <param name="offset">How many values to skip to reach the first value defining the vertex texture position.</param>
    public static void GenerateVertexAttribPointerForTextureId(this IGeometry geometry, IShader forShader, int stride, int offset = 0)
    {
        var texId = forShader.GetAttributeLocation("aTexId");
        GL.EnableVertexAttribArray(texId);
        GL.VertexAttribPointer(texId, 1, VertexAttribPointerType.Float, false, stride * sizeof(float), offset * sizeof(float));
    }

    /// <summary>
    /// Generates a vertex attribute pointer for a vertex texture position.
    /// </summary>
    /// <param name="geometry">A scene object geometry instance.</param>
    /// <param name="forShader">A shader for which we are generating the vertex attribute pointer.</param>
    /// <param name="stride">How many values we have per vertex.</param>
    /// <param name="offset">How many values to skip to reach the first value defining the vertex texture position.</param>
    public static void GenerateVertexAttribPointerForTextureCoords(this IGeometry geometry, IShader forShader, int stride, int offset = 0)
    {
        var texCoordLocation = forShader.GetAttributeLocation("aTexCoords");
        GL.EnableVertexAttribArray(texCoordLocation);
        GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, stride * sizeof(float), offset * sizeof(float));
    }
}
