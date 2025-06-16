/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Graphics;

/// <summary>
/// Image in RGBA format.
/// </summary>
public class Image
{
    /// <summary>
    /// Pixels of this image in RGBA format.
    /// </summary>
    public byte[] Pixels { get; }

    /// <summary>
    /// The height of this image.
    /// </summary>
    public int Height { get; }

    /// <summary>
    /// The width of this image.
    /// </summary>
    public int Width { get; }
    
    
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="width">The width of this image.</param>
    /// <param name="height">The height of this image.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public Image(int width, int height)
    {
        if (width < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(width), "The width must be at least 1.");
        }
        
        if (height < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(height), "The height must be at least 1.");
        }
        
        Width = width;
        Height = height;
        Pixels = new byte[width * height * 4];
    }

    /// <summary>
    /// Constructor. Creates an image with the specified width, height and pixel data.
    /// </summary>
    /// <param name="width">The width of this image.</param>
    /// <param name="height">The height of this image.</param>
    /// <param name="pixels">The pixel data of this image in RGBA format.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the width or height is less than 1.</exception>
    /// <exception cref="ArgumentException">Thrown when the pixel data is null or does not match the expected size.</exception>
    public Image(int width, int height, byte[] pixels)
    {
        if (width < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(width), "The width must be at least 1.");
        }
        
        if (height < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(height), "The height must be at least 1.");
        }
        
        if (pixels == null || pixels.Length != Width * Height * 4)
        {
            throw new ArgumentException("Invalid pixel data.", nameof(pixels));
        }
        
        Pixels = pixels;
    }   
    
    /// <summary>
    /// Gets a pixel at the specified position or 0, if the given coords are outside of this image dimensions.
    /// </summary>
    /// <param name="x">A X of the pixel's coords to get.</param>
    /// <param name="y">A Y of the pixel's coords to get.</param>
    /// <returns></returns>
    public uint GetPixel(int x, int y)
    {
        if (x < 0 || x >= Width)
        {
            return 0;
        }
        
        if (y < 0 || y >= Height)
        {
            return 0;
        }
        
        var index = (x + y * Width) * 4;
        
        return (uint)Pixels[index] << 24 | (uint)Pixels[index + 1] << 16 | (uint)Pixels[index + 2] << 8 | Pixels[index + 3];
    }
    
    /// <summary>
    /// Writes a pixel at the specified position or does nothing, if the given coords are outside of this image dimensions.
    /// </summary>
    /// <param name="x">The X of the pixel's coors to be written.</param>
    /// <param name="y">The Y of the pixel's coors to be written.</param>
    /// <param name="r">The red part of the color.</param>
    /// <param name="g">The green part of the color.</param>
    /// <param name="b">The blue part of the color.</param>
    /// <param name="a">The alpha part of the color. 255 by default.</param>
    public void PutPixel(int x, int y, byte r, byte g, byte b, byte a = 255)
    {
        if (x < 0 || x >= Width)
        {
            return;
        }
        
        if (y < 0 || y >= Height)
        {
            return;
        }
        
        var index = (x + y * Width) * 4;
        
        Pixels[index] = r;
        Pixels[index + 1] = g;
        Pixels[index + 2] = b;
        Pixels[index + 3] = a;
    }
    
    /// <summary>
    /// Writes a pixel at the specified position or does nothing, if the given coords are outside of this image dimensions.
    /// </summary>
    /// <param name="x">The X of the pixel's coors to be written.</param>
    /// <param name="y">The Y of the pixel's coors to be written.</param>
    /// <param name="color">The color of the pixel in the RGBA format.</param>
    public void PutPixel(int x, int y, uint color)
    {
        PutPixel(
            x, y,
            (byte)(color >> 24),
            (byte)(color >> 16),
            (byte)(color >> 8),
            (byte)color);
    }
    
    /// <summary>
    /// Draws a horizontal line.
    /// </summary>
    /// <param name="x">The X of the start of the line.</param>
    /// <param name="y">The Y of the line.</param>
    /// <param name="length">How many pixels is the line long.</param>
    /// <param name="r">The red part of the color.</param>
    /// <param name="g">The green part of the color.</param>
    /// <param name="b">The blue part of the color.</param>
    /// <param name="a">The alpha part of the color. 255 by default.</param>
    public void HorizontalLine(int x, int y, int length, byte r, byte g, byte b, byte a = 255)
    {
        if (y < 0 || y >= Height)
        {
            return;
        }
        
        if (length < 1)
        {
            return;
        }
        
        for (var i = 0; i < length; i++)
        {
            var xi = x + i;
            if (xi < 0)
            {
                continue;
            }
            
            if (xi >= Width)
            {
                break;
            }
            
            PutPixel(xi, y, r, g, b, a);
        }
    }

    /// <summary>
    /// Draws a horizontal line.
    /// </summary>
    /// <param name="x">The X of the start of the line.</param>
    /// <param name="y">The Y of the line.</param>
    /// <param name="length">How many pixels is the line long.</param>
    /// <param name="color">The color of the line in the RGBA format.</param>
    public void HorizontalLine(int x, int y, int length, uint color)
    {
        HorizontalLine(
            x, y, length,
            (byte)(color >> 24),
            (byte)(color >> 16),
            (byte)(color >> 8),
            (byte)color);
    }

    /// <summary>
    /// Draws a vertical line.
    /// </summary>
    /// <param name="x">The X of the line.</param>
    /// <param name="y">The Y of the line start.</param>
    /// <param name="length">How many pixels is the line long.</param>
    /// <param name="r">The red part of the color.</param>
    /// <param name="g">The green part of the color.</param>
    /// <param name="b">The blue part of the color.</param>
    /// <param name="a">The alpha part of the color. 255 by default.</param>
    public void VerticalLine(int x, int y, int length, byte r, byte g, byte b, byte a = 255)
    {
        if (x < 0 || x >= Width)
        {
            return;
        }
        
        if (length < 1)
        {
            return;
        }
        
        for (var i = 0; i < length; i++)
        {
            var yi = y + i;
            if (yi < 0)
            {
                continue;
            }
            
            if (yi >= Height)
            {
                break;
            }
            
            PutPixel(x, yi, r, g, b, a);
        }
    }

    /// <summary>
    /// Draws a vertical line.
    /// </summary>
    /// <param name="x">The X of the line.</param>
    /// <param name="y">The Y of the line start.</param>
    /// <param name="length">How many pixels is the line long.</param>
    /// <param name="color">The color of the line in the RGBA format.</param>
    public void VerticalLine(int x, int y, int length, uint color)
    {
        VerticalLine(
            x, y, length,
            (byte)(color >> 24),
            (byte)(color >> 16),
            (byte)(color >> 8),
            (byte)color);
    }

    /// <summary>
    /// Fills this image with a color.
    /// </summary>
    /// <param name="r">The red part of the color.</param>
    /// <param name="g">The green part of the color.</param>
    /// <param name="b">The blue part of the color.</param>
    /// <param name="a">The alpha part of the color. 255 by default.</param>
    public void Fill(byte r, byte g, byte b, byte a = 255)
    {
        var pixels = Pixels;
        for (var index = 0; index < pixels.Length; index += 4)
        {
            pixels[index] = r;
            pixels[index + 1] = g;
            pixels[index + 2] = b;
            pixels[index + 3] = a;
        }
    }
    
    /// <summary>
    /// Clears this image. Sets all its pixels to 0.
    /// </summary>
    public void Clear()
    {
        var pixels = Pixels;
        for (var index = 0; index < pixels.Length; index++)
        {
            pixels[index] = 0;
        }
    }
}