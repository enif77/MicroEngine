/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Extensions.Audio.Generators;

using MicroEngine.Audio;

/// <summary>
/// Various sound generation methods.
/// </summary>
public static class SoundsGenerator
{
    /// <summary>
    /// Generates a sine wave based sound using the specified parameters.
    /// </summary>
    /// <param name="samplesCount">How many samples to generate.</param>
    /// <param name="samplesPerSecond">How many samples per second.</param>
    /// <param name="frequency">Frequency of the sound.</param>
    /// <param name="amplitude">Amplitude of the sound - how loud it will be.</param>
    /// <returns>A a buffer containing the samplesCount of 16 bit mono sound samples.</returns>
    public static Sound Generate16BitSineWaveMonoSound(int samplesCount, int samplesPerSecond, double frequency, double amplitude)
    {
        if (frequency <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(frequency), "Frequency must be greater than zero.");
        }
        
        if (amplitude is < 0 or > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(amplitude), "Amplitude must be between 0 and 1.");
        }
        
        var sound = Sound.Create16BitMonoSound(samplesCount, samplesPerSecond);
        
        var deltaTime = 2 * Math.PI / samplesPerSecond;
        var samples = sound.Samples;
        
        for (var i = 0; i < samples.Length; ++i)
        {
            samples[i] = (short)(amplitude * short.MaxValue * Math.Sin(i * deltaTime * frequency));
        }
        
        return sound;
    }
}