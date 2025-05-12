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
    /// <returns>A buffer containing the samplesCount of 16 bit mono sound samples.</returns>
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
    
    /// <summary>
    /// Adds a fade-out effect to the sound.
    /// </summary>
    /// <param name="originalSound">The original sound.</param>
    /// <param name="fadeOutDurationMs">How many milliseconds should the fade-out long.</param>
    /// <param name="createNewSound">If true a copy of the original sound is returned. The original sound is not changed.</param>
    /// <returns>A sound with the fadeout effect applied.</returns>
    /// <exception cref="ArgumentOutOfRangeException">If the fade-out duration is less or equal to zero.</exception>
    /// <exception cref="ArgumentException">If the fade-out duration is longer than the original sound.</exception>
    public static Sound AddFadeOut(Sound originalSound, int fadeOutDurationMs, bool createNewSound = true)
    {
        if (fadeOutDurationMs <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(fadeOutDurationMs), "Fade-out duration must be greater than zero.");
        }

        var samples = originalSound.Samples;
        var sampleRate = originalSound.SamplesPerSecond;

        // The number of samples to fade out.
        var fadeOutSamplesCount = (int)(sampleRate * (fadeOutDurationMs / 1000.0));
        if (fadeOutSamplesCount > samples.Length)
        {
            throw new ArgumentException("Fade-out duration is longer than the sound duration.");
        }
        
        // Create a new sound if requested.
        Sound newSound;
        if (createNewSound)
        {
            // Create a new sound with the same sample rate and length.
            newSound = Sound.Create16BitMonoSound(samples.Length, sampleRate);
            
            // Copy the original samples to the new sound.
            Array.Copy(samples, newSound.Samples, samples.Length);
        }
        else
        {
            // Reuse the original sound.
            newSound = originalSound;
        }
        
        // Add the fade-out effect.
        var newSamples = newSound.Samples;
        for (var i = samples.Length - fadeOutSamplesCount; i < samples.Length; i++)
        {
            var fadeFactor = 1.0 - ((i - (samples.Length - fadeOutSamplesCount)) / (double)fadeOutSamplesCount);
            newSamples[i] = ConvertTo16Bit(ConvertToDouble(samples[i]) * fadeFactor);
        }

        return newSound;
    }
    
    /// <summary>
    /// Adds a reverb effect to the sound.
    /// </summary>
    /// <param name="originalSound">The original sound.</param>
    /// <param name="delayMs">How many milliseconds should the delay be.</param>
    /// <param name="decayFactor">How much the sound should decay. 0.0 = no decay, 1.0 = full decay.</param>
    /// <param name="echoCount">How many echoes should be added.</param>
    /// <param name="createNewSound">If true a copy of the original sound is returned. The original sound is not changed.</param>
    /// <returns>A sound with the reverb effect applied.</returns>
    /// <exception cref="ArgumentException">Thrown when the delay is less or equal to zero, the decay factor is not between 0 and 1, or the echo count is less or equal to zero.</exception>
    public static Sound AddReverb(Sound originalSound, int delayMs, double decayFactor, int echoCount, bool createNewSound = true)
    {
        if (delayMs <= 0)
        {
            throw new ArgumentException("Delay must be greater than zero.");
        }
        
        if (decayFactor <= 0 || decayFactor >= 1)
        {
            throw new ArgumentException("Decay factor must be between 0 and 1.");
        }
        
        if (echoCount <= 0)
        {
            throw new ArgumentException("Echo count must be greater than zero.");
        }

        var samples = originalSound.Samples;
        var sampleRate = originalSound.SamplesPerSecond;
        var delaySamples = (int)(sampleRate * (delayMs / 1000.0));

        // Create a new sound if requested.
        Sound newSound;
        if (createNewSound)
        {
            // Create a new sound with the same sample rate and length.
            newSound = Sound.Create16BitMonoSound(samples.Length, sampleRate);
            
            // Copy the original samples to the new sound.
            Array.Copy(samples, newSound.Samples, samples.Length);
        }
        else
        {
            // Reuse the original sound.
            newSound = originalSound;
        }

        // Add the reverb effect.
        var newSamples = newSound.Samples;
        for (var echo = 1; echo <= echoCount; echo++)
        {
            var echoDelay = echo * delaySamples;
            var echoAmplitude = Math.Pow(decayFactor, echo);

            for (var i = echoDelay; i < newSamples.Length; i++)
            {
                var echoSample = ConvertToDouble(newSamples[i - echoDelay]) * echoAmplitude;
                newSamples[i] = ConvertTo16Bit(ConvertToDouble(newSamples[i]) + echoSample);
            }
        }

        return newSound;
    }
    
    /// <summary>
    /// Adds an echo effect to the sound.
    /// </summary>
    /// <param name="originalSound">The original sound.</param>
    /// <param name="delayMs">Delay in milliseconds.</param>
    /// <param name="decayFactor">>Decay factor (0.0 - 1.0).</param>
    /// <param name="echoCount">>Number of echoes.</param>
    /// <param name="createNewSound">>True if a new sound should be created; false if the original sound should be modified.</param>
    /// <returns>>A sound with the echo effect applied.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the delay is less than or equal to zero, the decay factor is not between 0 and 1, or the echo count is less than or equal to zero.</exception>
    public static Sound AddEcho(Sound originalSound, int delayMs, double decayFactor, int echoCount, bool createNewSound = true)
    {
        if (delayMs <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(delayMs), "Delay must be greater than zero.");
        }

        if (decayFactor <= 0 || decayFactor >= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(decayFactor), "Decay factor must be between 0 and 1.");
        }

        if (echoCount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(echoCount), "Echo count must be greater than zero.");
        }

        var samples = originalSound.Samples;
        var sampleRate = originalSound.SamplesPerSecond;

        // Počet vzorků odpovídající zpoždění.
        var delaySamples = (int)(sampleRate * (delayMs / 1000.0));

        // Vytvoření nového zvuku, pokud je požadováno.
        Sound newSound;
        if (createNewSound)
        {
            newSound = Sound.Create16BitMonoSound(samples.Length, sampleRate);
            Array.Copy(samples, newSound.Samples, samples.Length);
        }
        else
        {
            newSound = originalSound;
        }

        var newSamples = newSound.Samples;

        // Přidání ozvěny.
        for (var echo = 1; echo <= echoCount; echo++)
        {
            var echoDelay = echo * delaySamples;
            var echoAmplitude = Math.Pow(decayFactor, echo);

            for (var i = echoDelay; i < newSamples.Length; i++)
            {
                var echoSample = ConvertToDouble(newSamples[i - echoDelay]) * echoAmplitude;
                newSamples[i] = ConvertTo16Bit(ConvertToDouble(newSamples[i]) + echoSample);
            }
        }

        return newSound;
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
}
