/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Extensions;

using OpenTK.Mathematics;

using MicroEngine.Lights;

/// <summary>
/// Lights related extensions.
/// </summary>
public static class LightExtensions
{
    /// <summary>
    /// Sets the light attenuation constants for the specified light based on the specified range.
    /// </summary>
    /// <param name="light">A PointLight or SpotLight instance.</param>
    /// <param name="forRange">A maximal distance this light influences a surface.</param>
    /// <exception cref="InvalidOperationException">If this method is called for an unsupported kind of light.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If the range parameter is not greater than zero.</exception>
    public static void SetLightAttenuationConstants(this ILight light, float forRange)
    {
        var pointLight = light as PointLight;
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
    
    private static AttenuationConstants FindFromAttenuationConstants(float forRange)
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
    
    
    private static AttenuationConstants FindToAttenuationConstants(float forRange)
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
        new AttenuationConstants(3250.0f, 1.0f, 0.0014f, 0.000007f),
        new AttenuationConstants( 600.0f, 1.0f, 0.007f, 0.0002f),
        new AttenuationConstants( 325.0f, 1.0f, 0.014f, 0.0007f),
        new AttenuationConstants( 200.0f, 1.0f, 0.022f, 0.0019f),
        new AttenuationConstants( 160.0f, 1.0f, 0.027f, 0.0028f),
        new AttenuationConstants( 100.0f, 1.0f, 0.045f, 0.0075f),
        new AttenuationConstants(  65.0f, 1.0f, 0.07f, 0.017f),
        new AttenuationConstants(  50.0f, 1.0f, 0.09f, 0.032f),
        new AttenuationConstants(  32.0f, 1.0f, 0.14f, 0.07f),
        new AttenuationConstants(  20.0f, 1.0f, 0.22f, 0.20f),
        new AttenuationConstants(  13.0f, 1.0f, 0.35f, 0.44f),
        new AttenuationConstants(   7.0f, 1.0f, 0.7f, 1.8f),
        new AttenuationConstants(   0.0f, 1.0f, 1.0f, 2.0f)
    ];
}
