/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Audio;

/// <summary>
/// Represents playable sound.
/// </summary>
public sealed class Sound
{
    /// <summary>
    /// The number of samples in the sound.
    /// </summary>
    public int Length => Samples.Length;
    
    /// <summary>
    /// How many samples per second.
    /// </summary>
    public int SamplesPerSecond { get; private init; }

    /// <summary>
    /// The number of channels - 1 for mono, 2 for stereo.
    /// </summary>
    public int Channels { get; }

    /// <summary>
    /// The number of bits per channel - 8 or 16.
    /// </summary>
    public int BitsPerChannel => 16;

    /// <summary>
    /// Actual sound samples.
    /// </summary>
    public short[] Samples { get; }
    
    
    private Sound(int channels, int samplesCount)
    {
        Channels = channels;
        Samples = new short[channels * samplesCount];
    }
    
    
    /// <summary>
    /// Create a sound based on 16-bit mono sound samples and the specified parameters.
    /// </summary>
    /// <param name="samplesCount">How many sound samples will the created sound contain?</param>
    /// <param name="samplesPerSecond">How many samples per second.</param>
    /// <returns>A Sound containing sound samples.</returns>
    public static Sound Create16BitMonoSound(int samplesCount, int samplesPerSecond)
    {
        if (samplesCount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(samplesCount), "Samples count must be greater than zero.");
        }
        
        if (samplesPerSecond <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(samplesPerSecond), "Sample frequency must be greater than zero.");
        }
        
        return new Sound(1, samplesCount)
        {
            SamplesPerSecond = samplesPerSecond
        };
    }
    
    /// <summary>
    /// Create a sound based on 16-bit stereo sound samples and the specified parameters.
    /// </summary>
    /// <param name="samplesCount">How many sound samples will the created sound contain?</param>
    /// <param name="samplesPerSecond">How many samples per second.</param>
    /// <returns>A Sound containing sound samples.</returns>
    public static Sound Create16BitStereoSound(int samplesCount, int samplesPerSecond)
    {
        if (samplesCount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(samplesCount), "Samples count must be greater than zero.");
        }
        
        if (samplesPerSecond <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(samplesPerSecond), "Sample frequency must be greater than zero.");
        }
        
        return new Sound(2, samplesCount)
        {
            SamplesPerSecond = samplesPerSecond
        };
    }
}
