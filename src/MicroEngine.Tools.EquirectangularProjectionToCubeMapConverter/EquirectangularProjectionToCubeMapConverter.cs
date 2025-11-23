/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Tools.EquirectangularProjectionToCubeMapConverter;

using MicroEngine.Tools.EquirectangularProjectionToCubeMapConverter.PixelInterpolators;

/// <summary>
/// Defines image interpolation strategies.
/// </summary>
public enum ImageInterpolationStrategy
{
    /// <summary>
    /// Bilinear interpolation.
    /// </summary>
    Bilinear,
    
    /// <summary>
    /// Nearest neighbor interpolation.
    /// </summary>
    NearestNeighbor,
    
    /// <summary>
    /// Bicubic interpolation.
    /// </summary>
    Bicubic,
    
    /// <summary>
    /// Lanczos interpolation.
    /// </summary>
    Lanczos
}

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


internal class RenderState
{
    public int Left { get; set; }
    public int Top { get; set; }
    public Image Image { get; set; } = new Image(1, 1);
}


/// <summary>
/// Converts a panorama 2:1 image (equirectangular projection) to a 6 cube map images.
/// </summary>
public class EquirectangularProjectionToCubeMapConverter
{
    private const string PositiveZOrientationFaceName = "pz";
    private const string NegativeZOrientationFaceName = "nz";
    private const string PositiveXOrientationFaceName = "px";
    private const string NegativeXOrientationFaceName = "nx";
    private const string PositiveYOrientationFaceName = "py";
    private const string NegativeYOrientationFaceName = "ny";

    /// <summary>
    /// Renders a cube face from a 2:1 panorama image.
    /// </summary>
    /// <param name="sourceImage">Source image data. The image must have 2:1 aspect ratio, width and height divisible and width >= 16.</param>
    /// <param name="cubeMapFaceOrientation">A cube map face orientation to be rendered.</param>
    /// <param name="rotation">A cube rotation. From 0 to 359.</param>
    /// <param name="interpolationStrategy">Which image interpolation should be used.</param>
    /// <param name="maxWidth">The maximum width of the generated image.</param>
    /// <param name="maxThreads">The maximum number of threads to use for rendering. Default is 1.</param>
    /// <returns>An image representing a cube face.</returns>
    public Image RenderFace(Image sourceImage, CubeMapFaceOrientation cubeMapFaceOrientation, double rotation, ImageInterpolationStrategy interpolationStrategy, int maxWidth = int.MaxValue, int maxThreads = 1)
    {
        if (sourceImage.Width / sourceImage.Height != 2) throw new Exception("The source image aspect ratio must be 2:1.");
        if (sourceImage.Width < 16) throw new Exception("The source image width can not be < 16.");
        if (sourceImage.Width % 4 != 0) throw new Exception("The source image width is not divisible by 4.");
        if (sourceImage.Height % 4 != 0) throw new Exception("The source image height is not divisible by 4.");

        var imageInterpolator = GetPixelInterpolator(interpolationStrategy);
        
        var faceSize = Math.Min(maxWidth, sourceImage.Width / 4);

        if (faceSize < 4) throw new Exception("Face size can not be < 4.");
        if (faceSize % 4 != 0) throw new Exception("Face size is not divisible by 4.");

        var subFaceSize = faceSize / 4;

        var destinationImage = new Image(faceSize, faceSize, 4);

        if (maxThreads < 1) maxThreads = 1;
        else if (maxThreads > 16) maxThreads = 16;

        var f = new Forker(maxThreads);

        f.ItemComplete += ItemComplete;

        var subFacesList = new List<RenderState>();

        for (var top = 0; top < faceSize; top += subFaceSize)
        {
            for (var left = 0; left < faceSize; left += subFaceSize)
            {
                var subFaceState = new RenderState()
                {
                    Left = left,
                    Top = top,
                    Image = new Image(subFaceSize, subFaceSize, 4)
                };

                subFacesList.Add(subFaceState);

                f.Fork(() =>
                {
                    RenderSubFace(sourceImage, cubeMapFaceOrientation, rotation, imageInterpolator, faceSize, subFaceState);
                });
            }
        }

        f.Join();

        foreach (var subFace in subFacesList)
        {
            BlitImage(subFace, destinationImage);
        }

        return destinationImage;
    }
    
    
    private void ItemComplete(object? sender, ParallelEventArgs eventArgs)
    {
        // No error to log?
        if (eventArgs.Exception == null)
        {
            return;
        }

        // TODO: Propagate exceptions up.

        Console.Error.WriteLine("ItemComplete exception: {0}", eventArgs.Exception);
    }


    private static void BlitImage(RenderState source, Image destination)
    {
        var left = source.Left;
        var top = source.Top;
        var width = source.Image.Width;
        var height = source.Image.Height;
        
        var right = left + width;
        var bottom = top + height;
        var toNextLineStart = 4 * ((destination.Width - (width + left)) + left);

        var from = 0;
        var to = 4 * (top * destination.Width + left);
        for (var y = top; y < bottom; y++)
        {
            for (var x = left; x < right; x++)
            {
                for (var i = 0; i < 4; i++)
                {
                    destination.Data[to++] = source.Image.Data[from++];
                }
            }

            to += toNextLineStart;
        }
    }

