/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.SceneObjects.Lights;

using OpenTK.Mathematics;

using MicroEngine.OGL;

public class PointLight : SceneObjectBase, ILight
{
    /// <summary>
    /// The index of the light in the shader.
    /// </summary>
    public int Id { get; }
    
    public virtual LightType LightType => LightType.Point;
    private string LightTypeUniformName { get; }
    
    // The Position property is inherited from SceneObjectBase class.
    private string PositionUniformName { get; }
    
    public Vector3 Ambient { get; set; }
    private string AmbientUniformName { get; }
    
    public Vector3 Diffuse { get; set; }
    private string DiffuseUniformName { get; }
    
    public Vector3 Specular { get; set; }
    private string SpecularUniformName { get; }
    
    public float Constant { get; set; }
    private string ConstantUniformName { get; }
    
    public float Linear { get; set; }
    private string LinearUniformName { get; }
    
    public float Quadratic { get; set; }
    private string QuadraticUniformName { get; }
    
    public float Range { get; set; }
    private string RangeUniformName { get; }


    public PointLight(int id)
    {
        Id = id;
        
        LightTypeUniformName = $"lights[{Id}].lightType";
        
        Position = new Vector3();
        PositionUniformName = $"lights[{Id}].position";
        
        Ambient = new Vector3(0.05f, 0.05f, 0.05f);
        AmbientUniformName = $"lights[{Id}].ambient";
        
        Diffuse = new Vector3(0.8f, 0.8f, 0.8f);
        DiffuseUniformName = $"lights[{Id}].diffuse";
        
        Specular = new Vector3(1.0f, 1.0f, 1.0f);
        SpecularUniformName = $"lights[{Id}].specular";
        
        Constant = 1.0f;
        ConstantUniformName = $"lights[{Id}].constant";
        
        Linear = 0.09f;
        LinearUniformName = $"lights[{Id}].linear";
        
        Quadratic = 0.032f;
        QuadraticUniformName = $"lights[{Id}].quadratic";
            
        Range = 100.0f;
        RangeUniformName = $"lights[{Id}].range";
    }
    
    
    public override void Update(float deltaTime)
    {
        if (NeedsModelMatrixUpdate)
        {
            if (Parent != null)
            {
                // If we have a parent, we are bound to it.
                Position = Parent.Position;
                Rotation = Parent.Rotation;
            }
            
            ModelMatrix *= Matrix4.CreateRotationZ(Rotation.Z);
            ModelMatrix *= Matrix4.CreateRotationX(Rotation.X);
            ModelMatrix *= Matrix4.CreateRotationY(Rotation.Y);

            ModelMatrix *= Matrix4.CreateTranslation(Position);
            
            // We are already bound to the parent, so we don't need to multiply by the parent's model matrix.

            NeedsModelMatrixUpdate = false;
        }

        foreach (var child in Children)
        {
            child.Update(deltaTime);
        }
    }

    
    public virtual void SetUniforms(GlslShader glslShader)
    {
        glslShader.SetInt(LightTypeUniformName, (int)LightType);
        
        glslShader.SetVector3(PositionUniformName, Position);
        glslShader.SetVector3(AmbientUniformName, Ambient);
        glslShader.SetVector3(DiffuseUniformName, Diffuse);
        glslShader.SetVector3(SpecularUniformName, Specular);
        
        glslShader.SetFloat(ConstantUniformName, Constant);
        glslShader.SetFloat(LinearUniformName, Linear);
        glslShader.SetFloat(QuadraticUniformName, Quadratic);
        
        glslShader.SetFloat(RangeUniformName, Range);
    }
    
    
    /// <summary>
    /// Sets the light attenuation constants for the specified light based on the specified range.
    /// </summary>
    /// <param name="forRange">A maximal distance this light influences a surface.</param>
    /// <exception cref="InvalidOperationException">If this method is called for an unsupported kind of light.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If the range parameter is not greater than zero.</exception>
    public void SetLightAttenuationConstants(float forRange)
    {
        var pointLight = this;
        if (pointLight == null)
        {
            throw new InvalidOperationException($"Only {nameof(PointLight)} supports attenuation constants.");
        }
        
        if (forRange <= 0.0f)
        {
            throw new ArgumentOutOfRangeException(nameof(forRange), forRange, "Range must be greater than to zero.");
        }
        
        // Clamp the range to the maximal range.
        if (forRange > AttenuationConstantsMap[0].Range)
        {
            forRange = AttenuationConstantsMap[0].Range;
        }
        
        // Find the attenuation constants for the beginning and the end of the range.
        var fromConstants = FindFromAttenuationConstants(forRange);
        if (Math.Abs(forRange - fromConstants.Range) < Tolerance)
        {
            pointLight.Range = forRange;
            pointLight.Constant = fromConstants.Constant;
            pointLight.Linear = fromConstants.Linear;
            pointLight.Quadratic = fromConstants.Quadratic;
            
            return;
        }
        
        var toConstants = FindToAttenuationConstants(forRange);
        if (Math.Abs(forRange - toConstants.Range) < Tolerance)
        {
            pointLight.Range = forRange;
            pointLight.Constant = toConstants.Constant;
            pointLight.Linear = toConstants.Linear;
            pointLight.Quadratic = toConstants.Quadratic;
            
            return;
        }
        
        // Interpolate the attenuation constants.
        var lerpFactor = (forRange - fromConstants.Range) / (toConstants.Range - fromConstants.Range);
        
        // Set the interpolated light attenuation constants.
        pointLight.Range = forRange;
        pointLight.Constant = MathHelper.Lerp(fromConstants.Constant, toConstants.Constant, lerpFactor);
        pointLight.Linear = MathHelper.Lerp(fromConstants.Linear, toConstants.Linear, lerpFactor);
        pointLight.Quadratic = MathHelper.Lerp(fromConstants.Quadratic, toConstants.Quadratic, lerpFactor);
    }

    
    private const float Tolerance = 0.0001f;
    
