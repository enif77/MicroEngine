/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.SceneObjects;

using OpenTK.Mathematics;

using MicroEngine.Extensions;

public class Camera2 : SceneObjectBase, ICamera
{
    private Vector3 _viewDirection = -Vector3.UnitZ;
    private Vector3 _upVector = Vector3.UnitY;
    private Vector3 _rightVector = Vector3.UnitX;
    
    
    // This is simply the aspect ratio of the viewport, used for the projection matrix.
    public float AspectRatio { private get; set; }
    
    
    // The field of view of the camera (radians)
    private float _fov = MathHelper.PiOver2;
    
    // The field of view (FOV) is the vertical angle of the camera view.
    // This has been discussed more in depth in a previous tutorial,
    // but in this tutorial, you have also learned how we can use this to simulate a zoom feature.
    // We convert from degrees to radians as soon as the property is set to improve performance.
    public float Fov
    {
        get => MathHelper.RadiansToDegrees(_fov);
        set
        {
            var angle = MathHelper.Clamp(value, 1f, 90f);
            _fov = MathHelper.DegreesToRadians(angle);
        }
    }

    
    public void Pitch(float angle)
    {
        // forwardVector = (forwardVector * cos(pitch)) + (upVector * sin(pitch))
        // upVector      = forwardVector x rightVector
        
        // keep track of how far we've gone around the axis
        this.SetRotationX(Rotation.X + MathHelper.DegreesToRadians(angle));
        //this.SetRotationX(MathHelper.DegreesToRadians(angle));
        
        // calculate the new forward vector
        _viewDirection = Vector3.Normalize(
            _viewDirection * (float)Math.Cos(MathHelper.DegreesToRadians(angle)) +
            _upVector * (float)Math.Sin(MathHelper.DegreesToRadians(angle))
        );

        // calculate the new up vector
        _upVector  = Vector3.Cross(_viewDirection, _rightVector);
        
        // invert so that positive goes down
        _upVector *= -1;    
    }

    
    public void Yaw(float angle)
    {
        // forwardVector = (forwardVector * cos(yaw)) - (rightVector * sin(yaw))
        // rightVector   = forwardVector x upVector
        
        // keep track of how far we've gone around this axis
        this.SetRotationY(Rotation.Y + MathHelper.DegreesToRadians(angle));
        //this.SetRotationY(MathHelper.DegreesToRadians(angle));

        // re-calculate the new forward vector
        _viewDirection = Vector3.Normalize(
            _viewDirection * (float)Math.Cos(MathHelper.DegreesToRadians(angle)) -
            _rightVector * (float)Math.Sin(MathHelper.DegreesToRadians(angle))
        );

        // re-calculate the new right vector
        _rightVector = Vector3.Cross(_viewDirection, _upVector);
    }
    
    
    public void Roll(float angle)
    {
        // rightVector = (rightVector * cos(roll)) + (upVector * sin(roll))
        // upVector    = forwardVector x rightVector
        
        // keep track of how far we've gone around this axis
        this.SetRotationZ(Rotation.Z + MathHelper.DegreesToRadians(angle));
        //this.SetRotationZ(MathHelper.DegreesToRadians(angle));

        // re-calculate the forward vector
        _rightVector = Vector3.Normalize(
            _rightVector * (float)Math.Cos(MathHelper.DegreesToRadians(angle)) +
            _upVector * (float)Math.Sin(MathHelper.DegreesToRadians(angle))
        );

        // re-calculate the up vector
        _upVector  = Vector3.Cross(_viewDirection, _rightVector);
        
        // invert the up vector so positive points down
        _upVector *= -1;
    }

    
    // z movement
    public void Advance(float distance)
    {
        Position += (_viewDirection * -distance);
    }

    // y movement
    public void Ascend(float distance)
    {
        Position += (_upVector * distance);
    }

    // x movement
    public void Strafe(float distance)
    {
        Position += (_rightVector * distance);
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
    

    public Camera2(Vector3 position, float aspectRatio)
    {
        Position = position;
        Rotation = new Vector3(0f, -MathHelper.PiOver2, 0f);
        AspectRatio = aspectRatio;
    }
}

/*
 
https://tuttlem.github.io/2013/12/30/a-camera-implementation-in-c.html 
https://gamedev.stackexchange.com/questions/136174/im-rotating-an-object-on-two-axes-so-why-does-it-keep-twisting-around-the-thir
https://gamedev.stackexchange.com/questions/183748/3d-camera-rotation-unwanted-roll-space-flight-cam  
https://swiftgl.github.io/learn/01-camera.html
  
 */