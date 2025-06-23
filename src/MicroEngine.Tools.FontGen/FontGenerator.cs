/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Tools.FontGen;

using System.Globalization;
using System.Text;

using SkiaSharp;

internal class RenderedCodePoint
{
    public Image? Image { get; set; }
    public int OriginX { get; set; }
    public int OriginY { get; set; }
    public double Advance { get; set; }
}


internal class Coordinate
{
    public int X { get; set; }
    public int Y { get; set; }
}


internal class PackedInfo
{
    public int Index { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    public List<Coordinate> Coordinates { get; set; } = new List<Coordinate>();
}

/// <summary>
/// Generates a bitmap font and its metadata from a given configuration.
/// </summary>
public static class FontGenerator
{
    /// <summary>
    /// Generates a bitmap font based on the provided configuration.
    /// </summary>
    /// <param name="fontConfiguration">Font configuration containing details such as font name, size, colors, and custom characters.</param>
    internal static RenderedFont Generate(FontConfiguration fontConfiguration)
    {
        var codePoints = GenerateCodePoints(fontConfiguration.CustomCharacters);

        // Create an SKPaint object for text rendering
        var paint = new SKPaint
        {
            IsAntialias = true,
            Color = new SKColor(fontConfiguration.TextColor.R, fontConfiguration.TextColor.G, fontConfiguration.TextColor.B, fontConfiguration.TextColor.A),
        };

        var strokePaint = new SKPaint
        {
            IsAntialias = true,
            Color = new SKColor(fontConfiguration.StrokeColor.R, fontConfiguration.StrokeColor.G, fontConfiguration.StrokeColor.B, fontConfiguration.StrokeColor.A),
            IsStroke = true,
            Style = SKPaintStyle.Stroke,
            StrokeMiter = 4f,      
            StrokeWidth = fontConfiguration.StrokeThickness,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = fontConfiguration.IsRoundStroke ? SKStrokeJoin.Round : SKStrokeJoin.Miter
        };
        
        var typeface = string.IsNullOrWhiteSpace(fontConfiguration.FontFilePath)
            ? (string.IsNullOrWhiteSpace(fontConfiguration.FontName) ? SKTypeface.Default : SKTypeface.FromFamilyName(fontConfiguration.FontName))
            : SKTypeface.FromFile(fontConfiguration.FontFilePath);

        var font = new SKFont
        {
            Typeface = typeface,
            Size = fontConfiguration.FontSize,
            Embolden = fontConfiguration.IsBold,
            SkewX = fontConfiguration.IsItalic ? -0.25f : 0
        };
        
        var images = new List<RenderedCodePoint>();

        foreach (var codePoint in codePoints)
        {
            images.Add(RenderCodePoint(paint, strokePaint, font, fontConfiguration, codePoint, 2));
        }

        var packed = Pack(images, fontConfiguration);

        var fontImage = new Image(packed.Width, packed.Height);
        var data = fontImage.Pixels;

        if (fontConfiguration.UseDisplayTypeDistanceField)
        {
            for (var i = 0; i < data.Length; i += 4)
            {
                data[i] = 0;
                data[i + 1] = 0;
                data[i + 2] = 0;
                data[i + 3] = 255;
            }
        }
        else
        {
            for (var i = 0; i < data.Length; i += 4)
            {
                data[i] = fontConfiguration.BackgroundColor.R;
                data[i + 1] = fontConfiguration.BackgroundColor.G;
                data[i + 2] = fontConfiguration.BackgroundColor.B;
                data[i + 3] = fontConfiguration.BackgroundColor.A;
            }
        }

        for (var i = 0; i < codePoints.Length; i++)
        {
            var image = images[i].Image!;
            var coordinates = packed.Coordinates[i];

            for (var y = 0; y < image.Height; y++)
            {
                CopyImageRegion(
                    image,
                    fontImage,
                    0, 0, image.Width, image.Height,
                    coordinates.X, coordinates.Y
                );
            }
        }
        
        var fontJsonSb = new StringBuilder();

        fontJsonSb.AppendLine("{");

        fontJsonSb.AppendLine($"  \"Name\": \"{fontConfiguration.FontName}\",");
        fontJsonSb.AppendLine($"  \"Size\": {fontConfiguration.FontSize},");
        fontJsonSb.AppendLine($"  \"Width\": {packed.Width},");
        fontJsonSb.AppendLine($"  \"Height\": {packed.Height},");
        fontJsonSb.AppendLine($"  \"Bold\": {(fontConfiguration.IsBold ? "true" : "false")},");
        fontJsonSb.AppendLine($"  \"Italic\": {(fontConfiguration.IsItalic ? "true" : "false")},");
        fontJsonSb.AppendLine($"  \"Characters\": [");

        for (var i = 0; i < codePoints.Length; i++)
        {
            var image = images[i];

            // TODO: Support characters < ' '.

            var codePointStr = Helpers.CodePointToString(codePoints[i])
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"");
            
            var padding = fontConfiguration.Padding;

            fontJsonSb.Append($"    {{ \"CodePoint\": \"{codePointStr}\",");
            fontJsonSb.Append($" \"X\": {packed.Coordinates[i].X - padding},");
            fontJsonSb.Append($" \"Y\": {packed.Coordinates[i].Y - padding},");
            fontJsonSb.Append($" \"Width\": {image.Image!.Width + 2 * padding},");
            fontJsonSb.Append($" \"Height\": {image.Image!.Height + 2 * padding},");
            fontJsonSb.Append($" \"OriginX\": {image.OriginX + padding},");
            fontJsonSb.Append($" \"OriginY\": {image.OriginY + padding},");
            fontJsonSb.Append($" \"Advance\": {image.Advance.ToString(CultureInfo.InvariantCulture)}");

            if (i < codePoints.Length - 1)
            {
                fontJsonSb.AppendLine("},");

                continue;
            }

            fontJsonSb.AppendLine("}");
        }

