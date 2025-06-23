/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Tools.FontGen;

using SkiaSharp;

/// <summary>
/// Various helper methods for the font generation tool.
/// </summary>
internal static class Helpers
{
    public static string CodePointToString(char codePoint)
    {
        return codePoint <= 0xFFFF
            ? new string(codePoint, 1)
            : " ";  //new String.fromCharCode((codePoint - 0x10000 >> 10) + 0xD800, (codePoint - 0x10000 & 0x3FF) + 0xDC00);
    }
    
    
    public static Image? ExtractPixels(SKImage skImage)
    {
        using var pixmap = skImage.PeekPixels();

        if (pixmap == null)
        {
            return null;
        }
        
        var width = pixmap.Width;
        var height = pixmap.Height;
        var pixels = pixmap.GetPixels();

        var image = new Image(width, height);

        unsafe
        {
            var pixelPtr = (byte*)pixels;
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var offset = (y * pixmap.RowBytes) + (x * 4);
                    var b = pixelPtr[offset];
                    var g = pixelPtr[offset + 1];
                    var r = pixelPtr[offset + 2];
                    var a = pixelPtr[offset + 3];

                    image.PutPixel(x, y, r, g, b, a);
                }
            }
        }

        return image;
    }
    
    
    public static void SaveImageToRgbaBmp(Image image, string filePath)
    {
        using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        using (BinaryWriter writer = new BinaryWriter(fs))
        {
            var rowSize = image.Width * 4; // Size of one row in bytes (RGBA = 4 bytes per pixel)
            var paddingSize = (4 - (rowSize % 4)) % 4; // Padding to make row size divisible by 4
            var paddedRowSize = rowSize + paddingSize; // Total row size including padding
            var fileSize = 54 + paddedRowSize * image.Height; // BMP file size

            // BMP Header
            writer.Write((ushort)0x4D42); // 'BM' signature
            writer.Write(fileSize); // File size
            writer.Write((ushort)0); // Reserved
            writer.Write((ushort)0); // Reserved
            writer.Write((uint)54); // Data offset

            // DIB Header (BITMAPINFOHEADER)
            writer.Write((uint)40); // Header size
            writer.Write(image.Width); // Image width
            writer.Write(image.Height); // Image height
            writer.Write((ushort)1); // Planes
            writer.Write((ushort)32); // Bits per pixel
            writer.Write((uint)0); // Compression
            writer.Write(paddedRowSize * image.Height); // Image size
            writer.Write((int)0); // X pixels per meter
            writer.Write((int)0); // Y pixels per meter
            writer.Write((uint)0); // Total colors
            writer.Write((uint)0); // Important colors
            
            // Image data (bottom-to-top)
            var imageData = image.Pixels;
            for (var y = image.Height - 1; y >= 0; y--)
            {
                for (var x = 0; x < image.Width; x++)
                {
                    var pixelIndex = (y * image.Width + x) * 4;
                    
                    writer.Write(imageData[pixelIndex + 2]); // Blue component
                    writer.Write(imageData[pixelIndex + 1]); // Green component
                    writer.Write(imageData[pixelIndex]);     // Red component
                    writer.Write(imageData[pixelIndex + 3]); // Alpha component
                }

                // Write padding bytes
                for (var p = 0; p < paddingSize; p++)
                {
                    writer.Write((byte)0);
                }
            }
        }
    }
    
    
    public static void SaveImageToRgbBmp(Image image, string filePath)
    {
        using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        using (BinaryWriter writer = new BinaryWriter(fs))
        {
            var rowSize = image.Width * 3; // Size of one row in bytes (RGB = 3 bytes per pixel)
            var paddingSize = (4 - (rowSize % 4)) % 4; // Padding to make row size divisible by 4
            var paddedRowSize = rowSize + paddingSize; // Total row size including padding
            var fileSize = 54 + paddedRowSize * image.Height; // BMP file size

            // BMP Header
            writer.Write((ushort)0x4D42); // 'BM' signature
            writer.Write(fileSize); // File size
            writer.Write((ushort)0); // Reserved
            writer.Write((ushort)0); // Reserved
            writer.Write((uint)54); // Data offset

            // DIB Header (BITMAPINFOHEADER)
            writer.Write((uint)40); // Header size
            writer.Write(image.Width); // Image width
            writer.Write(image.Height); // Image height
            writer.Write((ushort)1); // Planes
            writer.Write((ushort)24); // Bits per pixel
            writer.Write((uint)0); // Compression
            writer.Write(paddedRowSize * image.Height); // Image size
            writer.Write((int)0); // X pixels per meter
            writer.Write((int)0); // Y pixels per meter
            writer.Write((uint)0); // Total colors
            writer.Write((uint)0); // Important colors
            
            // Image data (bottom-to-top)
            var rgbPixels = ConvertImageToRgb(image);
            for (var y = image.Height - 1; y >= 0; y--)
            {
                for (var x = 0; x < image.Width; x++)
                {
                    var pixelIndex = (y * image.Width + x) * 3;
                    
                    writer.Write(rgbPixels[pixelIndex + 2]); // Blue component
                    writer.Write(rgbPixels[pixelIndex + 1]); // Green component
                    writer.Write(rgbPixels[pixelIndex]);     // Red component
                }

                // Write padding bytes
                for (var p = 0; p < paddingSize; p++)
                {
                    writer.Write((byte)0);
                }
            }
        }
    }
    
    
    private static byte[] ConvertImageToRgb(Image rgbaImage)
    {
        var rgbPixels = new byte[rgbaImage.Width * rgbaImage.Height * 3];
        var rgbaPixels = rgbaImage.Pixels;
        
        for (var i = 0; i < rgbaImage.Width * rgbaImage.Height; i++)
        {
            var rgbaIndex = i * 4;
            var rgbIndex = i * 3;
            
            rgbPixels[rgbIndex + 0] = rgbaPixels[rgbaIndex + 0]; // Red
            rgbPixels[rgbIndex + 1] = rgbaPixels[rgbaIndex + 1]; // Green
            rgbPixels[rgbIndex + 2] = rgbaPixels[rgbaIndex + 2]; // Blue
        }

        return rgbPixels;
    }
}
