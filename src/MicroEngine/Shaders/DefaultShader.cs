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
            
            uniform mat4 model;
            uniform mat4 view;
            uniform mat4 projection;
            
            out vec3 Normal;
            out vec3 FragPos;
            out vec2 TexCoords;
            
            void main()
            {
                gl_Position = vec4(aPos, 1.0) * model * view * projection;
                FragPos = vec3(vec4(aPos, 1.0) * model);
                Normal = aNormal * mat3(transpose(inverse(model)));
                TexCoords = aTexCoords;
            }
            """,
            /*language=glsl*/
            """
            #version 330 core
            
            struct Material
            {
                sampler2D diffuse;
                sampler2D specular;
                float     shininess;
                int isTransparent;
                int transparencyThreshold;
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
            
                // This propertiy of a spotlight and a directional light.
                vec3  direction;
            
                // These are the properties of a spotlight, we need the direction of the light, the cutoff and the outer cutoff.
                float cutOff;
                float outerCutOff;
                
                float range;
            };
            
            #define NR_LIGHTS 16
            uniform Light lights[NR_LIGHTS];
            
            // This is the number of point lights we have, we need this to loop through the point lights in the main function.
            uniform int numLights;
            
            // Transparency effect.
            uniform int isTransparent;          // Turns on the transparency effect.
            uniform int transparencyThreshold;  // The higher the number, the less transparent the object will be.
            
            uniform Material material;
            uniform vec3 viewPos;
            
            in vec3 Normal;
            in vec3 FragPos;
            in vec2 TexCoords;
            
            out vec4 FragColor;
            
            // Here we have some function prototypes, these are the signatures the gpu will use to know how the
            // parameters of each light calculation is layed out.
            // We have one function per light, since this makes it so we dont have to take up to much space in the main function.
            vec3 CalcDirLight(Light light, vec3 normal, vec3 viewDir);
            vec3 CalcPointLight(Light light, vec3 normal, vec3 fragPos, vec3 viewDir);
            vec3 CalcSpotLight(Light light, vec3 normal, vec3 fragPos, vec3 viewDir);
            
            void main()
            {
                // Transparency effect.
                if (material.isTransparent != 0 && ((int(gl_FragCoord.x) + int(gl_FragCoord.y) + int(FragPos.z)) % material.transparencyThreshold) == 1)
                {
                    discard;
                }
                
                // Holes in the texture effect.
                float alpha = texture(material.diffuse, TexCoords).a;
                if (alpha < 0.1)
                {
                    discard;
                }
                
                vec3 norm = normalize(Normal);
                vec3 viewDir = normalize(viewPos - FragPos);
            
                vec3 result = vec3(0.0);
                for (int i = 0; i < clamp(numLights, 0, NR_LIGHTS); i++)
                {
                    Light light = lights[i];
                    if (light.lightType == LIGHT_TYPE_DIRECTIONAL)
                    {
                        result += CalcDirLight(light, norm, viewDir);
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
            
            
            vec3 CalcDirLight(Light light, vec3 normal, vec3 viewDir)
            {
                vec3 lightDir = normalize(-light.direction);
                
                // diffuse shading
                float diff = max(dot(normal, lightDir), 0.0);
                
                // specular shading
                vec3 reflectDir = reflect(-lightDir, normal);
                float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
                
                // combine results
                vec3 ambient  = light.ambient  * vec3(texture(material.diffuse, TexCoords));
                vec3 diffuse  = light.diffuse  * diff * vec3(texture(material.diffuse, TexCoords));
                vec3 specular = light.specular * spec * vec3(texture(material.specular, TexCoords));
                
                return (ambient + diffuse + specular);
            }
            
            
            vec3 CalcPointLight(Light light, vec3 normal, vec3 fragPos, vec3 viewDir)
            {
                float distance = length(light.position - fragPos);
                if (distance > light.range)
                {
                    return vec3(0.0);
                }
            
                vec3 lightDir = normalize(light.position - fragPos);
                
                // diffuse shading
                float diff = max(dot(normal, lightDir), 0.0);
                
                // specular shading
                vec3 reflectDir = reflect(-lightDir, normal);
                float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
                
                // attenuation
                //float distance    = length(light.position - fragPos);
                float attenuation = 1.0 / (light.constant + light.linear * distance +
                  light.quadratic * (distance * distance));
                
                // combine results
                vec3 ambient  = light.ambient  * vec3(texture(material.diffuse, TexCoords)) * attenuation;
                vec3 diffuse  = light.diffuse  * diff * vec3(texture(material.diffuse, TexCoords)) * attenuation;
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
            
                // diffuse shading
                float diff = max(dot(normal, lightDir), 0.0);
            
                // specular shading
                vec3 reflectDir = reflect(-lightDir, normal);
                float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
            
                // attenuation
                //float distance    = length(light.position - fragPos);
                float attenuation = 1.0 / (light.constant + light.linear * distance +
                light.quadratic * (distance * distance));
            
                // spotlight intensity
                float theta     = dot(lightDir, normalize(-light.direction));
                float epsilon   = light.cutOff - light.outerCutOff;
                float intensity = clamp((theta - light.outerCutOff) / epsilon, 0.0, 1.0);
            
                // combine results
                vec3 ambient  = light.ambient  * vec3(texture(material.diffuse, TexCoords)) * attenuation;
                vec3 diffuse  = light.diffuse  * diff * vec3(texture(material.diffuse, TexCoords)) * (attenuation * intensity);
                vec3 specular = light.specular * spec * vec3(texture(material.specular, TexCoords)) * (attenuation * intensity);
            
                return (ambient + diffuse + specular);
            }
            """);
    }

    
    public int GetAttributeLocation(string name)
    {
        return _glslShader.GetAttribLocation(name);
    }
    
    
    public void Use(Scene scene, ISceneObject sceneObject)
    {
        var camera = scene.Camera;
        var material = sceneObject.Material;
        
        material.DiffuseMap.Use(TextureUnit.Texture0);
        material.SpecularMap.Use(TextureUnit.Texture1);
        
        _glslShader.Use();
        
        _glslShader.SetInt("material.diffuse", 0);
        _glslShader.SetInt("material.specular", 1);
        _glslShader.SetFloat("material.shininess", material.Shininess);
        _glslShader.SetInt("material.isTransparent", material.IsTransparent ? 1 : 0);
        _glslShader.SetInt("material.transparencyThreshold", material.TransparencyThreshold);
        
        _glslShader.SetMatrix4("view", camera.GetViewMatrix());
        _glslShader.SetMatrix4("projection", camera.GetProjectionMatrix());
        _glslShader.SetVector3("viewPos", camera.Position);
        _glslShader.SetMatrix4("model", sceneObject.ModelMatrix);
        
        /*
           Here we set all the uniforms for the 5/6 types of lights we have. We have to set them manually and index
           the proper PointLight struct in the array to set each uniform variable. This can be done more code-friendly
           by defining light types as classes and set their values in there, or by using a more efficient uniform approach
           by using 'Uniform buffer objects', but that is something we'll discuss in the 'Advanced GLSL' tutorial.
        */
        
        _glslShader.SetInt("numLights", scene.Lights.Count);
        
        foreach (var light in scene.Lights)
        {
            light.SetUniforms(_glslShader);
        }
    }
}
