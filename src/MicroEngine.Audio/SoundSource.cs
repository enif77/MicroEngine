/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Audio;

using OpenTK.Audio.OpenAL;

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
    
    public bool IsLooping
    {
        get
        {
            CheckInitialized();    
            
            return AL.GetSource(ALSourceId, ALSourceb.Looping);
        }
        
        set
        {
            CheckInitialized();
            
            AL.Source(ALSourceId, ALSourceb.Looping, value);
        }
    }
    
    public ALSourceState State
    {
        get
        {
            CheckInitialized();
            
            AL.GetSource(ALSourceId, ALGetSourcei.SourceState, out var state);
            
            return (ALSourceState)state;
        }
    }
    
    
    public void Initialize()
    {
        if (_isInitialized)
        {
            return;
        }
        
        AL.GetError();
        
        ALSourceId = AL.GenSource();
        
        var error = AL.GetError();
        if (error != ALError.NoError)
        {
            throw new InvalidOperationException($"Failed to create OpenAL source. OpenAL error: {error}");
        }
        
        // Set the source to not loop.
        AL.Source(ALSourceId, ALSourceb.Looping, false);
        
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

        AL.GetError();
        
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
        
        AL.SourceStop(ALSourceId);
        AL.Source(ALSourceId, ALSourcei.Buffer, 0);
        
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
        
        AL.GetError();
        
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
        
        AL.GetSource(ALSourceId, ALGetSourcei.BuffersQueued, out var count);
        
        return count;
    }
    
    
    public int GetProcessedSoundBuffersCount()
    {
        CheckInitialized();
        
        AL.GetSource(ALSourceId, ALGetSourcei.BuffersProcessed, out var count);
        
        return count;
    }
    
    
    public ISoundBuffer? UnqueueSoundBuffer()
    {
        CheckInitialized();
        
        var buffer = AL.SourceUnqueueBuffer(ALSourceId);
        
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
        
        AL.SourcePlay(ALSourceId);
    }
    

    public void Pause()
    {
        CheckInitialized();
        
        AL.SourcePause(ALSourceId);
    }
    

    public void Stop()
    {
        CheckInitialized();
        
        AL.SourceStop(ALSourceId);
    }
    

    public void Rewind()
    {
        CheckInitialized();
        
        AL.SourceRewind(ALSourceId);
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
