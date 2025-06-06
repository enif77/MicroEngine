/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Physics;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class PhysicsEngine
{
    private readonly List<PhysicsObject> _objects;
    private CancellationTokenSource? _cancellationTokenSource;
    
    private int _updatesPerSecond = 60; // Default to 60 updates per second

    // Events for engine lifecycle
    public event Action? EngineStarted;
    public event Action? EngineStopped;

     // Events for update lifecycle
    public event Action? UpdateStarted;
    public event Action? UpdateCompleted;
    public event Action? UpdateSkipped;


    public PhysicsEngine()
    {
        _objects = new List<PhysicsObject>();
    }


    public void AddObject(PhysicsObject obj)
    {
        lock (_objects)
        {
            _objects.Add(obj);
        }
    }


    public void RemoveObject(PhysicsObject obj)
    {
        lock (_objects)
        {
            _objects.Remove(obj);
        }
    }


    public void Start(int updatesPerSecond)
    {
        if (_cancellationTokenSource != null)
        {
            return; // Already running
        }

        // Clamp updatesPerSecond to the range [1, 100]
        _updatesPerSecond = Math.Clamp(updatesPerSecond, 1, 100);
        _cancellationTokenSource = new CancellationTokenSource();

        Task.Run(() => Update(_cancellationTokenSource.Token), _cancellationTokenSource.Token);
        
        EngineStarted?.Invoke();
    }


    public void Stop()
    {
        if (_cancellationTokenSource == null)
        {
            return; // Not running
        }

        _cancellationTokenSource.Cancel();
        _cancellationTokenSource = null;

        EngineStopped?.Invoke();
    }


    private async Task Update(CancellationToken cancellationToken)
    {
        var stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();

         // Interval in milliseconds
        var updateInterval = 1000.0 / _updatesPerSecond;

        // When we wish to update next.
        var nextUpdateTime = stopwatch.ElapsedMilliseconds;

        while (!cancellationToken.IsCancellationRequested)
        {
            // Calculate the current time
            var currentTime = stopwatch.ElapsedMilliseconds;

            // If the current time is less than the next update time, skip this iteration
            if (currentTime < nextUpdateTime)
            {
                try
                {
                    await Task.Delay((int)(nextUpdateTime - currentTime), cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    break; // Task was canceled
                }

                continue;
            }

            // If the next update time is in the past, skip the update and adjust the next update time
            if (currentTime > nextUpdateTime + updateInterval)
            {
                nextUpdateTime += (long)updateInterval;
                
                // Invoke UpdateSkipped event
                UpdateSkipped?.Invoke();

                continue;
            }

            // Calculate deltaTime in seconds
            var deltaTime = (float)(updateInterval / 1000.0);

            // Invoke UpdateStarted event
            UpdateStarted?.Invoke();

            lock (_objects)
            {
                // Update all physics objects
                foreach (var obj in _objects)
                {
                    obj.Update(deltaTime);
                }
                
                // Perform collision detection
                PerformCollisionDetection();
            }

            // Invoke UpdateCompleted event
            UpdateCompleted?.Invoke();

            // Increment the next update time by the update interval
            nextUpdateTime += (long)updateInterval;
        }

        stopwatch.Stop();
    }
    
    
    private void PerformCollisionDetection()
    {
        for (var i = 0; i < _objects.Count; i++)
        {
            var objA = _objects[i];
            if (objA.CollisionObject == null)
            {
                continue;
            }

            for (var j = i + 1; j < _objects.Count; j++)
            {
                var objB = _objects[j];
                if (objB.CollisionObject == null)
                {
                    continue;
                }

                if (objA.CollisionObject.CheckCollision(objB.CollisionObject))
                {
                    objA.CollisionObject.OnCollision(objB.CollisionObject);
                    objB.CollisionObject.OnCollision(objA.CollisionObject);
                }
            }
        }
    }
}
