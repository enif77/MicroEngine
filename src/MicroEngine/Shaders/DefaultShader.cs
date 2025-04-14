/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Shaders;

using OpenTK.Graphics.OpenGL4;
using MicroEngine.Core;

public class DefaultShader : IShader
{
    private readonly GlslShader _glslShader;

    public DefaultShader(IResourcesManager resourcesManager)
    {
        ArgumentNullException.ThrowIfNull(resourcesManager);

        _glslShader = new GlslShader(
            /*language=glsl*/
            """
            #version 330 core

            layout (location = 0) in vec3 aPos;
            layout (location = 1) in vec3 aNormal;
            layout (location = 2) in vec2 aTexCoords;
            layout (location = 3) in vec3 aTangent;
            layout (location = 4) in vec3 aBitangent;

            uniform mat4 model;
            uniform mat4 view;
            uniform mat4 projection;
            uniform mat4 lightSpaceMatrix;

            out vec3 Normal;
            out vec3 FragPos;
            out vec2 TexCoords;
            out mat3 TBN;
            out vec4 FragPosLightSpace;

            void main()
            {
                gl_Position = vec4(aPos, 1.0) * model * view * projection;
                FragPos = vec3(vec4(aPos, 1.0) * model);
                Normal = aNormal * mat3(transpose(inverse(model)));
                TexCoords = aTexCoords;

                vec3 T = normalize(aTangent * mat3(transpose(inverse(model))));
                vec3 B = normalize(aBitangent * mat3(transpose(inverse(model))));
                vec3 N = normalize(Normal);
                TBN = mat3(T, B, N);

                FragPosLightSpace = vec4(aPos, 1.0) * model * lightSpaceMatrix;
            }
            """,
            /*language=glsl*/
            """
            #version 330 core

            struct Material
            {
                sampler2D diffuse;
                sampler2D specular;
                sampler2D normalMap;
                float shininess;
                int opacityLevel;
                int opacityBias;
            };

            #define LIGHT_TYPE_DIRECTIONAL 1
            #define LIGHT_TYPE_POINT       2
            #define LIGHT_TYPE_SPOT        3

            struct Light
            {
                int lightType;
                vec3 position;
                vec3 ambient;
                vec3 diffuse;
                vec3 specular;
                float constant;
                float linear;
                float quadratic;
                vec3 direction;
                float cutOff;
                float outerCutOff;
                float range;
            };

            #define NR_LIGHTS 16
            uniform Light lights[NR_LIGHTS];
            uniform int numLights;
            uniform Material material;
            uniform vec3 viewPos;
            uniform bool useNormalMapping;
            uniform bool useShadowMapping;
            uniform sampler2D shadowMap;
            uniform int shadowCastingLightIndex;

            in vec3 Normal;
            in vec3 FragPos;
            in vec2 TexCoords;
            in mat3 TBN;
            in vec4 FragPosLightSpace;

            out vec4 FragColor;

            vec3 CalcDirLight(Light light, vec3 normal, vec3 viewDir, float shadow);
            vec3 CalcPointLight(Light light, vec3 normal, vec3 fragPos, vec3 viewDir);
            vec3 CalcSpotLight(Light light, vec3 normal, vec3 fragPos, vec3 viewDir);
            float ShadowCalculation(vec4 fragPosLightSpace, sampler2D shadowMap);

            void main()
            {
                float alpha = texture(material.diffuse, TexCoords).a;
                if (alpha < 0.1)
                {
                    discard;
                }

                if (material.opacityLevel > 1 && (
                    (int(gl_FragCoord.x) + int(gl_FragCoord.y) + material.opacityBias) % material.opacityLevel) == 1)
                {
                    discard;
                }

                vec3 norm;
                if (useNormalMapping)
                {
                    vec3 normal = texture(material.normalMap, TexCoords).rgb;
                    normal = normal * 2.0 - 1.0;
                    norm = normalize(TBN * normal);
                }
                else
                {
                    norm = normalize(Normal);
                }

                vec3 viewDir = normalize(viewPos - FragPos);
                vec3 result = vec3(0.0);

                for (int i = 0; i < clamp(numLights, 0, NR_LIGHTS); i++)
                {
                    Light light = lights[i];
                    float shadow = 0.0;
                    if (useShadowMapping && i == shadowCastingLightIndex && light.lightType == LIGHT_TYPE_DIRECTIONAL)
                    {
                        shadow = ShadowCalculation(FragPosLightSpace, shadowMap);
                    }

                    if (light.lightType == LIGHT_TYPE_DIRECTIONAL)
                    {
                        result += CalcDirLight(light, norm, viewDir, shadow);
                    }
                    else if (light.lightType == LIGHT_TYPE_POINT)
                    {
                        result += CalcPointLight(light, norm, FragPos, viewDir);
                    }
                    else if (light.lightType == LIGHT_TYPE_SPOT)
                    {
                        result += CalcSpotLight(light, norm, FragPos, viewDir);
                    }
                }

                FragColor = vec4(result, 1.0);
            }

            vec3 CalcDirLight(Light light, vec3 normal, vec3 viewDir, float shadow)
            {
                vec3 lightDir = normalize(-light.direction);
                float diff = max(dot(normal, lightDir), 0.0);
                vec3 reflectDir = reflect(-lightDir, normal);
                float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);

                vec3 ambient = light.ambient * vec3(texture(material.diffuse, TexCoords));
                vec3 diffuse = light.diffuse * diff * vec3(texture(material.diffuse, TexCoords));
                vec3 specular = light.specular * spec * vec3(texture(material.specular, TexCoords));
                return (ambient + (1.0 - shadow) * (diffuse + specular));
            }

            vec3 CalcPointLight(Light light, vec3 normal, vec3 fragPos, vec3 viewDir)
            {
                float distance = length(light.position - fragPos);
                if (distance > light.range)
                {
                    return vec3(0.0);
                }

                vec3 lightDir = normalize(light.position - fragPos);
                float diff = max(dot(normal, lightDir), 0.0);
                vec3 reflectDir = reflect(-lightDir, normal);
                float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);

                float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));
                vec3 ambient = light.ambient * vec3(texture(material.diffuse, TexCoords)) * attenuation;
                vec3 diffuse = light.diffuse * diff * vec3(texture(material.diffuse, TexCoords)) * attenuation;
                vec3 specular = light.specular * spec * vec3(texture(material.specular, TexCoords)) * attenuation;
                return (ambient + diffuse + specular);
            }

            vec3 CalcSpotLight(Light light, vec3 normal, vec3 fragPos, vec3 viewDir)
            {
                float distance = length(light.position - fragPos);
                if (distance > light.range)
                {
                    return vec3(0.0);
                }

                vec3 lightDir = normalize(light.position - fragPos);
                float diff = max(dot(normal, lightDir), 0.0);
                vec3 reflectDir = reflect(-lightDir, normal);
                float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);

                float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));
                float theta = dot(lightDir, normalize(-light.direction));
                float epsilon = light.cutOff - light.outerCutOff;
                float intensity = clamp((theta - light.outerCutOff) / epsilon, 0.0, 1.0);

                vec3 ambient = light.ambient * vec3(texture(material.diffuse, TexCoords)) * attenuation;
                vec3 diffuse = light.diffuse * diff * vec3(texture(material.diffuse, TexCoords)) * (attenuation * intensity);
                vec3 specular = light.specular * spec * vec3(texture(material.specular, TexCoords)) * (attenuation * intensity);
                return (ambient + diffuse + specular);
            }

            float ShadowCalculation(vec4 fragPosLightSpace, sampler2D shadowMap)
            {
                vec3 projCoords = fragPosLightSpace.xyz / fragPosLightSpace.w;
                projCoords = projCoords * 0.5 + 0.5;
                float closestDepth = texture(shadowMap, projCoords.xy).r;
                float currentDepth = projCoords.z;
                float shadow = currentDepth > closestDepth ? 1.0 : 0.0;
                return shadow;
            }
            """
        );
    }

