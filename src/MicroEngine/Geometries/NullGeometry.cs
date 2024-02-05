/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Geometries;

/// <summary>
/// Represents a null geometry. This geometry is used for scene objects without geometry.
/// </summary>
public class NullGeometry()
    : GeometryBase(Array.Empty<float>(), Array.Empty<uint>())
{
    public override void Build(IShader forShader)
    {
        // Nothing to do here.
    }
}
