/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.SceneObjects;

using OpenTK.Mathematics;

using MicroEngine.Extensions;

public class Camera3 : SceneObjectBase, ICamera
{
    private Vector3 _viewDirection = -Vector3.UnitZ;
    private Vector3 _upVector = Vector3.UnitY;
    private Vector3 _rightVector = Vector3.UnitX;
    
    
    // This is simply the aspect ratio of the viewport, used for the projection matrix.
    public float AspectRatio { get; set; }
    
    
    public Vector3 Direction => _viewDirection;
    
    
    // The field of view of the camera (radians)
    private float _fov = MathHelper.PiOver2;
    
    // The field of view (FOV) is the vertical angle of the camera view.
    // This has been discussed more in depth in a previous tutorial,
    // but in this tutorial, you have also learned how we can use this to simulate a zoom feature.
    // We convert from degrees to radians as soon as the property is set to improve performance.
    public float Fov
    {
        get => MathHelper.RadiansToDegrees(_fov);
        set => _fov = MathHelper.DegreesToRadians(MathHelper.Clamp(value, 1f, 90f));
    }

    
    public void Yaw(float angle)
    {
        this.SetRotationY(Rotation.Y + MathHelper.DegreesToRadians(angle));
        
        var m = Matrix4.CreateFromAxisAngle(_upVector, MathHelper.DegreesToRadians(angle));  // up vector
        
        // _rightVector = Vector3.TransformVector(Vector3.UnitX, m);
        // _viewDirection = Vector3.TransformVector(-Vector3.UnitZ, m);
        
        _rightVector = Vector3.TransformVector(_rightVector, m);
        _viewDirection = Vector3.TransformVector(_viewDirection, m);
        
        NeedsModelMatrixUpdate = true;
    }
    
    
    public void Pitch(float angle)
    {
        this.SetRotationX(Rotation.X + MathHelper.DegreesToRadians(angle));
        
        //var m = Matrix4.CreateFromAxisAngle(Vector3.UnitX, Rotation.X);  // right vector
        var m = Matrix4.CreateFromAxisAngle(_rightVector, MathHelper.DegreesToRadians(angle));
        
        // _upVector = Vector3.TransformVector(Vector3.UnitY, m);
        // _viewDirection = Vector3.TransformVector(-Vector3.UnitZ, m);
        
        _upVector = Vector3.TransformVector(_upVector, m);
        _viewDirection = Vector3.TransformVector(_viewDirection, m);
        
        NeedsModelMatrixUpdate = true;
    }

    
    public void Roll(float angle)
    {
        this.SetRotationZ(MathHelper.DegreesToRadians(angle));
        
        //var m = Matrix4.CreateFromAxisAngle(-Vector3.UnitZ, Rotation.Z);  // front vector
        var m = Matrix4.CreateFromAxisAngle(_viewDirection, MathHelper.DegreesToRadians(angle)); 
        
        // _rightVector = Vector3.TransformVector(Vector3.UnitX, m);
        // _upVector = Vector3.TransformVector(Vector3.UnitY, m);
        
        _rightVector = Vector3.TransformVector(_rightVector, m);
        _upVector = Vector3.TransformVector(_upVector, m);
        
        NeedsModelMatrixUpdate = true;
    }

    
    // z movement
    public void Advance(float distance)
    {
        Position += _viewDirection * -distance;
    }

    // y movement
    public void Ascend(float distance)
    {
        Position += _upVector * distance;
    }

    // x movement
    public void Strafe(float distance)
    {
        Position += _rightVector * distance;
    }
    
    
    
    // Get the view matrix using the amazing LookAt function described more in depth on the web tutorials
    public Matrix4 GetViewMatrix()
    {
        return Matrix4.LookAt(Position, Position + _viewDirection, _upVector);
    }
    
    // Get the projection matrix using the same method we have used up until this point
    public Matrix4 GetProjectionMatrix()
    {
        return Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, 0.01f, 100f);
    }
    

    public Camera3(Vector3 position, float aspectRatio)
    {
        Position = position;
        Rotation = new Vector3(0f, -MathHelper.PiOver2, 0f);
        AspectRatio = aspectRatio;
    }
}

/*
 
https://cboard.cprogramming.com/game-programming/135390-how-properly-move-strafe-yaw-pitch-camera-opengl-glut-using-glulookat.html
 
https://tuttlem.github.io/2013/12/30/a-camera-implementation-in-c.html 
https://gamedev.stackexchange.com/questions/136174/im-rotating-an-object-on-two-axes-so-why-does-it-keep-twisting-around-the-thir
https://gamedev.stackexchange.com/questions/183748/3d-camera-rotation-unwanted-roll-space-flight-cam  
https://swiftgl.github.io/learn/01-camera.html
  
 */