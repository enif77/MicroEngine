/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine;

/// <summary>
/// Generic interface for a scene object geometry.
/// </summary>
public interface IGeometry
{
    #region Geometry
    
    /// <summary>
    /// Vertices defining the geometry.
    /// </summary>
    float[] Vertices { get; }
    
    /// <summary>
    /// Defines the order of vertices to form triangles.
    /// </summary>
    uint[] Indices { get; }

    /// <summary>
    /// Indices count = number of triangles * number of vertices per triangle.
    /// </summary>
    int IndicesCount { get; }
    
    /// <summary>
    /// An OpenGL vertex buffer object ID.
    /// </summary>
    int VertexBufferObject { get; set; }
    
    /// <summary>
    /// An OpenGL element buffer object ID.
    /// </summary>
    int ElementBufferObject { get; set; }
    
    /// <summary>
    /// An OpenGL vertex array object ID.
    /// </summary>
    int VertexArrayObject { get; set; }
    
    
    /// <summary>
    /// Generates OpenGL buffer objects for this geometry.
    /// </summary>
    /// <argument name="forShader">The shader to generate the geometry for.</argument>
    void Build(IShader forShader);
    
    #endregion
}