        fontJsonSb.AppendLine("  ]");
        fontJsonSb.AppendLine("}");
        
        return new RenderedFont(fontImage, fontJsonSb.ToString());
    }
    
    
    private static PackedInfo Pack(List<RenderedCodePoint> images, FontConfiguration fontConfiguration)
    {
        var maxWidth = 0;
        var totalArea = 0;
        var coordinates = new List<Coordinate>();

        var sorted = new List<PackedInfo>();

        for (var i = 0; i < images.Count; i++)
        {
            var image = images[i].Image!;
            maxWidth = Math.Max(maxWidth, image.Width);
            totalArea += image.Width * image.Height;
            coordinates.Add(new Coordinate());

            sorted.Add(new PackedInfo()
            {
                Index = i,
                Width = image.Width + 2 * fontConfiguration.Padding,
                Height = image.Height + 2 * fontConfiguration.Padding,
            });
        }

        sorted = sorted
            .OrderByDescending(p => p.Height)
            .ThenByDescending(p => p.Width)
            .ToList();

        var width = Math.Max(maxWidth, Math.Ceiling(Math.Sqrt(totalArea) * 2));
        if (width > 4096)
        {
            width = Math.Max(maxWidth, Math.Ceiling(Math.Sqrt(totalArea) * 1.25));
        }

        if (fontConfiguration.UseResolutionPowerOf2)
        {
            width = NextPowerOf2((int)width);
        }

        var currentX = 0;
        var currentY = 0;
        var rowWidth = 0;
        var rowHeight = 0;

        for (var i = 0; i < sorted.Count; i++)
        {
            var image = sorted[i];
            if (currentX + image.Width > width)
            {
                currentX = 0;
                currentY += rowHeight;
                rowHeight = 0;
            }

            coordinates[image.Index] = new Coordinate()
            {
                X = currentX + fontConfiguration.Padding,
                Y = currentY + fontConfiguration.Padding,
            };

            currentX += image.Width;
            rowWidth = Math.Max(rowWidth, currentX);
            rowHeight = Math.Max(rowHeight, image.Height);
        }

        var height = currentY + rowHeight;

        if (fontConfiguration.UseResolutionPowerOf2)
        {
            height = NextPowerOf2(height);
        }

        return new PackedInfo()
        {
            Width = (int)(fontConfiguration.UseResolutionPowerOf2 ? width : rowWidth),
            Height = height,
            Coordinates = coordinates.ToList(),
        };
    }


