#version 460 core
out vec4 FragColor;
in vec2 TexCoord;

uniform sampler2D mirrorTexture;

void main()
{
//    FragColor = vec4(1.0, 1.0, 1.0, 1.0);
    FragColor = texture(mirrorTexture, TexCoord);
}
