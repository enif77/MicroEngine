/* Copyright (C) Premysl Fara and Contributors */

using OpenTK.Mathematics;

namespace MicroEngine.Audio;

using OpenTK.Audio.OpenAL;

/// <summary>
/// The general audio mixer.
/// </summary>
public sealed class Mixer : IDisposable
{
    private bool _isInitialized;
    
    private ALDevice _device = ALDevice.Null;
    private ALContext _context = ALContext.Null;
    
    private readonly IDictionary<int, ISoundSource> _soundSources = new Dictionary<int, ISoundSource>();
    private readonly IDictionary<int, ISoundBuffer> _soundBuffers = new Dictionary<int, ISoundBuffer>();
    
    public static string GetAudioContextInfoJson()
    {
        var audioContextInfo = new AudioContextInfo
        {
            Version = AL.Get(ALGetString.Version),
            Vendor = AL.Get(ALGetString.Vendor),
            Renderer = AL.Get(ALGetString.Renderer)
        };
        
        return System.Text.Json.JsonSerializer.Serialize(audioContextInfo);
    }

    
    /// <summary>
    /// Gets or sets the master volume.
    /// </summary>
    /// <exception cref="InvalidOperationException">If an OpenAL error occurs.</exception>
    public float ListenerVolume
    {
        get
        {
            CheckInitialized();

            _ = AL.GetError();
            
            var masterVolume = AL.GetListener(ALListenerf.Gain);
            
            var error = AL.GetError();
            if (error != ALError.NoError)
            {
                throw new InvalidOperationException($"Failed to get master volume. OpenAL error: {error}");
            }
            
            return masterVolume;
        }
        
        set
        {
            CheckInitialized();

            _ = AL.GetError();
            
            AL.Listener(ALListenerf.Gain, value);
            
            var error = AL.GetError();
            if (error != ALError.NoError)
            {
                throw new InvalidOperationException($"Failed to set master volume. OpenAL error: {error}");
            }
        }
    }
    
    /// <summary>
    /// Position of the listener in 3D space.
    /// </summary>
    /// <exception cref="InvalidOperationException">If an OpenAL error occurs.</exception>
    public Vector3 ListenerPosition
    {
        get
        {
            CheckInitialized();

            _ = AL.GetError();
            
            var position = AL.GetListener(ALListener3f.Position);
            
            var error = AL.GetError();
            if (error != ALError.NoError)
            {
                throw new InvalidOperationException($"Failed to get listener position. OpenAL error: {error}");
            }
            
            return position;
        }
        
        set
        {
            CheckInitialized();

            _ = AL.GetError();
            
            AL.Listener(ALListener3f.Position, value.X, value.Y, value.Z);
            
            var error = AL.GetError();
            if (error != ALError.NoError)
            {
                throw new InvalidOperationException($"Failed to set listener position. OpenAL error: {error}");
            }
        }
    }
    
    /// <summary>
    /// Velocity of the listener in 3D space.
    /// </summary>
    /// <exception cref="InvalidOperationException">If an OpenAL error occurs.</exception>
    public Vector3 ListenerVelocity
    {
        get
        {
            CheckInitialized();

            _ = AL.GetError();
            
            var velocity = AL.GetListener(ALListener3f.Velocity);
            
            var error = AL.GetError();
            if (error != ALError.NoError)
            {
                throw new InvalidOperationException($"Failed to get listener velocity. OpenAL error: {error}");
            }
            
            return velocity;
        }
        
        set
        {
            CheckInitialized();

            _ = AL.GetError();
            
            AL.Listener(ALListener3f.Velocity, value.X, value.Y, value.Z);
            
            var error = AL.GetError();
            if (error != ALError.NoError)
            {
                throw new InvalidOperationException($"Failed to set listener velocity. OpenAL error: {error}");
            }
        }
    }
    
