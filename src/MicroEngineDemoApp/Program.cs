/* Copyright (C) Premysl Fara and Contributors */

using MicroEngine;

namespace MicroEngineDemoApp;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;

using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

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

        // An example of how to get the primary monitor.
        //var primaryMonitor = Monitors.GetPrimaryMonitor();

        var games = new Dictionary<string, IGame>()
        {
            { "game-with-cubes", new Game() },
            { "rotating-cube-demo", new RotatingCubeDemo() },
            { "minimal-rotating-cube-demo", new MinimalRotatingCubeDemo() },
        };
        
        var nativeWindowSettings = new NativeWindowSettings()
        {
            ClientSize = new Vector2i(Settings.WindowWidth, Settings.WindowHeight),
            //ClientSize = new Vector2i(primaryMonitor.HorizontalResolution, primaryMonitor.VerticalResolution),
            
            Title = Defaults.AppVersionInfo,
            
            // This is needed to run on macos
            Flags = ContextFlags.ForwardCompatible | ContextFlags.Debug,
            Vsync = Settings.EnableVSync ? VSyncMode.Adaptive : VSyncMode.Off,
            WindowState = Settings.EnableFullscreen ? WindowState.Fullscreen : WindowState.Normal
        };

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
