/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Physics;

/// <summary>
/// Defines a contract for resolving collisions between two objects in a physics simulation.
/// </summary>
public interface ICollisionResolver
{
    /// <summary>
    /// Resolves the collision between two objects.
    /// </summary>
    /// <param name="objectA">A reference to the first collision object.</param>
    /// <param name="objectB">A reference to the second collision object.</param>
    /// <param name="deltaTime">What is the time step for this collision resolution? How much time passed since the last physics world update.</param>
    /// <returns>True if the collision was resolved successfully, false otherwise.</returns>
    bool ResolveCollision(
        ICollisionObject objectA,
        ICollisionObject objectB,
        float deltaTime);
}