/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Physics;

/// <summary>
/// Physics world is a container for all physics objects.
/// </summary>
public class PhysicsWorld
{
    /// <summary>
    /// User data that can be used to store any additional information.
    /// </summary>
    public object? UserData { get; private set; }
    
    /// <summary>
    /// The maximum linear speed of the physics world.
    /// </summary>
    public float MaximumLinearSpeed { get; private set; }
    
    /// <summary>
    /// The maximum angular speed of the physics world.
    /// </summary>
    public float MaximumAngularSpeed { get; private set; }
    
    
    /// <summary>
    /// Physics word must be Created. 
    /// </summary>
    private PhysicsWorld()
    {
    }
    
    
    /// <summary>
    /// Creates a new instance of the physics world based on the provided definition.
    /// </summary>
    /// <param name="definition">A definition of the physics world.</param>
    /// <returns>A new instance of the <see cref="PhysicsWorld"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the definition is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the maximum linear or angular speed is not a positive number.</exception>
    public static PhysicsWorld Create(PhysicsWorldDefinition definition)
    {
        if (definition == null)
        {
            throw new ArgumentNullException(nameof(definition));
        }

        if (definition.MaximumLinearSpeed <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(definition.MaximumLinearSpeed), "Maximum linear speed must be a positive number.");
        }

        if (definition.MaximumAngularSpeed <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(definition.MaximumAngularSpeed), "Maximum angular speed must be a positive number.");
        }

        var word = new PhysicsWorld
        {
            MaximumLinearSpeed = definition.MaximumLinearSpeed,
            MaximumAngularSpeed = definition.MaximumAngularSpeed,
            UserData = definition.UserData
        };

        return word;
    }
}