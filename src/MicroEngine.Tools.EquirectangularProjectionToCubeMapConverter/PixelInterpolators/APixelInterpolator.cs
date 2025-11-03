/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Tools.EquirectangularProjectionToCubeMapConverter.PixelInterpolators;

using System;

/// <summary>
/// A pixel interpolation strategy base class.
/// </summary>
public abstract class APixelInterpolator : IPixelInterpolator
{
    public abstract void CopyPixel(Image inputImage, Image outputImage, double xFrom, double yFrom, int to);

    /// <summary>
    /// Clamp a value to the specified range.
    /// </summary>
    /// <param name="x">A value to clamp.</param>
    /// <param name="min">The minimum allowed value.</param>
    /// <param name="max">The maximum allowed value.</param>
    /// <returns>>The clamped value.</returns>
    protected static double Clamp(double x, double min, double max)
    {
        return Math.Min(max, Math.Max(x, min));
    }

    /// <summary>
    /// Gets the read index for the specified RGBA pixel coordinates in an image with the specified width.
    /// </summary>
    /// <param name="x">The X coordinate of the pixel.</param>
    /// <param name="y">The Y coordinate of the pixel.</param>
    /// <param name="width">The width of the image in pixels.</param>
    /// <returns>The read index.</returns>
    protected static int GetReadIndex(int x, int y, int width)
    {
        return 4 * (y * width + x);
    }
}
