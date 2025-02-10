#version 460 core
in vec3 pos;
in vec3 col;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

out vec3 Color;

void main()
{
    gl_Position = vec4(pos, 1.0) * model * view * projection;
    Color = col;
}
