/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngineDemoApp;

using System.Text.Json;

/// <summary>
/// Represents application settings.
/// </summary>
public class Settings
{
    #region main window
    
    /// <summary>
    /// Game window width.
    /// </summary>
    public int WindowWidth { get; init; } = Defaults.DefaultWindowWidth;
    
    /// <summary>
    /// Game window height.
    /// </summary>
    public int WindowHeight { get; init; } = Defaults.DefaultWindowHeight;
    
    /// <summary>
    /// Viewport size scale factor.
    /// Normally its 1, but on retina displays it's 2 or more.
    /// </summary>
    public int ViewportSizeScaleFactor { get; init; } = Defaults.DefaultViewportSizeScaleFactor;
    
    /// <summary>
    /// Enables or disables vertical synchronization.
    /// </summary>
    public bool EnableVSync { get; init; } = Defaults.DefaultEnableVSync;
    
    /// <summary>
    /// Enables or disables fullscreen mode.
    /// </summary>
    public bool EnableFullscreen { get; init; } = Defaults.DefaultEnableFullscreen;
    
    /// <summary>
    /// Force OpenGL ES rendering.
    /// We cannot detect it automatically, so this is a setting.
    /// </summary>
    public bool UseOpenGLES { get; init; } = Defaults.DefaultUseOpenGLES;
    
    #endregion

    
    #region game

    /// <summary>
    /// A name of a game to run.
    /// </summary>
    public string Game { get; init; } = Defaults.DefaultGame;
    
    #endregion
    
    /// <summary>
    /// Returns default settings.
    /// </summary>
    public static Settings DefaultSettings
        => new();

    /// <summary>
    /// Converts this instance to JSON.
    /// </summary>
    /// <returns>JSON representation of this instance.</returns>
    public string ToJson()
    {
        return JsonSerializer.Serialize(
            new SettingsContainer()
            {
                Settings = this
            },
            JsonSerializerOptions);
    }

    
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        WriteIndented = true
    };
    

    private class SettingsContainer
    {
        public Settings? Settings { get; set; }
    }
}
