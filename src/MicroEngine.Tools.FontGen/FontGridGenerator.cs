/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Tools.FontGen;

using System.Text;

using SkiaSharp;

/// <summary>
/// Generates a 16x16 grid of characters in a specified font.
/// </summary>
public static class FontGridGenerator
{
    private const int UnsupportedCharReplacementCode = 127;

    /// <summary>
    /// Generates a 16x16 grid of characters in a specified font.
    /// </summary>
    /// <param name="fontConfiguration">Defines the font configuration including output path, font file path, font size, text color, background color, and optional styles.</param>
    /// <exception cref="ArgumentException">Thrown when image size is not a positive integer or not a multiple of 16, or when font size is not a positive integer.</exception>
    internal static RenderedFont Generate(FontConfiguration fontConfiguration)
    {
        if (fontConfiguration.ImageSize <= 0 || fontConfiguration.ImageSize % 16 != 0)
        {
            throw new ArgumentException("Image size must be a positive integer and a multiple of 16.");
        }
        
        if (fontConfiguration.FontSize <= 0)
        {
            throw new ArgumentException("Font size must be a positive integer.");
        }
        
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
        
        using var surface = SKSurface.Create(new SKImageInfo(fontConfiguration.ImageSize, fontConfiguration.ImageSize));
        var canvas = surface.Canvas;
        
        var charSize = fontConfiguration.ImageSize / 16;
        
        var horizontalCharShift = (charSize - fontConfiguration.FontSize) / 2;
        if (horizontalCharShift < 0)
        {
            horizontalCharShift = 0;
        }

        // TODO: Vertical alignment might need adjustment based on the font metrics.
        
        var verticalCharShift = (charSize - fontConfiguration.FontSize) / 2;
        if (verticalCharShift < 0)
        {
            verticalCharShift = 0;
        }
        //verticalCharShift = fontConfiguration.FontSize - verticalCharShift;
        
        // Fill background.
        canvas.Clear(new SKColor(fontConfiguration.BackgroundColor.R, fontConfiguration.BackgroundColor.G, fontConfiguration.BackgroundColor.B, fontConfiguration.BackgroundColor.A));

        var fontJsonSb = new StringBuilder(256);
        
        fontJsonSb.AppendLine("{");
        
        fontJsonSb.AppendLine($"  \"Name\": \"{fontConfiguration.FontName}\",");
        fontJsonSb.AppendLine($"  \"Size\": {fontConfiguration.FontSize},");
        fontJsonSb.AppendLine($"  \"Width\": {fontConfiguration.ImageSize},");
        fontJsonSb.AppendLine($"  \"Height\": {fontConfiguration.ImageSize},");
        fontJsonSb.AppendLine($"  \"Bold\": {(fontConfiguration.IsBold ? "true" : "false")},");
        fontJsonSb.AppendLine($"  \"Italic\": {(fontConfiguration.IsItalic ? "true" : "false")},");
        fontJsonSb.AppendLine($"  \"Characters\": [");
        
        // Generate the font grid.
        for (var r = 0; r < 16; r++)
        {
            for (var c = 0; c < 16; c++)
            {
                var ch = r * 16 + c;
                if (ch < 32)
                {
                    ch = UnsupportedCharReplacementCode;
                }
                
                var posX = c * charSize + horizontalCharShift;
                var posY = r * charSize + verticalCharShift;
                
                // Draw the character.
                var character = ((char)ch).ToString();
                if (fontConfiguration.HasStroke && fontConfiguration.StrokeThickness > 0)
                {
                    canvas.DrawText(character, posX, posY, SKTextAlign.Left, font, strokePaint);
                }
                canvas.DrawText(character, posX, posY, SKTextAlign.Left, font, paint);
                
                var codePointStr = Helpers.CodePointToString((char)ch)
                    .Replace("\\", "\\\\")
                    .Replace("\"", "\\\"");
                
                // Convert to Unicode escape sequence for non-printable characters.
                if (codePointStr[0] < 32 || (codePointStr[0] >= 127 && codePointStr[0] <= 160))
                {
                    codePointStr = $"\\u{(int)codePointStr[0]:X4}";
                }
                
                fontJsonSb.Append($"    {{ \"CodePoint\": \"{codePointStr}\",");
                fontJsonSb.Append($" \"X\": {c * charSize},");
                fontJsonSb.Append($" \"Y\": {r * charSize},");
                fontJsonSb.Append($" \"Width\": {charSize},");
                fontJsonSb.Append($" \"Height\": {charSize},");
                fontJsonSb.Append($" \"OriginX\": 0,");
                fontJsonSb.Append($" \"OriginY\": 0,");
                fontJsonSb.Append($" \"Advance\": {charSize}");
                
                if (r * c < 255)
                {
                    fontJsonSb.AppendLine("},");

                    continue;
                }

                fontJsonSb.AppendLine("}");
            }
        }
        
        fontJsonSb.AppendLine("  ]");
        fontJsonSb.AppendLine("}");
        
        // Get the rendered image from the surface.
        using var image = surface.Snapshot();

        var extractedPixels = Helpers.ExtractPixels(image);
        if (extractedPixels == null)
        {
            throw new InvalidOperationException("Failed to extract pixels from the generated font image.");
        }
        
        return new RenderedFont(
            extractedPixels,
            fontJsonSb.ToString());
    }
}

// https://cs.wikipedia.org/wiki/Windows-1250
// https://www.websiteplanet.com/blog/best-free-fonts/
