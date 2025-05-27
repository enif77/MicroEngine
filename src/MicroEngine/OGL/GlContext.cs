/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.OGL;

// We are using OpenTK.Graphics.OpenGL4 for OpenGL strings and capabilities,
// but, at least for those used, they are compatible with OpenGL ES 3.0+.
using OpenTK.Graphics.OpenGL4;

/// <summary>
/// Gets information about the current OpenGL context.
/// </summary>
public static class GlContext
{
    /// <summary>
    /// Forces the OpenGL context to use OpenGL ES, even if the system supports desktop OpenGL.
    /// </summary>
    public static bool ForceOpenGLES { get; set; } = false;
    
    /// <summary>
    /// Is the OpenGL context using OpenGL ES?
    /// Note: This is not a reliable way to detect OpenGL ES, as some desktop OpenGL implementations may report "OpenGL ES" in the version string.
    /// </summary>
    public static bool IsGLES
    { 
        get
        {
            var version = GL.GetString(StringName.Version);

            return
                ForceOpenGLES ||
                (version != null
                && (version.Contains("OpenGL ES", StringComparison.OrdinalIgnoreCase) || version.Contains("3.1 (Core Profile) Mesa", StringComparison.OrdinalIgnoreCase)));
        }
    }

    /// <summary>
    /// Gets the OpenGL version string.
    /// </summary>
    public static string Version => GL.GetString(StringName.Version) ?? "Unknown";
    
    /// <summary>
    /// Gets the GLSL version string.
    /// </summary>
    public static string ShadingLanguageVersion => GL.GetString(StringName.ShadingLanguageVersion) ?? "Unknown";
    
    /// <summary>
    ///  Gets the vendor and renderer strings of the OpenGL context.
    /// </summary>
    public static string Vendor => GL.GetString(StringName.Vendor) ?? "Unknown";
    
    /// <summary>
    /// Gets the renderer string of the OpenGL context.
    /// </summary>
    public static string Renderer => GL.GetString(StringName.Renderer) ?? "Unknown";
}
