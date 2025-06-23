/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Tools.FontGen;

using System.Text.Json;

/// <summary>
/// Generates font textures for the game.
/// </summary>
internal static class Program
{
    static void Main(string[] args)
    {
        var fontConfiguration = new FontConfiguration();
        
        // The first argument is the path to the font file configuration.
        // If no argument is provided, the default font configuration will be used.
        if (args.Length > 0)
        {
            var fontConfigPath = args[0];
            if (File.Exists(fontConfigPath))
            {
                var fontConfigJson = File.ReadAllText(fontConfigPath);
                var fontConfigurationFromJson = JsonSerializer.Deserialize<FontConfiguration>(fontConfigJson);
                if (fontConfigurationFromJson != null)
                {
                    fontConfiguration = fontConfigurationFromJson;
                }
                else
                {
                    Console.WriteLine($"Failed to deserialize font configuration from {fontConfigPath}");
                    
                    return;
                }
            }
        }
        
        // // Save the font configuration to a JSON file for reference
        // File.WriteAllText(
        //     Path.Combine(fontConfiguration.OutputFolderPath, $"{fontConfiguration.FontName}_{fontConfiguration.FontSize}_config.json"),
        //     fontConfiguration.ToJson());

        var renderedFont = (fontConfiguration.GenerateFontGridImage)
            ? FontGridGenerator.Generate(fontConfiguration)
            : FontGenerator.Generate(fontConfiguration);
        
        var fontNameSuffix = (fontConfiguration.IsBold ? "_bold" : string.Empty) +
                              (fontConfiguration.IsItalic ? "_italic" : string.Empty) +
                              (fontConfiguration.UseDisplayTypeDistanceField ? "_display" : string.Empty) +
                              (fontConfiguration.GenerateFontGridImage ? "_grid" : string.Empty);
        
        var fontImagePath = Path.Combine(fontConfiguration.OutputFolderPath, $"{fontConfiguration.FontName}_{fontConfiguration.FontSize}{fontNameSuffix}.bmp");
        
        Console.WriteLine("Saving image to BMP: " + fontImagePath + ", Width: " + renderedFont.FontImage.Width + ", Height: " + renderedFont.FontImage.Height);
        
        if (fontConfiguration.GenerateRgbaFontImage)
        {
            Helpers.SaveImageToRgbaBmp(renderedFont.FontImage, fontImagePath);
        }
        else
        {
            Helpers.SaveImageToRgbBmp(renderedFont.FontImage, fontImagePath);
        }
        
        if (fontConfiguration.GenerateCSharpStructures)
        {
            var fontStructuresCs = """
                public class Character
                {
                    public string CodePoint { get; set; } = string.Empty;
                    public int X { get; set; }
                    public int Y { get; set; }
                    public int Width { get; set; }
                    public int Height { get; set; }
                    public int OriginX { get; set; }
                    public int OriginY { get; set; }
                    public float Advance { get; set; }
                }

                public class Font
                {
                    public string Name { get; set; } = string.Empty;
                    public int Size { get; set; }
                    public int Width { get; set; }
                    public int Height { get; set; }
                    public bool Bold { get; set; }
                    public bool Italic { get; set; }
                    public List<Character> Characters { get; set; } = new List<Character>();
                }
                """;

            File.WriteAllText(
                Path.Combine(fontConfiguration.OutputFolderPath, $"{fontConfiguration.FontName}_{fontConfiguration.FontSize}{fontNameSuffix}.cs"),
                fontStructuresCs);
        }
        
        File.WriteAllText(
            Path.Combine(fontConfiguration.OutputFolderPath, $"{fontConfiguration.FontName}_{fontConfiguration.FontSize}{fontNameSuffix}.json"),
            renderedFont.FontJson);
    }
}
