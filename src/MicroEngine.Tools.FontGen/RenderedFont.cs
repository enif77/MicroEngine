/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Tools.FontGen;

internal class RenderedFont
{
    public Image FontImage { get; }
    public string FontJson { get; }
    
    
    public RenderedFont(Image fontImage, string fontJson)
    {
        FontImage = fontImage;
        FontJson = fontJson;
    }
}
