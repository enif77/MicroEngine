/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.SceneObjects;

using OpenTK.Mathematics;

using MicroEngine;
using MicroEngine.Extensions;

/// <summary>
/// Adds Yaw/Pitch/Roll/Advance/Ascend/Strafe methods to the SceneObjectBase class.
/// Can be used to control a camera or any other scene object.
/// </summary>
public class SceneObjectController : SceneObjectBase
{
    private Vector3 _frontVector = -Vector3.UnitZ;
    private Vector3 _upVector = Vector3.UnitY;
    private Vector3 _rightVector = Vector3.UnitX;
    
    
    /// <summary>
    /// Updates the axes of this controller based on its actual rotation angles.
    /// Call this method after you change the rotation angles so the axes are updated.
    /// </summary>
    public void UpdateAxes()
    {
        // Remember the current rotation.
        var thisRotation = Rotation;
        
        // Reset the rotation.
        Rotation = Vector3.Zero;
        
        // Reset the axes.
        _frontVector = -Vector3.UnitZ;
        _upVector = Vector3.UnitY;
        _rightVector = Vector3.UnitX;
       
        // Apply the rotation.
        // Yaw(thisRotation.Y);
        // Pitch(thisRotation.X);
        // Roll(thisRotation.Z);
        Roll(thisRotation.Z);
        Pitch(thisRotation.X);
        Yaw(thisRotation.Y);
    }
    
    /// <summary>
    /// Turns this camera to the left or right.
    /// </summary>
    /// <param name="angle">The amount of degrees in radians this camera should turn left or right.</param>
    public void Yaw(float angle)
    {
        this.SetRotationY(Rotation.Y + angle);
        
        var m = Matrix4.CreateFromAxisAngle(_upVector, angle);
        
        _rightVector = Vector3.TransformVector(_rightVector, m);
        _frontVector = Vector3.TransformVector(_frontVector, m);
    }
    
    /// <summary>
    /// Turns this camera up or down.
    /// </summary>
    /// <param name="angle">The amount of degrees in radians this camera should turn left or right.</param>
    public void Pitch(float angle)
    {
        this.SetRotationX(Rotation.X + angle);
        
        var m = Matrix4.CreateFromAxisAngle(_rightVector, angle);
        
        _upVector = Vector3.TransformVector(_upVector, m);
        _frontVector = Vector3.TransformVector(_frontVector, m);
    }

    /// <summary>
    /// Rolls this camera to the left or right.
    /// </summary>
    /// <param name="angle">The amount of degrees in radians this camera should roll left or right.</param>
    public void Roll(float angle)
    {
        this.SetRotationZ(Rotation.Z + angle);
        
        var m = Matrix4.CreateFromAxisAngle(_frontVector, angle); 
        
        _rightVector = Vector3.TransformVector(_rightVector, m);
        _upVector = Vector3.TransformVector(_upVector, m);
    }
    
    /// <summary>
    /// Moves the camera forward or backward.
    /// </summary>
    /// <param name="distance">How far this camera should move from its current position.</param>
    public void Advance(float distance)
    {
        Position += _frontVector * distance;
    }

    /// <summary>
    /// Moves the camera up or down.
    /// </summary>
    /// <param name="distance">How far this camera should move from its current position.</param>
    public void Ascend(float distance)
    {
        Position += _upVector * -distance;
    }

    /// <summary>
    /// Moves the camera left or right.
    /// </summary>
    /// <param name="distance">How far this camera should move from its current position.</param>
    public void Strafe(float distance)
    {
        Position += _rightVector * -distance;
    }
    
    
    public override void Update(float deltaTime)
    {
        if (NeedsModelMatrixUpdate)
        {
            var modelMatrix = Matrix4.CreateScale(Scale);
           
            // right   = glm::vec3(matrix[0][0], matrix[0][1], matrix[0][2]);
            modelMatrix.M11 = -_rightVector.X;
            modelMatrix.M12 = -_rightVector.Y;
            modelMatrix.M13 = -_rightVector.Z;

            // up      = glm::vec3(matrix[1][0], matrix[1][1], matrix[1][2]);
            modelMatrix.M21 = -_upVector.X;
            modelMatrix.M22 = -_upVector.Y;
            modelMatrix.M23 = -_upVector.Z;

            // forward = glm::vec3(matrix[2][0], matrix[2][1], matrix[2][2]);
            modelMatrix.M31 = -_frontVector.X;
            modelMatrix.M32 = -_frontVector.Y;
            modelMatrix.M33 = -_frontVector.Z;

            modelMatrix *= Matrix4.CreateTranslation(Position);
        
            ModelMatrix = modelMatrix;
        
            if (Parent != null)
            {
                ModelMatrix *= Parent.ModelMatrix;
            }

            NeedsModelMatrixUpdate = false;
        }

        foreach (var child in Children)
        {
            child.Update(deltaTime);
        }
    }
}
