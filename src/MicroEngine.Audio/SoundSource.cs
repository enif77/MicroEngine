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
            
            var position = new Vector3();
            AL.GetSource3f(ALSourceId, SourceGetPName3F.Position, out position.X, out position.Y, out position.Z);
            
            var error = AL.GetError();
            if (error != ErrorCode.NoError)
            {
                throw new InvalidOperationException($"Failed to get source position. OpenAL error: {error}");
            }
            
            return position;
        }
        
        set
        {
            CheckInitialized();
            
            _ = AL.GetError();
            
            AL.Source3f(ALSourceId, SourcePName3F.Position, value.X, value.Y, value.Z);
            
            var error = AL.GetError();
            if (error != ErrorCode.NoError)
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
            
            AL.GetSourcef(ALSourceId, SourceGetPNameF.Gain, out var gain);
            
            var error = AL.GetError();
            if (error != ErrorCode.NoError)
            {
                throw new InvalidOperationException($"Failed to get source volume. OpenAL error: {error}");
            }
            
            return gain;
        }
        
        set
        {
            CheckInitialized();
            
            _ = AL.GetError();
            
            AL.Sourcef(ALSourceId, SourcePNameF.Gain, value);
            
            var error = AL.GetError();
            if (error != ErrorCode.NoError)
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
            
            var isLoopingValue =  AL.GetSourcei(ALSourceId, SourceGetPNameI.Looping);
            
            var error = AL.GetError();
            if (error != ErrorCode.NoError)
            {
                throw new InvalidOperationException($"Failed to get source looping state. OpenAL error: {error}");
            }
            
            return isLoopingValue != 0;
        }
        
        set
        {
            CheckInitialized();
            
            _ = AL.GetError();
            
            AL.Sourcei(ALSourceId, SourcePNameI.Looping, value ? 1 : 0);
            
            var error = AL.GetError();
            if (error != ErrorCode.NoError)
            {
                throw new InvalidOperationException($"Failed to set source looping state. OpenAL error: {error}");
            }
        }
    }
    
    public SourceState State
    {
        get
        {
            CheckInitialized();
            
            _ = AL.GetError();
            
            AL.GetSourcei(ALSourceId, SourceGetPNameI.SourceState, out var state);
            
            var error = AL.GetError();
            if (error != ErrorCode.NoError)
            {
                throw new InvalidOperationException($"Failed to get source state. OpenAL error: {error}");
            }
            
            return (SourceState)state;
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
        if (error != ErrorCode.NoError)
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
        
        AL.Sourcei(ALSourceId, SourcePNameI.Buffer, soundBuffer.ALBufferId);
        
        var error = AL.GetError();
        if (error != ErrorCode.NoError)
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
        
        AL.Sourcei(ALSourceId, SourcePNameI.Buffer, 0);
        
        var error = AL.GetError();
        if (error != ErrorCode.NoError)
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
        
        var alBufferId = (uint)soundBuffer.ALBufferId;
        AL.SourceQueueBuffers(ALSourceId, 1, ref alBufferId);
        
        var error = AL.GetError();
        if (error != ErrorCode.NoError)
        {
            throw new InvalidOperationException($"Failed to queue sound buffer. OpenAL error: {error}");
        }
        
        _queuedSoundBuffers.Add(soundBuffer);
    }
    
    
    public int GetQueuedSoundBuffersCount()
    {
        CheckInitialized();
        
        _ = AL.GetError();
        
        AL.GetSourcei(ALSourceId, SourceGetPNameI.BuffersQueued, out var count);
        
        var error = AL.GetError();
        if (error != ErrorCode.NoError)
        {
            throw new InvalidOperationException($"Failed to get queued sound buffers count. OpenAL error: {error}");
        }
        
        return count;
    }
    
    
    public int GetProcessedSoundBuffersCount()
    {
        CheckInitialized();
        
        _ = AL.GetError();
        
        AL.GetSourcei(ALSourceId, SourceGetPNameI.BuffersProcessed, out var count);
        
        var error = AL.GetError();
        if (error != ErrorCode.NoError)
        {
            throw new InvalidOperationException($"Failed to get processed sound buffers count. OpenAL error: {error}");
        }
        
        return count;
    }
    
    
    public ISoundBuffer? UnqueueSoundBuffer()
    {
        CheckInitialized();
        
        _ = AL.GetError();

        var bufferId = 0;
        AL.SourceUnqueueBuffers(ALSourceId, 1, ref bufferId);
        
        var error = AL.GetError();
        if (error != ErrorCode.NoError)
        {
            throw new InvalidOperationException($"Failed to unqueue sound buffer. OpenAL error: {error}");
        }
        
        var soundBuffer = _queuedSoundBuffers.FirstOrDefault(b => b.ALBufferId == bufferId);
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
        if (error != ErrorCode.NoError)
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
        if (error != ErrorCode.NoError)
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
        if (error != ErrorCode.NoError)
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
        if (error != ErrorCode.NoError)
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
        AL.Sourcei(ALSourceId, SourcePNameI.Buffer, 0);
        AL.DeleteSource(ALSourceId);
        
        ALSourceId = 0;
        
        // We expect that all sound buffers were created by the Mixer and are destroyed by it or by the user.
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
