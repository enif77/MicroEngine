/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Audio;

using OpenTK.Audio.OpenAL;

/// <summary>
/// Represents a sound source.
/// </summary>
internal sealed class SoundSource : ISoundSource
{
    private bool _isInitialized;
    

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
        
        ALSourceId = AL.GenSource();
        if (ALSourceId == 0)
        {
            throw new InvalidOperationException("Failed to create OpenAL source.");
        }
        
        // Set the source to not loop.
        AL.Source(ALSourceId, ALSourceb.Looping, false);
        
        _isInitialized = true;
    }
    
    
    public void AttachSoundBuffer(ISoundBuffer soundBuffer)
    {
        CheckInitialized();
        
        if (soundBuffer == null)
        {
            throw new ArgumentNullException(nameof(soundBuffer), "Sound buffer cannot be null.");
        }
        
        if (!soundBuffer.IsInitialized)
        {
            throw new InvalidOperationException("Cannot attach uninitialized sound buffer.");
        }
        
        AL.Source(ALSourceId, ALSourcei.Buffer, soundBuffer.ALBufferId);
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
