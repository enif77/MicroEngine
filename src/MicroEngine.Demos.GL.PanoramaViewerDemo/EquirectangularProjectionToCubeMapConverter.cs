/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Demos.GL.PanoramaViewerDemo;

using MicroEngine.Graphics;

/// <summary>
/// Defines cube map face orientations.
/// </summary>
public enum CubeMapFaceOrientation
{
    PositiveZ,
    NegativeZ,
    PositiveX,
    NegativeX,
    PositiveY,
    NegativeY
}


/// <summary>
/// Represents a 3D vector.
/// </summary>
internal class Vector3d
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
    /// Creates a new instance of the <see cref="Vector3d"/> class.
    /// </summary>
    /// <param name="x">A X component.</param>
    /// <param name="y">A Y component.</param>
    /// <param name="z">A Z component.</param>
    public Vector3d(double x, double y, double z)
    {
        X = x;
        Y = y;
        Z = z;
    }
}


/// <summary>
/// Converts a panorama 2:1 image (equirectangular projection) to a 6 cube map images.
/// </summary>
public class EquirectangularProjectionToCubeMapConverter
{
    /// <summary>
    /// Renders a cube face from a 2:1 panorama image.
    /// </summary>
    /// <param name="sourceImage">Source image data. The image must have a 2:1 aspect ratio, width and height must be divisible by 4, and width must be >= 16.</param>
    /// <param name="cubeMapFaceOrientation">A cube map face orientation to be rendered.</param>
    /// <param name="rotation">A cube rotation in degrees.</param>
    /// <param name="maxWidth">The maximum width of the generated image. Should be >= 2 and divisible by 2.</param>
    /// <returns>An image representing a cube face.</returns>
    public Image RenderFace(Image sourceImage, CubeMapFaceOrientation cubeMapFaceOrientation, double rotation, int maxWidth = int.MaxValue)
    {
        ArgumentNullException.ThrowIfNull(sourceImage);

        if (sourceImage.Width != sourceImage.Height * 2) throw new ArgumentException("The source image aspect ratio must be 2:1.", nameof(sourceImage));
        if (sourceImage.Width < 16) throw new ArgumentOutOfRangeException(nameof(sourceImage), "The source image width can not be < 16.");
        if (sourceImage.Width % 4 != 0) throw new ArgumentException("The source image width is not divisible by 4.", nameof(sourceImage));
        if (sourceImage.Height % 4 != 0) throw new ArgumentException("The source image height is not divisible by 4.", nameof(sourceImage));

        if (double.IsNaN(rotation) || double.IsInfinity(rotation))
        {
            throw new ArgumentException("Rotation must be a finite number.", nameof(rotation));
        }
        
        if (maxWidth < 2)
        {
            throw new ArgumentOutOfRangeException(nameof(maxWidth), "maxWidth must be >= 2.");
        }
        
        if ((maxWidth & 1) != 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxWidth), "maxWidth must be divisible by 2.");
        }
        
        var faceSize = Math.Min(maxWidth, sourceImage.Width / 2);
        if (faceSize < 2) throw new ArgumentOutOfRangeException(nameof(sourceImage), "Face size can not be < 2.");
        if (faceSize % 2 != 0) throw new ArgumentException("Face size is not divisible by 2.", nameof(sourceImage));

        var pixelInterpolator = new PixelInterpolator();
        var destinationImage = new Image(faceSize, faceSize);
        
        const double twoPi = 2.0 * Math.PI;
        const double cubeSize = 2.0;
        const double cubeSize2 = cubeSize / 2.0;
        
        var cubeOrientation = GetCubeOrientation(cubeMapFaceOrientation, GetCubeOrientationVector(cubeMapFaceOrientation));
        var rotationRad = Rad(rotation);
        
        var right = destinationImage.Width;
        var bottom = destinationImage.Height;
        
        var srcW = sourceImage.Width;
        var srcH = sourceImage.Height;

        var invFaceSize = 1.0 / faceSize;
        var cubeStep = cubeSize * invFaceSize;                 // 2 / faceSize
        var srcXScale = srcW / twoPi;                          // width / (2π)
        var srcYScale = srcH / Math.PI;                        // height / π

        var to = 0;
        for (var y = 0; y < bottom; y++)
        {
            var cubeY = (cubeStep * (y + 0.5)) - cubeSize2;
            
            for (var x = 0; x < right; x++)
            {
                var cubeX = (cubeStep * (x + 0.5)) - cubeSize2;
                
                // Get the position on cube a face.
                // Cube is centered at the origin with a side length of cubeSize. The "normal" cube size is 2.0.
                var cube = cubeOrientation(cubeX, cubeY);

                // Project cube face onto a unit sphere by converting cartesian to spherical coordinates.
                var r = Math.Sqrt((cube.X * cube.X) + (cube.Y * cube.Y) + (cube.Z * cube.Z));
                
                // lon/lat -> source pixel coordinates.
                var lon = Mod(Math.Atan2(cube.Y, cube.X) + rotationRad, twoPi);
                var lat = Math.Acos(Math.Clamp(cube.Z / r, -1.0, 1.0));
                
                // X should wrap (panorama is periodic in longitude), Y should clamp (poles).
                var xFrom = (srcXScale * lon) - 0.5;
                xFrom = Mod(xFrom, srcW);
                
                var yFrom = (srcYScale * lat) - 0.5;
                
                // Copy pixel from source image to destination image with interpolation.
                pixelInterpolator.CopyPixel(
                    sourceImage,
                    destinationImage,
                    xFrom,
                    yFrom,
                    to);

                // Fill alpha channel.
                destinationImage.Pixels[to + 3] = 255;
                
                // Move to the next pixel.
                to += 4;
            }
        }
        
        return destinationImage;
    }

    
    private static double Rad(double angle)
    {
        return angle * (Math.PI / 180.0);
    }


    private static double Mod(double x, double n)
    {
        return ((x % n) + n) % n;
    }


    private static Vector3d GetCubeOrientationVector(CubeMapFaceOrientation cubeMapFaceOrientation)
    {
        return cubeMapFaceOrientation switch
        {
            CubeMapFaceOrientation.PositiveZ => new Vector3d(-1, 0, 0),
            CubeMapFaceOrientation.NegativeZ => new Vector3d(1, 0, 0),
            CubeMapFaceOrientation.PositiveX => new Vector3d(0, -1, 0),
            CubeMapFaceOrientation.NegativeX => new Vector3d(0, 1, 0),
            CubeMapFaceOrientation.PositiveY => new Vector3d(0, 0, 1),
            CubeMapFaceOrientation.NegativeY => new Vector3d(0, 0, -1),
            
            _ => throw new ArgumentException("Unknown face orientation: " + cubeMapFaceOrientation)
        };
    }


    private static Func<double, double, Vector3d> GetCubeOrientation(CubeMapFaceOrientation cubeMapFaceOrientation, Vector3d v)
    {
        return cubeMapFaceOrientation switch
        {
            CubeMapFaceOrientation.PositiveZ => (x, y) =>
            {
                v.Y = -x;
                v.Z = -y;
                
                return v;
            },
            CubeMapFaceOrientation.NegativeZ => (x, y) =>
            {
                v.Y = x;
                v.Z = -y;
                
                return v;
            },
            CubeMapFaceOrientation.PositiveX => (x, y) =>
            {
                v.X = x;
                v.Z = -y;
                
                return v;
            },
            CubeMapFaceOrientation.NegativeX => (x, y) =>
            {
                v.X = -x;
                v.Z = -y;
                
                return v;
            },
            CubeMapFaceOrientation.PositiveY => (x, y) =>
            {
                v.X = -y;
                v.Y = -x;
                
                return v;
            },
            CubeMapFaceOrientation.NegativeY => (x, y) =>
            {
                v.X = y;
                v.Y = -x;
                
                return v;
            },
            
            _ => throw new ArgumentException("Unknown face orientation: " + cubeMapFaceOrientation)
        };
    }
}
