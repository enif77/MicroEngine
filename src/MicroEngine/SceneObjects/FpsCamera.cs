/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.SceneObjects;

using OpenTK.Mathematics;

using MicroEngine.Extensions;

// This is the camera class as it could be set up after the tutorials on the website.
// It is important to note there are a few ways you could have set up this camera.
// For example, you could have also managed the player input inside the camera class,
// and a lot of the properties could have been made into functions.

// TL;DR: This is just one of many ways in which we could have set up the camera.
// Check out the web version if you don't know why we are doing a specific thing or want to know more about the code.
public class FpsCamera : SceneObjectBase, ICamera
{
    // Those vectors are directions pointing outwards from the camera to define how it rotated.
    private Vector3 _front = -Vector3.UnitZ;
    private Vector3 _up = Vector3.UnitY;
    private Vector3 _right = Vector3.UnitX;
    
    // The field of view of the camera (radians)
    private float _fov = MathHelper.PiOver2;

    
    public FpsCamera(Vector3 position, float aspectRatio)
    {
        Position = position;
        Rotation = new Vector3(0f, -MathHelper.PiOver2, 0f);
        AspectRatio = aspectRatio;
    }
    
    
    // This is simply the aspect ratio of the viewport, used for the projection matrix.
    public float AspectRatio { get; set; }

    public Vector3 Front => _front;

    public Vector3 Up => _up;

    public Vector3 Right => _right;

    
    public Vector3 Direction => _front;
    
    
    // We convert from degrees to radians as soon as the property is set to improve performance.
    public float Pitch
    {
        get => MathHelper.RadiansToDegrees(Rotation.X);
        set
        {
            // We clamp the pitch value between -89 and 89 to prevent the camera from going upside down, and a bunch
            // of weird "bugs" when you are using euler angles for rotation.
            // If you want to read more about this you can try researching a topic called gimbal lock
            var angle = MathHelper.Clamp(value, -89f, 89f);
            this.SetRotationX(MathHelper.DegreesToRadians(angle));
            UpdateVectors();
        }
    }

    // We convert from degrees to radians as soon as the property is set to improve performance.
    public float Yaw
    {
        get => MathHelper.RadiansToDegrees(Rotation.Y);
        set
        {
            this.SetRotationY(MathHelper.DegreesToRadians(value));
            UpdateVectors();
        }
    }
    
    // We convert from degrees to radians as soon as the property is set to improve performance.
    public float Roll
    {
        get => MathHelper.RadiansToDegrees(Rotation.Z);
        set
        {
            this.SetRotationZ(MathHelper.DegreesToRadians(value));
            UpdateVectors();
        }
    }

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

    // Get the view matrix using the amazing LookAt function described more in depth on the web tutorials
    public Matrix4 GetViewMatrix()
    {
        return Matrix4.LookAt(Position, Position + _front, _up);
    }
    
    // Get the projection matrix using the same method we have used up until this point
    public Matrix4 GetProjectionMatrix()
    {
        return Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, 0.01f, 100f);
    }

    
    // public override void Update(float deltaTime)
    // {
    //     if (NeedsModelMatrixUpdate)
    //     {
    //         // The view matrix gives us the rotation of the camera.
    //         ModelMatrix = GetViewMatrix();
    //         
    //         // ModelMatrix = Matrix4.CreateScale(Scale);
    //         //
    //         // // We do not need to rotate the camera again - the view matrix is enough.
    //         // ModelMatrix *= Matrix4.CreateRotationZ(Rotation.Z);
    //         // ModelMatrix *= Matrix4.CreateRotationX(Rotation.X);
    //         // ModelMatrix *= Matrix4.CreateRotationY(Rotation.Y);
    //         
    //         ModelMatrix *= Matrix4.CreateTranslation(Position);
    //         
    //         if (Parent != null)
    //         {
    //             ModelMatrix *= Parent.ModelMatrix;
    //         }
    //
    //         NeedsModelMatrixUpdate = false;
    //     }
    //
    //     foreach (var child in Children)
    //     {
    //         child.Update(deltaTime);
    //     }
    // }
    
    
    // This function is going to update the direction vertices using some of the math learned in the web tutorials.
    private void UpdateVectors()
    {
        var pitch = Rotation.X;
        var yaw = Rotation.Y;
        
        // First, the front matrix is calculated using some basic trigonometry.
        _front.X = MathF.Cos(pitch) * MathF.Cos(yaw);
        _front.Y = MathF.Sin(pitch);
        _front.Z = MathF.Cos(pitch) * MathF.Sin(yaw);

        // We need to make sure the vectors are all normalized, as otherwise we would get some funky results.
        _front = Vector3.Normalize(_front);

        // Calculate both the right and the up vector using cross product.
        // Note that we are calculating the right from the global up; this behaviour might
        // not be what you need for all cameras so keep this in mind if you do not want a FPS camera.
        _right = Vector3.Normalize(Vector3.Cross(_front, Vector3.UnitY));
        _up = Vector3.Normalize(Vector3.Cross(_right, _front));
        
        NeedsModelMatrixUpdate = true;
    }
}
