/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Physics;

using OpenTK.Mathematics;

/// <summary>
/// Static rigid body is a body that does not move and is not affected by forces.
/// It does not have a velocity and is used for static objects like terrain or buildings.
/// Static bodies do not collide with other static or kinematic bodies, but they can collide with dynamic bodies.
/// </summary>
internal sealed class StaticRigidBody : IRigidBody
{
    public RigidBodyType Type => RigidBodyType.STATIC_BODY;
    public bool IsEnabled { get; set; } = true;
    public string Name { get; private init; } = string.Empty;
    
    public bool FastRotationAllowed => false;

    /// <summary>
    /// Static bodies do not have angular velocity, so this property always returns zero.
    /// The setter does nothing, as static bodies do not rotate.
    /// This property is here to satisfy the interface contract, but it is not used in practice.
    /// </summary>
    public Vector3 AngularVelocity
    {
        get => Vector3.Zero;
        set { }
    }
    
    /// <summary>
    /// It is possible to set the orientation of a static body, but it does not change its behavior.
    /// The orientation is used for rendering purposes or to align the body with other objects in the
    /// physics world.
    /// The static body does not rotate, so the orientation is always the same unless explicitly changed.
    /// </summary>
    public Quaternion Orientation { get; set; } = Quaternion.Identity;
    
    public bool FastMovementAllowed => false;

    /// <summary>
    /// Static bodies do not have linear velocity, so this property always returns zero.
    /// The setter does nothing, as static bodies do not move.
    /// This property is here to satisfy the interface contract, but it is not used in practice.
    /// </summary>
    public Vector3 LinearVelocity
    {
        get => Vector3.Zero;
        set { }
    } 
    
    /// <summary>
    /// It is possible to set the position of a static body, but it does not change its behavior.
    /// The position is used for rendering purposes or to align the body with other objects in the
    /// physics world.
    /// The static body does not move, so the position is always the same unless explicitly changed.
    /// </summary>
    public Vector3 Position { get; set; } = Vector3.Zero;
    
    public object? UserData { get; set; }

    
    private StaticRigidBody()
    {
        // Private constructor to enforce the use of the Create method.
    }
    

    public void Update(float deltaTime)
    {
    }
    
    
    /// <summary>
    /// Creates a new instance of the static rigid body based on the provided definition.
    /// The definition must not be null, and it should contain the necessary properties to initialize the
    /// static rigid body, such as name, position, orientation, and user data.
    /// The static rigid body will have its type set to STATIC_BODY, and it will not
    /// have any velocity or angular velocity, as it is a static object in the physics world.
    /// </summary>
    /// <param name="definition">A definition of the static rigid body.</param>
    /// <exception cref="ArgumentNullException">Thrown if the definition is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the definition contains invalid values.</exception>
    /// <remarks>
    /// The definition should be an instance of <see cref="RigidBodyDefinition"/> that contains
    /// the necessary properties to initialize the static rigid body.
    /// The static rigid body will be created with the specified name, position, orientation, and
    /// user data, but it will not have any velocity or angular velocity, as it is a static object in the physics world.
    /// The static rigid body will not be affected by forces or collisions, and it will not move or rotate.
    /// It is used for static objects like terrain or buildings that do not change their position or orientation during the simulation.
    /// </remarks>
    /// <returns>Instance of the <see cref="StaticRigidBody"/>.</returns>
    public static StaticRigidBody Create(RigidBodyDefinition definition)
    {
        if (definition is null)
        {
            throw new ArgumentNullException(nameof(definition), "Definition cannot be null.");
        }

        var body = new StaticRigidBody
        {
            Name = definition.Name,
            IsEnabled = definition.IsEnabled,
            
            Orientation = definition.Orientation,
            Position = definition.Position,
            
            UserData = definition.UserData
        };

        return body;
    }
}
