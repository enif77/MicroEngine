/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Audio;

/// <summary>
/// Represents playable sound.
/// </summary>
public class Sound
{
    /// <summary>
    /// The number of samples in the sound.
    /// </summary>
    public int SamplesCount => Samples.Length;
    
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
    public short[] Samples { get; init; }
    
    
    private Sound(int samplesCount)
    {
        Samples = new short[samplesCount];
    }
    
    
    /// <summary>
    /// Create a sound based on 16 bit mono sound samples and the specified parameters.
    /// </summary>
    /// <param name="samples">Sound samples to use.</param>
    /// <param name="sampleFreq">How many samples per second.</param>
    /// <param name="frequency">Frequency of the sound.</param>
    /// <param name="amplitude">Amplitude of the sound - how loud it will be.</param>
    /// <returns>A Sound containing sound samples.</returns>
    public static Sound Create16BitMonoSound(short[] samples, int sampleFreq, double frequency, double amplitude)
    {
        return new Sound(samples.Length)
        {
            SamplesPerSecond = sampleFreq,
            Frequency = frequency,
            Amplitude = amplitude,
            Samples = samples
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
    public static Sound Generate16BitMonoSound(int samplesCount, int sampleFreq, double frequency, double amplitude)
    {
        var deltaTime = 2 * Math.PI / sampleFreq;
        var samples = new short[samplesCount];
        
        for (var i = 0; i < samples.Length; ++i)
        {
            samples[i] = (short)(amplitude * short.MaxValue * Math.Sin(i * deltaTime * frequency));
        }

        return Create16BitMonoSound(samples, sampleFreq, frequency, amplitude);
    }
}