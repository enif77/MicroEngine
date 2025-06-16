/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Graphics;

/// <summary>
/// A struct that represents a color with red, green, blue and alpha channels.
/// </summary>
/// <param name="r">Red.</param>
/// <param name="g">Green.</param>
/// <param name="b">Blue.</param>
/// <param name="a">Alpha. Optional; 255 by default.</param>
public readonly struct Color(byte r, byte g, byte b, byte a = 255)
{
    public byte R { get; } = r;
    public byte G { get; } = g;
    public byte B { get; } = b;
    public byte A { get; } = a;


    /// <summary>
    /// Encodes a color to a single unsigned 32-bit integer in RGBA format.
    /// </summary>
    /// <param name="color">A color to encode.</param>
    public static uint Encode(Color color)
        => (uint)color.R << 24 | (uint)color.G << 16 | (uint)color.B << 8 | color.A;
    
    /// <summary>
    /// Encodes a color to a single unsigned 32-bit integer in RGBA format.
    /// </summary>
    /// <param name="r">Red.</param>
    /// <param name="g">Green.</param>
    /// <param name="b">Blue.</param>
    /// <param name="a">Alpha.</param>
    public static uint Encode(byte r, byte g, byte b, byte a)
        => (uint)r << 24 | (uint)g << 16 | (uint)b << 8 | a;
    
    /// <summary>
    /// Decodes a color in RGBA format to a new instance of Color.
    /// </summary>
    /// <param name="color">A color to decode.</param>
    public static Color Decode(uint color)
        => new Color((byte)(color >> 24), (byte)(color >> 16), (byte)(color >> 8), (byte)color);
}
