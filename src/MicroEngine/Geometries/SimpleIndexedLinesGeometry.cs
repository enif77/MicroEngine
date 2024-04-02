/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Geometries;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using MicroEngine.Extensions;

/// <summary>
/// A geometry with vertexes and indices, used for rendering lines.
/// </summary>
public class SimpleIndexedLinesGeometry : GeometryBase
{
    /// <summary>
    /// A geometry with vertexes and indices, used for rendering lines.
    /// </summary>
    public SimpleIndexedLinesGeometry(float[] vertices, uint[] indices, bool isDynamic = false)
        : base(vertices, indices, isDynamic)
    {
    }

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
        Renderer.DrawIndexedLines(this);
    }
}

/*
 
Example for a cube: 
 
vertices = [
    -0.5f,  0.5f,  0.5f,  // Front
     0.5f,  0.5f,  0.5f,
     0.5f, -0.5f,  0.5f,
    -0.5f, -0.5f,  0.5f,

    -0.5f,  0.5f, -0.5f,  // Back
     0.5f,  0.5f, -0.5f,
     0.5f, -0.5f, -0.5f,
    -0.5f, -0.5f, -0.5f
];

indices = [
    0, 1,  // Front
    1, 2,
    2, 3,
    3, 0,
   
    4, 5,  // Back
    5, 6,
    6, 7,
    7, 4,
   
    0, 4,  // Connections
    1, 5,
    2, 6,
    3, 7
]; 
  
 */