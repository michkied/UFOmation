#version 460 core
in vec3 pos;
in vec3 normal;
in vec2 tex;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

out vec2 texCoord;
out vec3 FragPos;
out vec3 Normal;

void main()
{
    gl_Position = vec4(pos, 1.0) * model * view * projection;
    FragPos = vec3(vec4(pos, 1.0) * model * view);
    Normal = normal * mat3(transpose(inverse(model * view)));
    texCoord = tex;
}