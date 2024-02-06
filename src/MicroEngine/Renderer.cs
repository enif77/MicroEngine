/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine;

using OpenTK.Graphics.OpenGL4;

/// <summary>
/// General rendering related tools.
/// </summary>
public static class Renderer
{
    /// <summary>
    /// Sets the viewport.
    /// </summary>
    /// <param name="x">The X of the lower left corner of the viewport rectangle, in pixels. The initial value is 0.</param>
    /// <param name="y">The Y of the lower left corner of the viewport rectangle, in pixels. The initial value is 0.</param>
    /// <param name="width">The width of the viewport. When a GL context is first attached to a window, width and height are set to the dimensions of that window.</param>
    /// <param name="height">The height of the viewport. When a GL context is first attached to a window, width and height are set to the dimensions of that window.</param>
    public static void SetViewport(int x, int y, int width, int height)
    {
        GL.Viewport(x, y, width, height);
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

    // /// <summary>
    // /// Renders the given geometry directly without using any indices (EBOs).
    // /// </summary>
    // /// <param name="geometry">A geometry to be rendered.</param>
    // public static void DrawTriangles(IGeometry geometry)
    // {
    //     GL.BindVertexArray(geometry.VertexArrayObject);
    //     GL.DrawArrays(PrimitiveType.Triangles, 0, geometry.IndicesCount);
    // }
    //
    // /// <summary>
    // /// Renders the given geometry directly using indices (EBOs).
    // /// </summary>
    // /// <param name="geometry">A geometry to be rendered.</param>
    // public static void DrawTrianglesWithIndices(IGeometry geometry)
    // {
    //     GL.BindVertexArray(geometry.VertexArrayObject);
    //     GL.DrawElements(PrimitiveType.Triangles, geometry.IndicesCount, DrawElementsType.UnsignedInt, 0);
    // }
}