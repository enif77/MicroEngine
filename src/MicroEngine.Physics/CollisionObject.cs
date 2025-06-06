/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Physics;

using OpenTK.Mathematics;

public class CollisionObject
{
    public Vector3 Position { get; set; }
    public float Radius { get; set; }

    // Event triggered when a collision occurs
    public event Action<CollisionObject, CollisionObject>? CollisionDetected;


    // Method to check for collision with another object
    public bool CheckCollision(CollisionObject other)
    {
        var distance = Vector3.Distance(Position, other.Position);
        
        return distance <= (Radius + other.Radius);
    }


    // Invoke the collision event
    public void OnCollision(CollisionObject other)
    {
        CollisionDetected?.Invoke(this, other);
    }
}
