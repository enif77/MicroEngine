/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Geometries;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using MicroEngine.Extensions;

/// <summary>
/// Geometry with normals and texture.
/// </summary>
public class DefaultIndexedGeometry : GeometryBase
{
    /// <summary>
    /// Constructor for a geometry with indices defined in a list.
    /// </summary>
    /// <param name="vertices">A list of vertices.</param>
    /// <param name="indices">A list of indices.</param>
    /// <param name="isDynamic">A hint that marks a geometry as dynamically changing.</param>
    public DefaultIndexedGeometry(float[] vertices, uint[] indices, bool isDynamic = false) 
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
        this.GenerateVertexAttribPointerForPosition(forShader, 8);
        this.GenerateVertexAttribPointerForNormals(forShader, 8, 3);
        this.GenerateVertexAttribPointerForTextureCoords(forShader, 8, 6);
        
        // Unbind.
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
    }

    
    public override IEnumerable<Vector3> GetVertices()
    {
        for (var i = 0; i < Vertices.Length; i += 8)
        {
            yield return new Vector3(Vertices[i], Vertices[i + 1], Vertices[i + 2]);
        }
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
 