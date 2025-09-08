/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.PhysicsNg;

using OpenTK.Mathematics;

using MicroEngine.PhysicsNg.CollisionObjects;

/// <summary>
/// Contains a kinematic rigid body not affected by forces like gravity or collisions.
/// It can be moved by setting its position or velocity directly.
/// Kinematic bodies do not collide with any other kinds of physical bodies.
/// They can be used for moving platforms or other objects that need to move in a controlled way.
/// </summary>
internal sealed class KinematicRigidBody : IRigidBody
{
    public RigidBodyType Type => RigidBodyType.KINEMATIC_BODY;
    public bool IsEnabled { get; set; } = true;
    public string Name { get; private init; } = string.Empty;

    public bool FastRotationAllowed { get; private init; }
    public Vector3 AngularVelocity { get; set; }
    public Quaternion Orientation { get; set; } = Quaternion.Identity;
    
    public bool FastMovementAllowed { get; private init; }
    public Vector3 LinearVelocity { get; set; } 
    public Vector3 Position { get; set; } = Vector3.Zero;
    
    /// <summary>
    /// Defines the collision object associated with this kinematic rigid body.
    /// It is always the NullCollisionShape, meaning it does not have a physical shape
    /// and does not participate in collision detection.
    /// This is because kinematic bodies are not affected by collisions and do not collide with other
    /// bodies.
    /// </summary>
    public ICollisionObject CollisionObject { get; } = new NullCollisionShape(Vector3.Zero);
    
    public object? UserData { get; set; }

    
    private KinematicRigidBody()
    {
        // Private constructor to enforce the use of the Create method.
    }
    

    public void Update(float deltaTime)
    {
        // If the body is not enabled, do not update its internal state
        if (!IsEnabled)
        {
            return; 
        }
        
        // TODO: Clamp the linear and angular velocities to the maximum speeds defined in the physics world if so configured.
        
        // Update the position based on the linear velocity and delta time.
        Position += LinearVelocity * deltaTime;
        
        // TODO: Clamp the angular velocity to the maximum angular speed defined in the physics world if so configured.
        
        // Update orientation based on angular velocity.
        var angularVelocityQuaternion = new Quaternion(AngularVelocity * deltaTime, 0);
        Orientation += 0.5f * angularVelocityQuaternion * Orientation;
        Orientation = Orientation.Normalized();
        
        // Update the collision object's position to match the rigid body's position.
        // This ensures that the collision detection system knows where the body is located.
        CollisionObject.Position = Position;
    }
    
    
    /// <summary>
    /// Creates a new kinematic rigid body based on the provided definition.
    /// It is used to create a kinematic body that can be moved by setting its position
    /// or velocity directly, without being affected by forces like gravity or collisions.
    /// </summary>
    /// <param name="definition">A rigid body definition used to create a kinematic rigid body.</param>
    /// <returns>Returns a new instance of <see cref="KinematicRigidBody"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the definition is null.</exception>
    public static KinematicRigidBody Create(RigidBodyDefinition definition)
    {
        if (definition is null)
        {
            throw new ArgumentNullException(nameof(definition), "Definition cannot be null.");
        }

        // TODO: Validate the definition properties if needed, such as ensuring that the position and orientation are valid.
        
        var body = new KinematicRigidBody
        {
            Name = definition.Name,
            IsEnabled = definition.IsEnabled,
            
            FastRotationAllowed = definition.AllowFastRotation,
            AngularVelocity = definition.AngularVelocity,
            Orientation = definition.Orientation,
            
            FastMovementAllowed = definition.AllowFastMovement,
            LinearVelocity = definition.LinearVelocity,
            Position = definition.Position,
            
            CollisionObject =
            {
                Position = definition.Position
            },
            
            UserData = definition.UserData
        };

        return body;
    }
}
