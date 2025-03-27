/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.SceneObjects.Cameras;

using OpenTK.Mathematics;

// This is the camera class as it could be set up after the tutorials on the website.
// It is important to note there are a few ways you could have set up this camera.
// For example, you could have also managed the player input inside the camera class,
// and a lot of the properties could have been made into functions.

// TL;DR: This is just one of many ways in which we could have set up the camera.
// Check out the web version if you don't know why we are doing a specific thing or want to know more about the code.
public class FpsCamera : SceneObjectBase, ICamera
{
    // Those vectors are directions pointing outwards from the camera to define how it rotated.
    private Vector3 _frontVector = -Vector3.UnitZ;
    private Vector3 _upVector = -Vector3.UnitY;
    private Vector3 _rightVector = Vector3.UnitX;
    
    
    /// <summary>
    /// The field of view of the camera in radians.
    /// </summary>
    private float _fov = MathHelper.PiOver2;
    
    /// <summary>
    /// Constructor.
    /// </summary>
    public FpsCamera()
    {
        Position = Vector3.Zero;
        Rotation = Vector3.Zero;
    }
    
    
    public Vector3 FrontVector => _frontVector;

    public Vector3 UpVector => _upVector;

    public Vector3 RightVector => _rightVector;

    
    public Vector3 Direction => _frontVector;
    
    
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
            SetRotationX(MathHelper.DegreesToRadians(angle));
            UpdateVectors();
        }
    }

    // We convert from degrees to radians as soon as the property is set to improve performance.
    public float Yaw
    {
        get => MathHelper.RadiansToDegrees(Rotation.Y);
        set
        {
            SetRotationY(MathHelper.DegreesToRadians(value));
            UpdateVectors();
        }
    }
    
    // We convert from degrees to radians as soon as the property is set to improve performance.
    public float Roll
    {
        get => MathHelper.RadiansToDegrees(Rotation.Z);
        set
        {
            SetRotationZ(MathHelper.DegreesToRadians(value));
            UpdateVectors();
        }
    }
    
    
    public float Fov
    {
        get => MathHelper.RadiansToDegrees(_fov);
        set => _fov = MathHelper.DegreesToRadians(MathHelper.Clamp(value, 1f, 90f));
    }

    // Get the view matrix using the amazing LookAt function described more in depth on the web tutorials
    public Matrix4 GetViewMatrix()
    {
        var position = ModelMatrix.ExtractTranslation();
        var front = Vector3.TransformVector(-Vector3.UnitZ, ModelMatrix);
        var up = Vector3.TransformVector(-Vector3.UnitY, ModelMatrix);
        
        return Matrix4.LookAt(position, position + front, up);
    }
    

    public Matrix4 GetProjectionMatrix()
    {
        return Renderer.CreatePerspectiveProjectionMatrix(_fov);
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
    
    
    // This function is going to update the direction vertices using some of the math learned in the web tutorials.
    private void UpdateVectors()
    {
        var pitch = Rotation.X;
        var yaw = Rotation.Y;
        
        // First, the front matrix is calculated using some basic trigonometry.
        _frontVector.X = MathF.Cos(pitch) * MathF.Cos(yaw);
        _frontVector.Y = MathF.Sin(pitch);
        _frontVector.Z = MathF.Cos(pitch) * MathF.Sin(yaw);

        // We need to make sure the vectors are all normalized, as otherwise we would get some funky results.
        _frontVector = Vector3.Normalize(_frontVector);

        // Calculate both the right and the up vector using cross product.
        // Note that we are calculating the right from the global up; this behaviour might
        // not be what you need for all cameras so keep this in mind if you do not want an FPS camera.
        _rightVector = Vector3.Normalize(Vector3.Cross(_frontVector, Vector3.UnitY));
        _upVector = Vector3.Normalize(Vector3.Cross(_rightVector, _frontVector));
        
        NeedsModelMatrixUpdate = true;
    }
}
