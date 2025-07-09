/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Physics;

using OpenTK.Mathematics;

/// <summary>
/// A rigid body definition used for creating a rigid body.
/// </summary>
public class RigidBodyDefinition
{
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
    /// Used to disable a body. A disabled body does not move or colide.
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
    /// An optional unique body name. This can be used to identify the body in the physics world.
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
    /// A user data that can be used to store any additional information about the rigid body.
    /// This can be any object, such as a reference to a game entity or other relevant data.
    /// </summary>
    public object? UserData { get; set; }
}
