#version 330 core

struct Sampler
{
    sampler2D tex;
};

#define NR_SAMPLERS 16
uniform Sampler samplers[NR_SAMPLERS];

flat in int TexId;
in vec2 TexCoords;

out vec4 FragColor;

void main()
{
    FragColor = texture(samplers[TexId].tex, TexCoords);
}
