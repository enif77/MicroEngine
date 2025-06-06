/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Physics;

using OpenTK.Mathematics;

/// <summary>
/// Defines the interface for a physics object.
/// </summary>
public interface IPhysicsObject
{
    /// <summary>
    /// Gets the root physics object in the hierarchy.
    /// </summary>
    IPhysicsObject Root { get; }
    
    /// <summary>
    /// Gets or sets the parent of this physics object in the hierarchy.
    /// </summary>
    IPhysicsObject? Parent { get; set; }
    
    /// <summary>
    /// A position of the object in the world space.
    /// </summary>
    Vector3 Position { get; set; }
    
    /// <summary>
    /// An orientation of the object in the world space.
    /// </summary>
    Quaternion Orientation { get; set; }
    
    /// <summary>
    /// The mass of this physics object, used for physics calculations.
    /// </summary>
    float Mass { get; }
    
    /// <summary>
    /// Recursively calculates the total mass of this object and all its children.
    /// NOTE: This can be calculated dynamically and may be expensive if the hierarchy is deep.
    /// </summary>
    float TotalMass { get; }
    
    /// <summary>
    /// Combined center of mass of this object and all its children.
    /// NOTE: This can be calculated dynamically and may be expensive if the hierarchy is deep.
    /// </summary>
    Vector3 CombinedCenterOfMass { get; }
    
    /// <summary>
    /// Gets or sets a value indicating whether the model matrix needs to be updated.
    /// Setting this to true propagates the update requirement to all child objects.
    /// </summary>
    bool NeedsModelMatrixUpdate { get; set; }
    
    /// <summary>
    /// Gets or sets the model matrix of this object, representing its transformation
    /// in 3D space.
    /// </summary>
    Matrix4 ModelMatrix { get; }


    /// <summary>
    /// Computes the transformation matrix for this object by combining its local
    /// transformation with the parent's transformation matrix (if any).
    /// </summary>
    /// <returns>The computed transformation matrix.</returns>
    Matrix4 GetTransformationMatrix();

    /// <summary>
    /// Updates the internal state of this physics object, including its position,
    /// orientation, and model matrix, based on the elapsed time and its velocity
    /// and angular velocity. Recursively updates all child objects.
    /// </summary>
    /// <param name="deltaTime">The time elapsed since the last update, in seconds.</param>
    void Update(float deltaTime);

    /// <summary>
    /// Resets the state of this physics object, including its position, orientation,
    /// velocity, angular velocity, and model matrix. Optionally resets all child objects recursively.
    /// </summary>
    /// <param name="resetChildren">If true, resets all child objects recursively.</param>
    void Reset(bool resetChildren = true);

    /// <summary>
    /// Stops the physics object by setting its velocity and angular velocity to zero.
    /// Optionally stops all child objects recursively.
    /// </summary>
    /// <param name="stopChildren">If true, stops all child objects recursively.</param>
    void Stop(bool stopChildren = true);
}