/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.PhysicsNg;

/// <summary>
/// A definition of a physics word used for creating one.
/// </summary>
public sealed class PhysicsWorldDefinition
{
    /// <summary>
    /// Maximum linear speed. Usually meters per second.
    /// Must be a positive number.
    /// </summary>
    public float MaximumLinearSpeed { get; set; }
    
    /// <summary>
    /// Maximal speed of rotation. Radians per second.
    /// Must be a positive number.
    /// </summary>
    public float MaximumAngularSpeed { get; set; }
    
    /// <summary>
    /// User data that can be used to store any additional information.
    /// </summary>
    public object? UserData { get; set; }
}
