/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.SceneObjects;

using OpenTK.Mathematics;

using MicroEngine.Geometries;

/// <summary>
/// An axis-aligned boundary box, that is automatically changing its size, when its parent object changes its size or rotation.
/// </summary>
public sealed class DynamicAxisAlignedBoundaryBox : RenderableSceneObject, IAxisAlignedBoundaryBox
{
    // The base constructor requires a geometry, but we will update it in our constructor.
    private static readonly IGeometry NullGeometry = new NullGeometry();
    
    public Vector3 Min { get; private set; } = new(-0.5f, -0.5f, -0.5f);
    public Vector3 Max { get; private set; } = new(0.5f, 0.5f, 0.5f);

    
    public DynamicAxisAlignedBoundaryBox()
        : base(NullGeometry)
    {
        Geometry = new SimpleIndexedLinesGeometry(
            _vertices,
            [
                0, 1,  // Front
                1, 2,
                2, 3,
                3, 0,
   
                4, 5,  // Back
                5, 6,
                6, 7,
                7, 4,
   
                0, 4,  // Connections
                1, 5,
                2, 6,
                3, 7
            ],
            true
        );
    }
    
    
    // Buffer for the 8 vertices of the box.
    private readonly float[] _vertices =
    [
        -0.5f,  0.5f,  0.5f,
         0.5f,  0.5f,  0.5f,
         0.5f, -0.5f,  0.5f,
        -0.5f, -0.5f,  0.5f,
            
        -0.5f,  0.5f, -0.5f,
         0.5f,  0.5f, -0.5f,
         0.5f, -0.5f, -0.5f,
        -0.5f, -0.5f, -0.5f
    ];
    

    public override void Update(float deltaTime)
    {
        if (NeedsModelMatrixUpdate)
        {
            // We are updating the geometry using world coordinates,
            // so we do not need any transformations here.
            // Note: This breaks the scene graph hierarchy, but we need to do it this way.
            ModelMatrix = Matrix4.Identity;
            
            var parent = Parent ?? throw new InvalidOperationException("Parent is not set.");
            
            // We need this to calculate Min/Max in world space.
            var worldMatrix = parent.ModelMatrix;
            
            // Min/max in world space.
            var minX = float.MaxValue;
            var minY = float.MaxValue;
            var minZ = float.MaxValue;
            
            var maxX = float.MinValue;
            var maxY = float.MinValue;
            var maxZ = float.MinValue;

            foreach (var vertexIndex in parent.Geometry.GetVertexData())
            {
                var x = parent.Geometry.VertexData[vertexIndex];
                var y = parent.Geometry.VertexData[vertexIndex + 1];
                var z = parent.Geometry.VertexData[vertexIndex + 2];
                
                //var transformedVertex = Vector3.TransformPosition(vertex, worldMatrix)
                var transformedVertexX = (float) (x * (double) worldMatrix.Row0.X + y * (double) worldMatrix.Row1.X + z * (double) worldMatrix.Row2.X) + worldMatrix.Row3.X;
                var transformedVertexY = (float) (x * (double) worldMatrix.Row0.Y + y * (double) worldMatrix.Row1.Y + z * (double) worldMatrix.Row2.Y) + worldMatrix.Row3.Y;
                var transformedVertexZ = (float) (x * (double) worldMatrix.Row0.Z + y * (double) worldMatrix.Row1.Z + z * (double) worldMatrix.Row2.Z) + worldMatrix.Row3.Z;
                
                if (transformedVertexX < minX)
                {
                    minX = transformedVertexX;
                }
                if (transformedVertexY < minY)
                {
                    minY = transformedVertexY;
                }
                if (transformedVertexZ < minZ)
                {
                    minZ = transformedVertexZ;
                }
                
                if (transformedVertexX > maxX)
                {
                    maxX = transformedVertexX;
                }
                if (transformedVertexY > maxY)
                {
                    maxY = transformedVertexY;
                }
                if (transformedVertexZ > maxZ)
                {
                    maxZ = transformedVertexZ;
                }
            }
            
            // We have the min/max in world space.
            Min = new Vector3(minX, minY, minZ);
            Max = new Vector3(maxX, maxY, maxZ);

            if (IsVisible)
            {
                // Reusing a single array.
                _vertices[0] = minX;
                _vertices[1] = maxY;
                _vertices[2] = maxZ;
            
                _vertices[3] = maxX;
                _vertices[4] = maxY;
                _vertices[5] = maxZ;
            
                _vertices[6] = maxX;
                _vertices[7] = minY;
                _vertices[8] = maxZ;
            
                _vertices[9] = minX;
                _vertices[10] = minY;
                _vertices[11] = maxZ;
            
                _vertices[12] = minX;
                _vertices[13] = maxY;
                _vertices[14] = minZ;
            
                _vertices[15] = maxX;
                _vertices[16] = maxY;
                _vertices[17] = minZ;
            
                _vertices[18] = maxX;
                _vertices[19] = minY;
                _vertices[20] = minZ;
            
                _vertices[21] = minX;
                _vertices[22] = minY;
                _vertices[23] = minZ;
            
                Geometry.UpdateVertices(_vertices);
            }
            
            NeedsModelMatrixUpdate = false;
        }
        
        foreach (var child in Children)
        {
            child.Update(deltaTime);
        }
    }
}
