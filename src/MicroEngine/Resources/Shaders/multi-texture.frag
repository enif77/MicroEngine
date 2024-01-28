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
    for (int i = 0; i < NR_SAMPLERS; i++)
    {
        if (TexId == i)
        {
            FragColor = texture(samplers[i].tex, TexCoords);

            break;
        }
    }
    
    //FragColor = texture(samplers[0].tex, TexCoords);
}
