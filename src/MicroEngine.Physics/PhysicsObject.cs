/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Physics;

using OpenTK.Mathematics;

/// <summary>
/// Represents a physics object in a 3D space, capable of hierarchical transformations
/// and dynamic updates based on velocity and angular velocity.
/// </summary>
public class PhysicsObject
{
    /// <summary>
    /// Gets the root physics object in the hierarchy.
    /// </summary>
    public PhysicsObject Root
    {
        get
        {
            var root = this;
            
            // Traverse up the hierarchy to find the root object.
            while (root.Parent != null)
            {
                root = root.Parent;
            }
            
            return root;
        }
    }
    
    /// <summary>
    /// Gets or sets the parent of this physics object in the hierarchy.
    /// </summary>
    public PhysicsObject? Parent { get; set; }

    /// <summary>
    /// Gets the list of child physics objects attached to this object.
    /// </summary>
    public List<PhysicsObject> Children { get; }

    /// <summary>
    /// Optional collision object.
    /// </summary>
    public CollisionObject? CollisionObject { get; set; }

    private Vector3 _position = Vector3.Zero;

    /// <summary>
    /// Gets or sets the local position of this object in 3D space.
    /// Setting this property marks the model matrix for an update.
    /// </summary>
    public Vector3 Position
    {
        get => _position;

        set
        {
            _position = value;
            NeedsModelMatrixUpdate = true;
        }
    }

    /// <summary>
    /// Gets the world position of this object by transforming the local position
    /// using the model matrix.
    /// </summary>
    public Vector3 WorldPosition
        => Vector3.TransformPosition(Position, ModelMatrix);
        
    /// <summary>
    /// Gets or sets the velocity of this object in 3D space.
    /// This is a local velocity that influences the object's position over time.
    /// </summary>
    public Vector3 Velocity { get; set; } = Vector3.Zero;
    
    /// <summary>
    /// The acceleration of this object in 3D space.
    /// </summary>
    public Vector3 Acceleration { get; set; } = Vector3.Zero;
    
    private Quaternion _orientation = Quaternion.Identity;
    
    /// <summary>
    /// Gets or sets the orientation of this object as a quaternion.
    /// Setting this property marks the model matrix for an update.
    /// </summary>
    public Quaternion Orientation
    {
        get => _orientation;

        set
        {
            _orientation = value;
            NeedsModelMatrixUpdate = true;
        }
    }

    /// <summary>
    /// Gets or sets the angular velocity of this object in 3D space.
    /// This is a local angular velocity that influences the object's orientation over time.
    /// </summary>
    public Vector3 AngularVelocity { get; set; } = Vector3.Zero;
    
    /// <summary>
    /// Gets or sets the angular acceleration of this object in 3D space.
    /// This is a local angular acceleration that influences the object's angular velocity over time.
    /// </summary>
    public Vector3 AngularAcceleration { get; set; } = Vector3.Zero;
    
    /// <summary>
    /// The mass of this physics object, used for physics calculations.
    /// </summary>
    public float Mass { get; set; } = 1.0f;
    
    /// <summary>
    /// Recursively calculates the total mass of this object and all its children.
    /// NOTE: This is calculated dynamically and may be expensive if the hierarchy is deep.
    /// </summary>
    public float TotalMass
    {
        get
        {
            // Calculate the total mass of this object and all its children.
            var totalMass = Mass;
            foreach (var child in Children)
            {
                totalMass += child.TotalMass;
            }
            
            return totalMass;
        }
    }
    
    /// <summary>
    /// The center of mass of this physics object, relative to its position, used for physics calculations.
    /// </summary>
    public Vector3 CenterOfMass { get; set; } = Vector3.Zero;
    
    /// <summary>
    /// Combined center of mass of this object and all its children.
    /// NOTE: This is calculated dynamically and may be expensive if the hierarchy is deep.
    /// </summary>
    public Vector3 CombinedCenterOfMass
    {
        get
        {
            // Calculate the combined center of mass of this object and all its children.
            var totalMass = TotalMass;
            if (totalMass == 0)
            {
                return Vector3.Zero;
            }

            var combinedCenterOfMass = CenterOfMass * Mass;
            foreach (var child in Children)
            {
                combinedCenterOfMass += child.CombinedCenterOfMass * child.Mass;
            }

            return combinedCenterOfMass / totalMass;
        }
    }
    
