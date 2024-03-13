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
    protected Vector3 FrontVector = -Vector3.UnitZ;
    protected Vector3 UpVector = Vector3.UnitY;
    protected Vector3 RightVector = Vector3.UnitX;
    
    
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
        FrontVector = -Vector3.UnitZ;
        UpVector = Vector3.UnitY;
        RightVector = Vector3.UnitX;
        
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
        
        var m = Matrix4.CreateFromAxisAngle(UpVector, angle);
        
        RightVector = Vector3.TransformVector(RightVector, m);
        FrontVector = Vector3.TransformVector(FrontVector, m);
    }
    
    /// <summary>
    /// Turns this camera up or down.
    /// </summary>
    /// <param name="angle">The amount of degrees in radians this camera should turn left or right.</param>
    public void Pitch(float angle)
    {
        this.SetRotationX(Rotation.X + angle);
        
        var m = Matrix4.CreateFromAxisAngle(RightVector, angle);
        
        UpVector = Vector3.TransformVector(UpVector, m);
        FrontVector = Vector3.TransformVector(FrontVector, m);
    }

    /// <summary>
    /// Rolls this camera to the left or right.
    /// </summary>
    /// <param name="angle">The amount of degrees in radians this camera should roll left or right.</param>
    public void Roll(float angle)
    {
        this.SetRotationZ(Rotation.Z + angle);
        
        var m = Matrix4.CreateFromAxisAngle(FrontVector, angle); 
        
        RightVector = Vector3.TransformVector(RightVector, m);
        UpVector = Vector3.TransformVector(UpVector, m);
    }
    
    /// <summary>
    /// Moves the camera forward or backward.
    /// </summary>
    /// <param name="distance">How far this camera should move from its current position.</param>
    public void Advance(float distance)
    {
        Position += FrontVector * distance;
    }

    /// <summary>
    /// Moves the camera up or down.
    /// </summary>
    /// <param name="distance">How far this camera should move from its current position.</param>
    public void Ascend(float distance)
    {
        Position += UpVector * -distance;
    }

    /// <summary>
    /// Moves the camera left or right.
    /// </summary>
    /// <param name="distance">How far this camera should move from its current position.</param>
    public void Strafe(float distance)
    {
        Position += RightVector * -distance;
    }
    
    
    public override void Update(float deltaTime)
    {
        if (NeedsModelMatrixUpdate)
        {
            var modelMatrix = Matrix4.CreateScale(Scale);
           
            // right   = glm::vec3(matrix[0][0], matrix[0][1], matrix[0][2]);
            modelMatrix.M11 = -RightVector.X;
            modelMatrix.M12 = -RightVector.Y;
            modelMatrix.M13 = -RightVector.Z;

            // up      = glm::vec3(matrix[1][0], matrix[1][1], matrix[1][2]);
            modelMatrix.M21 = -UpVector.X;
            modelMatrix.M22 = -UpVector.Y;
            modelMatrix.M23 = -UpVector.Z;

            // forward = glm::vec3(matrix[2][0], matrix[2][1], matrix[2][2]);
            modelMatrix.M31 = -FrontVector.X;
            modelMatrix.M32 = -FrontVector.Y;
            modelMatrix.M33 = -FrontVector.Z;

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