    /// <summary>
    /// Gets or sets the listener orientation.
    /// The value is a tuple of two vectors: at (the direction the listener is facing) and up (the up direction of the listener).
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if an OpenAL error occurs.</exception>
    public (Vector3, Vector3) ListenerOrientation
    {
        get
        {
            CheckInitialized();

            _ = AL.GetError();
            
            AL.GetListener(ALListenerfv.Orientation, out var at, out var up);
            
            var error = AL.GetError();
            if (error != ALError.NoError)
            {
                throw new InvalidOperationException($"Failed to get listener orientation. OpenAL error: {error}");
            }
            
            return (at, up);
        }
        
        set
        {
            CheckInitialized();

            _ = AL.GetError();
            
            AL.Listener(ALListenerfv.Orientation, ref value.Item1, ref value.Item2);
            
            var error = AL.GetError();
            if (error != ALError.NoError)
            {
                throw new InvalidOperationException($"Failed to set listener orientation. OpenAL error: {error}");
            }
        }
    }
    
    
    /// <summary>
    /// Initializes the audio mixer.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when failed to initialize.</exception>
    public void Initialize()
    {
        if (_isInitialized)    
        {
            return;
        }
        
        _device = ALC.OpenDevice(null);
        if (_device == ALDevice.Null)
        {
            throw new InvalidOperationException("Failed to open audio device.");
        }
        
        _context = ALC.CreateContext(_device, new ALContextAttributes());
        if (_context == ALContext.Null)
        {
            ALC.CloseDevice(_device);
            _device = ALDevice.Null;
            
            throw new InvalidOperationException("Failed to create audio context.");
        }

        var makeContextCurrentResult = ALC.MakeContextCurrent(_context);
        if (!makeContextCurrentResult)
        {
            ALC.DestroyContext(_context);
            _context = ALContext.Null;
            
            ALC.CloseDevice(_device);
            _device = ALDevice.Null;
            
            throw new InvalidOperationException("Failed to make audio context current.");
        }
        
        _isInitialized = true;
    }
    
    /// <summary>
    /// Creates a new initialized sound source.
    /// </summary>
    /// <returns>An initialized sound source.</returns>
    public ISoundSource CreateSoundSource()
    {
        CheckInitialized();
        
        var soundSource = new SoundSource();
        
        soundSource.Initialize();
        
        _soundSources.Add(soundSource.ALSourceId, soundSource);
        
        return soundSource;
    }
    
    /// <summary>
    /// Destroys a sound source created by this mixer.
    /// </summary>
    /// <param name="soundSource">A sound source.</param>
    /// <exception cref="ArgumentNullException">When the soundSource parameter is null.</exception>
    /// <exception cref="InvalidOperationException">When the given sound source is not created by this mixer.</exception>
    public void DestroySoundSource(ISoundSource soundSource)
    {
        CheckInitialized();
        
        if (soundSource == null)
        {
            throw new ArgumentNullException(nameof(soundSource));
        }

        if (!_soundSources.ContainsKey(soundSource.ALSourceId))
        {
            throw new InvalidOperationException($"The sound source with ID {soundSource.ALSourceId} is not managed by this mixer.");
        }
        
        soundSource.Destroy();
        _soundSources.Remove(soundSource.ALSourceId);
    }
    
    /// <summary>
    /// Creates a new initialized sound buffer.
    /// </summary>
    /// <returns>An initialized sound buffer.</returns>
    public ISoundBuffer CreateSoundBuffer()
    {
        CheckInitialized();
        
        var soundBuffer = new SoundBuffer();
        
        soundBuffer.Initialize();
        
        _soundBuffers.Add(soundBuffer.ALBufferId, soundBuffer);
        
        return soundBuffer;
    }
    
    /// <summary>
    /// Destroys a sound buffer created by this mixer.
    /// </summary>
    /// <param name="soundBuffer">A sound buffer.</param>
    /// <exception cref="ArgumentNullException">When the soundBuffer parameter is null.</exception>
    /// <exception cref="InvalidOperationException">When the given sound buffer is not created by this mixer.</exception>
    public void DestroySoundBuffer(ISoundBuffer soundBuffer)
    {
        CheckInitialized();
        
        if (soundBuffer == null)
        {
            throw new ArgumentNullException(nameof(soundBuffer));
        }

        if (!_soundBuffers.ContainsKey(soundBuffer.ALBufferId))
        {
            throw new InvalidOperationException($"The sound buffer with ID {soundBuffer.ALBufferId} is not managed by this mixer.");
        }
        
        soundBuffer.Destroy();
        _soundBuffers.Remove(soundBuffer.ALBufferId);
    }
    
    /// <summary>
    /// Cleans up the audio mixer.
    /// The mixer can be re-initialized later.
    /// This method can be called multiple times.
    /// </summary>
    public void Shutdown()
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
        if (!_isInitialized)
        {
            return;
        }
        
        // Close opened audio sources.
        foreach (var soundSource in _soundSources.Values)
        {
            soundSource.Destroy();
        }
        _soundSources.Clear();
        
        // Close allocated audio buffers.
        foreach (var soundBuffer in _soundBuffers.Values)
        {
            soundBuffer.Destroy();
        }
        _soundBuffers.Clear();
        
        if (_context != ALContext.Null) {
            ALC.MakeContextCurrent(ALContext.Null);
            ALC.DestroyContext(_context);
        }
        _context = ALContext.Null;

        if (_device != IntPtr.Zero) {
            ALC.CloseDevice(_device);
        }
        _device = ALDevice.Null;
    }

    
    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    
    ~Mixer()
    {
        ReleaseUnmanagedResources();
    }
}