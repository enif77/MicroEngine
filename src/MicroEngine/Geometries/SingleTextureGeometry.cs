/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Geometries;

using OpenTK.Graphics.OpenGL4;

using MicroEngine.Extensions;

/// <summary>
/// A geometry with a single texture. No normals.
/// </summary>
public class SingleTextureGeometry : GeometryBase
{
    /// <summary>
    /// Constructor for a geometry with a given number of indices.
    /// </summary>
    /// <param name="vertices">A list of vertices.</param>
    /// <param name="indicesCount">The number of indices. Example for a cube is 36 = 6 sides * 2 triangles per side * 3 vertices per triangle.</param>
    /// <exception cref="ArgumentOutOfRangeException">If the number of indices is less than zero.</exception>
    public SingleTextureGeometry(float[] vertices, int indicesCount) 
        : base(vertices, Array.Empty<uint>())
    {
        if (indicesCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(indicesCount), "Indices count must be a non-negative number.");
        }
        
        IndicesCount = indicesCount;
    }
    
    /// <summary>
    /// Constructor for a geometry with indices defined in a list.
    /// </summary>
    /// <param name="vertices">A list of vertices.</param>
    /// <param name="indices">A list of indices.</param>
    public SingleTextureGeometry(float[] vertices, uint[] indices) 
        : base(vertices, indices)
    {
        IndicesCount = indices.Length;
        _hasIndicesDefined = true;
    }

    
    private readonly bool _hasIndicesDefined;
    
    public override void Build(IShader forShader)
    {
        // Vertex array object.
        VertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(VertexArrayObject);
        
        // Vertex buffer object.
        this.GenerateVertexBufferObject();
        
        // Element buffer object.
        if (_hasIndicesDefined)
        {
            this.GenerateElementBufferObject();
        }
        
        // Vertex attributes.
        this.GenerateVertexAttribPointerForPosition(forShader, 5);
        this.GenerateVertexAttribPointerForTextureCoords(forShader, 5, 3);
        
        // Unbind.
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
    }
    
    
    public override void Render()
    {
        GL.BindVertexArray(VertexArrayObject);

        if (_hasIndicesDefined)
        {
            GL.DrawElements(PrimitiveType.Triangles, IndicesCount, DrawElementsType.UnsignedInt, 0);
        }
        else
        {
            GL.DrawArrays(PrimitiveType.Triangles, 0, IndicesCount);
        }
    }
}

/*

Example of a cube geometry:

vertices =
[
    // Each side has 2 triangles, each triangle has 3 vertices.
    
    // Positions         Texture coords
    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,  // Front
     0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
    -0.5f,  0.5f,  0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
                          
    -0.5f,  0.5f,  0.5f,  1.0f, 1.0f,  // Left
    -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
    -0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
    -0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
                          
     0.5f,  0.5f, -0.5f,  0.0f, 1.0f,  // Back
     0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
    -0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
    -0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
    -0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
     0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
                          
     0.5f,  0.5f,  0.5f,  0.0f, 1.0f,  // Right
     0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
     0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
     0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
     0.5f,  0.5f,  0.5f,  0.0f, 1.0f,
                          
    -0.5f,  0.5f,  0.5f,  0.0f, 0.0f,  // Top
     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
    -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
    -0.5f,  0.5f,  0.5f,  0.0f, 0.0f,
                          
     0.5f, -0.5f, -0.5f,  0.0f, 1.0f,  // Bottom
     0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
    -0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
    -0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
    -0.5f, -0.5f, -0.5f,  1.0f, 1.0f,
     0.5f, -0.5f, -0.5f,  0.0f, 1.0f
];

// 36 = 6 sides * 2 triangles per side * 3 vertices per triangle.
indicesCount = 36; 
  
 */