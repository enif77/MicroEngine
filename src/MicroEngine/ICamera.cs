/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine;

using OpenTK.Mathematics;

/// <summary>
/// Generic interface for all cameras.
/// </summary>
public interface ICamera : ISceneObject
{
    /// <summary>
    /// The field of view (FOV) is the vertical angle of the camera view in degrees.
    /// </summary>
    float Fov { get; set; }

    /// <summary>
    /// Aspect ratio of the camera.
    /// </summary>
    float AspectRatio { get; set; }
    
    /// <summary>
    /// Where the camera is looking at.
    /// </summary>
    Vector3 Direction { get; }
    
    // Get the view matrix using the amazing LookAt function described more in depth on the web tutorials
    Matrix4 GetViewMatrix();
    
    // Get the projection matrix using the same method we have used up until this point
    Matrix4 GetProjectionMatrix();
}