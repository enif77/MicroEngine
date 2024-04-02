/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Geometries;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using MicroEngine.Extensions;

/// <summary>
/// A geometry with vertexes and indices.
/// </summary>
public class SimpleIndexedGeometry(float[] vertices, uint[] indices, bool isDynamic = false)
    : GeometryBase(vertices, indices, isDynamic)
{
    protected override void BuildImpl(IShader forShader)
    {
        // Vertex array object.
        VertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(VertexArrayObject);
        
        // Vertex buffer object.
        this.GenerateVertexBufferObject();
        
        // Element buffer object.
        this.GenerateElementBufferObject();
        
        // Vertex attributes.
        this.GenerateVertexAttribPointerForPosition(forShader, 3);
        
        // Unbind.
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
    }
    
    
    public override IEnumerable<Vector3> GetVertices()
    {
        for (var i = 0; i < Vertices.Length; i += 3)
        {
            yield return new Vector3(Vertices[i], Vertices[i + 1], Vertices[i + 2]);
        }
    }
    
    
    public override IEnumerable<int> GetRawVertices()
    {
        for (var i = 0; i < Vertices.Length; i += 3)
        {
            yield return i;
        }
        
        yield return -1;
    }
    
    
    protected override void RenderImpl()
    {
        Renderer.DrawIndexedTriangles(this);
    }
}

/*
 
Example for a cube: 
 
vertices = [
    -0.5f,  0.5f,  0.5f,
     0.5f,  0.5f,  0.5f,
     0.5f, -0.5f,  0.5f,
    -0.5f, -0.5f,  0.5f,

    -0.5f,  0.5f, -0.5f,
     0.5f,  0.5f, -0.5f,
     0.5f, -0.5f, -0.5f,
    -0.5f, -0.5f, -0.5f
];

indices = [
    // Each side has 2 triangles, each triangle has 3 vertices.
    0, 3, 2,  2, 1, 0, // Front
    1, 2, 6,  6, 5, 1, // Right
    5, 6, 7,  7, 4, 5, // Back
    4, 7, 3,  3, 0, 4, // Left
    0, 1, 5,  5, 4, 0, // Top
    6, 2, 3,  3, 7, 6  // Bottom
] 
  
 */