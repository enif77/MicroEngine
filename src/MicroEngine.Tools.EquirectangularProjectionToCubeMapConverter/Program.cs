/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Tools.EquirectangularProjectionToCubeMapConverter;

internal static class Program
{
    private const string PositiveZOrientationFaceName = "pz";
    private const string NegativeZOrientationFaceName = "nz";
    private const string PositiveXOrientationFaceName = "px";
    private const string NegativeXOrientationFaceName = "nx";
    private const string PositiveYOrientationFaceName = "py";
    private const string NegativeYOrientationFaceName = "ny";
    
    
    static int Main(string[] args)
    {
        Console.WriteLine("Equirectangular Projection Image to Cube Map Converter v1.0.0");
            
        var imagePath = args.Length > 0 ? args[0] : "";
        if (string.IsNullOrEmpty(imagePath))
        {
            Console.WriteLine("Usage: EquirectangularProjectionToCubeMapConverter.exe <image-path>");

            return 1;
        }

        var imageExtension = Path.GetExtension(imagePath);
        var renderedImagePathTemplate = string.IsNullOrEmpty(imageExtension)
            ? imagePath + "_{cube-map-face-name}.bmp"
            : imagePath.Replace(imageExtension, "_{cube-map-face-name}.bmp");
        
        var imageData = LoadImageFromBmp(imagePath);

        GenerateCubeFace(imageData, CubeMapFaceOrientation.NegativeX, renderedImagePathTemplate);
        GenerateCubeFace(imageData, CubeMapFaceOrientation.PositiveX, renderedImagePathTemplate);
        GenerateCubeFace(imageData, CubeMapFaceOrientation.NegativeY, renderedImagePathTemplate);
        GenerateCubeFace(imageData, CubeMapFaceOrientation.PositiveY, renderedImagePathTemplate);
        GenerateCubeFace(imageData, CubeMapFaceOrientation.NegativeZ, renderedImagePathTemplate);
        GenerateCubeFace(imageData, CubeMapFaceOrientation.PositiveZ, renderedImagePathTemplate);
        
        return 0;
    }
    
    
    private static void GenerateCubeFace(Image image, CubeMapFaceOrientation faceOrientation, string pathTemplate)
    {
        var convertor = new EquirectangularProjectionToCubeMapConverter();
                
        var face = convertor.RenderFace(
            image,
            faceOrientation,
            0,
            ImageInterpolationStrategy.NearestNeighbor,
            1024,
            1);

        SaveImageToRgbaBmp(
            face,
            pathTemplate.Replace("{cube-map-face-name}", GetCubeMapFaceName(faceOrientation)));
    }
    
    
    private static string GetCubeMapFaceName(CubeMapFaceOrientation cubeMapFaceOrientation)
    {
        return cubeMapFaceOrientation switch
        {
            CubeMapFaceOrientation.PositiveZ => PositiveZOrientationFaceName,
            CubeMapFaceOrientation.NegativeZ => NegativeZOrientationFaceName,
            CubeMapFaceOrientation.PositiveX => PositiveXOrientationFaceName,
            CubeMapFaceOrientation.NegativeX => NegativeXOrientationFaceName,
            CubeMapFaceOrientation.PositiveY => PositiveYOrientationFaceName,
            CubeMapFaceOrientation.NegativeY => NegativeYOrientationFaceName,
            
            _ => throw new ArgumentException("Unknown cube map face orientation: " + cubeMapFaceOrientation)
        };
    }
    
    
    private static Image LoadImageFromBmp(string filePath)
    {
        using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        using var reader = new BinaryReader(fs);
        
        // BMP Header
        if (reader.ReadUInt16() != 0x4D42) // 'BM' signature
        {
            throw new InvalidOperationException("Invalid file format.");
        }

        reader.BaseStream.Seek(18, SeekOrigin.Begin); // Move to width/height in the header
        var width = reader.ReadInt32(); // Image width
        var height = reader.ReadInt32(); // Image height

        reader.BaseStream.Seek(28, SeekOrigin.Begin); // Move to bits per pixel

        var bitsPerPixel = (int)reader.ReadUInt16();
                
        // BGR
        if (bitsPerPixel == 24) 
        {
            reader.BaseStream.Seek(54, SeekOrigin.Begin); // Move to pixel data

            var rowSize = width * 3; // Size of one row in bytes (RGB = 3 bytes per pixel)
            var paddingSize = (4 - (rowSize % 4)) % 4; // Padding to make row size divisible by 4

            var imageData = new byte[width * height * 4]; // RGBA format (4 bytes per pixel)

            // BMP stores pixel data bottom-to-top
            var ty = height - 1;
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var pixelIndex = (ty * width + x) * 4;

                    imageData[pixelIndex + 2] = reader.ReadByte(); // Blue component
                    imageData[pixelIndex + 1] = reader.ReadByte(); // Green component
                    imageData[pixelIndex + 0] = reader.ReadByte(); // Red component
                    imageData[pixelIndex + 3] = 255;               // Alpha component
                }

                ty--;
                        
                // Skip padding bytes
                reader.BaseStream.Seek(paddingSize, SeekOrigin.Current);
            }

