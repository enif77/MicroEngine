
#version 330 core

layout (location = 0) in vec3 aPos;
layout (location = 1) in float aTexId;
layout (location = 2) in vec2 aTexCoords;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

flat out int TexId;
out vec2 TexCoords;

void main()
{
    //gl_Position = vec4(aPos, 1.0) * model * view * projection;
    vec4 pos = vec4(aPos, 1.0) * model * view * projection;
    gl_Position = pos.xyww;  // This forces the z value to be 1.0.
    
    TexId = int(aTexId);
    TexCoords = aTexCoords;
}
