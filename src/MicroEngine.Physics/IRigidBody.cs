/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Physics;

using OpenTK.Mathematics;

/// <summary>
/// Defines a rigid body in the physics world.
/// </summary>
public interface IRigidBody
{
    /// <summary>
    /// A type of the rigid body.
    /// </summary>
    RigidBodyType Type { get; }
    
    /// <summary>
    /// Used to disable a body. A disabled body does not move or collide.
    /// </summary>
    bool IsEnabled { get; set; }
    
    /// <summary>
    /// Optional unique body name. This can be used to identify the body in the physics world.
    /// It is not used for any other purpose and can be set to an empty string if not needed.
    /// It is useful for debugging or when you need to find a specific body in the physics world.
    /// The name should be unique within the physics world, but it is not enforced.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Is true, if this rigid body is allowed to rotate faster than PhysicsWorld.MaximumAngularSpeed.
    /// Should be used on wheels or similar objects.
    /// </summary>
    bool FastRotationAllowed { get; }
    
    /// <summary>
    /// The actual angular velocity of the rigid body in radians per second.
    /// It is a vector representing the rotation around each axis (X, Y, Z).
    /// It is always zero for static bodies.
    /// </summary>
    Vector3 AngularVelocity { get; set; }
    
    /// <summary>
    /// The actual rotation/orientation of the rigid body in the physics world.
    /// </summary>
    Quaternion Orientation { get; set; }
    
    /// <summary>
    /// Is true, if this rigid body is allowed to move faster than PhysicsWorld.MaximumLinearSpeed.
    /// This should be used on bullets or similar objects that need to move quickly without being affected
    /// by the physics engine's speed limits.
    /// </summary>
    bool FastMovementAllowed { get; }
    
    /// <summary>
    /// The actual linear velocity of the rigid body in meters per second.
    /// It is a vector representing the movement along each axis (X, Y, Z).
    /// It is always zero for static bodies.
    /// For kinematic bodies, it is the velocity set by the user.
    /// For dynamic bodies, it is the velocity calculated by the physics engine based on forces and collisions.
    /// </summary>
    Vector3 LinearVelocity { get; set; }
    
    /// <summary>
    /// The actual position of the rigid body in the physics world.
    /// </summary>
    Vector3 Position { get; set; }
    
    /// <summary>
    /// Optional user data associated with the rigid body.
    /// </summary>
    object? UserData { get; set; }
    
    /// <summary>
    /// A collision object associated with this rigid body.
    /// It contains the collision shape and other properties related to collisions.
    /// This object is used by the physics engine to detect collisions and manage interactions
    /// with other collision objects in the physics world.
    /// It is essential for the physics simulation and should be set when creating the rigid body.
    /// </summary>
    ICollisionObject CollisionObject { get; }
    
    
    /// <summary>
    /// Updates the internal state of this physics object, including its position,
    /// orientation, and model matrix, based on the elapsed time and its velocity
    /// and angular velocity.
    /// </summary>
    /// <param name="deltaTime">The time elapsed since the last update, in seconds.</param>
    void Update(float deltaTime);
}