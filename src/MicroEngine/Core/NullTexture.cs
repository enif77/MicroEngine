/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Core;

using OpenTK.Graphics.OpenGL4;

public class NullTexture : ITexture
{
    public void Use(TextureUnit unit)
    {
        throw new NotSupportedException("Null texture cannot be used.");
    }
}
