/* Copyright (C) Premysl Fara and Contributors */

using OpenTK.Mathematics;

namespace MicroEngine;

/// <summary>
/// Generic interface for a scene object geometry.
/// </summary>
public interface IGeometry : IRenderable
{
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
    /// If true, this geometry needs to be built calling the Build() method.
    /// Its true for newly created geometries. After the geometry is built, its set to false.
    /// </summary>
    bool NeedsToBeBuild { get; }
    
    /// <summary>
    /// Generates OpenGL buffer objects for this geometry.
    /// Needs to be called once and before its used for rendering.
    /// </summary>
    /// <argument name="forShader">The shader to generate the geometry for.</argument>
    void Build(IShader forShader);
    
    
    #region iterators

    /// <summary>
    /// Iterator, that returns the vertices of the geometry.
    /// </summary>
    /// <returns>An enumerator, that is returning vertices as Vector3 instances.</returns>
    IEnumerable<Vector3> GetVertices();

    #endregion
}
