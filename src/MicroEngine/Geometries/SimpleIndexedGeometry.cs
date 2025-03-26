/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Geometries;

using OpenTK.Graphics.OpenGL4;

/// <summary>
/// A geometry with vertexes and indices.
/// </summary>
public class SimpleIndexedGeometry(float[] vertexData, uint[] indices, bool isDynamic = false)
    : GeometryBase(vertexData, indices, isDynamic)
{
    public override int VertexDataStride => 3;
    
    
    protected override void BuildImpl(IShader forShader)
    {
        // Vertex array object.
        VertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(VertexArrayObject);
        
        // Vertex buffer object.
        GenerateVertexBufferObject();
        
        // Element buffer object.
        GenerateElementBufferObject();
        
        // Vertex attributes.
        GenerateVertexAttribPointerForPosition(forShader, VertexDataStride);
        
        // Unbind.
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
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