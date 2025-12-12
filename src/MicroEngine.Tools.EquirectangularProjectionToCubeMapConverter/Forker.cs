/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Tools.EquirectangularProjectionToCubeMapConverter;

using System;
using System.Collections.Generic;
using System.Threading;

/// <summary>
/// Event arguments representing the completion of a parallel action.
/// </summary>
public class ParallelEventArgs : EventArgs
{
    internal ParallelEventArgs(object? state = null, Exception? exception = null)
    {
        State = state;
        Exception = exception;
    }

    /// <summary>
    /// The opaque state object that identifies the action (null otherwise).
    /// </summary>
    public object? State { get; }

    /// <summary>
    /// The exception thrown by the parallel action, or null if it completed without exception.
    /// </summary>
    public Exception? Exception { get; }
}


/// <summary>
/// Provides a caller-friendly wrapper around parallel actions.
/// </summary>
public sealed class Forker
{
    public Forker(int maxAllowed)
    {
        _maxAllowed = (maxAllowed <= 0) ? int.MaxValue : maxAllowed;
    }


    private readonly object _joinLock = new();
    private readonly Lock _eventLock = new();

    private int _running;
    private readonly int _maxAllowed;
    private readonly Queue<ThreadStart> _actionQueue = new();


    /// <summary>
    /// Raised when all operations have completed.
    /// </summary>
    public event EventHandler AllComplete
    {
        add
        {
            lock (_eventLock) { _allComplete += value; }
        }

        remove
        {
            lock (_eventLock) { _allComplete -= value; }
        }
    }

    private EventHandler? _allComplete;


    /// <summary>
    /// Raised when each operation completes.
    /// </summary>
    public event EventHandler<ParallelEventArgs> ItemComplete
    {
        add
        {
            lock (_eventLock) { _itemComplete += value; }
        }

        remove
        {
            lock (_eventLock) { _itemComplete -= value; }
        }
    }

    private EventHandler<ParallelEventArgs>? _itemComplete;

    
    /// <summary>
    /// Adds a callback to invoke when each operation completes.
    /// </summary>
    /// <returns>Current instance (for fluent API).</returns>
    public Forker OnItemComplete(EventHandler<ParallelEventArgs> handler)
    {
        ItemComplete += handler ?? throw new ArgumentNullException(nameof(handler));

        return this;
    }

    /// <summary>
    /// Adds a callback to invoke when all operations are complete.
    /// </summary>
    /// <returns>Current instance (for fluent API).</returns>
    public Forker OnAllComplete(EventHandler handler)
    {
        AllComplete += handler ?? throw new ArgumentNullException(nameof(handler));

        return this;
    }

    /// <summary>
    /// Waits for all operations to complete.
    /// </summary>
    public void Join()
    {
        _ = Join(-1);
    }

    /// <summary>
    /// Waits (with timeout) for all operations to complete.
    /// </summary>
    /// <returns>Whether all operations had completed before the timeout.</returns>
    public bool Join(int millisecondsTimeout)
    {
        lock (_joinLock)
        {
            if (CountRunning() == 0)
            {
                return true;
            }

            Thread.SpinWait(1); // Try our luck...

            return (CountRunning() == 0) || Monitor.Wait(_joinLock, millisecondsTimeout);
        }
    }

    /// <summary>
    /// Indicates the number of incomplete operations.
    /// </summary>
    /// <returns>The number of incomplete operations.</returns>
    public int CountRunning()
    {
        return Interlocked.CompareExchange(ref _running, 0, 0);
    }

    /// <summary>
    /// Enqueues an operation.
    /// </summary>
    /// <param name="action">The operation to perform.</param>
    /// <param name="state">An opaque object, allowing the caller to identify operations.</param>
    /// <returns>The current instance (for fluent API).</returns>
    public Forker Fork(ThreadStart action, object? state = null)
    {
        ArgumentNullException.ThrowIfNull(action);

        // Do not run infinite amount od tasks...
        if (CountRunning() >= _maxAllowed)
        {
            _actionQueue.Enqueue(action);

            return this;
        }

        StartAction(action, state);

        return this;
    }
    
    
    private void StartAction(ThreadStart action, object? state)
    {
        Interlocked.Increment(ref _running);

        ThreadPool.QueueUserWorkItem(_ =>
        {
            Exception? exception = null;
            try
            {
                action();
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            OnItemComplete(state, exception);
        });
    }
    
    
    private void OnItemComplete(object? state, Exception? exception)
    {
        var itemHandler = _itemComplete; // Don't need to lock.
        itemHandler?.Invoke(this, new ParallelEventArgs(state, exception));

        // Lower the count of running tasks.
        var running = Interlocked.Decrement(ref _running);

        // If we have something queued...
        if (_actionQueue.Count > 0)
        {
            // ... run it.
            StartAction(_actionQueue.Dequeue(), state);

            return;
        }

        // If the last running task just finished...
        if (running != 0)
        {
            return;
        }

        var allHandler = _allComplete; // Don't need to lock.
        allHandler?.Invoke(this, EventArgs.Empty);

        lock (_joinLock)
        {
            Monitor.PulseAll(_joinLock);
        }
    }
}
