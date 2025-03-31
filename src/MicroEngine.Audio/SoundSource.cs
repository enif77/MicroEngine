/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Audio;

using OpenTK.Audio.OpenAL;

public sealed class SoundSource : IDisposable
{
    private bool _isInitialized;
    
    /// <summary>
    /// Is the OpenAL source initialized?
    /// </summary>
    public bool IsInitialized => _isInitialized;

    /// <summary>
    /// The OpenAL source ID.
    /// </summary>
    public int SourceId { get; private set; }

    /// <summary>
    /// Indicates whether this sound source is looping.
    /// </summary>
    /// <exception cref="InvalidOperationException">When this sound source is not initialized yet.</exception>
    public bool IsLooping
    {
        get => IsInitialized && AL.GetSource(SourceId, ALSourceb.Looping);
        
        set
        {
            CheckInitialized();
            
            AL.Source(SourceId, ALSourceb.Looping, value);
        }
    }
    
    /// <summary>
    /// Gets the state of the OpenAL source.
    /// </summary>
    /// <exception cref="InvalidOperationException">When this sound source is not initialized yet.</exception>
    public ALSourceState State
    {
        get
        {
            if (!IsInitialized)
            {
                return ALSourceState.Initial;
            }
            
            AL.GetSource(SourceId, ALGetSourcei.SourceState, out var state);
            
            return (ALSourceState)state;
        }
    }
    

    /// <summary>
    /// Initializes the OpenAL source.
    /// </summary>
    /// <exception cref="InvalidOperationException">If the AL source cannot be created.</exception>
    public void Initialize()
    {
        if (_isInitialized)
        {
            return;
        }
        
        SourceId = AL.GenSource();
        if (SourceId == 0)
        {
            throw new InvalidOperationException("Failed to create OpenAL source.");
        }
        
        // Set the source to not loop.
        AL.Source(SourceId, ALSourceb.Looping, false);
        
        _isInitialized = true;
    }

    /// <summary>
    /// This function plays, replays or resumes a source. The playing source will have it's state changed
    /// to ALSourceState.Playing. When called on a source which is already playing, the source will restart
    /// at the beginning. When the attached buffer(s) are done playing, the source will progress to the
    /// ALSourceState.Stopped state.
    /// </summary>
    /// <exception cref="InvalidOperationException">When this sound source is not initialized.</exception>
    public void Play()
    {
        CheckInitialized();
        
        AL.SourcePlay(SourceId);
    }
    
    
    /// <summary>
    /// Releases the OpenAL source and stops playback.
    /// This source can be re-initialized later.
    /// This method can be called multiple times.
    /// </summary>
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
        if (!_isInitialized || SourceId == 0)
        {
            return;
        }
        
        // Detach the buffer from the source and stop the source.
        AL.Source(SourceId, ALSourcei.Buffer, 0);
        AL.SourceStop(SourceId);
        
        // Delete the source.
        AL.DeleteSource(SourceId);
        
        SourceId = 0;
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
