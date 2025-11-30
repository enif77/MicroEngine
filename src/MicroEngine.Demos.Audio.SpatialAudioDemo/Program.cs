/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Demos.Audio.SpatialAudioDemo;

using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

internal static class Program
{
    static void Main(string[] args)
    {
        GLFWProvider.SetErrorCallback(
            (error, description) =>
            {
                Console.Error.WriteLine($"GLFW Error: {error} - {description}");
            });
        
        var primaryMonitor = Monitors.GetPrimaryMonitor();

        var windowWidth = 1280;
        var windowHeight = 800;
        var updateFrequency = 60.0;
        var viewportSizeScaleFactor = 2;
        
        var nativeWindowSettings = new NativeWindowSettings()
        {
            ClientSize = new Vector2i(windowWidth, windowHeight),
            Location = new Vector2i(
                    (primaryMonitor.HorizontalResolution - windowWidth) / 2,
                    (primaryMonitor.VerticalResolution - windowHeight) / 2),
            Title = "MicroEngine Spatial Audio Demo",
            Flags = ContextFlags.ForwardCompatible | ContextFlags.Debug,
            Vsync = VSyncMode.Adaptive,
            WindowState = WindowState.Normal
        };
        
        using (var game = new Game())
        {
            using (var gameWindow = new Window(
                       viewportSizeScaleFactor,
                       GameWindowSettings.Default,
                       nativeWindowSettings,
                       game))
            {
                gameWindow.UpdateFrequency = updateFrequency;
            
                // Run the game...
                gameWindow.Run();
            }
        }
    }
}
