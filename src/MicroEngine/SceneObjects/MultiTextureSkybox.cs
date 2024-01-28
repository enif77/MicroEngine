/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.SceneObjects;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using MicroEngine.Extensions;

/// <summary>
/// Skybox.
/// </summary>
public class MultiTextureSkybox : SceneObjectBase
{
    /// <summary>
    /// Indices count = number of triangles * number of vertices per triangle.
    /// </summary>
    public int IndicesCount { get; }

    
    public MultiTextureSkybox(IMaterial material)
    {
        Vertices = new[]
        {
            // Each side has 2 triangles, each triangle has 3 vertices.
            
            // Positions          Texture coords
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
        };
        
        Material = material ?? throw new ArgumentNullException(nameof(material));
        
        // 36 = 6 sides * 2 triangles per side * 3 vertices per triangle.
        IndicesCount = 36;
    }
    
    
    public override void GenerateGeometry()
    {
        // Vertex array object.
        VertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(VertexArrayObject);
        
        // Vertex buffer object.
        this.GenerateVertexBufferObject();
        
        // Vertex attributes.
        this.GenerateVertexAttribPointerForPosition(6);
        this.GenerateVertexAttribPointerForTextureId(6, 3);
        this.GenerateVertexAttribPointerForTextureCoords(6, 4);
        
        // Unbind.
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
    }
    

    private Scene? _scene;

    public override void Render()
    {
        _scene ??= this.GetScene();

        // Skybox should be rendered at the camera position.
        ModelMatrix = Matrix4.CreateTranslation(_scene.Camera.Position);
        
        // Sets shader and its properties.
        Material.Shader.Use(_scene, this);
        
        // Bind skybox data.
        GL.BindVertexArray(VertexArrayObject);
        
        // Render.
        GL.DepthFunc(DepthFunction.Lequal);
        GL.DrawArrays(PrimitiveType.Triangles, 0, IndicesCount);
        GL.DepthFunc(DepthFunction.Less);
       
        base.Render();
    }
}