    private static int NextPowerOf2(int target)
    {
        var value = 2;
        while (value < target)
        {
            value *= 2;
        }

        return value;
    }
    
    
    private static RenderedCodePoint RenderCodePoint(
        SKPaint paint,
        SKPaint strokePaint,
        SKFont font,
        FontConfiguration fontConfiguration, char codePoint, int multiplier)
    {
        // Sample distance field at 2x for anti-aliasing
        var distanceFieldScaleFactor = fontConfiguration.UseDisplayTypeDistanceField ? 2 : 1; 

        var size = (int)font.Size * distanceFieldScaleFactor;
        var originX = (int)Math.Round((double)font.Size * (multiplier - 1) / 2) * distanceFieldScaleFactor;
        var originY = (int)Math.Round((double)font.Size * (multiplier + 1) / 2) * distanceFieldScaleFactor;
        var character = Helpers.CodePointToString(codePoint);
        var width = size * multiplier;
        var height = size * multiplier;
        var bg = fontConfiguration.UseDisplayTypeDistanceField
            ? new Color(0, 0, 0, 0)
            : fontConfiguration.BackgroundColor;

        // Measure text size
        var textWidth = font.MeasureText(character.ToString());

        // Create an image surface
        using var surface = SKSurface.Create(new SKImageInfo(width, height));
        var canvas = surface.Canvas;
        
        if (fontConfiguration.UseDisplayTypeDistanceField)
        {
            paint.Color = new SKColor(0, 0, 0);

            // Draw the character
            canvas.DrawText(character, originX, originY, font, paint);
        }
        else
        {
            // Fill background
            canvas.Clear(new SKColor(bg.R, bg.G, bg.B, bg.A));    

            // Draw the character
            if (fontConfiguration.HasStroke && fontConfiguration.StrokeThickness > 0)
            {
                canvas.DrawText(character, originX, originY, SKTextAlign.Left, font, strokePaint);
            }
            canvas.DrawText(character, originX, originY, SKTextAlign.Left, font, paint);
        } 
            
        // Save the image
        using var image = surface.Snapshot();

        // using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        // File.WriteAllBytes(
        //     Path.Combine(fontConfiguration.OutputFolderPath, $"rendered_char_{codePoint}.png"),
        //     data.ToArray());
        
        var extractedPixels = Helpers.ExtractPixels(image) ?? new Image(16, 16);
        
        var xmin = GetMinX(extractedPixels, bg);
        var ymin = GetMinY(extractedPixels, bg);
        var xmax = GetMaxX(extractedPixels, bg, xmin);
        var ymax = GetMaxY(extractedPixels, bg, ymin);

        // Attempt to handle fonts with unusually large glyphs (Zapfino for example)
        if ((xmin == 0 || ymin == 0 || xmax == width || ymax == height) && multiplier < 5)
        {
            return RenderCodePoint(paint, strokePaint, font, fontConfiguration, codePoint, multiplier + 1);
        }

        var trimmedWidth = xmax - xmin;
        var trimmedHeight = ymax - ymin;
        var trimmedPixels = new Image(trimmedWidth, trimmedHeight);

        CopyImageRegion(
            extractedPixels,
            trimmedPixels,
            xmin, ymin, trimmedWidth, trimmedHeight,
            0, 0);

        // Empty glyphs shouldn't have offsets
        if (trimmedWidth == 1 && trimmedHeight == 1 && IsSameColor(bg, xmin, ymin, extractedPixels))
        {
            originX = xmin;
            originY = ymin;
        }

        var imageInfo = new RenderedCodePoint()
        {
            Image = trimmedPixels,
            OriginX = originX - xmin,
            OriginY = originY - ymin,
            Advance = textWidth
        };
        
        if (fontConfiguration.UseDisplayTypeDistanceField)
        {
            // Anti-aliasing
            imageInfo = ConvertToDistanceField(imageInfo, fontConfiguration.DistanceFieldFalloff * distanceFieldScaleFactor);
            imageInfo = DownsampleLuminanceBy2(imageInfo); 
        }
        
        return imageInfo;
    }
    
    
    private static void CopyImageRegion(
        Image sourceImage,
        Image targetImage,
        int sourceX, int sourceY, int regionWidth, int regionHeight,
        int targetX, int targetY)
    {
        // Ensure the region is within the bounds of the source image
        if (sourceX < 0 || sourceY < 0 || sourceX + regionWidth > sourceImage.Width || sourceY + regionHeight > sourceImage.Height)
        {
            throw new ArgumentException("Region is out of bounds in the source image.");
        }
        
        var sourceImagePixels = sourceImage.Pixels;
        var targetImagePixels = targetImage.Pixels;

        // Iterate over the region
        for (int y = 0; y < regionHeight; y++)
        {
            for (int x = 0; x < regionWidth; x++)
            {
                int sourcePixelX = sourceX + x;
                int sourcePixelY = sourceY + y;

                int targetPixelX = targetX + x;
                int targetPixelY = targetY + y;

                // Check if the target pixel is within the bounds of the target image
                if (targetPixelX >= 0 && targetPixelX < targetImage.Width && targetPixelY >= 0 && targetPixelY < targetImage.Height)
                {
                    var sourceIndex = (sourcePixelY * sourceImage.Width + sourcePixelX) * 4;
                    var targetIndex = (targetPixelY * targetImage.Width + targetPixelX) * 4;

                    // Copy RGB values from source to target
                    targetImagePixels[targetIndex + 0] = sourceImagePixels[sourceIndex + 0]; // Red
                    targetImagePixels[targetIndex + 1] = sourceImagePixels[sourceIndex + 1]; // Green
                    targetImagePixels[targetIndex + 2] = sourceImagePixels[sourceIndex + 2]; // Blue
                    targetImagePixels[targetIndex + 3] = sourceImagePixels[sourceIndex + 3]; // Alpha
                }
            }
        }
    }
    
    
    private static RenderedCodePoint DownsampleLuminanceBy2(RenderedCodePoint image)
    {
        var oldWidth = image.Image!.Width;
        var oldHeight = image.Image.Height;
        var newWidth = oldWidth >> 1;
        var newHeight = oldHeight >> 1;
        var oldData = image.Image.Pixels;
        var newData = new byte[newWidth * newHeight << 2];
        var oldStride = oldWidth << 2;

        for (var y = 0; y < newHeight; y++)
        {
            var from = (y << 1) * oldStride;
            var to = y * newWidth << 2;

            for (var x = 0; x < newWidth; x++)
            {
                newData[to] = newData[to + 1] = newData[to + 2] = (byte)((
                    oldData[from] +
                    oldData[from + 4] +
                    oldData[from + oldStride] +
                    oldData[from + oldStride + 4]
                ) >> 2);
                newData[to + 3] = 255;
                from += 8;
                to += 4;
            }
        }

        var imageInfo = new RenderedCodePoint()
        {
            Image = new Image(newWidth, newHeight),
            OriginX = image.OriginX >> 1,
            OriginY = image.OriginY >> 1,
            Advance = image.Advance / 2
        };

        Array.Copy(newData, imageInfo.Image.Pixels, newData.Length);

        return imageInfo;
    }
    
    
    private static RenderedCodePoint ConvertToDistanceField(RenderedCodePoint image, int falloff)
    {
        var oldWidth = image.Image!.Width;
        var oldHeight = image.Image.Height;
        var data = image.Image.Pixels;

        var newWidth = oldWidth + 2 * falloff;
        var newHeight = oldHeight + 2 * falloff;

        var ping = new float[newWidth * newHeight * 4];
        var pong = new float[newWidth * newHeight * 4];

        // Initialize ping buffer
        for (int y = 0, i = 0; y < newHeight; y++)
        {
            for (var x = 0; x < newWidth; x++, i += 4)
            {
                if (falloff <= x && x < falloff + oldWidth &&
                    falloff <= y && y < falloff + oldHeight &&
                    data[((x - falloff) + (y - falloff) * oldWidth) * 4 + 3] > 127)
                {
                    ping[i] = x;
                    ping[i + 1] = y;
                    ping[i + 2] = float.NaN;
                    ping[i + 3] = float.NaN;
                }
                else
                {
                    ping[i] = float.NaN;
                    ping[i + 1] = float.NaN;
                    ping[i + 2] = x;
                    ping[i + 3] = y;
                }
            }
        }

        var step = 1;
        while (step < newWidth || step < newHeight)
        {
            step <<= 1;
        }

        // Use jump flooding to compute the distance transform quickly. This
        // actually computes two distance transforms to get the signed distance,
        // one for the negative distances and one for the positive distances.
        while (step > 0)
        {
            for (int y = 0, i = 0; y < newHeight; y++)
            {
                for (var x = 0; x < newWidth; x++, i += 4)
                {
                    var bestFirstDistance = float.PositiveInfinity;
                    var bestFirstX = float.NaN;
                    var bestFirstY = float.NaN;

                    var bestSecondDistance = float.PositiveInfinity;
                    var bestSecondX = float.NaN;
                    var bestSecondY = float.NaN;

                    for (var neighbor = 0; neighbor < 9; neighbor++)
                    {
                        var nx = x + (neighbor % 3 - 1) * step;
                        var ny = y + (neighbor / 3 - 1) * step;

                        if (0 <= nx && nx < newWidth && 0 <= ny && ny < newHeight)
                        {
                            var j = (nx + ny * newWidth) * 4;
                            var oldBestFirstX = ping[j];
                            var oldBestFirstY = ping[j + 1];
                            var oldBestSecondX = ping[j + 2];
                            var oldBestSecondY = ping[j + 3];

                            if (!float.IsNaN(oldBestFirstX) && !float.IsNaN(oldBestFirstY))
                            {
                                var dx = x - oldBestFirstX;
                                var dy = y - oldBestFirstY;
                                var d = dx * dx + dy * dy;

                                if (d < bestFirstDistance)
                                {
                                    bestFirstX = oldBestFirstX;
                                    bestFirstY = oldBestFirstY;
                                    bestFirstDistance = d;
                                }
                            }

                            if (!float.IsNaN(oldBestSecondX) && !float.IsNaN(oldBestSecondY))
                            {
                                var dx = x - oldBestSecondX;
                                var dy = y - oldBestSecondY;
                                var d = dx * dx + dy * dy;

                                if (d < bestSecondDistance)
                                {
                                    bestSecondX = oldBestSecondX;
                                    bestSecondY = oldBestSecondY;
                                    bestSecondDistance = d;
                                }
                            }
                        }
                    }

                    pong[i] = bestFirstX;
                    pong[i + 1] = bestFirstY;
                    pong[i + 2] = bestSecondX;
                    pong[i + 3] = bestSecondY;
                }
            }

            var swap = ping;
            ping = pong;
            pong = swap;
            step >>= 1;
        }

        var bytes = new byte[newWidth * newHeight * 4];

        // Merge the two distance transforms together to get an RGBA signed distance field
        for (int y = 0, i = 0; y < newHeight; y++)
        {
            for (var x = 0; x < newWidth; x++, i += 4)
            {
                var firstX = ping[i] - x;
                var firstY = ping[i + 1] - y;
                var firstD = (float)Math.Sqrt(firstX * firstX + firstY * firstY);

                var secondX = ping[i + 2] - x;
                var secondY = ping[i + 3] - y;
                var secondD = (float)Math.Sqrt(secondX * secondX + secondY * secondY);

                bytes[i] = bytes[i + 1] = bytes[i + 2] = float.IsNaN(firstD)
                    ? (byte)0
                    : (byte)(firstD > secondD
                        ? Math.Max(0, Math.Round(255 * (0.5 - 0.5 * (firstD - 0.5) / (falloff + 0.5))))
                        : Math.Min(255, Math.Round(255 * (0.5 + 0.5 * (secondD - 0.5) / (falloff + 0.5))))
                );
                bytes[i + 3] = 255;
            }
        }

        var imageInfo = new RenderedCodePoint()
        {
            Image = new Image(newWidth, newHeight),
            OriginX = image.OriginX + falloff,
            OriginY = image.OriginY + falloff,
            Advance = image.Advance
        };

        Array.Copy(bytes, imageInfo.Image.Pixels, bytes.Length);

        return imageInfo;
    }
    
    
    private static int GetMinX(Image image, Color bg)
    {
        var xMin = 0;
        var xMax = image.Width;

        while (xMin + 1 < xMax)
        {
            for (var y = 0; y < image.Height; y++)
            {
                if (!IsSameColor(bg, xMin, y, image))
                {
                    return xMin;
                }
            }

            xMin++;
        }        

        return xMin;
    }


