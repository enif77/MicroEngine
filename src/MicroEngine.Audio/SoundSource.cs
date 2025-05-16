/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Audio;

using OpenTK.Audio.OpenAL;
using OpenTK.Mathematics;

/// <summary>
/// Represents a sound source.
/// </summary>
internal sealed class SoundSource : ISoundSource
{
    private bool _isInitialized;
    
    private ISoundBuffer? _attachedSoundBuffer;
    private readonly IList<ISoundBuffer> _queuedSoundBuffers = new List<ISoundBuffer>();
    

    public bool IsInitialized => _isInitialized;
    
    public int ALSourceId { get; private set; }
    
    public Vector3 Position
    {
        get
        {
            CheckInitialized();

            _ = AL.GetError();
            
            AL.GetSource(ALSourceId, ALSource3f.Position, out var position);
            
            var error = AL.GetError();
            if (error != ALError.NoError)
            {
                throw new InvalidOperationException($"Failed to get source position. OpenAL error: {error}");
            }
            
            return position;
        }
        
        set
        {
            CheckInitialized();
            
            _ = AL.GetError();
            
            AL.Source(ALSourceId, ALSource3f.Position, value.X, value.Y, value.Z);
            
            var error = AL.GetError();
            if (error != ALError.NoError)
            {
                throw new InvalidOperationException($"Failed to set source position. OpenAL error: {error}");
            }
        }
    }
    
    public float Volume
    {
        get
        {
            CheckInitialized();
            
            _ = AL.GetError();
            
            AL.GetSource(ALSourceId, ALSourcef.Gain, out var gain);
            
            var error = AL.GetError();
            if (error != ALError.NoError)
            {
                throw new InvalidOperationException($"Failed to get source volume. OpenAL error: {error}");
            }
            
            return gain;
        }
        
        set
        {
            CheckInitialized();
            
            _ = AL.GetError();
            
            AL.Source(ALSourceId, ALSourcef.Gain, value);
            
            var error = AL.GetError();
            if (error != ALError.NoError)
            {
                throw new InvalidOperationException($"Failed to set source volume. OpenAL error: {error}");
            }
        }
    }
    
    public bool IsLooping
    {
        get
        {
            CheckInitialized();    
            
            _ = AL.GetError();
            
            var isLoopingValue =  AL.GetSource(ALSourceId, ALSourceb.Looping);
            
            var error = AL.GetError();
            if (error != ALError.NoError)
            {
                throw new InvalidOperationException($"Failed to get source looping state. OpenAL error: {error}");
            }
            
            return isLoopingValue;
        }
        
        set
        {
            CheckInitialized();
            
            _ = AL.GetError();
            
            AL.Source(ALSourceId, ALSourceb.Looping, value);
            
            var error = AL.GetError();
            if (error != ALError.NoError)
            {
                throw new InvalidOperationException($"Failed to set source looping state. OpenAL error: {error}");
            }
        }
    }
    
