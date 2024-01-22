/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngineDemoApp;

using System.Runtime.InteropServices;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

using MicroEngine;
using MicroEngine.Managers;

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
        
        InputManager.Instance.InitializeKeyboard(KeyboardState);
        InputManager.Instance.InitializeMouse(MouseState);
        
        if (_game.Initialize(ClientSize.X, ClientSize.Y) == false)
        {
            Close();
        }
        
        _viewportSizeScaleFactor = Program.Settings.ViewportSizeScaleFactor;
        
        #if DEBUG

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) == false)
        {
            GL.DebugMessageCallback(DebugMessageDelegate, IntPtr.Zero);
            GL.Enable(EnableCap.DebugOutput);
            //GL.Enable(EnableCap.DebugOutputSynchronous);
        }

        #endif
        
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
        
        if (_game.Update((float)e.Time) == false)
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

        GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
        _game.SetCameraAspectRatio(ClientSize.X / (float)ClientSize.Y);
    }
    
    
    #if DEBUG
    
    // https://opentk.net/learn/appendix_opengl/debug_callback.html?tabs=debug-context-4%2Cdelegate-gl%2Cenable-gl

    private static DebugProc DebugMessageDelegate = OnDebugMessage;

    private static void OnDebugMessage(
        DebugSource source,     // Source of the debugging message.
        DebugType type,         // Type of the debugging message.
        int id,                 // ID associated with the message.
        DebugSeverity severity, // Severity of the message.
        int length,             // Length of the string in pMessage.
        IntPtr pMessage,        // Pointer to message string.
        IntPtr pUserParam)      // The pointer you gave to OpenGL, explained later.
    {
        // In order to access the string pointed to by pMessage, you can use Marshal
        // class to copy its contents to a C# string without unsafe code. You can
        // also use the new function Marshal.PtrToStringUTF8 since .NET Core 1.1.
        var message = Marshal.PtrToStringAnsi(pMessage, length);

        // The rest of the function is up to you to implement, however a debug output
        // is always useful.
        Console.WriteLine("[{0} source={1} type={2} id={3}] {4}", severity, source, type, id, message);

        // Potentially, you may want to throw from the function for certain severity
        // messages.
        if (type == DebugType.DebugTypeError)
        {
            throw new Exception(message);
        }
    }
    
    #endif
}
