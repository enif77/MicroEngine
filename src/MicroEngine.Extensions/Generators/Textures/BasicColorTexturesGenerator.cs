/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Extensions.Generators.Textures;

using MicroEngine.Graphics;

/// <summary>
/// Generator of basic color textures.
/// </summary>
public static class BasicColorTexturesGenerator
{
    /// <summary>
    /// Generates a red texture.
    /// </summary>
    /// <param name="width">A width of the texture.</param>
    /// <param name="height">A height of the texture.</param>
    /// <returns>A texture.</returns>
    /// <exception cref="ArgumentOutOfRangeException">If the width or the height is less than zero.</exception>
    public static Image GenerateRedTexture(int width, int height)
    {
        CheckDimensions(width, height);
        
        var image = new Image(width, height);
        
        image.Fill(255, 0, 0);
        
        return image;
    }
    
    /// <summary>
    /// Generates a green texture.
    /// </summary>
    /// <param name="width">A width of the texture.</param>
    /// <param name="height">A height of the texture.</param>
    /// <returns>A texture.</returns>
    /// <exception cref="ArgumentOutOfRangeException">If the width or the height is less than zero.</exception>
    public static Image GenerateGreenTexture(int width, int height)
    {
        CheckDimensions(width, height);
        
        var image = new Image(width, height);
        
        image.Fill(0, 255, 0);
        
        return image;
    }
    
    /// <summary>
    /// Generates a blue texture.
    /// </summary>
    /// <param name="width">A width of the texture.</param>
    /// <param name="height">A height of the texture.</param>
    /// <returns>A texture.</returns>
    /// <exception cref="ArgumentOutOfRangeException">If the width or the height is less than zero.</exception>
    public static Image GenerateBluETexture(int width, int height)
    {
        CheckDimensions(width, height);
        
        var image = new Image(width, height);
        
        image.Fill(0, 0, 255);
        
        return image;
    }
    
    /// <summary>
    /// Generates a yellow texture.
    /// </summary>
    /// <param name="width">A width of the texture.</param>
    /// <param name="height">A height of the texture.</param>
    /// <returns>A texture.</returns>
    /// <exception cref="ArgumentOutOfRangeException">If the width or the height is less than zero.</exception>
    public static Image GenerateYellowTexture(int width, int height)
    {
        CheckDimensions(width, height);
        
        var image = new Image(width, height);
        
        image.Fill(255, 255, 0);
        
        return image;
    }
    
    /// <summary>
    /// Generates a pink texture.
    /// </summary>
    /// <param name="width">A width of the texture.</param>
    /// <param name="height">A height of the texture.</param>
    /// <returns>A texture.</returns>
    /// <exception cref="ArgumentOutOfRangeException">If the width or the height is less than zero.</exception>
    public static Image GeneratePinkTexture(int width, int height)
    {
        CheckDimensions(width, height);
        
        var image = new Image(width, height);
        
        image.Fill(255, 192, 203);
        
        return image;
    }
    
    /// <summary>
    /// Generates a cyan texture.
    /// </summary>
    /// <param name="width">A width of the texture.</param>
    /// <param name="height">A height of the texture.</param>
    /// <returns>A texture.</returns>
    /// <exception cref="ArgumentOutOfRangeException">If the width or the height is less than zero.</exception>
    public static Image GenerateCyanTexture(int width, int height)
    {
        CheckDimensions(width, height);
        
        var image = new Image(width, height);
        
        image.Fill(0, 255, 255);
        
        return image;
    }
    
    /// <summary>
    /// Generates a purple texture.
    /// </summary>
    /// <param name="width">A width of the texture.</param>
    /// <param name="height">A height of the texture.</param>
    /// <returns>A texture.</returns>
    /// <exception cref="ArgumentOutOfRangeException">If the width or the height is less than zero.</exception>
    public static Image GeneratePurpleTexture(int width, int height)
    {
        CheckDimensions(width, height);
        
        var image = new Image(width, height);
        
        image.Fill(255, 0, 255);
        
        return image;
    }
    
    /// <summary>
    /// Generates a orange texture.
    /// </summary>
    /// <param name="width">A width of the texture.</param>
    /// <param name="height">A height of the texture.</param>
    /// <returns>A texture.</returns>
    /// <exception cref="ArgumentOutOfRangeException">If the width or the height is less than zero.</exception>
    public static Image GenerateOrangeTexture(int width, int height)
    {
        CheckDimensions(width, height);
        
        var image = new Image(width, height);
        
        image.Fill(255, 128, 0);
        
        return image;
    }
    
    /// <summary>
    /// Generates a black texture.
    /// </summary>
    /// <param name="width">A width of the texture.</param>
    /// <param name="height">A height of the texture.</param>
    /// <returns>A texture.</returns>
    /// <exception cref="ArgumentOutOfRangeException">If the width or the height is less than zero.</exception>
    public static Image GenerateBlackTexture(int width, int height)
    {
        CheckDimensions(width, height);
        
        var image = new Image(width, height);
        
        image.Fill(0, 0, 0);
        
        return image;
    }
    
    /// <summary>
    /// Generates a white texture.
    /// </summary>
    /// <param name="width">A width of the texture.</param>
    /// <param name="height">A height of the texture.</param>
    /// <returns>A texture.</returns>
    /// <exception cref="ArgumentOutOfRangeException">If the width or the height is less than zero.</exception>
    public static Image GenerateWhiteTexture(int width, int height)
    {
        CheckDimensions(width, height);
        
        var image = new Image(width, height);
        
        image.Fill(255, 255, 255);
        
        return image;
    }
    
    /// <summary>
    /// Generates a gray texture.
    /// </summary>
    /// <param name="width">A width of the texture.</param>
    /// <param name="height">A height of the texture.</param>
    /// <returns>A texture.</returns>
    /// <exception cref="ArgumentOutOfRangeException">If the width or the height is less than zero.</exception>
    public static Image GenerateGrayTexture(int width, int height)
    {
        CheckDimensions(width, height);
        
        var image = new Image(width, height);
        
        image.Fill(200, 200, 200);
        
        return image;
    }
    

    private static void CheckDimensions(int width, int height)
    {
        if (width < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(width), "The width must be at least 1.");
        }
        
        if (height < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(height), "The height must be at least 1.");
        }
    }
}
