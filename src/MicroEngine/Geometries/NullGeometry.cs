/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Geometries;

using OpenTK.Mathematics;

/// <summary>
/// Represents a null geometry. This geometry is used for scene objects without geometry.
/// </summary>
public class NullGeometry()
    : GeometryBase(Array.Empty<float>(), Array.Empty<uint>(), false)
{
    public override int VertexDataStride => 0;
    
    
    protected override void BuildImpl(IShader forShader)
    {
        // Nothing to do here.
    }
    
    
    protected override void RenderImpl()
    {
        // Nothing to do here.
    }
    
    
    public override IEnumerable<Vector3> GetVertices()
    {
        yield break;
    }
    
    
    public override IEnumerable<int> GetVertexData()
    {
        yield break;
    }
}
