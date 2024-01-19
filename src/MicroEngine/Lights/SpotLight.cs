/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Lights;

using OpenTK.Mathematics;

using MicroEngine.Core;

public class SpotLight : PointLight
{
    public override LightType LightType => LightType.Spot;
    
    public Vector3 Direction { get; set; }
    private string DirectionUniformName { get; }
    
    public float CutOff { get; set; }
    private string CutOffUniformName { get; }
    
    public float OuterCutOff { get; set; }
    private string OuterCutOffUniformName { get; }
    
    
    public SpotLight(int id)
        : base(id)
    {
        Direction = -Vector3.UnitZ;
        DirectionUniformName = $"lights[{Id}].direction";
        
        CutOff = MathF.Cos(MathHelper.DegreesToRadians(12.5f));
        CutOffUniformName = $"lights[{Id}].cutOff";
        
        OuterCutOff = MathF.Cos(MathHelper.DegreesToRadians(17.5f));
        OuterCutOffUniformName = $"lights[{Id}].outerCutOff";
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
                
                // This uses the pitch (X) and yaw (Y) angles to calculate the front vector.
                // We ignore the roll angle (Z), because our light is a circle..
                var front = new Vector3
                {
                    X = MathF.Cos(Rotation.X) * MathF.Cos(Rotation.Y),
                    Y = MathF.Sin(Rotation.X),
                    Z = MathF.Cos(Rotation.X) * MathF.Sin(Rotation.Y)
                };
                
                Direction = Vector3.Normalize(front);
            }
            
            ModelMatrix *= Matrix4.CreateRotationZ(Rotation.Z);
            ModelMatrix *= Matrix4.CreateRotationX(Rotation.X);
            ModelMatrix *= Matrix4.CreateRotationY(Rotation.Y);

            ModelMatrix *= Matrix4.CreateTranslation(Position);
            
            // We are already bound to the parent, so we don't need to multiply by the parent's model matrix.
            // if (Parent != null)
            // {
            //     ModelMatrix *= Parent.ModelMatrix;
            // }

            NeedsModelMatrixUpdate = false;
        }

        foreach (var child in Children)
        {
            child.Update(deltaTime);
        }
    }


    public override void SetUniforms(Shader shader)
    {
        base.SetUniforms(shader);
        
        shader.SetVector3(DirectionUniformName, Direction);
        shader.SetFloat(CutOffUniformName, CutOff);
        shader.SetFloat(OuterCutOffUniformName, OuterCutOff);
    }
}
