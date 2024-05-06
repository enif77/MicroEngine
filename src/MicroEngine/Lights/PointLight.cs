/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Lights;

using OpenTK.Mathematics;

using MicroEngine.Core;

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
}
