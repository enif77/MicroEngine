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
    /// Updates the internal state of this physics object, including its position,
    /// orientation, and model matrix, based on the elapsed time and its velocity
    /// and angular velocity. Recursively updates all child objects.
    /// </summary>
    /// <param name="deltaTime">The time elapsed since the last update, in seconds.</param>
    public void Update(float deltaTime)
    {
        // TODO: Implement the update logic for the physics world.
        
        // This method should update the state of all rigid bodies in the world,
        // including their positions and orientations based on their velocities and angular velocities.
        // It should also handle collisions, apply forces, and manage the physics simulation.
        // The deltaTime parameter can be used to scale the updates based on the time elapsed.
        // This is a placeholder for the actual implementation.
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