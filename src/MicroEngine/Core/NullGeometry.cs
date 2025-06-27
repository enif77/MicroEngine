/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Core;

using OpenTK.Mathematics;

/// <summary>
/// Represents a null geometry. This geometry is used for scene objects without geometry.
/// </summary>
public sealed class NullGeometry : IGeometry
{
    private static readonly Lazy<NullGeometry> Singleton = new(() => new NullGeometry());

    /// <summary>
    /// Gets the singleton instance of the NullSceneObject.
    /// </summary>
    public static NullGeometry Instance => Singleton.Value;
    
    public float[] VertexData => [];
    public int VertexDataStride => 0;
    public uint[] Indices => [];
    public int IndicesCount => 0;
    
    public int VertexBufferObject
    {
        get => -1;
        set
        {
            // Do nothing, as this is a null geometry.
        }
    }
    
    public int ElementBufferObject
    {
        get => -1;
        set
        {
            // Do nothing, as this is a null geometry.
        }
    }
    
    public int VertexArrayObject
    {
        get => -1;
        set
        {
            // Do nothing, as this is a null geometry.
        }
    }
    
    public bool IsDynamic => false;
    public bool NeedsToBeBuild => false;
    
    
    private NullGeometry()
    {
        // Private constructor to prevent instantiation.
    }
    
    
    public void Build(IShader forShader)
    {
        // No operation for null geometry
    }

    
    public void UpdateVertices(float[] vertexData)
    {
        // No operation for null geometry
    }


    public IEnumerable<Vector3> GetVertices()
    {
        yield break;
    }
    
    
    public IEnumerable<int> GetVertexData()
    {
        yield break;
    }

    
    public void Render()
    {
        // No rendering for null geometry
    }
}
