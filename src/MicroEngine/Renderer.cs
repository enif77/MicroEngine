/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

/// <summary>
/// General rendering related tools.
/// </summary>
public static class Renderer
{
    public static readonly float DefaultAspectRatio = 16.0f / 9.0f;
    public static readonly float DefaultNearClipPlaneDepth = 0.01f;
    public static readonly float DefaultFarClipPlaneDepth = 100.0f;
    
    
    /// <summary>
    /// Constructor.
    /// </summary>
    static Renderer()
    {
        _aspectRatio = DefaultAspectRatio;
        SetClipPlanes(DefaultNearClipPlaneDepth, DefaultFarClipPlaneDepth);
    }
    

    /// <summary>
    /// Sets the viewport.
    /// </summary>
    /// <param name="x">The X of the lower left corner of the viewport rectangle, in pixels. The initial value is 0.</param>
    /// <param name="y">The Y of the lower left corner of the viewport rectangle, in pixels. The initial value is 0.</param>
    /// <param name="width">The width of the viewport. When a GL context is first attached to a window, width and height are set to the dimensions of that window.</param>
    /// <param name="height">The height of the viewport. When a GL context is first attached to a window, width and height are set to the dimensions of that window.</param>
    public static void SetViewport(int x, int y, int width, int height)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(width, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(height, 1);

        _aspectRatio = (float)width / height;
        
        GL.Viewport(x, y, width, height);
    }

    private static float _aspectRatio;
    private static float _nearClipPlaneDepth;
    private static float _farClipPlaneDepth;

    /// <summary>
    /// Sets the near and far clip planes.
    /// </summary>
    /// <param name="depthNear">Distance to the near clip plane.</param>
    /// <param name="depthFar">Distance to the far clip plane.</param>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// Thrown under the following conditions:
    ///  <list type="bullet">
    ///  <item>depthNear is negative or zero</item>
    ///  <item>depthFar is negative or zero</item>
    ///  <item>depthNear is larger than depthFar</item>
    ///  </list>
    /// </exception>
    private static void SetClipPlanes(float depthNear, float depthFar)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(depthNear);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(depthFar);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(depthNear, depthFar);

        _nearClipPlaneDepth = depthNear;
        _farClipPlaneDepth = depthFar;
    }

    /// <summary>
    /// Creates a perspective projection matrix.  
    /// </summary>
    /// <param name="fovy">Angle of the field of view in the y direction (in radians).</param>
    /// <returns></returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// Thrown under the following conditions:
    ///  <list type="bullet">
    ///  <item>fovy is zero, less than zero or larger than Math.PI</item>
    ///  </list>
    /// </exception>
    public static Matrix4 CreatePerspectiveProjectionMatrix(float fovy)
    {
        if (fovy <= 0.0 || fovy > Math.PI) throw new ArgumentOutOfRangeException(nameof (fovy));
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(_aspectRatio, 0.0f);

        return Matrix4.CreatePerspectiveFieldOfView(fovy, _aspectRatio, _nearClipPlaneDepth, _farClipPlaneDepth);
    }

    /// <summary>
    /// Sets the clear color.
    /// </summary>
    /// <param name="red">The red value of the color used when the color buffers are cleared. The initial value is 0.</param>
    /// <param name="green">The green value of the color used when the color buffers are cleared. The initial value is 0.</param>
    /// <param name="blue">The blue value of the color used when the color buffers are cleared. The initial value is 0.</param>
    /// <param name="alpha">The alpha value of the color used when the color buffers are cleared. The initial value is 0.</param>
    public static void SetClearColor(float red, float green, float blue, float alpha)
    {
        GL.ClearColor(red, green, blue, alpha);
    }

    /// <summary>
    /// Enables the depth test.
    /// </summary>
    public static void EnableDepthTest()
    {
        GL.Enable(EnableCap.DepthTest);
    }
    
    /// <summary>
    /// Disables the depth test.
    /// </summary>
    public static void DisableDepthTest()
    {
        GL.Disable(EnableCap.DepthTest);
    }
    
    /// <summary>
    /// Enables the face culling. Sets the cull face mode to back and front face direction to counter-clockwise.
    /// </summary>
    public static void EnableFaceCulling()
    {
        GL.Enable(EnableCap.CullFace);
        GL.CullFace(CullFaceMode.Back);
        GL.FrontFace(FrontFaceDirection.Ccw);
    }
    
    /// <summary>
    /// Disables the face culling.
    /// </summary>
    public static void DisableFaceCulling()
    {
        GL.Disable(EnableCap.CullFace);
    }

    /// <summary>
    /// Clears the color buffer and the depth buffer of the screen.
    /// </summary>
    public static void ClearScreen()
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
    }
    
    /// <summary>
    /// Renders the given geometry as lines without using any indices (EBOs).
    /// </summary>
    /// <param name="geometry">A geometry to be rendered.</param>
    public static void DrawLines(IGeometry geometry)
    {
        GL.BindVertexArray(geometry.VertexArrayObject);
        GL.DrawArrays(PrimitiveType.Lines, 0, geometry.IndicesCount);
    }
    
    /// <summary>
    /// Renders the given geometry as lines using indices (EBOs).
    /// </summary>
    /// <param name="geometry">A geometry to be rendered.</param>
    public static void DrawIndexedLines(IGeometry geometry)
    {
        GL.BindVertexArray(geometry.VertexArrayObject);
        GL.DrawElements(PrimitiveType.Lines, geometry.IndicesCount, DrawElementsType.UnsignedInt, 0);
    }

    /// <summary>
    /// Renders the given geometry as triangles without using any indices (EBOs).
    /// </summary>
    /// <param name="geometry">A geometry to be rendered.</param>
    public static void DrawTriangles(IGeometry geometry)
    {
        GL.BindVertexArray(geometry.VertexArrayObject);
        GL.DrawArrays(PrimitiveType.Triangles, 0, geometry.IndicesCount);
    }
    
    /// <summary>
    /// Renders the given geometry as triangles using indices (EBOs).
    /// </summary>
    /// <param name="geometry">A geometry to be rendered.</param>
    public static void DrawIndexedTriangles(IGeometry geometry)
    {
        GL.BindVertexArray(geometry.VertexArrayObject);
        GL.DrawElements(PrimitiveType.Triangles, geometry.IndicesCount, DrawElementsType.UnsignedInt, 0);
    }
}