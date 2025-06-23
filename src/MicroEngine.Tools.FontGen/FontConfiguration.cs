/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Tools.FontGen;

using System.Text.Json;

/// <summary>
/// Font configuration for generating bitmap fonts.
/// </summary>
public class FontConfiguration
{
    /// <summary>
    /// The size of the generated font grid image in pixels.
    /// 1024x1024 pixels by default.
    /// </summary>
    public int ImageSize { get; set; } = 1024;
    
    /// <summary>
    /// Output folder path where the generated font files will be saved.
    /// The default is the current directory.
    /// </summary>
    public string OutputFolderPath { get; set; } = ".";
    
    /// <summary>
    /// An optional path to a font file. If specified, this font will be used for rendering.
    /// If not specified, A system font will be used.
    /// </summary>
    public string FontFilePath { get; set; } = string.Empty;
    
    /// <summary>
    /// A name of the font to be used for rendering.
    /// If not specified, the default font "Arial" will be used.
    /// </summary>
    public string FontName { get; set; } = "Arial";
    
    /// <summary>
    /// A size of the font in pixels. This determines the height of the characters in the font image.
    /// 16 pixels by default.
    /// </summary>
    public int FontSize { get; set; } = 16;
    
    /// <summary>
    /// A flag indicating whether the font should be bold.
    /// False by default.
    /// </summary>
    public bool IsBold { get; set; }
    
    /// <summary>
    /// A flag indicating whether the font should be italic.
    /// False by default.
    /// </summary>
    public bool IsItalic { get; set; }

    /// <summary>
    /// A color used for rendering the text of the font image.
    /// White by default.
    /// </summary>
    public Color TextColor { get; set; } = new Color(255, 255, 255);
    
    /// <summary>
    /// A color used for the background of the font image.
    /// Black by default.
    /// </summary>
    public Color BackgroundColor { get; set; } = new Color(0, 0, 0);

    /// <summary>
    /// A flag indicating whether the font should have a stroke.
    /// False by default.
    /// </summary>
    public bool HasStroke { get; set; }
    
    /// <summary>
    /// A color used for the stroke of the font image.
    /// Gray by default.
    /// </summary>
    public Color StrokeColor { get; set; } = new(127, 127, 127);
    
    /// <summary>
    /// The thickness of the stroke in pixels.
    /// 4 pixels by default.
    /// </summary>
    public int StrokeThickness { get; set; } = 4;
    
    /// <summary>
    /// If true, the stroke will be rounded or mitered.
    /// True by default.
    /// </summary>
    public bool IsRoundStroke { get; set; } = true;
    
    /// <summary>
    /// A flag indicating whether the resolution of the font image should be a power of 2.
    /// Used for the non-grid font image generation.
    /// False by default.
    /// </summary>
    public bool UseResolutionPowerOf2 { get; set; }

    /// <summary>
    /// A flag indicating whether to use distance field rendering for the font.
    /// Used for the non-grid font image generation.
    /// False by default.
    /// </summary>
    public bool UseDisplayTypeDistanceField { get; set; }
    
    /// <summary>
    /// Specifies the falloff distance for the distance field rendering.
    /// This value determines how quickly the distance field fades out.
    /// Used only if UseDisplayTypeDistanceField is true.
    /// 5 distance units by default.
    /// </summary>
    public int DistanceFieldFalloff { get; set; } = 5;
    
    /// <summary>
    /// Padding around each character in the font image.
    /// 1 pixel by default.
    /// </summary>
    public int Padding { get; set; } = 1;
    
    /// <summary>
    /// Optional custom characters to include in the font image.
    /// If empty, the font will use the ASCII character set.
    /// </summary>
    public string CustomCharacters { get; set; } = string.Empty;
    
    /// <summary>
    /// A flag indicating whether to generate C# structures for the font JSON file.
    /// False by default.
    /// </summary>
    public bool GenerateCSharpStructures { get; set; }
    
    /// <summary>
    /// A flag indicating whether to generate an RGBA font image. RGB font images are generated, if this is false.
    /// True by default.
    /// </summary>
    public bool GenerateRgbaFontImage { get; set; } = true;
    
    /// <summary>
    /// If true, generates a 16x16 grid of characters in the specified font.
    /// Uses the ImageSize property to determine the size of the grid image.
    /// False by default.
    /// </summary>
    public bool GenerateFontGridImage { get; set; }
    
    
    /// <summary>
    /// Converts this instance to JSON.
    /// </summary>
    /// <returns>JSON representation of this instance.</returns>
    public string ToJson()
    {
        return JsonSerializer.Serialize(
            this,
            JsonSerializerOptions);
    }

    
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        WriteIndented = true
    };
}
