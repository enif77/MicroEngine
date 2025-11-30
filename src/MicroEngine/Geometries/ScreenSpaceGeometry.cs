/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Geometries;

using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

using MicroEngine;
using MicroEngine.OGL;

/// <summary>
/// A 2D geometry for the screen space shader with a single texture.
/// </summary>
public class ScreenSpaceGeometry : GeometryBase
{
    public override int VertexDataStride => 4;
    
    
    /// <summary>
    /// Constructor for a geometry with a given number of indices.
    /// </summary>
    /// <param name="vertexData">A list of floats describing vertices.</param>
    /// <param name="indicesCount">The number of indices. Example for a character is 6 = 2 triangles per character * 3 vertices per triangle.</param>
    /// <param name="isDynamic">A hint that marks a geometry as dynamically changing.</param>
    /// <exception cref="ArgumentOutOfRangeException">If the number of indices is less than zero.</exception>
    public ScreenSpaceGeometry(float[] vertexData, int indicesCount, bool isDynamic = false) 
        : base(vertexData, [], isDynamic)
    {
        if (indicesCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(indicesCount), "Indices count must be a non-negative number.");
        }
        
        IndicesCount = indicesCount;
    }

    
    public override IEnumerable<Vector3> GetVertices()
    {
        for (var i = 0; i < VertexData.Length; i += VertexDataStride)
        {
            // We have just X and Y coordinates in the vertex data, Z is always 0.
            yield return new Vector3(VertexData[i], VertexData[i + 1], 0);
        }
    }
    
    
    protected override void BuildImpl(IShader forShader)
    {
        // Vertex array object.
        VertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(VertexArrayObject);
        
        // Vertex buffer object.
        GenerateVertexBufferObject();
        
        // Vertex attributes.
        GenerateVertexAttribPointerFor2DPosition(forShader, VertexDataStride);
        GenerateVertexAttribPointerForTextureCoords(forShader, VertexDataStride, 2);
        
        // Unbind.
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
    }
    
    
    protected override void RenderImpl()
    {
        GlRenderer.DrawTriangles(this);
    }
}

/*

Example of a character geometry:

Each character has 2 triangles, each triangle has 3 vertices.

vertices = [
    // Positions   Texture coords
    -0.5f, -0.5f,  0.0f, 0.0f,
     0.5f, -0.5f,  1.0f, 0.0f,
     0.5f,  0.5f,  1.0f, 1.0f,
     0.5f,  0.5f,  1.0f, 1.0f,
    -0.5f,  0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f,  0.0f, 0.0f,
];

// 6 = 1 sides * 2 triangles per side * 3 vertices per triangle.
indices-count = 6; 
  
 */