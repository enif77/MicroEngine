/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Physics;

using OpenTK.Mathematics;

/// <summary>
/// Calculates the inertia tensor for various shapes used in physics simulations.
/// </summary>
public static class InertiaTensorCalculator
{
    /// <summary>
    /// Calculates the inertia tensor for a sphere with uniformly distributed mass.
    /// </summary>
    /// <param name="mass">The mass of the sphere.</param>
    /// <param name="radius">The radius of the sphere.</param>
    /// <returns>Inertia tensor as a 3x3 matrix.</returns>
    public static Matrix3 CalculateSphereInertiaTensor(float mass, float radius)
    {
        // The moment of inertia of a sphere around any axis is (2/5) * m * r^2.
        var moment = (2.0f / 5.0f) * mass * radius * radius;

        // The inertia tensor is a diagonal matrix.
        return new Matrix3(
            moment, 0, 0,
            0, moment, 0,
            0, 0, moment
        );
    }
    
    /*
     
    Here is an example of calculating the inertia tensor for a cubic spaceship with uniformly distributed mass.
    This example assumes the cube has an edge length (a) and mass (m).
       
    Explanation:
   
    Moment of inertia of a cube: For a cube with uniformly distributed mass, the moment of inertia about each 
    axis is the same and is calculated as ((1/6) dot m dot a^2), where:
     
    (m) is the mass of the cube,
    (a) is the edge length of the cube.
     
    Diagonal matrix: The inertia tensor is a diagonal matrix because the cube has a symmetric shape.
      
    */
    
    /// <summary>
    /// Calculates the inertia tensor for a cube with uniformly distributed mass.
    /// </summary>
    /// <param name="mass">The mass of the cube.</param>
    /// <param name="sideLength">The length of the sides of the cube.</param>
    /// <returns>Inertia tensor as a 3x3 matrix.</returns>
    public static Matrix3 CalculateCubeInertiaTensor(float mass, float sideLength)
    {
        // The moment of inertia of a cube around an axis is (1/6) * m * a^2.
        var moment = (1.0f / 6.0f) * mass * sideLength * sideLength;

        // The inertia tensor is a diagonal matrix.
        return new Matrix3(
            moment, 0, 0,
            0, moment, 0,
            0, 0, moment
        );
    }
    
    
    /*
     
    For a general cuboid with dimensions (a), (b), (c) (edge lengths along axes (x), (y), (z)) and
    mass (m), the inertia tensor can be calculated using the following formulas:
         
         Moment of inertia about the (x) axis: (1/12) * m * (b^2 + c^2)
         Moment of inertia about the (y) axis: (1/12) * m * (a^2 + c^2)
         Moment of inertia about the (z) axis: (1/12) * m * (a^2 + b^2)
         
    The inertia tensor is a diagonal matrix because the cuboid has a symmetric shape.
        
    Explanation:
       
    Parameters:
         
         mass: Mass of the cuboid.
         width, height, depth: Dimensions of the cuboid along the (x), (y), (z) axes.
         
    Calculation:
         
         Each moment of inertia is calculated according to the formulas above.
         
    Output:
         
         The inertia tensor is represented as a diagonal matrix because the cuboid is symmetric. 
      
     */
    
    /// <summary>
    /// Calculates the inertia tensor for a cuboid with specified dimensions and mass.
    /// </summary>
    /// <param name="mass">The mass of the cuboid.</param>
    /// <param name="width">The width of the cuboid along the x-axis.</param>
    /// <param name="height">The height of the cuboid along the y-axis.</param>
    /// <param name="depth">The depth of the cuboid along the z-axis.</param>
    /// <returns>Inertia tensor as a 3x3 matrix.</returns>
    public static Matrix3 CalculateCuboidInertiaTensor(float mass, float width, float height, float depth)
    {
        // Calculation of moments of inertia
        var iX = (1.0f / 12.0f) * mass * (height * height + depth * depth);
        var iY = (1.0f / 12.0f) * mass * (width * width + depth * depth);
        var iZ = (1.0f / 12.0f) * mass * (width * width + height * height);

        // The inertia tensor is a diagonal matrix.
        return new Matrix3(
            iX, 0, 0,
            0, iY, 0,
            0, 0, iZ
        );
    }
}
