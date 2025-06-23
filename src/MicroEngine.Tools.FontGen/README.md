# FontGen

A simple tool to generate bitmap fonts for use in MicroEngine.

A font is defined by a JSON file defined by the `FontConfiguration` class. This json in passwd to the `FontGen` tool
as its first argument. The tool then generates a BMP file with the font bitmap and a JSON file with the font metrics.
It can optionally generate a C# file, that can be used to deserialize font metrics for use in MicroEngine.

The generated BMP file contains the font bitmap, where each character is represented by a rectangle of pixels.
The JSON file contains the font metrics, such as the character width, height, and offsets.
The C# file contains a class that can be used to deserialize the font metrics from the JSON file.

There are two major font image types. The default one contains packed characters. The result is as small image as possible.
This format can be used for as many characters as possible and is suitable for most use cases. The rendered text has
has minimal gaps between characters and each character has different width and height.

The second type is a grid font, where each character is represented by a square of pixels. This format is suitable for
fixed-width fonts, where each character has the same width and height. The grid font is useful for rendering text
in a grid-like layout, such as in a console or terminal. It is always a 16x16 grid of characters and the first
256 ASCII chars from UTF16 are used.

See example configuration files in the `Resources` folder.
