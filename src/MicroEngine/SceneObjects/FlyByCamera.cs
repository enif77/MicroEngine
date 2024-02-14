/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.SceneObjects;

using OpenTK.Mathematics;

using MicroEngine.Extensions;

public class FlyByCamera : SceneObjectBase, ICamera
{
    private Vector3 _frontVector = -Vector3.UnitZ;
    private Vector3 _upVector = Vector3.UnitY;
    private Vector3 _rightVector = Vector3.UnitX;
    
    
    public float AspectRatio { get; set; }
    public Vector3 Direction => _frontVector;
    
    private float _fov = MathHelper.PiOver2;
    
    /// <summary>
    /// The field of view (FOV) is the vertical angle of the camera view.
    /// </summary>
    public float Fov
    {
        get => MathHelper.RadiansToDegrees(_fov);
        set => _fov = MathHelper.DegreesToRadians(MathHelper.Clamp(value, 1f, 90f));
    }

    
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="position">The initial position of this camera.</param>
    /// <param name="aspectRatio">The initial aspect ratio of this camera.</param>
    public FlyByCamera(Vector3 position, float aspectRatio)
    {
        Position = position;
        Rotation = new Vector3(0f, -MathHelper.PiOver2, 0f);
        AspectRatio = aspectRatio;
    }
    
    
    /// <summary>
    /// Turns this camera to the left or right.
    /// </summary>
    /// <param name="angle">The amount of degrees this camera should turn left or right.</param>
    public void Yaw(float angle)
    {
        this.SetRotationY(Rotation.Y + MathHelper.DegreesToRadians(angle));
        
        var m = Matrix4.CreateFromAxisAngle(_upVector, MathHelper.DegreesToRadians(angle));
        
        _rightVector = Vector3.TransformVector(_rightVector, m);
        _frontVector = Vector3.TransformVector(_frontVector, m);
        
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
        
        _upVector = Vector3.TransformVector(_upVector, m);
        _frontVector = Vector3.TransformVector(_frontVector, m);
        
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
        
        _rightVector = Vector3.TransformVector(_rightVector, m);
        _upVector = Vector3.TransformVector(_upVector, m);
        
        NeedsModelMatrixUpdate = true;
    }
    
    /// <summary>
    /// Moves the camera forward or backward.
    /// </summary>
    /// <param name="distance">How far this camera should move from its current position.</param>
    public void Advance(float distance)
    {
        Position += _frontVector * -distance;
    }

    /// <summary>
    /// Moves the camera up or down.
    /// </summary>
    /// <param name="distance">How far this camera should move from its current position.</param>
    public void Ascend(float distance)
    {
        Position += _upVector * distance;
    }

    /// <summary>
    /// Moves the camera left or right.
    /// </summary>
    /// <param name="distance">How far this camera should move from its current position.</param>
    public void Strafe(float distance)
    {
        Position += _rightVector * distance;
    }
    
    /// <summary>
    /// Get the view matrix using the LookAt function.
    /// </summary>
    /// <returns>The view matrix.</returns>
    public Matrix4 GetViewMatrix()
    {
        // TODO: Get position and rotation from the parent.

        // var position = Vector3.TransformPosition(Position, ModelMatrix);
        var position = new Vector3(ModelMatrix.M41, ModelMatrix.M42, ModelMatrix.M43);
        var front = Vector3.TransformVector(_frontVector, ModelMatrix);
        var up = Vector3.TransformVector(_upVector, ModelMatrix);
        
        return Matrix4.LookAt(position, position + front, up);
        
        //return Matrix4.LookAt(Position, Position + _frontVector, _upVector);
    }
    
    /// <summary>
    /// Gets the projection matrix for perspective field of view.
    /// </summary>
    /// <returns>The projection matrix.</returns>
    public Matrix4 GetProjectionMatrix()
    {
        return Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, 0.01f, 100f);
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

/*
 
https://cboard.cprogramming.com/game-programming/135390-how-properly-move-strafe-yaw-pitch-camera-opengl-glut-using-glulookat.html
 
https://tuttlem.github.io/2013/12/30/a-camera-implementation-in-c.html 
https://gamedev.stackexchange.com/questions/136174/im-rotating-an-object-on-two-axes-so-why-does-it-keep-twisting-around-the-thir
https://gamedev.stackexchange.com/questions/183748/3d-camera-rotation-unwanted-roll-space-flight-cam  
https://swiftgl.github.io/learn/01-camera.html
  
 */