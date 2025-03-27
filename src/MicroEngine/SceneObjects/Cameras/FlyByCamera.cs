/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.SceneObjects.Cameras;

using OpenTK.Mathematics;

public class FlyByCamera : SceneObjectBase, ICamera
{
    private readonly Matrix4 _modelMatrixBase;

    
    public Vector3 Direction => -Vector3.UnitZ;
    
    
    /// <summary>
    /// The field of view of the camera in radians.
    /// </summary>
    private float _fov = MathHelper.PiOver2;
    
    public float Fov
    {
        get => MathHelper.RadiansToDegrees(_fov);
        set => _fov = MathHelper.DegreesToRadians(MathHelper.Clamp(value, 1f, 90f));
    }

    
    /// <summary>
    /// Constructor.
    /// </summary>
    public FlyByCamera()
    {
        var modelMatrix = Matrix4.CreateScale(1.0f);
           
        // right   = glm::vec3(matrix[0][0], matrix[0][1], matrix[0][2]);
        var rightVector = -Vector3.UnitX;
        modelMatrix.M11 = rightVector.X;
        modelMatrix.M12 = rightVector.Y;
        modelMatrix.M13 = rightVector.Z;

        // up      = glm::vec3(matrix[1][0], matrix[1][1], matrix[1][2]);
        var upVector = Vector3.UnitY;
        modelMatrix.M21 = upVector.X;
        modelMatrix.M22 = upVector.Y;
        modelMatrix.M23 = upVector.Z;

        // forward = glm::vec3(matrix[2][0], matrix[2][1], matrix[2][2]);
        var frontVector = Vector3.UnitZ;
        modelMatrix.M31 = frontVector.X;
        modelMatrix.M32 = frontVector.Y;
        modelMatrix.M33 = frontVector.Z;

        modelMatrix *= Matrix4.CreateTranslation(Position);
        
        _modelMatrixBase = modelMatrix;
    }
    
    
    /// <summary>
    /// Get the view matrix using the LookAt function.
    /// </summary>
    /// <returns>The view matrix.</returns>
    public Matrix4 GetViewMatrix()
    {
        var position = ModelMatrix.ExtractTranslation();
        var front = Vector3.TransformVector(-Vector3.UnitZ, ModelMatrix);
        var up = Vector3.TransformVector(Vector3.UnitY, ModelMatrix);
        
        return Matrix4.LookAt(position, position + front, up);
    }
    
    /// <summary>
    /// Gets the projection matrix for perspective field of view.
    /// </summary>
    /// <returns>The projection matrix.</returns>
    public Matrix4 GetProjectionMatrix()
    {
        return Renderer.CreatePerspectiveProjectionMatrix(_fov);
    }
    
    
    public override void Update(float deltaTime)
    {
        if (NeedsModelMatrixUpdate)
        {
            ModelMatrix = _modelMatrixBase;
        
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
