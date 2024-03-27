/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Geometries;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using MicroEngine.Extensions;

/// <summary>
/// A geometry with multiple textures. No lights.
/// </summary>
public class MultiTextureGeometry : GeometryBase
{
    /// <summary>
    /// Constructor for a geometry with a given number of indices.
    /// </summary>
    /// <param name="vertices">A list of vertices.</param>
    /// <param name="indicesCount">The number of indices. Example for a cube is 36 = 6 sides * 2 triangles per side * 3 vertices per triangle.</param>
    /// <param name="isDynamic">A hint that marks a geometry as dynamically changing.</param>
    /// <exception cref="ArgumentOutOfRangeException">If the number of indices is less than zero.</exception>
    public MultiTextureGeometry(float[] vertices, int indicesCount, bool isDynamic = false) 
        : base(vertices, Array.Empty<uint>(), isDynamic)
    {
        if (indicesCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(indicesCount), "Indices count must be a non-negative number.");
        }
        
        IndicesCount = indicesCount;
    }

    
    protected override void BuildImpl(IShader forShader)
    {
        // Vertex array object.
        VertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(VertexArrayObject);
        
        // Vertex buffer object.
        this.GenerateVertexBufferObject();
        
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
    
    
    protected override void RenderImpl()
    {
        Renderer.DrawTriangles(this);
    }
}

/*

Example of a cube geometry:

Each side has 2 triangles, each triangle has 3 vertices.
      
vertices = [  
  // Positions          Texture ID and coords
  -0.5f, -0.5f, -0.5f,  0,  0.0f, 0.0f,  // Front face.
   0.5f, -0.5f, -0.5f,  0,  1.0f, 0.0f,
   0.5f,  0.5f, -0.5f,  0,  1.0f, 1.0f,
   0.5f,  0.5f, -0.5f,  0,  1.0f, 1.0f,
  -0.5f,  0.5f, -0.5f,  0,  0.0f, 1.0f,
  -0.5f, -0.5f, -0.5f,  0,  0.0f, 0.0f,

   0.5f, -0.5f, -0.5f,  1,  0.0f, 0.0f,  // Right face.
   0.5f, -0.5f,  0.5f,  1,  1.0f, 0.0f,
   0.5f,  0.5f,  0.5f,  1,  1.0f, 1.0f,
   0.5f,  0.5f,  0.5f,  1,  1.0f, 1.0f,
   0.5f,  0.5f, -0.5f,  1,  0.0f, 1.0f,
   0.5f, -0.5f, -0.5f,  1,  0.0f, 0.0f,
  
   0.5f, -0.5f,  0.5f,  2,  0.0f, 0.0f,  // Back face.
  -0.5f, -0.5f,  0.5f,  2,  1.0f, 0.0f,
  -0.5f,  0.5f,  0.5f,  2,  1.0f, 1.0f,
  -0.5f,  0.5f,  0.5f,  2,  1.0f, 1.0f,
   0.5f,  0.5f,  0.5f,  2,  0.0f, 1.0f,
   0.5f, -0.5f,  0.5f,  2,  0.0f, 0.0f,

  -0.5f, -0.5f,  0.5f,  3,  0.0f, 0.0f,  // Left face.
  -0.5f, -0.5f, -0.5f,  3,  1.0f, 0.0f,
  -0.5f,  0.5f, -0.5f,  3,  1.0f, 1.0f,
  -0.5f,  0.5f, -0.5f,  3,  1.0f, 1.0f,
  -0.5f,  0.5f,  0.5f,  3,  0.0f, 1.0f,
  -0.5f, -0.5f,  0.5f,  3,  0.0f, 0.0f,

  -0.5f,  0.5f, -0.5f,  4,  0.0f, 0.0f,  // Top face.
   0.5f,  0.5f, -0.5f,  4,  1.0f, 0.0f,
   0.5f,  0.5f,  0.5f,  4,  1.0f, 1.0f,
   0.5f,  0.5f,  0.5f,  4,  1.0f, 1.0f,
  -0.5f,  0.5f,  0.5f,  4,  0.0f, 1.0f,
  -0.5f,  0.5f, -0.5f,  4,  0.0f, 0.0f,
  
  -0.5f, -0.5f,  0.5f,  5,  0.0f, 0.0f,  // Bottom face.
   0.5f, -0.5f,  0.5f,  5,  1.0f, 0.0f,
   0.5f, -0.5f, -0.5f,  5,  1.0f, 1.0f,
   0.5f, -0.5f, -0.5f,  5,  1.0f, 1.0f,
  -0.5f, -0.5f, -0.5f,  5,  0.0f, 1.0f,
  -0.5f, -0.5f,  0.5f,  5,  0.0f, 0.0f,
];
      
// 36 = 6 sides * 2 triangles per side * 3 vertices per triangle.
indices-count = 36;
  
 */