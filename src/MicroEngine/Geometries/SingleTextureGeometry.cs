/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Geometries;

using OpenTK.Graphics.OpenGL4;

using MicroEngine.OGL;

/// <summary>
/// A geometry with a single texture. No lights.
/// </summary>
public class SingleTextureGeometry : GeometryBase
{
    public override int VertexDataStride => 5;
    
    
    /// <summary>
    /// Constructor for a geometry with a given number of indices.
    /// </summary>
    /// <param name="vertexData">A list of floats describing vertices.</param>
    /// <param name="indicesCount">The number of indices. Example for a cube is 36 = 6 sides * 2 triangles per side * 3 vertices per triangle.</param>
    /// <param name="isDynamic">A hint that marks a geometry as dynamically changing.</param>
    /// <exception cref="ArgumentOutOfRangeException">If the number of indices is less than zero.</exception>
    public SingleTextureGeometry(float[] vertexData, int indicesCount, bool isDynamic = false) 
        : base(vertexData, Array.Empty<uint>(), isDynamic)
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
        GenerateVertexBufferObject();
        
        // Vertex attributes.
        GenerateVertexAttribPointerForPosition(forShader, VertexDataStride);
        GenerateVertexAttribPointerForTextureCoords(forShader, VertexDataStride, 3);
        
        // Unbind.
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
    }
    
    
    protected override void RenderImpl()
    {
        GlRenderer.DrawTriangles(this);
    }
}

/*

Example of a cube geometry:

Each side has 2 triangles, each triangle has 3 vertices.

vertices = [
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
indices-count = 36; 
  
 */