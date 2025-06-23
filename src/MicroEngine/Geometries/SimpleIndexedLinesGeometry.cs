/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Geometries;

using OpenTK.Graphics.OpenGL4;

using MicroEngine.OGL;

/// <summary>
/// A geometry with vertexes and indices, used for rendering lines.
/// </summary>
public class SimpleIndexedLinesGeometry : GeometryBase
{
    public override int VertexDataStride => 3;

    
    /// <summary>
    /// A geometry with vertexes and indices, used for rendering lines.
    /// </summary>
    public SimpleIndexedLinesGeometry(float[] vertexData, uint[] indices, bool isDynamic = false)
        : base(vertexData, indices, isDynamic)
    {
    }

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
        GenerateVertexAttribPointerFor3DPosition(forShader, VertexDataStride);
        
        // Unbind.
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
    }
    
    
    protected override void RenderImpl()
    {
        GlRenderer.DrawIndexedLines(this);
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