    /// <summary>
    /// The inertia tensor of this physics object, used for physics calculations.
    /// </summary>
    public Matrix3 InertiaTensor { get; set; } = Matrix3.Identity;
    
    private bool _needsModelMatrixUpdate = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether the model matrix needs to be updated.
    /// Setting this to true propagates the update requirement to all child objects.
    /// </summary>
    public bool NeedsModelMatrixUpdate
    {
        get => _needsModelMatrixUpdate;

        set
        {
            _needsModelMatrixUpdate = value;
            if (value == false)
            {
                return;
            }

            foreach (var child in Children)
            {
                child.NeedsModelMatrixUpdate = true;
            }
        }
    }

    /// <summary>
    /// Gets or sets the model matrix of this object, representing its transformation
    /// in 3D space.
    /// </summary>
    public Matrix4 ModelMatrix { get; private set; } = Matrix4.Identity;


    /// <summary>
    /// Initializes a new instance of the <see cref="PhysicsObject"/> class.
    /// </summary>
    public PhysicsObject()
    {
        Children = [];
    }


    /// <summary>
    /// Adds a child physics object to this object, setting this object as the parent
    /// of the child.
    /// </summary>
    /// <param name="child">The child physics object to add.</param>
    public void AddChild(PhysicsObject child)
    {
        child.Parent = this;
        Children.Add(child);
    }

    /// <summary>
    /// Removes a child physics object from this object, clearing the parent reference
    /// of the child.
    /// </summary>
    /// <param name="child">The child physics object to remove.</param>
    public void RemoveChild(PhysicsObject child)
    {
        if (Children.Remove(child))
        {
            child.Parent = null;
        }
    }
    
    /// <summary>
    /// Sets rotation around X axis.
    /// </summary>
    /// <param name="angle">An angle in radians.</param>
    public void SetRotationX(float angle)
    {
        // Create a quaternion representing the rotation around the X axis.
        var rotationX = Quaternion.FromAxisAngle(Vector3.UnitX, angle);
        
        // Combine the current orientation with the new rotation.
        Orientation = rotationX * Orientation;
    }
    
    /// <summary>
    /// Sets rotation around Y axis.
    /// </summary>
    /// <param name="angle">An angle in radians.</param>
    public void SetRotationY(float angle)
    {
        // Create a quaternion representing the rotation around the Y axis.
        var rotationY = Quaternion.FromAxisAngle(Vector3.UnitY, angle);
        
        // Combine the current orientation with the new rotation.
        Orientation = rotationY * Orientation;
    }
    
    /// <summary>
    /// Sets rotation around Z axis.
    /// </summary>
    /// <param name="angle">An angle in radians.</param>
    public void SetRotationZ(float angle)
    {
        // Create a quaternion representing the rotation around the Z axis.
        var rotationZ = Quaternion.FromAxisAngle(Vector3.UnitZ, angle);
        
        // Combine the current orientation with the new rotation.
        Orientation = rotationZ * Orientation;
    }
    
    /// <summary>
    /// Apply a force to the object, which will affect its velocity.
    /// Will update the acceleration based on the force applied.
    /// </summary>
    /// <param name="force">The force vector to apply to the object. It is in the world space.</param>
    public void ApplyForce(Vector3 force)
    {
        // Assuming a simple F = m * a relationship.
        Acceleration = force / TotalMass; 
    }

    /// <summary>
    /// Applies a torque to the object, which will affect its angular velocity.
    /// </summary>
    /// <param name="torque">The torque vector to apply to the object. It is in the objects space.</param>
    public void ApplyTorque(Vector3 torque)
    {   
        // Assuming a simple T = I * α relationship, where T is torque, I is inertia tensor, and α is angular acceleration.
        AngularAcceleration = InertiaTensor.Inverted() * torque;
    }
    
    /// <summary>
    /// Applies a force in the direction of the object's orientation.
    /// Can be used to simulate a thruster connected to the physics object.
    /// </summary>
    /// <param name="forcePosition">The position where the force is applied, typically the center of mass or a specific point on the object.</param>
    /// <param name="forceOrientation">The normalized force vector to apply, typically representing thrust.
    /// It is in the local space of the physics object.</param>
    /// <param name="forceStrength">A factor to scale the force.</param>
    public void ApplyLocalForce(Vector3 forcePosition, Vector3 forceOrientation, float forceStrength)
    {
        // Calculate the oriented direction of the force based on the object's orientation.
        var orientedDirection = Vector3
            .Transform(forceOrientation, Orientation)
            .Normalized();

        var orientedForce = orientedDirection * forceStrength;
        
        // Apply the force in the direction of this object's orientation, scaled by the thrust factor.
        ApplyForce(orientedForce);
        
        // Calculate the torque generated by the force.
        // The torque is calculated as the cross product of the position vector (from the center of mass to the force position).
        // The position vector is transformed to the physics object's local space using the physics object's orientation.
        var r = Vector3.Transform(forcePosition, Orientation) - CombinedCenterOfMass;
        
        // Apply the torque to the object.
        ApplyTorque(Vector3.Cross(r, orientedForce));
    }
    
