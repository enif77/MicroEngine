/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Geometries;

using OpenTK.Graphics.OpenGL4;

/// <summary>
/// Geometry with normals and texture.
/// </summary>
public class DefaultIndexedGeometry : GeometryBase
{
    public override int VertexDataStride => 8;
    
    
    /// <summary>
    /// Constructor for a geometry with indices defined in a list.
    /// </summary>
    /// <param name="vertexData">A list of floats describing vertices.</param>
    /// <param name="indices">A list of indices.</param>
    /// <param name="isDynamic">A hint that marks a geometry as dynamically changing.</param>
    public DefaultIndexedGeometry(float[] vertexData, uint[] indices, bool isDynamic = false) 
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
        GenerateVertexAttribPointerForNormals(forShader, VertexDataStride, 3);
        GenerateVertexAttribPointerForTextureCoords(forShader, VertexDataStride, 6);
        
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
 
Example of a plane geometry:

vertices = [
   // Positions         Normals              Texture coords
   -0.5f, 0.0f,  0.5f,  0.0f, 1.0f, 0.0f,  0.0f, 0.0f,
    0.5f, 0.0f,  0.5f,  0.0f, 1.0f, 0.0f,  1.0f, 0.0f,
    0.5f, 0.0f, -0.5f,  0.0f, 1.0f, 0.0f,  1.0f, 1.0f,
   -0.5f, 0.0f, -0.5f,  0.0f, 1.0f, 0.0f,  0.0f, 1.0f,
];
   
indices = [
   // Each side has 2 triangles, each triangle has 3 vertices.
   0, 1, 2,  2, 3, 0
]
  
 */
 