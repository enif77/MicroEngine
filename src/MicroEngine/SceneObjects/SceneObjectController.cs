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
    /// Turns this camera to the left or right.
    /// </summary>
    /// <param name="angle">The amount of degrees this camera should turn left or right.</param>
    public void Yaw(float angle)
    {
        this.SetRotationY(Rotation.Y + MathHelper.DegreesToRadians(angle));
        
        var m = Matrix4.CreateFromAxisAngle(_upVector, MathHelper.DegreesToRadians(angle));
        
        //_upVector = Vector3.TransformVector(_upVector, m);
        _rightVector = Vector3.TransformVector(_rightVector, m);
        _frontVector = Vector3.TransformVector(_frontVector, m);
        
        NeedsModelMatrixUpdate = true;
    }
    
    
    public void YawAbs(float angle)
    {
        this.SetRotationY(MathHelper.DegreesToRadians(angle));
        
        var m = Matrix4.CreateFromAxisAngle(Vector3.UnitY, MathHelper.DegreesToRadians(angle));
        
        _rightVector = Vector3.TransformVector(Vector3.UnitX, m);
        _frontVector = Vector3.TransformVector(-Vector3.UnitZ, m);
        
        NeedsModelMatrixUpdate = true;
    }
    
    /// <summary>
    /// Turns this camera up or down.
    /// </summary>
    /// <param name="angle">The amount of degrees this camera should turn left or right.</param>
    public void Pitch(float angle)
    {
        this.SetRotationX(Rotation.X + MathHelper.DegreesToRadians(angle));
        
        var m = Matrix4.CreateFromAxisAngle(_rightVector, MathHelper.DegreesToRadians(angle));
        
        //_rightVector = Vector3.TransformVector(_rightVector, m);
        _upVector = Vector3.TransformVector(_upVector, m);
        _frontVector = Vector3.TransformVector(_frontVector, m);
        
        NeedsModelMatrixUpdate = true;
    }
    
    
    public void PitchAbs(float angle)
    {
        this.SetRotationX(MathHelper.DegreesToRadians(angle));
        
        var m = Matrix4.CreateFromAxisAngle(Vector3.UnitX, MathHelper.DegreesToRadians(angle));
        
        _upVector = Vector3.TransformVector(Vector3.UnitY, m);
        _frontVector = Vector3.TransformVector(-Vector3.UnitZ, m);
        
        NeedsModelMatrixUpdate = true;
    }
    

    /// <summary>
    /// Rolls this camera to the left or right.
    /// </summary>
    /// <param name="angle">The amount of degrees this camera should roll left or right.</param>
    public void Roll(float angle)
    {
        this.SetRotationZ(Rotation.Z + MathHelper.DegreesToRadians(angle));
        
        var m = Matrix4.CreateFromAxisAngle(_frontVector, MathHelper.DegreesToRadians(angle)); 
        
        //_frontVector = Vector3.TransformVector(_frontVector, m);
        _rightVector = Vector3.TransformVector(_rightVector, m);
        _upVector = Vector3.TransformVector(_upVector, m);
        
        NeedsModelMatrixUpdate = true;
    }
    
    
    public void RollAbs(float angle)
    {
        this.SetRotationZ(MathHelper.DegreesToRadians(angle));
        
        var m = Matrix4.CreateFromAxisAngle(-Vector3.UnitZ, MathHelper.DegreesToRadians(angle)); 
        
        _rightVector = Vector3.TransformVector(Vector3.UnitX, m);
        _upVector = Vector3.TransformVector(Vector3.UnitY, m);
        
        NeedsModelMatrixUpdate = true;
    }
    
    /// <summary>
    /// Moves the camera forward or backward.
    /// </summary>
    /// <param name="distance">How far this camera should move from its current position.</param>
    public void Advance(float distance)
    {
        Position += _frontVector * -distance;
        NeedsModelMatrixUpdate = true;
    }

    /// <summary>
    /// Moves the camera up or down.
    /// </summary>
    /// <param name="distance">How far this camera should move from its current position.</param>
    public void Ascend(float distance)
    {
        Position += _upVector * distance;
        NeedsModelMatrixUpdate = true;
    }

    /// <summary>
    /// Moves the camera left or right.
    /// </summary>
    /// <param name="distance">How far this camera should move from its current position.</param>
    public void Strafe(float distance)
    {
        Position += _rightVector * distance;
        NeedsModelMatrixUpdate = true;
    }
}
