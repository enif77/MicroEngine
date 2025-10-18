/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Demos.GL.RotatingCubeDemo;

using System.Runtime.InteropServices;

using OpenTK.Graphics.OpenGL4;
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
    private readonly Settings _settings;
    private readonly IGame _game;
    
    
    public Window(
        Settings settings,
        GameWindowSettings gameWindowSettings,
        NativeWindowSettings nativeWindowSettings,
        IGame game)
        : base(gameWindowSettings, nativeWindowSettings)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
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
        
        #if DEBUG

        PrintGlContextInfo();
        
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) == false)
        {
            GL.DebugMessageCallback(DebugMessageDelegate, IntPtr.Zero);
            GL.Enable(EnableCap.DebugOutput);
            //GL.Enable(EnableCap.DebugOutputSynchronous);
        }

        #endif
        
        GlRenderer.SetViewport(0, 0, ClientSize.X, ClientSize.Y);
        GlRenderer.SetClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        GlRenderer.EnableDepthTest();
        
        CursorState = CursorState.Grabbed;
    }
    
    
    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        var viewportSizeScaleFactor = _settings.ViewportSizeScaleFactor;
        
        // macOS needs to update the viewport on each frame.
        // FramebufferSize returns correct values, only Cocoa visual render size is halved.
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) && viewportSizeScaleFactor > 1)
        {
            GlRenderer.SetViewport(0, 0, ClientSize.X * viewportSizeScaleFactor, ClientSize.Y * viewportSizeScaleFactor);
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

    
    #if DEBUG
    
    public static void PrintGlContextInfo()
    {
        Console.WriteLine("GL Version: " + GlContext.Version);
        Console.WriteLine("GLSL Version: " + GlContext.ShadingLanguageVersion);
        Console.WriteLine("Vendor: " + GlContext.Vendor);
        Console.WriteLine("Renderer: " + GlContext.Renderer);
    }
    
    
    private static readonly DebugProc DebugMessageDelegate = OnDebugMessage;

    private static void OnDebugMessage(
        DebugSource source,     // Source of the debugging message.
        DebugType type,         // Type of the debugging message.
        int id,                 // ID associated with the message.
        DebugSeverity severity, // Severity of the message.
        int length,             // Length of the string in pMessage.
        IntPtr pMessage,        // Pointer to message string.
        IntPtr pUserParam)      // The pointer you gave to OpenGL, explained later.
    {
        var message = Marshal.PtrToStringAnsi(pMessage, length);

        // https://stackoverflow.com/questions/62248552/opengl-debug-context-warning-will-use-video-memory-as-the-source-for-buffer-o
        // [DebugSeverityNotification source=DebugSourceApi type=DebugTypeOther id=131185]
        // Buffer detailed info: Buffer object 16 (bound to GL_ARRAY_BUFFER_ARB, usage hint is GL_DYNAMIC_DRAW) will use VIDEO memory as the source for buffer object operations.
        if (id == 131185)
        {
            return;
        }

        Console.WriteLine("[{0} source={1} type={2} id={3}] {4}", severity, source, type, id, message);
    }
    
    #endif
}
