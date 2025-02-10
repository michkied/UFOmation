#version 460 core
in vec3 pos;
in vec2 tex;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

out vec2 TexCoord;

void main()
{
    gl_Position = vec4(pos, 1.0) * model * view * projection;
    TexCoord = tex;
}
