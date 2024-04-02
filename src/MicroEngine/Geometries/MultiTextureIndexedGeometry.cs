/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Geometries;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using MicroEngine.Extensions;

/// <summary>
/// A geometry with multiple textures and indices. No lights.
/// </summary>
public class MultiTextureIndexedGeometry : GeometryBase
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="vertices">A list of vertices.</param>
    /// <param name="indices">A list of indices.</param>
    /// <param name="isDynamic">A hint that marks a geometry as dynamically changing.</param>
    public MultiTextureIndexedGeometry(float[] vertices, uint[] indices, bool isDynamic = false) 
        : base(vertices, indices, isDynamic)
    {
        IndicesCount = indices.Length;
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
        this.GenerateVertexAttribPointerForPosition(forShader, 6);
        this.GenerateVertexAttribPointerForTextureId(forShader, 6, 3);
        this.GenerateVertexAttribPointerForTextureCoords(forShader, 6, 4);
        
        // Unbind.
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
    }
    
    
    public override IEnumerable<Vector3> GetVertices()
    {
        for (var i = 0; i < Vertices.Length; i += 6)
        {
            yield return new Vector3(Vertices[i], Vertices[i + 1], Vertices[i + 2]);
        }
    }
    
    
    public override IEnumerable<int> GetRawVertices()
    {
        for (var i = 0; i < Vertices.Length; i += 6)
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

Example of a cube geometry:

vertices = [
   // Positions          Texture ID and coords
   -0.5f, -0.5f, -0.5f,  0,  0.0f, 0.0f,  // Front face.
    0.5f, -0.5f, -0.5f,  0,  1.0f, 0.0f,
    0.5f,  0.5f, -0.5f,  0,  1.0f, 1.0f,
   -0.5f,  0.5f, -0.5f,  0,  0.0f, 1.0f,

    0.5f, -0.5f, -0.5f,  1,  0.0f, 0.0f,  // Right face.
    0.5f, -0.5f,  0.5f,  1,  1.0f, 0.0f,
    0.5f,  0.5f,  0.5f,  1,  1.0f, 1.0f,
    0.5f,  0.5f, -0.5f,  1,  0.0f, 1.0f,
   
    0.5f, -0.5f,  0.5f,  2,  0.0f, 0.0f,  // Back face.
   -0.5f, -0.5f,  0.5f,  2,  1.0f, 0.0f,
   -0.5f,  0.5f,  0.5f,  2,  1.0f, 1.0f,
    0.5f,  0.5f,  0.5f,  2,  0.0f, 1.0f,

   -0.5f, -0.5f,  0.5f,  3,  0.0f, 0.0f,  // Left face.
   -0.5f, -0.5f, -0.5f,  3,  1.0f, 0.0f,
   -0.5f,  0.5f, -0.5f,  3,  1.0f, 1.0f,
   -0.5f,  0.5f,  0.5f,  3,  0.0f, 1.0f,

   -0.5f,  0.5f, -0.5f,  4,  0.0f, 0.0f,  // Top face.
    0.5f,  0.5f, -0.5f,  4,  1.0f, 0.0f,
    0.5f,  0.5f,  0.5f,  4,  1.0f, 1.0f,
   -0.5f,  0.5f,  0.5f,  4,  0.0f, 1.0f,
   
   -0.5f, -0.5f,  0.5f,  5,  0.0f, 0.0f,  // Bottom face.
    0.5f, -0.5f,  0.5f,  5,  1.0f, 0.0f,
    0.5f, -0.5f, -0.5f,  5,  1.0f, 1.0f,
   -0.5f, -0.5f, -0.5f,  5,  0.0f, 1.0f,
];
   
indices = [
   // Each side has 2 triangles, each triangle has 3 vertices.
    0,  1,  2,   2,  3,  0, // Front
    4,  5,  6,   6,  7,  4, // Right
    8,  9, 10,  10, 11,  8, // Back
   12, 13, 14,  14, 15, 12, // Left
   16, 17, 18,  18, 19, 16, // Top
   20, 21, 22,  22, 23, 20  // Bottom
];   

*/
 