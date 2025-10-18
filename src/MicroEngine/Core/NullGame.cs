/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Core;

using OpenTK.Windowing.GraphicsLibraryFramework;

using MicroEngine;
using MicroEngine.Managers;

/// <summary>
/// Minimalistic IGAme implementation that does nothing.
/// </summary>
public class NullGame : IGame
{
    public string Name => "null";

    
    public bool Initialize()
    {
        // Nothing to initialize here.
        return true;
    }

    
    public bool Update(float deltaTime)
    {
        // If the user presses the Escape key, we will exit the application.
        return !InputManager.Instance.KeyboardState.IsKeyDown(Keys.Escape);
    }

    
    public void Render()
    {
        // Nothing to render here.
    }
}
