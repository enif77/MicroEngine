// /* Copyright (C) Premysl Fara and Contributors */
//
// namespace MicroEngine.SceneObjects;
//
// using OpenTK.Graphics.OpenGL4;
// using OpenTK.Mathematics;
//
// using MicroEngine.Extensions;
//
// /// <summary>
// /// Skybox.
// /// </summary>
// public class Skybox : SceneObjectBase
// {
//     private const float TexCoordsFix = 0.001f;
//
//     /// <summary>
//     /// Indices count = number of triangles * number of vertices per triangle.
//     /// </summary>
//     public int IndicesCount { get; }
//
//     
//     public Skybox(IMaterial material)
//     {
//         Vertices = new[]
//         {
//             // Each side has 2 triangles, each triangle has 3 vertices.
//             
//             // Positions          Texture coords
//             -0.5f, -0.5f, -0.5f,  0.25f + TexCoordsFix, 1f / 3f + TexCoordsFix,  // Front face.
//              0.5f, -0.5f, -0.5f,  0.5f  - TexCoordsFix, 1f / 3f + TexCoordsFix,
//              0.5f,  0.5f, -0.5f,  0.5f  - TexCoordsFix, 2f / 3f - TexCoordsFix,
//              0.5f,  0.5f, -0.5f,  0.5f  - TexCoordsFix, 2f / 3f - TexCoordsFix,
//             -0.5f,  0.5f, -0.5f,  0.25f + TexCoordsFix, 2f / 3f - TexCoordsFix,
//             -0.5f, -0.5f, -0.5f,  0.25f + TexCoordsFix, 1f / 3f + TexCoordsFix,
//         
//              0.5f, -0.5f,  0.5f,  0.75f + TexCoordsFix, 1f / 3f + TexCoordsFix,  // Back face.
//             -0.5f, -0.5f,  0.5f,  1.0f  - TexCoordsFix, 1f / 3f + TexCoordsFix,
//             -0.5f,  0.5f,  0.5f,  1.0f  - TexCoordsFix, 2f / 3f - TexCoordsFix,
//             -0.5f,  0.5f,  0.5f,  1.0f  - TexCoordsFix, 2f / 3f - TexCoordsFix,
//              0.5f,  0.5f,  0.5f,  0.75f + TexCoordsFix, 2f / 3f - TexCoordsFix,
//              0.5f, -0.5f,  0.5f,  0.75f + TexCoordsFix, 1f / 3f + TexCoordsFix,
//         
//             -0.5f, -0.5f,  0.5f,  0.0f  + TexCoordsFix, 1f / 3f + TexCoordsFix,  // Left face.
//             -0.5f, -0.5f, -0.5f,  0.25f - TexCoordsFix, 1f / 3f + TexCoordsFix,
//             -0.5f,  0.5f, -0.5f,  0.25f - TexCoordsFix, 2f / 3f - TexCoordsFix,
//             -0.5f,  0.5f, -0.5f,  0.25f - TexCoordsFix, 2f / 3f - TexCoordsFix,
//             -0.5f,  0.5f,  0.5f,  0.0f  + TexCoordsFix, 2f / 3f - TexCoordsFix,
//             -0.5f, -0.5f,  0.5f,  0.0f  + TexCoordsFix, 1f / 3f + TexCoordsFix,
//         
//              0.5f, -0.5f, -0.5f,  0.5f  + TexCoordsFix, 1f / 3f + TexCoordsFix,  // Right face.
//              0.5f, -0.5f,  0.5f,  0.75f - TexCoordsFix, 1f / 3f + TexCoordsFix,
//              0.5f,  0.5f,  0.5f,  0.75f - TexCoordsFix, 2f / 3f - TexCoordsFix,
//              0.5f,  0.5f,  0.5f,  0.75f - TexCoordsFix, 2f / 3f - TexCoordsFix,
//              0.5f,  0.5f, -0.5f,  0.5f  + TexCoordsFix, 2f / 3f - TexCoordsFix,
//              0.5f, -0.5f, -0.5f,  0.5f  + TexCoordsFix, 1f / 3f + TexCoordsFix,
//         
//             -0.5f, -0.5f,  0.5f,  0.25f + TexCoordsFix, 1f / 3f - TexCoordsFix,  // Bottom face.
//              0.5f, -0.5f,  0.5f,  0.5f  - TexCoordsFix, 1f / 3f - TexCoordsFix,
//              0.5f, -0.5f, -0.5f,  0.5f  - TexCoordsFix, 0.0f + TexCoordsFix,
//              0.5f, -0.5f, -0.5f,  0.5f  - TexCoordsFix, 0.0f + TexCoordsFix,
//             -0.5f, -0.5f, -0.5f,  0.25f + TexCoordsFix, 0.0f + TexCoordsFix,
//             -0.5f, -0.5f,  0.5f,  0.25f + TexCoordsFix, 1f / 3f - TexCoordsFix,
//         
//             -0.5f,  0.5f, -0.5f,  0.25f + TexCoordsFix, 2f / 3f + TexCoordsFix,  // Top face.
//              0.5f,  0.5f, -0.5f,  0.5f  - TexCoordsFix, 2f / 3f + TexCoordsFix,
//              0.5f,  0.5f,  0.5f,  0.5f  - TexCoordsFix, 1.0f - TexCoordsFix,
//              0.5f,  0.5f,  0.5f,  0.5f  - TexCoordsFix, 1.0f - TexCoordsFix,
//             -0.5f,  0.5f,  0.5f,  0.25f + TexCoordsFix, 1.0f - TexCoordsFix,
//             -0.5f,  0.5f, -0.5f,  0.25f + TexCoordsFix, 2f / 3f + TexCoordsFix
//         };
//         
//         Material = material ?? throw new ArgumentNullException(nameof(material));
//         
//         // 36 = 6 sides * 2 triangles per side * 3 vertices per triangle.
//         IndicesCount = 36;
//     }
//     
//     
//     public override void GenerateGeometry()
//     {
//         // Vertex array object.
//         VertexArrayObject = GL.GenVertexArray();
//         GL.BindVertexArray(VertexArrayObject);
//         
//         // Vertex buffer object.
//         this.GenerateVertexBufferObject();
//         
//         // Vertex attributes.
//         this.GenerateVertexAttribPointerForPosition(Material.Shader, 5);
//         this.GenerateVertexAttribPointerForTextureCoords(Material.Shader, 5, 3);
//         
//         // Unbind.
//         GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
//         GL.BindVertexArray(0);
//     }
//     
//
//     private Scene? _scene;
//
//     public override void Render()
//     {
//         _scene ??= this.GetScene();
//
//         // Skybox should be rendered at the camera position.
//         ModelMatrix = Matrix4.CreateTranslation(_scene.Camera.Position);
//         
//         // Sets shader and its properties.
//         Material.Shader.Use(_scene, this);
//         
//         // Bind skybox data.
//         GL.BindVertexArray(VertexArrayObject);
//         
//         // Render.
//         GL.DepthFunc(DepthFunction.Lequal);
//         GL.DrawArrays(PrimitiveType.Triangles, 0, IndicesCount);
//         GL.DepthFunc(DepthFunction.Less);
//        
//         base.Render();
//     }
// }
