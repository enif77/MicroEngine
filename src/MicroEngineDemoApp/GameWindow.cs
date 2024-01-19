/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngineDemoApp;

using System.Runtime.InteropServices;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

using MicroEngine;

public class GameWindow : OpenTK.Windowing.Desktop.GameWindow
{
    private int _viewportSizeScaleFactor = 1;
    private readonly IGame _game;
    
    
    public GameWindow(
        GameWindowSettings gameWindowSettings,
        NativeWindowSettings nativeWindowSettings,
        IGame game)
        : base(gameWindowSettings, nativeWindowSettings)
    {
        _game = game ?? throw new ArgumentNullException(nameof(game));
    }
    
    
    protected override void OnLoad()
    {
        base.OnLoad();
        
        if (_game.Initialize(Size.X, Size.Y) == false)
        {
            Close();
        }
        
        _viewportSizeScaleFactor = Program.Settings.ViewportSizeScaleFactor;
        
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        GL.Enable(EnableCap.DepthTest);
        
        CursorState = CursorState.Grabbed;
    }
    
    
    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        // MacOS needs to update the viewport on each frame.
        // FramebufferSize returns correct values, only Cocoa visual render size is halved.
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) && _viewportSizeScaleFactor > 1)
        {
            var cs = ClientSize;
            GL.Viewport(0, 0, cs.X * _viewportSizeScaleFactor, cs.Y * _viewportSizeScaleFactor);
        }
        
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        
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
        
        if (_game.Update((float)e.Time, KeyboardState, MouseState) == false)
        {
            Close();
        }
    }

    
    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);

        _game.UpdateCameraFov(-e.OffsetY);
    }

    
    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);

        GL.Viewport(0, 0, Size.X, Size.Y);
        _game.SetCameraAspectRatio(Size.X / (float)Size.Y);
    }
}
