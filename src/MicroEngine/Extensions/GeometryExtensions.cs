/* Copyright (C) Premysl Fara and Contributors */

using OpenTK.Mathematics;

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
            geometry.VertexData.Length * sizeof(float),
            geometry.VertexData,
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
        GL.BufferData(BufferTarget.ArrayBuffer, geometry.VertexData.Length * sizeof(float), geometry.VertexData, bufferUsageHint);
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
    
    
    /// <summary>
    /// Gets the bounding box of a geometry in world space.
    /// </summary>
    /// <param name="geometry">A scene object geometry instance.</param>
    /// <param name="worldMatrix">A matrix, that transforms geometry vertices to the world space.</param>
    /// <returns></returns>
    public static (Vector3 Min, Vector3 Max) GetBoundingBox(this IGeometry geometry, Matrix4 worldMatrix)
    {
        // Min/max in world space.
        var minX = float.MaxValue;
        var minY = float.MaxValue;
        var minZ = float.MaxValue;
            
        var maxX = float.MinValue;
        var maxY = float.MinValue;
        var maxZ = float.MinValue;    
            
        // We need to transform all parent's vertices to world space.
        foreach (var vertex in geometry.GetVertices())
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
}