    public ALSourceState State
    {
        get
        {
            CheckInitialized();
            
            _ = AL.GetError();
            
            AL.GetSource(ALSourceId, ALGetSourcei.SourceState, out var state);
            
            var error = AL.GetError();
            if (error != ALError.NoError)
            {
                throw new InvalidOperationException($"Failed to get source state. OpenAL error: {error}");
            }
            
            return (ALSourceState)state;
        }
    }
    
    
    public void Initialize()
    {
        if (_isInitialized)
        {
            return;
        }
        
        _ = AL.GetError();
        
        ALSourceId = AL.GenSource();
        
        var error = AL.GetError();
        if (error != ALError.NoError)
        {
            throw new InvalidOperationException($"Failed to create OpenAL source. OpenAL error: {error}");
        }
        
        _isInitialized = true;
    }
    
    
    public void AttachSoundBuffer(ISoundBuffer soundBuffer)
    {
        CheckInitialized();

        if (_queuedSoundBuffers.Count > 0)
        {
            throw new InvalidOperationException("Cannot attach a sound buffer while there are queued sound buffers.");
        }

        if (_attachedSoundBuffer != null)
        {
            DetachSoundBuffer();
        }
        
        if (soundBuffer == null)
        {
            throw new ArgumentNullException(nameof(soundBuffer), "Sound buffer cannot be null.");
        }
        
        if (!soundBuffer.IsInitialized)
        {
            throw new InvalidOperationException("Cannot attach uninitialized sound buffer.");
        }

        _ = AL.GetError();
        
        AL.Source(ALSourceId, ALSourcei.Buffer, soundBuffer.ALBufferId);
        
        var error = AL.GetError();
        if (error != ALError.NoError)
        {
            throw new InvalidOperationException($"Failed to attach sound buffer. OpenAL error: {error}");
        }
        
        _attachedSoundBuffer = soundBuffer;
    }

    
    public void DetachSoundBuffer()
    {
        CheckInitialized();
        
        Stop();
        
        _ = AL.GetError();
        
        AL.Source(ALSourceId, ALSourcei.Buffer, 0);
        
        var error = AL.GetError();
        if (error != ALError.NoError)
        {
            throw new InvalidOperationException($"Failed to detach sound buffer. OpenAL error: {error}");
        }
        
        _attachedSoundBuffer = null;
    }
    
    
    public void QueueSoundBuffer(ISoundBuffer soundBuffer)
    {
        CheckInitialized();
        
        if (_attachedSoundBuffer != null)
        {
            throw new InvalidOperationException("Cannot queue a sound buffer while one is attached.");
        }
        
        if (soundBuffer == null)
        {
            throw new ArgumentNullException(nameof(soundBuffer), "Sound buffer cannot be null.");
        }
        
        if (!soundBuffer.IsInitialized)
        {
            throw new InvalidOperationException("Cannot attach uninitialized sound buffer.");
        }
        
        if (_queuedSoundBuffers.Contains(soundBuffer))
        {
            throw new InvalidOperationException("The sound buffer is already queued.");
        }
        
        _ = AL.GetError();
        
        AL.SourceQueueBuffer(ALSourceId, soundBuffer.ALBufferId);
        
        var error = AL.GetError();
        if (error != ALError.NoError)
        {
            throw new InvalidOperationException($"Failed to queue sound buffer. OpenAL error: {error}");
        }
        
        _queuedSoundBuffers.Add(soundBuffer);
    }
    
    
    public int GetQueuedSoundBuffersCount()
    {
        CheckInitialized();
        
        _ = AL.GetError();
        
        AL.GetSource(ALSourceId, ALGetSourcei.BuffersQueued, out var count);
        
        var error = AL.GetError();
        if (error != ALError.NoError)
        {
            throw new InvalidOperationException($"Failed to get queued sound buffers count. OpenAL error: {error}");
        }
        
        return count;
    }
    
    
    public int GetProcessedSoundBuffersCount()
    {
        CheckInitialized();
        
        _ = AL.GetError();
        
        AL.GetSource(ALSourceId, ALGetSourcei.BuffersProcessed, out var count);
        
        var error = AL.GetError();
        if (error != ALError.NoError)
        {
            throw new InvalidOperationException($"Failed to get processed sound buffers count. OpenAL error: {error}");
        }
        
        return count;
    }
    
    
    public ISoundBuffer? UnqueueSoundBuffer()
    {
        CheckInitialized();
        
        _ = AL.GetError();
        
        var buffer = AL.SourceUnqueueBuffer(ALSourceId);
        
        var error = AL.GetError();
        if (error != ALError.NoError)
        {
            throw new InvalidOperationException($"Failed to unqueue sound buffer. OpenAL error: {error}");
        }
        
        var soundBuffer = _queuedSoundBuffers.FirstOrDefault(b => b.ALBufferId == buffer);
        if (soundBuffer != null)
        {
            _queuedSoundBuffers.Remove(soundBuffer);
        }
        
        return soundBuffer;
    }
    
    
    public void Play()
    {
        CheckInitialized();
        
        _ = AL.GetError();
        
        AL.SourcePlay(ALSourceId);
        
        var error = AL.GetError();
        if (error != ALError.NoError)
        {
            throw new InvalidOperationException($"Failed to play source. OpenAL error: {error}");
        }
    }
    

    public void Pause()
    {
        CheckInitialized();
        
        _ = AL.GetError();
        
        AL.SourcePause(ALSourceId);
        
        var error = AL.GetError();
        if (error != ALError.NoError)
        {
            throw new InvalidOperationException($"Failed to pause source. OpenAL error: {error}");
        }
    }
    

    public void Stop()
    {
        CheckInitialized();
        
        _ = AL.GetError();
        
        AL.SourceStop(ALSourceId);
        
        var error = AL.GetError();
        if (error != ALError.NoError)
        {
            throw new InvalidOperationException($"Failed to stop source. OpenAL error: {error}");
        }
    }
    

    public void Rewind()
    {
        CheckInitialized();
        
        _ = AL.GetError();
        
        AL.SourceRewind(ALSourceId);
        
        var error = AL.GetError();
        if (error != ALError.NoError)
        {
            throw new InvalidOperationException($"Failed to rewind source. OpenAL error: {error}");
        }
    }
    

    public void Destroy()
    {
        if (!_isInitialized)
        {
            return;
        }
        
        ReleaseUnmanagedResources();
        
        _isInitialized = false;
    }
    

    private void CheckInitialized()
    {
        if (!_isInitialized)
        {
            throw new InvalidOperationException("The OpenAL source is not initialized.");
        }
    }
    
    
    private void ReleaseUnmanagedResources()
    {
        if (!_isInitialized || ALSourceId == 0)
        {
            return;
        }
        
        AL.SourceStop(ALSourceId);
        AL.Source(ALSourceId, ALSourcei.Buffer, 0);
        AL.DeleteSource(ALSourceId);
        
        ALSourceId = 0;
        
        // We expect that all sound buffers were created by teh Mixer and are destroyed by it or by the user.
        _attachedSoundBuffer = null;
        _queuedSoundBuffers.Clear();
    }

    
    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    
    ~SoundSource()
    {
        ReleaseUnmanagedResources();
    }
}
