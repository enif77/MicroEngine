/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.SceneObjects;

using OpenTK.Graphics.OpenGL4;

using MicroEngine.Extensions;

/// <summary>
/// A cube without normals and texture coords.
/// </summary>
public class SimpleCube : SceneObjectBase
{
    public SimpleCube()
    {
        Vertices =
        [
            -0.5f,  0.5f,  0.5f,
             0.5f,  0.5f,  0.5f,
             0.5f, -0.5f,  0.5f,
            -0.5f, -0.5f,  0.5f,
            
            -0.5f,  0.5f, -0.5f,
             0.5f,  0.5f, -0.5f,
             0.5f, -0.5f, -0.5f,
            -0.5f, -0.5f, -0.5f
        ];

        Indices =
        [
            // Each side has 2 triangles, each triangle has 3 vertices.
            0, 3, 2,  2, 1, 0, // Front
            1, 2, 6,  6, 5, 1, // Right
            5, 6, 7,  7, 4, 5, // Back
            4, 7, 3,  3, 0, 4, // Left
            0, 1, 5,  5, 4, 0, // Top
            6, 2, 3,  3, 7, 6  // Bottom
        ];
    }
    
    
    public override void GenerateGeometry()
    {
        // Vertex array object.
        VertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(VertexArrayObject);
        
        // Vertex buffer object.
        this.GenerateVertexBufferObject();
        
        // Element buffer object.
        this.GenerateElementBufferObject();
        
        // Vertex attributes.
        this.GenerateVertexAttribPointerForPosition(3);
        
        // Unbind.
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
    }
    
    
    private Scene? _scene;

    public override void Render()
    {
        _scene ??= this.GetScene();
        
        Material.Shader.Use(_scene, this);
        
        GL.BindVertexArray(VertexArrayObject);
        GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
        
        base.Render();
    }
}