    public int GetAttributeLocation(string name)
    {
        return _glslShader.GetAttribLocation(name);
    }

    public void Use(Scene scene, ISceneObject sceneObject)
    {
        var camera = scene.Camera;
        var material = sceneObject.Material;

        if (material.OpacityLevel > 1)
        {
            Renderer.DisableFaceCulling();
        }
        else
        {
            Renderer.EnableFaceCulling();
        }

        material.DiffuseMap.Use(TextureUnit.Texture0);
        material.SpecularMap.Use(TextureUnit.Texture1);
        material.NormalMap?.Use(TextureUnit.Texture2);
        scene.ShadowMap?.Use(TextureUnit.Texture3);

        _glslShader.Use();

        _glslShader.SetInt("material.diffuse", 0);
        _glslShader.SetInt("material.specular", 1);
        _glslShader.SetInt("material.normalMap", 2);
        _glslShader.SetFloat("material.shininess", material.Shininess);
        _glslShader.SetInt("material.opacityLevel", material.OpacityLevel);
        _glslShader.SetInt("material.opacityBias", material.OpacityBias);

        _glslShader.SetMatrix4("view", camera.GetViewMatrix());
        _glslShader.SetMatrix4("projection", camera.GetProjectionMatrix());
        _glslShader.SetVector3("viewPos", camera.Position);
        _glslShader.SetMatrix4("model", sceneObject.ModelMatrix);

        _glslShader.SetBool("useNormalMapping", material.NormalMap != null);
        _glslShader.SetBool("useShadowMapping", scene.ShadowMap != null);
        _glslShader.SetInt("shadowMap", 3);
        _glslShader.SetInt("shadowCastingLightIndex", 0); // First light casts shadows
        _glslShader.SetMatrix4("lightSpaceMatrix", scene.LightSpaceMatrix);

        _glslShader.SetInt("numLights", scene.Lights.Count);

        foreach (var light in scene.Lights)
        {
            light.SetUniforms(_glslShader);
        }
    }
}
