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
            samples[i] = ConvertTo16Bit(amplitude * Math.Sin(i * deltaTime * frequency));
        }
        
        return sound;
    }

    /// <summary>
    /// Generates a white noise sound using the specified parameters.
    /// </summary>
    /// <param name="samplesCount">How many samples to generate.</param>
    /// <param name="samplesPerSecond">How many samples per second.</param>
    /// <param name="amplitude">Amplitude of the sound - how loud it will be.</param>
    /// <param name="baseSineToneFrequency">Base sine frequency mixed into the generated sound. If less or equal 0, no sine tone is added. Optional; the default is 0.</param>
    /// <param name="cutoffFrequency">A resonant low-pass filter cut off frequency. If less or equal 0, no resonant low-pass filter will be used. Optional; the default is 0.</param>
    /// <param name="resonance">A resonant low-pass filter resonance. 0.0 = no resonance, 1.0 = maximal resonance. Optional; the default is 0.5.</param>
    /// <param name="withExponentialFadeOut">Will add an exponential fade out. Optional; the default is false.</param>
    /// <param name="whiteNoiseGeneratorSeed">An optional white noise generator seed. If 0, the Random() method without a seed is used. Optional; the default is 0.</param>
    /// <returns></returns>
    public static Sound Generate16BitWhiteNoiseMonoSound(
        int samplesCount,
        int samplesPerSecond,
        double amplitude,
        double baseSineToneFrequency = 0,
        double cutoffFrequency = 0,
        double resonance = 0.5,
        bool withExponentialFadeOut = false,
        int whiteNoiseGeneratorSeed = 0)
    {
        var rand = (whiteNoiseGeneratorSeed == 0)
            ? new Random()
            : new Random(whiteNoiseGeneratorSeed);

        var sound = Sound.Create16BitMonoSound(samplesCount, samplesPerSecond);
        
        var samples = sound.Samples;
        
        for (var i = 0; i < samples.Length; i++)
        {
            // White noise.
            var whiteNoise = 2 * rand.NextDouble() - 1;
            
            // Fast fade out envelope.
            var envelope = 1.0;
            if (withExponentialFadeOut)
            {
                var t = (double)i / samples.Length;
                envelope = Math.Exp(-6 * t);
            }
            
            // Sine tone.
            if (baseSineToneFrequency > 0)
            {
                // Mix the white noise (60%) with a sine tone (40%).
                var sinTone = Math.Sin(2 * Math.PI * baseSineToneFrequency * i / samplesPerSecond);
                samples[i] = ConvertTo16Bit((whiteNoise * 0.6 + sinTone * 0.4) * envelope * amplitude);    
            }
            else
            {
                samples[i] = ConvertTo16Bit(whiteNoise * envelope * amplitude);    
            }
        }
        
        if (cutoffFrequency > 0)
        {
            // Apply a resonant low-pass filter to the white noise.
            // For example:
            //   cutoffFreq = 1200.0; // Hz – set to your taste.
            //   resonance = 0.7;     // 0.0 = no resonance, 0.9 = strong resonance.
           
            ApplyResonantLowPass(
                samples,
                samplesPerSecond,
                Coerce(cutoffFrequency, 1, 44100),
                Coerce(resonance, 0, 1));
        }
        
        return sound;
    }
    
    
    static void ApplyResonantLowPass(short[] buffer, int sampleRate, double cutoffFrequency, double resonance)
    {
        var w0 = 2 * Math.PI * cutoffFrequency / sampleRate;
        var alpha = Math.Sin(w0) / (2 * resonance);
        var cosw0 = Math.Cos(w0);

        var a0 = 1 + alpha;
        var a1 = -2 * cosw0;
        var a2 = 1 - alpha;
        var b0 = (1 - cosw0) / 2;
        var b1 = 1 - cosw0;
        var b2 = (1 - cosw0) / 2;

        double x1 = 0, x2 = 0;
        double y1 = 0, y2 = 0;

        for (var i = 0; i < buffer.Length; i++)
        {
            var x = ConvertToDouble(buffer[i]);
            var y = (b0 / a0) * x + (b1 / a0) * x1 + (b2 / a0) * x2 - (a1 / a0) * y1 - (a2 / a0) * y2;

            buffer[i] = ConvertTo16Bit(y);
            x2 = x1;
            x1 = x;
            y2 = y1;
            y1 = y;
        }
    }
    
    
    private static double ConvertToDouble(short sample) =>
        (double)sample / short.MaxValue;
    
    
    private static short ConvertTo16Bit(double sample) =>
        (short)Coerce((int)(sample * short.MaxValue), short.MinValue, short.MaxValue);
    
    
    private static double Coerce(double value, double min, double max)
    {
        return value < min
            ? min
            : value > max
                ? max
                : value;
    }
    

    // public static Sound Generate16BitWhiteNoiseWithExponentialFadeOutMonoSound(int samplesCount, int samplesPerSecond, int seed = 0)
    // {
    //     var rand = (seed == 0)
    //         ? new Random()
    //         : new Random(seed);
    //
    //     var sound = Sound.Create16BitMonoSound(samplesCount, samplesPerSecond);
    //     
    //     var samples = sound.Samples;
    //     
    //     for (var i = 0; i < samples.Length; i++)
    //     {
    //         var t = (double)i / samples.Length;
    //         var envelope = Math.Exp(-6 * t); // rychlý útlum
    //         var whiteNoise = 2 * rand.NextDouble() - 1;
    //         samples[i] = (short)(whiteNoise * envelope * short.MaxValue * 0.6); // 60% hlasitosti
    //     }
    //     
    //     return sound;
    // }

}