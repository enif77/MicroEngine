/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Tools.EquirectangularProjectionToCubeMapConverter.PixelInterpolators;

using System;

/// <summary>
/// Image pixel interpolator using the Lanczos resampling strategy.
/// </summary>
public class LanczosPixelInterpolator : AKernelInterpolator
{
    public override void CopyPixel(Image inputImage, Image outputImage, double xFrom, double yFrom, int to)
    {
        KernelResample(inputImage, outputImage, xFrom, yFrom, to, 5, x =>
        {
            const double filterSize = 5.0;
            if (x == 0)
            {
                return 1;
            }

            var xp = Math.PI * x;

            return filterSize * Math.Sin(xp) * Math.Sin(xp / filterSize) / (xp * xp);
        });
    }
}