            return new Image(width, height, imageData);
        }
                
        // BGRA
        if (bitsPerPixel == 32)
        {
            reader.BaseStream.Seek(54, SeekOrigin.Begin);
                    
            var imageData = new byte[width * height * 4]; 
                    
            var ty = 0;
            for (var y = height - 1; y >= 0; y--)
            {
                for (var x = 0; x < width; x++)
                {
                    var pixelIndex = (ty * width + x) * 4;

                    imageData[pixelIndex + 2] = reader.ReadByte(); // Blue component
                    imageData[pixelIndex + 1] = reader.ReadByte(); // Green component
                    imageData[pixelIndex + 0] = reader.ReadByte(); // Red component
                    imageData[pixelIndex + 3] = reader.ReadByte(); // Alpha component
                }
                        
                ty++;
            }

            return new Image(width, height, imageData);
        }
                
        throw new InvalidOperationException("Only 24-bit or 32-bit BMP files are supported.");
    }


    private static void SaveImageToRgbaBmp(Image image, string filePath)
    {
        using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        using var writer = new BinaryWriter(fs);
        
        var rowSize = image.Width * 4; // Size of one row in bytes (RGBA = 4 bytes per pixel)
        var paddingSize = (4 - (rowSize % 4)) % 4; // Padding to make the row size divisible by 4
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
        var imageData = image.Data;
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
    
    
    // private static void SaveImageToRgbBmp(Image image, string filePath)
    // {
    //     using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
    //     using var writer = new BinaryWriter(fs);
    //     
    //     var rowSize = image.Width * 3; // Size of one row in bytes (RGB = 3 bytes per pixel)
    //     var paddingSize = (4 - (rowSize % 4)) % 4; // Padding to make the row size divisible by 4
    //     var paddedRowSize = rowSize + paddingSize; // Total row size including padding
    //     var fileSize = 54 + paddedRowSize * image.Height; // BMP file size
    //
    //     // BMP Header
    //     writer.Write((ushort)0x4D42); // 'BM' signature
    //     writer.Write(fileSize); // File size
    //     writer.Write((ushort)0); // Reserved
    //     writer.Write((ushort)0); // Reserved
    //     writer.Write((uint)54); // Data offset
    //
    //     // DIB Header (BITMAPINFOHEADER)
    //     writer.Write((uint)40); // Header size
    //     writer.Write(image.Width); // Image width
    //     writer.Write(image.Height); // Image height
    //     writer.Write((ushort)1); // Planes
    //     writer.Write((ushort)24); // Bits per pixel
    //     writer.Write((uint)0); // Compression
    //     writer.Write(paddedRowSize * image.Height); // Image size
    //     writer.Write((int)0); // X pixels per meter
    //     writer.Write((int)0); // Y pixels per meter
    //     writer.Write((uint)0); // Total colors
    //     writer.Write((uint)0); // Important colors
    //         
    //     // Image data (bottom-to-top)
    //     var rgbPixels = ConvertImageToRgb(image);
    //     for (var y = image.Height - 1; y >= 0; y--)
    //     {
    //         for (var x = 0; x < image.Width; x++)
    //         {
    //             var pixelIndex = (y * image.Width + x) * 3;
    //                 
    //             writer.Write(rgbPixels[pixelIndex + 2]); // Blue component
    //             writer.Write(rgbPixels[pixelIndex + 1]); // Green component
    //             writer.Write(rgbPixels[pixelIndex]);     // Red component
    //         }
    //
    //         // Write padding bytes
    //         for (var p = 0; p < paddingSize; p++)
    //         {
    //             writer.Write((byte)0);
    //         }
    //     }
    // }
    //
    //
    // private static byte[] ConvertImageToRgb(Image rgbaImage)
    // {
    //     var rgbPixels = new byte[rgbaImage.Width * rgbaImage.Height * 3];
    //     var rgbaPixels = rgbaImage.Data;
    //     
    //     for (var i = 0; i < rgbaImage.Width * rgbaImage.Height; i++)
    //     {
    //         var rgbaIndex = i * 4;
    //         var rgbIndex = i * 3;
    //         
    //         rgbPixels[rgbIndex + 0] = rgbaPixels[rgbaIndex + 0]; // Red
    //         rgbPixels[rgbIndex + 1] = rgbaPixels[rgbaIndex + 1]; // Green
    //         rgbPixels[rgbIndex + 2] = rgbaPixels[rgbaIndex + 2]; // Blue
    //     }
    //
    //     return rgbPixels;
    // }
}
