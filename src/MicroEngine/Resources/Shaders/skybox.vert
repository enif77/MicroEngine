#version 330 core

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec2 aTexCoords;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

out vec2 TexCoords;

void main()
{
    vec4 pos = vec4(aPos, 1.0) * model * view * projection;
    gl_Position = pos.xyww;  // This forces the z value to be 1.0.
    TexCoords = aTexCoords;
}
