/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Geometries;

/// <summary>
/// Represents a null geometry. This geometry is used for scene objects without geometry.
/// </summary>
public class NullGeometry()
    : GeometryBase(Array.Empty<float>(), Array.Empty<uint>())
{
    protected override void BuildImpl(IShader forShader)
    {
        // Nothing to do here.
    }
    
    
    protected override void RenderImpl()
    {
        // Nothing to do here.
    }
}
