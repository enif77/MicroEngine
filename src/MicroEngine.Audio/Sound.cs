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
    public int Count => Samples.Length;
    
    /// <summary>
    /// How many samples per second.
    /// </summary>
    public int SamplesPerSecond { get; init; }
    
    /// <summary>
    /// Frequency of this sound.
    /// </summary>
    public double Frequency { get; init; }
    
    /// <summary>
    /// Amplitude of the sound - how loud it will be.
    /// </summary>
    public double Amplitude { get; init; }
    
    /// <summary>
    /// Actual sound samples.
    /// </summary>
    public short[] Samples { get; }
    
    
    private Sound(int samplesCount)
    {
        Samples = new short[samplesCount];
    }
    
    
    /// <summary>
    /// Create a sound based on 16 bit mono sound samples and the specified parameters.
    /// </summary>
    /// <param name="samplesCount">How many sound samples will the created sound contain.</param>
    /// <param name="sampleFreq">How many samples per second.</param>
    /// <param name="frequency">Frequency of the sound.</param>
    /// <param name="amplitude">Amplitude of the sound - how loud it will be.</param>
    /// <returns>A Sound containing sound samples.</returns>
    public static Sound Create16BitMonoSound(int samplesCount, int sampleFreq, double frequency, double amplitude)
    {
        if (samplesCount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(samplesCount), "Samples count must be greater than zero.");
        }
        
        if (sampleFreq <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(sampleFreq), "Sample frequency must be greater than zero.");
        }
        
        if (frequency <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(frequency), "Frequency must be greater than zero.");
        }
        
        if (amplitude is < 0 or > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(amplitude), "Amplitude must be between 0 and 1.");
        }
        
        return new Sound(samplesCount)
        {
            SamplesPerSecond = sampleFreq,
            Frequency = frequency,
            Amplitude = amplitude
        };
    }
    
    /// <summary>
    /// Generates a sine wave based sound using the specified parameters.
    /// </summary>
    /// <param name="samplesCount">How many samples to generate.</param>
    /// <param name="sampleFreq">How many samples per second.</param>
    /// <param name="frequency">Frequency of the sound.</param>
    /// <param name="amplitude">Amplitude of the sound - how loud it will be.</param>
    /// <returns>A a buffer containing the samplesCount of 16 bit mono sound samples.</returns>
    public static Sound Generate16BitSineWaveMonoSound(int samplesCount, int sampleFreq, double frequency, double amplitude)
    {
        var sound = Create16BitMonoSound(samplesCount, sampleFreq, frequency, amplitude);
        
        var deltaTime = 2 * Math.PI / sampleFreq;
        var samples = sound.Samples;
        
        for (var i = 0; i < samples.Length; ++i)
        {
            samples[i] = (short)(amplitude * short.MaxValue * Math.Sin(i * deltaTime * frequency));
        }
        
        return sound;
    }
}
