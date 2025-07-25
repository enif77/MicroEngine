/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.OGL;

using OpenTK.Graphics.OpenGL4;

using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

// A helper class, much like Shader, meant to simplify loading textures.
public class GlTexture : ITexture, IDisposable
{
    public readonly int Handle;


    public static GlTexture LoadFromRgbaBytes(byte[] bytes, int width, int height, TextureWrapMode wrapMode = TextureWrapMode.Repeat)
    {
        var handle = GL.GenTexture();

        // Bind the handle
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, handle);
        
        // Now that our pixels are prepared, it's time to generate a texture. We do this with GL.TexImage2D.
        // Arguments:
        //   The type of texture we're generating. There are various different types of textures, but the only one we need right now is Texture2D.
        //   Level of detail. We can use this to start from a smaller mipmap (if we want), but we don't need to do that, so leave it at 0.
        //   Target format of the pixels. This is the format OpenGL will store our image with.
        //   Width of the image
        //   Height of the image.
        //   Border of the image. This must always be 0; it's a legacy parameter that Khronos never got rid of.
        //   The format of the pixels, explained above. Since we loaded the pixels as RGBA earlier, we need to use PixelFormat.Rgba.
        //   Data type of the pixels.
        //   And finally, the actual pixels.
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, bytes);
        
        // Now that our texture is loaded, we can set a few settings to affect how the image appears on rendering.

        // First, we set the min and mag filter. These are used for when the texture is scaled down and up, respectively.
        // Here, we use Linear for both. This means that OpenGL will try to blend pixels, meaning that textures scaled too far will look blurred.
        // You could also use (amongst other options) Nearest, which just grabs the nearest pixel, which makes the texture look pixelated if scaled too far.
        // NOTE: The default settings for both of these are LinearMipmap. If you leave these as default but don't generate mipmaps,
        // your image will fail to render at all (usually resulting in pure black instead).
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        // Now, set the wrapping mode. S is for the X axis, and T is for the Y axis.
        // We set this to Repeat so that textures will repeat when wrapped. Not demonstrated here since the texture coordinates exactly match
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)wrapMode);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)wrapMode);

        // Next, generate mipmaps.
        // Mipmaps are smaller copies of the texture, scaled down. Each mipmap level is half the size of the previous one
        // Generated mipmaps go all the way down to just one pixel.
        // OpenGL will automatically switch between mipmaps when an object gets sufficiently far away.
        // This prevents moiré effects, as well as saving on texture bandwidth.
        // Here you can see and read about the morié effect https://en.wikipedia.org/wiki/Moir%C3%A9_pattern
        // Here is an example of mips in action https://en.wikipedia.org/wiki/File:Mipmap_Aliasing_Comparison.png
        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

        return new GlTexture(handle);
    }


    private GlTexture(int glHandle)
    {
        Handle = glHandle;
    }

    
    // Activate texture
    // Multiple textures can be bound, if your shader needs more than just one.
    // If you want to do that, use GL.ActiveTexture to set which slot GL.BindTexture binds to.
    // The OpenGL standard requires that there be at least 16, but there can be more depending on your graphics card.
    public void Use(TextureUnit unit)
    {
        GL.ActiveTexture(unit);
        GL.BindTexture(TextureTarget.Texture2D, Handle);
    }
    
    
    private bool _disposedValue = false;

    private void Dispose(bool disposing)
    {
        if (_disposedValue)
        {
            return;
        }

        GL.DeleteTexture(Handle);

        _disposedValue = true;
    }

    
    ~GlTexture()
    {
        if (_disposedValue == false)
        {
            Console.WriteLine("Texture: GPU Resource leak! Did you forget to call Dispose()?");
        }
    }


    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
