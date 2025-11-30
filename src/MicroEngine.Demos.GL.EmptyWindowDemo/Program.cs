/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Demos.GL.EmptyWindowDemo;

using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

using MicroEngine.Core;
using MicroEngine.OGL;

internal static class Program
{
    // Set this to 2 for high-DPI displays (MacBook), otherwise set it to 1.
    public static int ViewportSizeScaleFactor { get; } = 1;
    
    // Hardcoded settings.
    private const int WindowWidth = 800;
    private const int WindowHeight = 600;
    
    // Set to true to use OpenGL ES (e.g., on Raspberry Pi 5), false to use standard OpenGL.
    private const bool UseOpenGLES = false;
    
    static void Main(string[] args)
    {
        // This is a callback for GLFW errors.
        // We don't need to do anything special here, as GLFW will print the error to stderr.'
        // The default implementation throws the "GLFW Error: FeatureUnavailable - Wayland: The platform does not support setting the window position"
        // exception on Wayland platforms.
        GLFWProvider.SetErrorCallback(
            (error, description) =>
            {
                Console.Error.WriteLine($"GLFW Error: {error} - {description}");
            });
        
        // We want to center the window on the primary monitor.
        var primaryMonitor = Monitors.GetPrimaryMonitor();
        
        // Configure the native window settings.
        var nativeWindowSettings = new NativeWindowSettings()
        {
            ClientSize = new Vector2i(WindowWidth, WindowHeight),
            Location = new Vector2i(
                    (primaryMonitor.HorizontalResolution - WindowWidth) / 2,
                    (primaryMonitor.VerticalResolution - WindowHeight) / 2),
            Title = "MicroEngine Empty Window Demo 1.0.0",
            Flags = ContextFlags.ForwardCompatible | ContextFlags.Debug,
            WindowState = WindowState.Normal
        };
        
        // If we want to use OpenGL ES, we need to set the API.
        // Raspberry Pi 5 supports OpenGL ES 3.1, so we can use that.
        // If we want to use OpenGL, we can Use the default settings.
        if (UseOpenGLES)
        {
            GlContext.ForceOpenGLES = true;
            
            nativeWindowSettings.API = ContextAPI.OpenGLES;
            nativeWindowSettings.APIVersion = new Version(3, 1);
            nativeWindowSettings.Profile = ContextProfile.Any;
        }
        
        using (var gameWindow = new Window(
                   GameWindowSettings.Default,
                   nativeWindowSettings,
                   new NullGame()))
        {
            gameWindow.Run();
        }
    }
}
