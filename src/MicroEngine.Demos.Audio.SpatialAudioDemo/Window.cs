/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Demos.Audio.SpatialAudioDemo;

using System.Runtime.InteropServices;

using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

using MicroEngine;
using MicroEngine.Managers;
using MicroEngine.OGL;

/// <summary>
/// The main window of the application.
/// </summary>
internal class Window : GameWindow
{
    private readonly int _viewportSizeScaleFactor;
    private readonly IGame _game;
    
    
    public Window(
        int viewportSizeScaleFactor,
        GameWindowSettings gameWindowSettings,
        NativeWindowSettings nativeWindowSettings,
        IGame game)
        : base(gameWindowSettings, nativeWindowSettings)
    {
        _viewportSizeScaleFactor = viewportSizeScaleFactor;
        _game = game ?? throw new ArgumentNullException(nameof(game));
    }
    
    
    protected override void OnLoad()
    {
        base.OnLoad();
        
        InputManager.Instance.InitializeKeyboard(KeyboardState);
        InputManager.Instance.InitializeMouse(MouseState);
        
        if (_game.Initialize() == false)
        {
            Close();
        }
        
        GlRenderer.SetClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        GlRenderer.EnableDepthTest();
        
        CursorState = CursorState.Grabbed;
    }
    
    
    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);
        
        // MacOS needs to update the viewport on each frame.
        // FramebufferSize returns correct values, only Cocoa visual render size is halved.
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) && _viewportSizeScaleFactor > 1)
        {
            GlRenderer.SetViewport(0, 0, ClientSize.X * _viewportSizeScaleFactor, ClientSize.Y * _viewportSizeScaleFactor);
        }
        
        GlRenderer.ClearScreen();
        
        _game.Render();

        SwapBuffers();
    }
    
    
    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);

        if (!IsFocused)
        {
            return;
        }
        
        if (_game.Update((float)e.Time) == false)
        {
            Close();
        }
    }

    
    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        
        GlRenderer.SetViewport(0, 0, ClientSize.X, ClientSize.Y);
    }
}