    /// <summary>
    /// Renders a part of a cube face.
    /// </summary>
    /// <param name="sourceImage">The source image.</param>
    /// <param name="cubeMapFaceOrientation"></param>
    /// <param name="rotation">Resulting cube horizontal rotation.</param>
    /// <param name="imageInterpolator">A pixel interpolator.</param>
    /// <param name="faceSize">The full face width and height.</param>
    /// <param name="state">Render state.</param>
    /// <returns>The part of the face rendered from the source image.</returns>
    private static void RenderSubFace(
        Image sourceImage,
        CubeMapFaceOrientation cubeMapFaceOrientation,
        double rotation,
        IPixelInterpolator imageInterpolator,
        int faceSize,
        RenderState state)
    {
        const double twoPi = 2.0 * Math.PI;
        const double cubeSize = 2.0;
        const double cubeSize2 = cubeSize / 2.0;
        
        var cubeOrientation = GetCubeOrientation(cubeMapFaceOrientation, GetCubeOrientationVector(cubeMapFaceOrientation));
        var destinationImage = state.Image;
        var rotationRad = Rad(rotation);
        var right = state.Left + destinationImage.Width;
        var bottom = state.Top + destinationImage.Height;

        var to = 0;
        for (var y = state.Top; y < bottom; y++)
        {
            for (var x = state.Left; x < right; x++)
            {
                // fill alpha channel
                destinationImage.Data[to + 3] = 255;

                // Get the position on cube a face.
                // Cube is centered at the origin with a side length of cubeSize. The "normal" cube size is 2.0.
                //var cube = cubeOrientation((2.0 * (x + 0.5) / faceWidth) - 1.0, (2.0 * (y + 0.5) / faceHeight) - 1.0);
                var cube = cubeOrientation((cubeSize * (x + 0.5) / faceSize) - cubeSize2, (cubeSize * (y + 0.5) / faceSize) - cubeSize2);

                // Project cube face onto a unit sphere by converting cartesian to spherical coordinates.
                var r = Math.Sqrt((cube.X * cube.X) + (cube.Y * cube.Y) + (cube.Z * cube.Z));
                var lon = Mod(Math.Atan2(cube.Y, cube.X) + rotationRad, twoPi);
                var lat = Math.Acos(cube.Z / r);

                imageInterpolator.CopyPixel(sourceImage, destinationImage, (sourceImage.Width * lon / Math.PI / 2.0) - 0.5, (sourceImage.Height * lat / Math.PI) - 0.5, to);

                to += 4;
            }
        }
    }
    
    
    private static double Rad(double angle)
    {
        return angle * (Math.PI / 180.0);
    }


    private static double Mod(double x, double n)
    {
        return ((x % n) + n) % n;
    }


    private static Vector3 GetCubeOrientationVector(CubeMapFaceOrientation cubeMapFaceOrientation)
    {
        return cubeMapFaceOrientation switch
        {
            CubeMapFaceOrientation.PositiveZ => new Vector3(-1, 0, 0),
            CubeMapFaceOrientation.NegativeZ => new Vector3(1, 0, 0),
            CubeMapFaceOrientation.PositiveX => new Vector3(0, -1, 0),
            CubeMapFaceOrientation.NegativeX => new Vector3(0, 1, 0),
            CubeMapFaceOrientation.PositiveY => new Vector3(0, 0, 1),
            CubeMapFaceOrientation.NegativeY => new Vector3(0, 0, -1),
            
            _ => throw new ArgumentException("Unknown face orientation: " + cubeMapFaceOrientation)
        };
    }


    private static Func<double, double, Vector3> GetCubeOrientation(CubeMapFaceOrientation cubeMapFaceOrientation, Vector3 v)
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
    
    
    private static string GetCubeMapFaceName(CubeMapFaceOrientation cubeMapFaceOrientation)
    {
        return cubeMapFaceOrientation switch
        {
            CubeMapFaceOrientation.PositiveZ => PositiveZOrientationFaceName,
            CubeMapFaceOrientation.NegativeZ => NegativeZOrientationFaceName,
            CubeMapFaceOrientation.PositiveX => PositiveXOrientationFaceName,
            CubeMapFaceOrientation.NegativeX => NegativeXOrientationFaceName,
            CubeMapFaceOrientation.PositiveY => PositiveYOrientationFaceName,
            CubeMapFaceOrientation.NegativeY => NegativeYOrientationFaceName,
            
            _ => throw new ArgumentException("Unknown cube map face orientation: " + cubeMapFaceOrientation)
        };
    }
    
    
    private static IPixelInterpolator GetPixelInterpolator(ImageInterpolationStrategy interpolationStrategy)
    {
        return interpolationStrategy switch
        {
            ImageInterpolationStrategy.Bilinear => new BilinearPixelInterpolator(),
            ImageInterpolationStrategy.Bicubic => new BicubicPixelInterpolator(),
            ImageInterpolationStrategy.Lanczos => new LanczosPixelInterpolator(),
            
            _ => new NearestPixelInterpolator()
        };
    }
}
