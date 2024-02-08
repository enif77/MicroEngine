/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine;

using OpenTK.Mathematics;

/// <summary>
/// Generic interface for all cameras.
/// </summary>
public interface ICamera : ISceneObject
{
    // Get the view matrix using the amazing LookAt function described more in depth on the web tutorials
    Matrix4 GetViewMatrix();
    
    // Get the projection matrix using the same method we have used up until this point
    Matrix4 GetProjectionMatrix();
}