    private AttenuationConstants FindFromAttenuationConstants(float forRange)
    {
        var constants = AttenuationConstantsMap[AttenuationConstantsMap.Length - 1];
        
        for (var i = AttenuationConstantsMap.Length - 1; i >= 0; i--)
        {
            if (AttenuationConstantsMap[i].Range > forRange)
            {
                break;
            }
            
            constants = AttenuationConstantsMap[i];
        }

        return constants;
    }
    
    
    private AttenuationConstants FindToAttenuationConstants(float forRange)
    {
        var constants = AttenuationConstantsMap[0];
        
        for (var i = 0; i < AttenuationConstantsMap.Length; i++)
        {
            if (AttenuationConstantsMap[i].Range < forRange)
            {
                break;
            }
            
            constants = AttenuationConstantsMap[i];
        }

        return constants;
    }


    private class AttenuationConstants(float range, float constant, float linear, float quadratic)
    {
        public float Range { get; } = range;
        public float Constant { get; } = constant;
        public float Linear { get; } = linear;
        public float Quadratic { get; } = quadratic;
    }
    
    
    private static readonly AttenuationConstants[] AttenuationConstantsMap =
    [
        // https://opentk.net/learn/chapter2/5-light-casters.html
        // https://wiki.ogre3d.org/tiki-index.php?page=-Point+Light+Attenuation
        new (3250.0f, 1.0f, 0.0014f, 0.000007f),
        new ( 600.0f, 1.0f, 0.007f, 0.0002f),
        new ( 325.0f, 1.0f, 0.014f, 0.0007f),
        new ( 200.0f, 1.0f, 0.022f, 0.0019f),
        new ( 160.0f, 1.0f, 0.027f, 0.0028f),
        new ( 100.0f, 1.0f, 0.045f, 0.0075f),
        new (  65.0f, 1.0f, 0.07f, 0.017f),
        new (  50.0f, 1.0f, 0.09f, 0.032f),
        new (  32.0f, 1.0f, 0.14f, 0.07f),
        new (  20.0f, 1.0f, 0.22f, 0.20f),
        new (  13.0f, 1.0f, 0.35f, 0.44f),
        new (   7.0f, 1.0f, 0.7f, 1.8f),
        new (   0.0f, 1.0f, 1.0f, 2.0f)
    ];
}
