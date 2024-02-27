#version 330 core

uniform sampler2D texture0;

in vec2 TexCoords;

out vec4 FragColor;

void main()
{
    //FragColor = texture(texture0, TexCoords);
    vec4 texColor = texture(texture0, TexCoords);
    if (texColor.a < 0.1)
    {
        discard;
    }

    FragColor = texColor;
}
