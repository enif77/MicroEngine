/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Tools.EquirectangularProjectionToCubeMapConverter.PixelInterpolators;

using System;

public abstract class AKernelInterpolator : APixelInterpolator
{
    /// <summary>
    /// Performs a discrete convolution with a provided kernel.
    /// </summary>
    /// <param name="inputImage">The input image.</param>
    /// <param name="outputImage">The output image.</param>
    /// <param name="xFrom">The source pixel X position.</param>
    /// <param name="yFrom">The source pixel Y position.</param>
    /// <param name="to">Target index in the output image.</param>
    /// <param name="filterSize">The filter size.</param>
    /// <param name="kernel">The kernel/filter function.</param>
    protected void KernelResample(Image inputImage, Image outputImage, double xFrom, double yFrom, int to, int filterSize, Func<double, double> kernel)
    {
        var twoFilterSize = 2 * filterSize;
        var xMax = (double)(inputImage.Width - 1);
        var yMax = (double)(inputImage.Height - 1);
        var xKernel = new double[twoFilterSize];
        var yKernel = new double[twoFilterSize];

        var xStart = Math.Floor(xFrom) - filterSize + 1;
        var yStart = Math.Floor(yFrom) - filterSize + 1;

        for (var i = 0; i < twoFilterSize; i++)
        {
            xKernel[i] = kernel(xFrom - (xStart + i));
            yKernel[i] = kernel(yFrom - (yStart + i));
        }

        for (var channel = 0; channel < 3; channel++)
        {
            var q = 0.0;

            for (var i = 0; i < twoFilterSize; i++)
            {
                var yClamped = (int)Clamp(yStart + i, 0.0, yMax);
                var p = 0.0;
                for (var j = 0; j < twoFilterSize; j++)
                {
                    var index = GetReadIndex((int)Clamp(xStart + j, 0.0, xMax), yClamped, inputImage.Width);
                    p += inputImage.Data[index + channel] * xKernel[j];
                }

                q += p * yKernel[i];
            }

            outputImage.Data[to + channel] = (byte)Clamp(Math.Round(q), 0.0, 255.0);
        }
    }
}
    