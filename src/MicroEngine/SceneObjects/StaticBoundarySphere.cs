/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.SceneObjects;

using OpenTK.Mathematics;

using MicroEngine.Core;
using MicroEngine.Geometries;

/// <summary>
/// A boundary sphere, that is not automatically changing its size.
/// </summary>
public class StaticBoundarySphere : RenderableSceneObject
{
    /// <summary>
    /// World center of this boundary sphere.
    /// </summary>
    public Vector3 Center { get; private set; } = Vector3.Zero;
    
    
    public StaticBoundarySphere()
        : base(NullGeometry.Instance)
    {
        // 22.5 degrees splits circle to 16 segments.
        var circlePoints = GetCircularPoints(1.0f, Vector3.Zero, MathHelper.DegreesToRadians(22.5f));
       
        Geometry = new SimpleIndexedLinesGeometry(
            BuildVertices(circlePoints),
            BuildIndices(circlePoints.Count),
            true
        );
    }


    private uint[] BuildIndices(int circlePointsCount)
    {
        // var vertexIndices = new uint[]
        // {
        //     0,  1,   1,  2,   2,  3,   3,  4,
        //     4,  5,   5,  6,   6,  7,   7,  8,
        //     8,  9,   9, 10,  10, 11,  11, 12,
        //     12, 13,  13, 14,  14, 15,  15, 0
        // };
        //
        // // length * 3, because we have 3 circles.
        // var indices = new uint[vertexIndices.Length * 3];
        // for (var i = 0; i < vertexIndices.Length; i++)
        // {
        //     var vertexIndex = vertexIndices[i]; 
        //     
        //     indices[i] = vertexIndex;
        //     indices[i + vertexIndices.Length] = (uint)(vertexIndex + circlePointsCount);
        //     indices[i + vertexIndices.Length * 2] = (uint)(vertexIndex + circlePointsCount * 2);
        // }
        
        // count * 2 = number of indices.
        // count * 3 = number of circles.
        var indices = new uint[circlePointsCount * 2 * 3];
        
        for (var i = 0; i < circlePointsCount; i++)
        {
            var index = i * 2 * 3;
            var nextIndex = (i + 1) % circlePointsCount;
            
            // The X axis.
            indices[index] = (uint)i;
            indices[index + 1] = (uint)nextIndex;
            
            // The Y axis.
            indices[index + 2] = (uint)(i + circlePointsCount);
            indices[index + 3] = (uint)(nextIndex + circlePointsCount);
            
            // The Z axis.
            indices[index + 4] = (uint)(i + circlePointsCount * 2);
            indices[index + 5] = (uint)(nextIndex + circlePointsCount * 2);
        }
        
        return indices;
    }


    private float[] BuildVertices(IList<Vector2> circlePoints)
    {
        // count * XYZ * 3 circles.  
        var vertices = new float[circlePoints.Count * 3 * 3];
        var circleStride = circlePoints.Count * 3;
        for (var i = 0; i < circlePoints.Count; i++)
        {
            var point = circlePoints[i];
            var pointIndex = i * 3; 
            
            // The X axis.
            vertices[pointIndex] = 0.0f;
            vertices[pointIndex + 1] = point.Y;
            vertices[pointIndex + 2] = point.X;
            
            // The Y axis.
            vertices[circleStride + pointIndex] = point.X;
            vertices[circleStride + pointIndex + 1] = 0.0f;
            vertices[circleStride + pointIndex + 2] = point.Y;
            
            // The Z axis.
            vertices[circleStride * 2 + pointIndex] = point.X;
            vertices[circleStride * 2 + pointIndex + 1] = point.Y;
            vertices[circleStride * 2 + pointIndex + 2] = 0.0f;
        }

        return vertices;
    }


    private static List<Vector2> GetCircularPoints(float radius, Vector3 center, float angleInterval)
    {
        var points = new List<Vector2>();

        for (double interval = angleInterval; interval < 2.0 * Math.PI + 0.0000099; interval += angleInterval)
        {
            var x = center.X + (radius * Math.Cos(interval));
            var y = center.Y + (radius * Math.Sin(interval));

            points.Add(new Vector2((float)x, (float)y));
        }

        return points;
    }
    
    
    public override void Update(float deltaTime)
    {
        if (NeedsModelMatrixUpdate)
        {
            // We are ignoring the rotation.
            ModelMatrix = Matrix4.CreateScale(Scale);
            ModelMatrix *= Matrix4.CreateTranslation(Position);
            
            if (Parent != null)
            {
                // We are ignoring the parent's rotation.
                ModelMatrix *= Parent.ModelMatrix.ClearRotation();
            }
            
            // Calculate the world center.
            Center = Vector3.TransformPosition(Position, ModelMatrix);
            
            NeedsModelMatrixUpdate = false;
        }
        
        foreach (var child in Children)
        {
            child.Update(deltaTime);
        }
    }
}
