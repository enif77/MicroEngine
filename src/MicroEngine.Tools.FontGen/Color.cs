/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Tools.FontGen;

/// <summary>
/// A struct that represents a color with red, green, blue and alpha channels.
/// </summary>
/// <param name="r">Red.</param>
/// <param name="g">Green.</param>
/// <param name="b">Blue.</param>
/// <param name="a">Alpha. Optional; 255 by default.</param>
public class Color(byte r, byte g, byte b, byte a = 255)
{
    public byte R { get; set; } = r;
    public byte G { get; set; } = g;
    public byte B { get; set; } = b;
    public byte A { get; set; } = a;
}
