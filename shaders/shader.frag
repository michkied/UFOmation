#version 460

struct DirLight {
    vec3 direction;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

struct SpotLight {
    vec3 position;
    vec3 direction;
    float cutOff;
    float outerCutOff;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;

    float constant;
    float linear;
    float quadratic;
};

struct Material {
    sampler2D diffuse;
    sampler2D specular;
    float shininess;
};

in vec2 texCoord;
in vec3 Normal;
in vec3 FragPos;

uniform sampler2D texture0;
uniform mat4 view;
uniform DirLight dirLight;
uniform SpotLight spotLight;
#define NUM_UFO_LIGHTS 6
uniform SpotLight ufoLights[NUM_UFO_LIGHTS];
uniform Material material;
uniform float fogDensity;
uniform float specularStrength;

out vec4 FragColor;

vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir)
{
    vec3 lightDir = normalize(-light.direction);

    //diffuse shading
    float diff = max(dot(normal, lightDir), 0.0);

    //specular shading
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);

    //combine results
    vec3 ambient = light.ambient * vec3(texture(material.diffuse, texCoord));
    vec3 diffuse = light.diffuse * diff * vec3(texture(material.diffuse, texCoord));
    vec3 specular = light.specular * spec * vec3(texture(material.specular, texCoord)) * specularStrength;
    return (ambient + diffuse + specular);
}

vec3 CalcSpotLight(SpotLight light, vec3 normal, vec3 fragPos)
{
    //diffuse shading
    vec3 lightDir = normalize(light.position - FragPos);
    float diff = max(dot(normal, lightDir), 0.0);

    //specular shading
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(normalize(-fragPos), reflectDir), 0.0), material.shininess);

    //attenuation
    float distance = length(light.position - FragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance +
    light.quadratic * (distance * distance));

    //spotlight intensity
    float theta = dot(lightDir, normalize(-light.direction));
    float epsilon = light.cutOff - light.outerCutOff;
    float intensity = clamp((theta - light.outerCutOff) / epsilon, 0.0, 1.0);

    //combine results
    vec3 ambient = light.ambient * vec3(texture(material.diffuse, texCoord));
    vec3 diffuse = light.diffuse * diff * vec3(texture(material.diffuse, texCoord));
    vec3 specular = light.specular * spec * vec3(texture(material.specular, texCoord));
    ambient *= attenuation;
    diffuse *= attenuation * intensity;
    specular *= attenuation * intensity * specularStrength;
    return (ambient + diffuse + specular);
}

float CalcFogFactor(vec3 fragPos)
{
    if (fogDensity == 0) return 1;
    float gradient = (fogDensity * fogDensity - 5 * fogDensity + 6);
    float distance = length(fragPos);
    float fog = exp(-pow((distance / gradient), 4));
    fog = clamp(fog, 0.0, 1.0);
    return fog;
}

void main()
{
    vec3 result = CalcDirLight(dirLight, normalize(Normal), normalize(-FragPos));
    result += CalcSpotLight(spotLight, normalize(Normal), FragPos);
    for (int i = 0; i < NUM_UFO_LIGHTS; i++)
    {
        result += CalcSpotLight(ufoLights[i], normalize(Normal), FragPos);
    }
    float fogFactor = CalcFogFactor(FragPos);
    result = mix(vec3(1.0, 1.0, 1.0), result, fogFactor);

    FragColor = vec4(result, 1.0);
}