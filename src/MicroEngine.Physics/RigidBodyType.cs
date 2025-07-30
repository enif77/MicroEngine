/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Physics;

/// <summary>
/// Defines the type of rigid body in the physics world.
/// </summary>
public enum RigidBodyType
{
    /// <summary>
    /// Static body is a body that does not move and is not affected by forces. It does not have a velocity and is used for static objects like terrain or buildings.
    /// Static bodies do not collide with other static or kinematic bodies, but they can collide with dynamic bodies.
    /// </summary>
    STATIC_BODY,
    
    /// <summary>
    /// The kinematic body moves under simulation according to its velocity and acceleration, but it is not affected by forces like gravity or collisions.
    /// </summary>
    KINEMATIC_BODY,
    
    /// <summary>
    /// Fully simulated body that is affected by forces, collisions, and other physical interactions.
    /// </summary>
    DYNAMIC_BODY
}
