/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Extensions;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

/// <summary>
/// Game object related extensions.
/// </summary>
public static class SceneObjectExtensions
{
    /// <summary>
    /// Sets rotation around X axis.
    /// </summary>
    /// <param name="sceneObject">A scene object.</param>
    /// <param name="angle">An angle in radians.</param>
    public static void SetRotationX(this ISceneObject sceneObject, float angle)
    {
        sceneObject.Rotation = new Vector3(angle, sceneObject.Rotation.Y, sceneObject.Rotation.Z);
    }
    
    /// <summary>
    /// Sets rotation around Y axis.
    /// </summary>
    /// <param name="sceneObject">A scene object.</param>
    /// <param name="angle">An angle in radians.</param>
    public static void SetRotationY(this ISceneObject sceneObject, float angle)
    {
        sceneObject.Rotation = new Vector3(sceneObject.Rotation.X, angle, sceneObject.Rotation.Z);
    }
    
    /// <summary>
    /// Sets rotation around Z axis.
    /// </summary>
    /// <param name="sceneObject">A scene object.</param>
    /// <param name="angle">An angle in radians.</param>
    public static void SetRotationZ(this ISceneObject sceneObject, float angle)
    {
        sceneObject.Rotation = new Vector3(sceneObject.Rotation.X, sceneObject.Rotation.Y, angle);
    }
    
    /// <summary>
    /// Try to get a scene from a scene object.
    /// </summary>
    /// <param name="sceneObject">A scene object.</param>
    /// <returns>A Scene instance a scene object belongs to.</returns>
    /// <exception cref="InvalidOperationException">When no scene object was found as a parent of a game object.</exception>
    public static Scene GetScene(this ISceneObject sceneObject)
    {
        while (true)
        {
            if (sceneObject is Scene scene)
            {
                return scene;
            }

            sceneObject = sceneObject.Parent ?? throw new InvalidOperationException("Scene not found.");
        }
    }

    /// <summary>
    /// Adds a child to a scene object.
    /// </summary>
    /// <param name="sceneObject">A scene object.</param>
    /// <param name="child">A child to be added.</param>
    /// <exception cref="InvalidOperationException">Thrown, when such child already exists in scene object children.</exception>
    public static void AddChild(this ISceneObject sceneObject, ISceneObject child)
    {
        ArgumentNullException.ThrowIfNull(child);
        if (sceneObject.Children.Contains(child))
        {
            throw new InvalidOperationException("Child already exists in the parent object.");
        }
        
        child.Parent = sceneObject;
        sceneObject.Children.Add(child);
    }
    
    /// <summary>
    /// Generates a VBO for a scene object.
    /// </summary>
    /// <param name="sceneObject">A scene object instance.</param>
    public static void GenerateVertexBufferObject(this ISceneObject sceneObject)
    {
        sceneObject.VertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, sceneObject.VertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, sceneObject.Vertices.Length * sizeof(float), sceneObject.Vertices, BufferUsageHint.StaticDraw);
    }
    
    /// <summary>
    /// Generates a EBO for a scene object.
    /// </summary>
    /// <param name="sceneObject">A scene object instance.</param>
    public static void GenerateElementBufferObject(this ISceneObject sceneObject)
    {
        sceneObject.ElementBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, sceneObject.ElementBufferObject);
        GL.BufferData(BufferTarget.ElementArrayBuffer, sceneObject.Indices.Length * sizeof(uint), sceneObject.Indices, BufferUsageHint.StaticDraw);
    }
    
    /// <summary>
    /// Generates a vertex attribute pointer for a vertex position.
    /// </summary>
    /// <param name="sceneObject">A scene object instance.</param>
    /// <param name="stride">How many values we have per vertex.</param>
    /// <param name="offset">How many values to skip to reach the first value defining the vertex position.</param>
    public static void GenerateVertexAttribPointerForPosition(this ISceneObject sceneObject, int stride, int offset = 0)
    {
        var positionLocation = sceneObject.Material.Shader.GetAttributeLocation("aPos");
        GL.EnableVertexAttribArray(positionLocation);
        GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, stride * sizeof(float), offset);
    }
    
    /// <summary>
    /// Generates a vertex attribute pointer for a vertex normal.
    /// </summary>
    /// <param name="sceneObject">A scene object instance.</param>
    /// <param name="stride">How many values we have per vertex.</param>
    /// <param name="offset">How many values to skip to reach the first value defining the vertex normal.</param>
    public static void GenerateVertexAttribPointerForNormals(this ISceneObject sceneObject, int stride, int offset = 0)
    {
        var normalLocation = sceneObject.Material.Shader.GetAttributeLocation("aNormal");
        GL.EnableVertexAttribArray(normalLocation);
        GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, stride * sizeof(float), offset * sizeof(float));
    }
    
    /// <summary>
    /// Generates a vertex attribute pointer for a vertex texture ID.
    /// </summary>
    /// <param name="sceneObject">A scene object instance.</param>
    /// <param name="stride">How many values we have per vertex.</param>
    /// <param name="offset">How many values to skip to reach the first value defining the vertex texture position.</param>
    public static void GenerateVertexAttribPointerForTextureId(this ISceneObject sceneObject, int stride, int offset = 0)
    {
        var texId = sceneObject.Material.Shader.GetAttributeLocation("aTexId");
        GL.EnableVertexAttribArray(texId);
        GL.VertexAttribPointer(texId, 1, VertexAttribPointerType.Float, false, stride * sizeof(float), offset * sizeof(float));
    }
    
    /// <summary>
    /// Generates a vertex attribute pointer for a vertex texture position.
    /// </summary>
    /// <param name="sceneObject">A scene object instance.</param>
    /// <param name="stride">How many values we have per vertex.</param>
    /// <param name="offset">How many values to skip to reach the first value defining the vertex texture position.</param>
    public static void GenerateVertexAttribPointerForTextureCoords(this ISceneObject sceneObject, int stride, int offset = 0)
    {
        var texCoordLocation = sceneObject.Material.Shader.GetAttributeLocation("aTexCoords");
        GL.EnableVertexAttribArray(texCoordLocation);
        GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, stride * sizeof(float), offset * sizeof(float));
    }
}
