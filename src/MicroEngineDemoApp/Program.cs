/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngineDemoApp;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;

using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

using MicroEngine;
using MicroEngine.Managers;
using MicroEngine.OGL;

using MicroEngineDemoApp.Games;

internal static class Program
{
    /// <summary>
    /// Global application settings.
    /// </summary>
    public static Settings Settings { get; private set; } = Settings.DefaultSettings;
    
    
    public static void Main(string[] args)
    {
        Configure();

        var primaryMonitor = Monitors.GetPrimaryMonitor();

        var windowWidth = Settings.WindowWidth;
        if (windowWidth < 640)
        {
            windowWidth = 640;
        }
        else if (windowWidth > primaryMonitor.HorizontalResolution)
        {
            windowWidth = primaryMonitor.HorizontalResolution;
        }

        var windowHeight = Settings.WindowHeight;
        if (windowHeight < 480)
        {
            windowHeight = 480;
        }
        else if (windowHeight > primaryMonitor.VerticalResolution)
        {
            windowHeight = primaryMonitor.VerticalResolution;
        }
        
        ResourcesManager.Instance.RootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");
        
        var games = new Dictionary<string, IGame>()
        {
            { "game", new MinimalGameDemo(ResourcesManager.Instance) },  // This is the default game.
            { "many-simple-cubes-demo", new ManySimpleCubesDemo(ResourcesManager.Instance) },
            { "game-with-cubes", new Game(ResourcesManager.Instance) },
            { "rotating-cube-demo-multitex-skybox", new RotatingCubeWithMultiTextureSkyboxDemo(ResourcesManager.Instance) },
            { "game-with-cubes3", new Game3(ResourcesManager.Instance) }, 
            { "game-with-cubes4", new Game4(ResourcesManager.Instance) },  // Rotations too fast.
            { "rotating-cube-demo", new RotatingCubeDemo(ResourcesManager.Instance) },
            { "game-with-cubes5", new Game5(ResourcesManager.Instance) },  // Inverted axes when FpsCamera is connected to a scene object.
            //{ "game-with-cubes2", new Game2(ResourcesManager.Instance) },  // Missing map data.
        };
        
        // This is callback for GLFW errors.
        GLFWProvider.SetErrorCallback(
            (error, description) =>
            {
                Console.Error.WriteLine($"GLFW Error: {error} - {description}");
            });
        
        var nativeWindowSettings = new NativeWindowSettings()
        {
            ClientSize = Settings.EnableFullscreen
                ? new Vector2i(primaryMonitor.HorizontalResolution, primaryMonitor.VerticalResolution)
                : new Vector2i(windowWidth, windowHeight),
            
            Location = Settings.EnableFullscreen
                ? new Vector2i(0, 0)
                : new Vector2i(
                    (primaryMonitor.HorizontalResolution - windowWidth) / 2,
                    (primaryMonitor.VerticalResolution - windowHeight) / 2),
            
            Title = Defaults.AppVersionInfo,
            
            // This is needed to run on macos
            Flags = ContextFlags.ForwardCompatible | ContextFlags.Debug,
            Vsync = Settings.EnableVSync ? VSyncMode.Adaptive : VSyncMode.Off,
            WindowState = Settings.EnableFullscreen
                ? WindowState.Fullscreen
                : WindowState.Normal
        };
        
        // If we want to use OpenGL ES, we need to set the API.
        // Raspberry Pi 5 supports OpenGL ES 3.1, so we can use that.
        // If we want to use OpenGL, we can Use the default settings.
        if (Settings.UseOpenGLES)
        {
            GlContext.ForceOpenGLES = true;
            
            nativeWindowSettings.API = ContextAPI.OpenGLES;
            nativeWindowSettings.APIVersion = new Version(3, 1);
            nativeWindowSettings.Profile = ContextProfile.Any;
        }

        using (var gameWindow = new GameWindow(
                   GameWindowSettings.Default,
                   nativeWindowSettings,
                   games[Settings.Game]
                   ))
        {
            // An example of how to set the update frequency.
            gameWindow.UpdateFrequency = 60.0;
            
            gameWindow.Run();
        }
        
        //MicroEngine.App.Run();
    }
    
    
    private static void Configure()
    {
        const string configFileName = Defaults.ConfigFileName;
        
        // We are reading and writing config file in user's home directory.
        var configFileRootPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        
        // If config file does not exist, create it with default values.
        var configPath = Path.Combine(
            configFileRootPath,
            configFileName);
        if (File.Exists(configPath) == false)
        {
            try
            {
                File.WriteAllText(configPath, Settings.DefaultSettings.ToJson());
            }
            catch (IOException)
            {
                // We are OK with an IO related exception.
            }
        }
        
        var builder = new ConfigurationBuilder()
            .AddJsonFile(
                new PhysicalFileProvider(      // This provider allows us to load config from an absolute path.
                    configFileRootPath,
                    ExclusionFilters.System),  // We are reading from an hidden file.
                configFileName,
                optional: true,
                reloadOnChange: false);  // This slows down app startup. Also - not sure, how to react to config reloads.

        var config = builder.Build();

        Settings = config.GetSection("Settings")
            .Get<Settings>() ?? Settings.DefaultSettings;
    }
}
