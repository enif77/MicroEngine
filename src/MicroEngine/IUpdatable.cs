/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine;

/// <summary>
/// Defines an object, that can be updated over time.
/// </summary>
public interface IUpdatable
{
    /// <summary>
    /// Updates a game object.
    /// </summary>
    /// <param name="deltaTime">How much time passed since the last call of this method.</param>
    void Update(float deltaTime);
}
