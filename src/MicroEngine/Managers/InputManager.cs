/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Managers;

using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

/// <summary>
/// The default input manager.
/// </summary>
public class InputManager : IInputManager
{
    #region singleton
    
    /// <summary>
    /// Gets instance of the default input manager.
    /// </summary>
    public static InputManager Instance { get; } = new InputManager();
    
    
    /// <summary>
    /// This input manager is a singleton.
    /// </summary>
    private InputManager()
    {
    }
    
    #endregion
    
    
    #region keyboard
    
    /// <inheritdoc/>
    public KeyboardState KeyboardState => _keyboardState ?? throw KeyboardStateNotInitializedException;
    
    /// <inheritdoc/>
    public bool IsKeyPressed(Keys key)
    {
        return _keyboardState?.IsKeyDown(key) ?? throw KeyboardStateNotInitializedException;
    }
    
    /// <inheritdoc/>
    public void InitializeKeyboard(KeyboardState keyboardState)
    {
        _keyboardState = keyboardState ?? throw new ArgumentNullException(nameof(keyboardState));
    }
    
    
    private KeyboardState? _keyboardState;
    
    private static readonly Exception KeyboardStateNotInitializedException =
        new InvalidOperationException("Keyboard state is not initialized.");
    
    #endregion


    #region mouse
    
    /// <inheritdoc/>
    public MouseState MouseState => _mouseState ?? throw MouseStateNotInitializedException;
    
    /// <inheritdoc/>
    public event Action<MouseWheelEventArgs>? MouseWheel;
    
    /// <inheritdoc/>
    public bool IsMouseButtonPressed(MouseButton mouseButton)
    {
        return _mouseState?.IsButtonDown(mouseButton) ?? throw MouseStateNotInitializedException;
    }
    
    /// <inheritdoc/>
    public void InitializeMouse(MouseState mouseState)
    {
        _mouseState = mouseState ?? throw new ArgumentNullException(nameof(mouseState));
    }
    
    /// <inheritdoc/>
    public void OnMouseWheel(MouseWheelEventArgs e)
    {
        MouseWheel?.Invoke(e);
    }
    
    
    private MouseState? _mouseState;
    
    private static readonly Exception MouseStateNotInitializedException =
        new InvalidOperationException("Mouse state is not initialized.");
    
    #endregion
}