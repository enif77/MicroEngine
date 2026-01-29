/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Demos.GL.PanoramaViewerDemo;

using MicroEngine.Graphics;

/// <summary>
/// A bilinear pixel interpolator.
/// </summary>
public class PixelInterpolator
{
    /// <summary>
    /// Copies a pixel from the input image to the output image using a defined interpolation.
    /// </summary>
    /// <param name="inputImage">An input image.</param>
    /// <param name="outputImage">An output image.</param>
    /// <param name="xFrom">From X coordinate.</param>
    /// <param name="yFrom">From Y coordinate.</param>
    /// <param name="to">To index in the output image data array.</param>
    public void CopyPixel(Image inputImage, Image outputImage, double xFrom, double yFrom, int to)
    {
        var xl = (int)Math.Floor(xFrom);
        xl = ((xl % inputImage.Width) + inputImage.Width) % inputImage.Width;
        
        var xr = (xl + 1) % inputImage.Width;
        var xf = xFrom - Math.Floor(xFrom);

        var yl = Math.Clamp((int)Math.Floor(yFrom), 0, inputImage.Height - 1);
        var yr = Math.Clamp(yl + 1, 0, inputImage.Height - 1);
        var yf = yFrom - Math.Floor(yFrom);

        var p00 = GetReadIndex(xl, yl, inputImage.Width);
        var p10 = GetReadIndex(xr, yl, inputImage.Width);
        var p01 = GetReadIndex(xl, yr, inputImage.Width);
        var p11 = GetReadIndex(xr, yr, inputImage.Width);

        for (var channel = 0; channel < 3; channel++)
        {
            var p0 = inputImage.Pixels[p00 + channel] * (1 - xf) + inputImage.Pixels[p10 + channel] * xf;
            var p1 = inputImage.Pixels[p01 + channel] * (1 - xf) + inputImage.Pixels[p11 + channel] * xf;
            outputImage.Pixels[to + channel] = (byte)Math.Clamp(Math.Ceiling(p0 * (1 - yf) + p1 * yf), 0.0, 255.0);
        }
    }

    /// <summary>
    /// Gets the read index for the specified RGBA pixel coordinates in an image with the specified width.
    /// </summary>
    /// <param name="x">The X coordinate of the pixel.</param>
    /// <param name="y">The Y coordinate of the pixel.</param>
    /// <param name="width">The width of the image in pixels.</param>
    /// <returns>The read index.</returns>
    private static int GetReadIndex(int x, int y, int width)
    {
        return 4 * (y * width + x);
    }
}
