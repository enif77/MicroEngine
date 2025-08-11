/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Physics;

using OpenTK.Mathematics;

/// <summary>
/// A rigid body definition used for creating a rigid body.
/// </summary>
public class RigidBodyDefinition
{
    /// <summary>
    /// This allows fast movement of the rigid body. Should be used on bullets or similar objects.
    /// </summary>
    public bool AllowFastMovement { get; set; }
    
    /// <summary>
    /// This allows fast rotation of the rigid body. Should be used on wheels or similar objects.
    /// </summary>
    public bool AllowFastRotation { get; set; }
    
    /// <summary>
    /// Angular damping is used to slow down the rotation of the rigid body.
    /// Should be a positive number. Can be zero if no damping is needed. Values above 1.0f can lead to instability.
    /// </summary>
    public float AngularDamping { get; set; }
    
    /// <summary>
    /// The initial angular velocity of the rigid body. Radians per second.
    /// </summary>
    public Vector3 AngularVelocity { get; set; } = Vector3.Zero;
    
    /// <summary>
    /// Should this body be prevented from rotating?
    /// </summary>
    public bool FixedRotation { get; set; }
    
    /// <summary>
    /// Used to disable a body. A disabled body does not move or collide.
    /// </summary>
    public bool IsEnabled { get; set; } = true;
    
    /// <summary>
    /// Linear damping is used to slow down the movement of the rigid body. Should be a positive number. Values above 1.0f can lead to instability.
    /// </summary>
    public float LinearDamping { get; set; }
    
    /// <summary>
    /// The initial linear velocity of the rigid body. Meters per second.
    /// </summary>
    public Vector3 LinearVelocity { get; set; } = Vector3.Zero;
    
    /// <summary>
    /// Optional unique body name. This can be used to identify the body in the physics world.
    /// It is not used for any other purpose and can be set to an empty string if not needed.
    /// It is useful for debugging or when you need to find a specific body in the physics world.
    /// The name should be unique within the physics world, but it is not enforced.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// The initial position of the rigid body in the physics world.
    /// </summary>
    public Vector3 Position { get; set; } = Vector3.Zero;
    
    /// <summary>
    /// The initial rotation/orientation of the rigid body in the physics world.
    /// </summary>
    public Quaternion Orientation { get; set; } = Quaternion.Identity;
    
    /// <summary>
    /// What type of rigid body should be created?
    /// </summary>
    public RigidBodyType Type { get; set; } = RigidBodyType.STATIC_BODY;
    
    /// <summary>
    /// Optional user data associated with the rigid body.
    /// This can be used to store any additional information about the rigid body.
    /// This data is not used by the physics engine and is purely for user-defined purposes.
    /// It can be used to store metadata, references to game objects, or any other information that is relevant to the rigid body.
    /// The user data can be set or retrieved at any time, and it does not affect the physics simulation in any way.
    /// </summary>
    public object? UserData { get; set; }
}