    private static int GetMinY(Image image, Color bg)
    {
        var yMin = 0;
        var yMax = image.Height;

        while (yMin + 1 < yMax)
        {
            for (var x = 0; x < image.Width; x++)
            {
                if (!IsSameColor(bg, x, yMin, image))
                {
                    return yMin;
                }
            }

            yMin++;
        }

        return yMin;
    }


    private static int GetMaxX(Image image, Color bg, int xmin)
    {
        var xMax = image.Width;

        while (xMax - 1 > xmin)
        {
            for (var y = 0; y < image.Height; y++)
            {
                if (!IsSameColor(bg, xMax - 1, y, image))
                {
                    return xMax;
                }
            }

            xMax--;
        }

        return xMax;
    }


    private static int GetMaxY(Image image, Color bg, int ymin)
    {
        var yMax = image.Height;

        while (yMax - 1 > ymin)
        {
            for (var x = 0; x < image.Width; x++)
            {
                if (!IsSameColor(bg, x, yMax - 1, image))
                {
                    return yMax;
                }
            }

            yMax--;
        }

        return yMax;
    }
    
    
    private static bool IsSameColor(Color color, int x, int y, Image image)
    {
        var i = (x + y * image.Width) * 4;
        var data = image.Pixels;

        return (
            Math.Abs(data[i]     - color.R) < 2 &&
            Math.Abs(data[i + 1] - color.G) < 2 &&
            Math.Abs(data[i + 2] - color.B) < 2 &&
            Math.Abs(data[i + 3] - color.A) < 2);
    }
    
    
    
    private static string GenerateCodePoints(string customCharacters)
    {
        return string.IsNullOrWhiteSpace(customCharacters)
            ? GenerateCodePointsForAscii()
            : GenerateCodePointsFromText(customCharacters);
    }


    private static string GenerateCodePointsForAscii()
    {
        var codePointsSb = new StringBuilder();

        for (var i = 0x20; i < 0x7F; i++)
        {
            codePointsSb.Append((char)i);
        }

        return codePointsSb.ToString();
    }


    private static string GenerateCodePointsFromText(string text)
    {
        var codePointsMap = new Dictionary<char, char>();

        foreach (var c in text)
        {
            if (codePointsMap.ContainsKey(c))
            {
                continue;
            }

            codePointsMap.Add(c, c);
        }

        var codePointsSb = new StringBuilder();

        foreach (var codePoint in codePointsMap.Values)
        {
            codePointsSb.Append(codePoint);
        }

        return codePointsSb.ToString();
    }
}
