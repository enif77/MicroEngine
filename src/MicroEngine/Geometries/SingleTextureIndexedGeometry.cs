/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Geometries;

using OpenTK.Graphics.OpenGL4;

using MicroEngine.OGL;

/// <summary>
/// A geometry with a single texture and indices. No normals.
/// </summary>
public class SingleTextureIndexedGeometry : GeometryBase
{
    public override int VertexDataStride => 5;
    
    
    /// <summary>
    /// Constructor for a geometry with indices defined in a list.
    /// </summary>
    /// <param name="vertexData">A list of floats describing vertices.</param>
    /// <param name="indices">A list of indices.</param>
    /// <param name="isDynamic">A hint that marks a geometry as dynamically changing.</param>
    public SingleTextureIndexedGeometry(float[] vertexData, uint[] indices, bool isDynamic = false) 
        : base(vertexData, indices, isDynamic)
    {
        IndicesCount = indices.Length;
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
        GenerateVertexAttribPointerForPosition(forShader, VertexDataStride);
        GenerateVertexAttribPointerForTextureCoords(forShader, VertexDataStride, 3);
        
        // Unbind.
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
    }
    
    
    protected override void RenderImpl()
    {
        GlRenderer.DrawIndexedTriangles(this);
    }
}

/*

Example of a cube geometry:

vertices = [
   // Positions          Texture coords
   -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,  // Front face.
    0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
    0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
   -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,

    0.5f, -0.5f, -0.5f,  0.0f, 0.0f,  // Right face.
    0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
    0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
    0.5f,  0.5f, -0.5f,  0.0f, 1.0f,

    0.5f, -0.5f,  0.5f,  0.0f, 0.0f,  // Back face.
   -0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
   -0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
    0.5f,  0.5f,  0.5f,  0.0f, 1.0f,

   -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,  // Left face.
   -0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
   -0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
   -0.5f,  0.5f,  0.5f,  0.0f, 1.0f,

   -0.5f,  0.5f, -0.5f,  0.0f, 0.0f,  // Top face.
    0.5f,  0.5f, -0.5f,  1.0f, 0.0f,
    0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
   -0.5f,  0.5f,  0.5f,  0.0f, 1.0f,

   -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,  // Bottom face.
    0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
    0.5f, -0.5f, -0.5f,  1.0f, 1.0f,
   -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
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
