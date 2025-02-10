#version 460
in vec2 texCoord;
out vec4 FragColor;

//In order to calculate some basic lighting we need a few things per model basis, and a few things per fragment basis:
uniform sampler2D texture0; //The texture of the object.
uniform mat4 view;

in vec3 Normal; //The normal of the fragment is calculated in the vertex shader.
in vec3 FragPos; //The fragment position.

struct PointLight {
    vec3 position;

    float constant;
    float linear;
    float quadratic;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 objColor)
{
    vec3 lightDir = normalize(light.position - fragPos);

    //diffuse shading
    float diff = max(dot(normal, lightDir), 0.0);

    //specular shading
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(normalize(-fragPos), reflectDir), 0.0), 32);

    //attenuation
    float distance = length(light.position - fragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance +
    light.quadratic * (distance * distance));

    //combine results
    vec3 ambient = light.ambient * objColor;
    vec3 diffuse = light.diffuse * diff * objColor;
    vec3 specular = light.specular * spec * objColor;
    return (ambient + diffuse + specular) * attenuation;
}

float CalcFogFactor(vec3 fragPos)
{
    float fogIntensity = 0.1;
    if (fogIntensity == 0) return 1;
    float gradient = (fogIntensity * fogIntensity - 5 * fogIntensity + 6);
    float distance = length(fragPos);
    float fog = exp(-pow((distance / gradient), 4));
    fog = clamp(fog, 0.0, 1.0);
    return fog;
}

void main()
{
    vec3 objectColor = texture(texture0, texCoord).rgb; //The color of the object.
    vec3 lightColor = vec3(1.0, 1.0, 1.0); //The color of the light.

    PointLight light;
    light.position = vec3(vec4(0.0, 1.0, 0.0, 1.0) * view);
    light.constant = 1.0;
    light.linear = 0.09;
    light.quadratic = 0.032;
    light.ambient = vec3(0.2, 0.2, 0.2);
    light.diffuse = vec3(1.0, 1.0, 1.0);
    light.specular = vec3(0.5, 0.5, 0.5);

    vec3 result = CalcPointLight(light, normalize(Normal), FragPos, objectColor);
    float fogFactor = CalcFogFactor(FragPos);
    result = mix(vec3(1.0, 1.0, 1.0), result, fogFactor);

    FragColor = vec4(result, 1.0);
}