/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngineDemoApp;

using OpenTK.Windowing.GraphicsLibraryFramework;

using MicroEngine;

public class Game : IGame
{
    public bool Initialize(int width, int height)
    {
        // TODO: Initialize the game.
        
        return true;
    }

    
    public bool Update(float deltaTime, KeyboardState keyboardState, MouseState mouseState)
    {
        if (keyboardState.IsKeyDown(Keys.Escape))
        {
            return false;
        }
        
        // TODO: Update the game state.
        
        return true;
    }

    
    public void Render()
    {
        // TODO: Render the game state.
    }

    
    public void UpdateCameraFov(float fovChange)
    {
        
    }

    public void SetCameraAspectRatio(float aspectRatio)
    {
        
    }
}