    /// <summary>
    /// Computes the transformation matrix for this object by combining its local
    /// transformation with the parent's transformation matrix (if any).
    /// </summary>
    /// <returns>The computed transformation matrix.</returns>
    public Matrix4 GetTransformationMatrix()
    {
        // Calculate local transformation matrix.
        var localMatrix = Matrix4.CreateFromQuaternion(Orientation) * Matrix4.CreateTranslation(Position);

        // If there's a parent, combine with the parent's transformation matrix.
        if (Parent != null)
        {
            return localMatrix * Parent.GetTransformationMatrix();
        }

        return localMatrix;
    }

    /// <summary>
    /// Updates the internal state of this physics object, including its position,
    /// orientation, and model matrix, based on the elapsed time and its velocity
    /// and angular velocity. Recursively updates all child objects.
    /// </summary>
    /// <param name="deltaTime">The time elapsed since the last update, in seconds.</param>
    public void Update(float deltaTime)
    {
        // Acceleration can be updated based on forces applied to the object.
        // See the ApplyForce() method for an example of how to apply forces.
        Velocity += Acceleration * deltaTime;
        
        // Update position based on velocity.
        Position += Velocity * deltaTime;
        
        // Angular acceleration can be updated based on torques applied to the object.
        // See the ApplyTorque() method for an example of how to apply torques.
        AngularVelocity += AngularAcceleration * deltaTime;
        
        // Update orientation based on angular velocity.
        var angularVelocityQuat = new Quaternion(AngularVelocity * deltaTime, 0);
        Orientation += 0.5f * angularVelocityQuat * Orientation;
        Orientation = Orientation.Normalized();
        
        // Update the position of the collision object (if any).
        if (CollisionObject != null)
        {
            CollisionObject.Position = Position;
        }

        if (NeedsModelMatrixUpdate)
        {
            ModelMatrix = GetTransformationMatrix();
        
            if (Parent != null)
            {
                ModelMatrix *= Parent.ModelMatrix;
            }

            NeedsModelMatrixUpdate = false;
        }

        // Update children recursively.
        foreach (var child in Children)
        {
            child.Update(deltaTime);
        }
    }
    
    /// <summary>
    /// Resets the state of this physics object, including its position, orientation,
    /// velocity, angular velocity, and model matrix. Optionally resets all child objects recursively.
    /// </summary>
    /// <param name="resetChildren">If true, resets all child objects recursively.</param>
    public void Reset(bool resetChildren = true)
    {
        Position = Vector3.Zero;
        Orientation = Quaternion.Identity;
        Acceleration = Vector3.Zero;
        AngularAcceleration = Vector3.Zero;
        Velocity = Vector3.Zero;
        AngularVelocity = Vector3.Zero;
        CenterOfMass = Vector3.Zero;
        InertiaTensor = Matrix3.Identity;
        Mass = 1.0f;
        NeedsModelMatrixUpdate = true;

        // Reset the model matrix
        ModelMatrix = Matrix4.Identity;

        if (!resetChildren)
        {
            return;
        }
        
        // Reset children recursively
        foreach (var child in Children)
        {
            child.Reset();
        }
    }
    
    /// <summary>
    /// Stops the physics object by setting its velocity and angular velocity to zero.
    /// Optionally stops all child objects recursively.
    /// </summary>
    /// <param name="stopChildren">If true, stops all child objects recursively.</param>
    public void Stop(bool stopChildren = true)
    {
        Acceleration = Vector3.Zero;
        AngularAcceleration = Vector3.Zero;
        Velocity = Vector3.Zero;
        AngularVelocity = Vector3.Zero;

        if (!stopChildren)
        {
            return;
        }
            
        // Reset children recursively
        foreach (var child in Children)
        {
            child.Stop();
        }
    }
}
