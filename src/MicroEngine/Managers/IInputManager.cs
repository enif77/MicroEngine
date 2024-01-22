/* Copyright (C) Premysl Fara and Contributors */

using OpenTK.Windowing.Common;

namespace MicroEngine.Managers;

using OpenTK.Windowing.GraphicsLibraryFramework;

/// <summary>
/// Manages inputs.
/// </summary>
public interface IInputManager
{
    #region keyboard
    
    /// <summary>
    /// Gets the actual keyboard state.
    /// </summary>
    /// <exception cref="InvalidOperationException">If the InitializeKeyboard() method was not called yet.</exception>
    KeyboardState KeyboardState { get; }
    
    /// <summary>
    /// Initializes the keyboard state.
    /// </summary>
    /// <param name="keyboardState">A keyboard state.</param>
    /// <exception cref="ArgumentNullException">If the keyboardState is null.</exception>
    void InitializeKeyboard(KeyboardState keyboardState);
    
    /// <summary>
    /// Checks, if the given key is pressed.
    /// </summary>
    /// <param name="key">A key code.</param>
    /// <returns>true if key is in the down state; otherwise, false.</returns>
    /// <exception cref="InvalidOperationException">If the InitializeKeyboard() method was not called yet.</exception>
    bool IsKeyPressed(Keys key);
    
    #endregion

    
    #region mouse
    
    /// <summary>
    /// Gets the actual mouse state.
    /// </summary>
    /// <exception cref="InvalidOperationException">If the InitializeMouse() method was not called yet.</exception>
    MouseState? MouseState { get; }
    
    /// <summary>
    /// Initializes the mouse state.
    /// </summary>
    /// <param name="mouseState">A mouse state.</param>
    /// <exception cref="ArgumentNullException">If the mouseState is null.</exception>
    void InitializeMouse(MouseState mouseState);
    
    /// <summary>
    /// Checks, if the given mouse button is pressed.
    /// </summary>
    /// <param name="mouseButton">A mouse button code.</param>
    /// <returns>true if the button is down, otherwise false.</returns>
    /// <exception cref="InvalidOperationException">If the InitializeMouse() method was not called yet.</exception>
    bool IsMouseButtonPressed(MouseButton mouseButton);
    
    /// <summary>
    /// Occurs whenever a mouse wheel is moved.
    /// </summary>
    event Action<MouseWheelEventArgs> MouseWheel;
    
    /// <summary>
    /// Raises the MouseWheel event.
    /// Called, when a user uses a mouse wheel.
    /// </summary>
    /// <param name="e">Event data from the MouseWheel event.</param>
    void OnMouseWheel(MouseWheelEventArgs e);

    #endregion
}