/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Tools.EquirectangularProjectionToCubeMapConverter;

/// <summary>
/// Defines a pixel interpolation strategy.
/// </summary>
public interface IPixelInterpolator
{
    /// <summary>
    /// Copies a pixel from the input image to the output image using a defined interpolation.
    /// </summary>
    /// <param name="inputImage">An input image.</param>
    /// <param name="outputImage">An output image.</param>
    /// <param name="xFrom">From X coordinate.</param>
    /// <param name="yFrom">From Y coordinate.</param>
    /// <param name="to">To index in the output image data array.</param>
    void CopyPixel(Image inputImage, Image outputImage, double xFrom, double yFrom, int to);
}
