/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Tools.EquirectangularProjectionToCubeMapConverter.PixelInterpolators;

using System;

/// <summary>
/// Image pixel interpolator using the bicubic strategy.
/// </summary>
public class BicubicPixelInterpolator : AKernelInterpolator
{
    public override void CopyPixel(Image inputImage, Image outputImage, double xFrom, double yFrom, int to)
    {
        KernelResample(inputImage, outputImage, xFrom, yFrom, to, 2, x =>
        {
            const double b = -0.5;
            var x1 = Math.Abs(x);
            var x2 = x1 * x1;
            var x3 = x2 * x1;

            return (x1 <= 1.0)
                ? ((b + 2.0) * x3) - ((b + 3.0) * x2) + 1.0
                : (b * x3) - (5.0 * b * x2) + (8.0 * b * x1) - (4.0 * b);
        });
    }
}
