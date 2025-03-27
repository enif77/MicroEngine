/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Graphics;

using OpenTK.Graphics.OpenGL4;

public class NullTexture : ITexture
{
    public void Use(TextureUnit unit)
    {
        // Do nothing.
    }
}
