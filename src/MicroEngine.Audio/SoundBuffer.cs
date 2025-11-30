/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Audio;

using OpenTK.Audio.OpenAL;

/// <summary>
/// Represents a sound buffer.
/// </summary>
internal sealed class SoundBuffer : ISoundBuffer
{
    private bool _isInitialized;
    
    
    public bool IsInitialized => _isInitialized;
    
    public int ALBufferId { get; private set; }
    

    public void Initialize()
    {
        if (_isInitialized)
        {
            return;
        }
        
        ALBufferId = AL.GenBuffer();
        if (ALBufferId == 0)
        {
            throw new InvalidOperationException("Failed to create OpenAL buffer.");
        }
        
        _isInitialized = true;
    }
    

    public void LoadData(Sound sound)
    {
        CheckInitialized();
        
        if (sound == null)
        {
            throw new ArgumentNullException(nameof(sound), "Sound cannot be null.");
        }
        
        AL.BufferData(ALBufferId, GetSoundFormat(sound.Channels, sound.BitsPerChannel), sound.Samples, sound.Length, sound.SamplesPerSecond);
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
            throw new InvalidOperationException("The OpenAL buffer is not initialized.");
        }
    }
    
    
    private static Format GetSoundFormat(int channels, int bits)
    {
        switch (channels)
        {
            case 1:
                return bits == 8
                    ? Format.FormatMono8
                    : Format.FormatMono16;
            
            case 2: 
                return bits == 8
                    ? Format.FormatStereo8
                    : Format.FormatStereo16;
            
            default: throw new NotSupportedException("The specified sound format is not supported.");
        }
    }
    
    
    private void ReleaseUnmanagedResources()
    {
        if (!_isInitialized || ALBufferId == 0)
        {
            return;
        }
        
        AL.DeleteBuffer(ALBufferId);
        ALBufferId = 0;
    }

    
    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    
    ~SoundBuffer()
    {
        ReleaseUnmanagedResources();
    }
}
