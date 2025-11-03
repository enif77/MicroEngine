/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Tools.EquirectangularProjectionToCubeMapConverter;

/// <summary>
/// Represents a 3D vector.
/// </summary>
public class Vector3
{
    /// <summary>
    /// The X component of the 3D vector.
    /// </summary>
    public double X { get; set; }
    
    /// <summary>
    /// The Y component of the 3D vector.
    /// </summary>
    public double Y { get; set; }
    
    /// <summary>
    /// The Z component of the 3D vector.
    /// </summary>
    public double Z { get; set; }
    
    /// <summary>
    /// Creates a new instance of the <see cref="Vector3"/> class.
    /// </summary>
    /// <param name="x">A X component.</param>
    /// <param name="y">A Y component.</param>
    /// <param name="z">A Z component.</param>
    public Vector3(double x, double y, double z)
    {
        X = x;
        Y = y;
        Z = z;
    }
}
