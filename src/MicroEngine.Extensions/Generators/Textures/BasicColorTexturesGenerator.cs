/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Extensions.Generators.Textures;

using MicroEngine.Graphics;

/// <summary>
/// Provides methods to generate solid color textures.
/// </summary>
public static class BasicColorTexturesGenerator
{
    /// <summary>
    /// Generates a solid-color texture with the specified RGBA values.
    /// </summary>
    /// <param name="width">Texture width (≥1 pixel).</param>
    /// <param name="height">Texture height (≥1 pixel).</param>
    /// <param name="r">Red component [0–255].</param>
    /// <param name="g">Green component [0–255].</param>
    /// <param name="b">Blue component [0–255].</param>
    /// <param name="a">Alpha component [0–255], optional, default = 255 (opaque).</param>
    /// <returns>Generated texture as an <see cref="Image"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if dimensions or color values are invalid.</exception>
    public static Image GenerateColorTexture(int width, int height, byte r, byte g, byte b, byte a = 255)
    {
        ValidateDimensions(width, height);

        var img = new Image(width, height);

        img.Fill(r, g, b, a); // Assuming Fill accepts alpha; if not, adjust accordingly.

        return img;
    }

    /// <summary>
    /// Generates a solid-color texture with RGB and optional alpha specified in an array or comma-separated list.
    /// </summary>
    /// <param name="width">Texture width (≥1 pixel).</param>
    /// <param name="height">Texture height (≥1 pixel).</param>
    /// <param name="channels">RGB or RGBA bytes. 3 or 4 values accepted.</param>
    /// <returns>Generated texture.</returns>
    /// <exception cref="ArgumentException">If the number of channels is not 3 or 4.</exception>
    public static Image GenerateColorTexture(int width, int height, params byte[] channels)
    {
        if (channels == null || (channels.Length != 3 && channels.Length != 4))
            throw new ArgumentException("Please provide 3 (RGB) or 4 (RGBA) channel values.", nameof(channels));

        byte r = channels[0], g = channels[1], b = channels[2];
        byte a = channels.Length == 4 ? channels[3] : (byte)255;

        return GenerateColorTexture(width, height, r, g, b, a);
    }

    // Standard color helpers:

    /// <inheritdoc cref="GenerateColorTexture(int, int, byte, byte, byte, byte)"/>
    public static Image GenerateRedTexture(int width, int height) =>
        GenerateColorTexture(width, height, 255, 0, 0);

    public static Image GenerateGreenTexture(int width, int height) =>
        GenerateColorTexture(width, height, 0, 255, 0);

    public static Image GenerateBlueTexture(int width, int height) =>
        GenerateColorTexture(width, height, 0, 0, 255);

    public static Image GenerateYellowTexture(int width, int height) =>
        GenerateColorTexture(width, height, 255, 255, 0);

    public static Image GeneratePinkTexture(int width, int height) =>
        GenerateColorTexture(width, height, 255, 192, 203);

    public static Image GenerateCyanTexture(int width, int height) =>
        GenerateColorTexture(width, height, 0, 255, 255);

    public static Image GeneratePurpleTexture(int width, int height) =>
        GenerateColorTexture(width, height, 128, 0, 128);

    public static Image GenerateOrangeTexture(int width, int height) =>
        GenerateColorTexture(width, height, 255, 165, 0);

    public static Image GenerateBlackTexture(int width, int height) =>
        GenerateColorTexture(width, height, 0, 0, 0);

    public static Image GenerateWhiteTexture(int width, int height) =>
        GenerateColorTexture(width, height, 255, 255, 255);

    public static Image GenerateGrayTexture(int width, int height) =>
        GenerateColorTexture(width, height, 128, 128, 128);

    /// <summary>
    /// Validates texture dimensions.
    /// </summary>
    /// <param name="width">Texture width.</param>
    /// <param name="height">Texture height.</param>
    /// <exception cref="ArgumentOutOfRangeException">If either dimension is less than 1.</exception>
    private static void ValidateDimensions(int width, int height)
    {
        if (width < 1)
            throw new ArgumentOutOfRangeException(nameof(width), "Texture width must be positive and non-zero.");

        if (height < 1)
            throw new ArgumentOutOfRangeException(nameof(height), "Texture height must be positive and non-zero.");
    }
}
