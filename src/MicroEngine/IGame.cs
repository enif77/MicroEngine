/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine;

/// <summary>
/// Defines a game.
/// </summary>
public interface IGame
{
    /// <summary>
    /// A name of the game.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Initializes the game.
    /// </summary>
    /// <returns></returns>
    bool Initialize();

    /// <summary>
    /// Updates the game state.
    /// </summary>
    /// <param name="deltaTime">A delta time in seconds.</param>
    /// <returns>true if a next Update() call is requested.</returns>
    bool Update(float deltaTime);

    /// <summary>
    /// Renders the current game state.
    /// </summary>
    void Render();
    

    /// <summary>
    /// Sets the camera aspect ratio.
    /// </summary>
    /// <param name="aspectRatio">A camera aspect ratio.</param>
    void SetCameraAspectRatio(float aspectRatio);
}
