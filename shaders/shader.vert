#version 460 core
in vec3 pos;
//in vec3 color;

out vec3 ourColor;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
    gl_Position = vec4(pos, 1.0) * model * view * projection;
    ourColor = vec3(1.0, 1.0, 1.0);
}