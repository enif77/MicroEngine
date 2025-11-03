/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Tools.EquirectangularProjectionToCubeMapConverter.PixelInterpolators;

using System;

/// <summary>
/// Image pixel interpolator using the bilinear strategy.
/// </summary>
public class BilinearPixelInterpolator : APixelInterpolator
{
    public override void CopyPixel(Image inputImage, Image outputImage, double xFrom, double yFrom, int to)
    {
        var xl = Clamp(Math.Floor(xFrom), 0, inputImage.Width - 1);
        var xr = Clamp(Math.Ceiling(xFrom), 0, inputImage.Width - 1);
        var xf = xFrom - xl;

        var yl = Clamp(Math.Floor(yFrom), 0, inputImage.Height - 1);
        var yr = Clamp(Math.Ceiling(yFrom), 0, inputImage.Height - 1);
        var yf = yFrom - yl;

        var p00 = GetReadIndex((int)xl, (int)yl, inputImage.Width);
        var p10 = GetReadIndex((int)xr, (int)yl, inputImage.Width);
        var p01 = GetReadIndex((int)xl, (int)yr, inputImage.Width);
        var p11 = GetReadIndex((int)xr, (int)yr, inputImage.Width);

        for (var channel = 0; channel < 3; channel++)
        {
            var p0 = inputImage.Data[p00 + channel] * (1 - xf) + inputImage.Data[p10 + channel] * xf;
            var p1 = inputImage.Data[p01 + channel] * (1 - xf) + inputImage.Data[p11 + channel] * xf;
            outputImage.Data[to + channel] = (byte)Clamp(Math.Ceiling(p0 * (1 - yf) + p1 * yf), 0.0, 255.0);
        }
    }
}
