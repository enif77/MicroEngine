namespace MicroEngine.Demos.Audio.SoundSourceStreamingDataDemo;

/// <summary>
/// Sound generator.
/// </summary>
public class SoundGenerator
{
    private const double Amplitude = 0.5;
    private const int SamplesPerSecond = 44100;
    
    private int _frequency = 440;
    
    
    /// <summary>
    /// Resets the sound generator to its initial state.
    /// </summary>
    public void Reset()
    {
        _frequency = 440;
    }
    
    /// <summary>
    /// Generates sound data.
    /// </summary>
    /// <param name="soundData">A buffer for generated data.</param>
    /// <returns>True, if sound data were generated and will be next time. False, when no more data will be generated.</returns>
    public bool GenerateSound(short[] soundData)
    {
        if (_frequency > 2000)
        {
            return false; // Stop generating sound after reaching 2000 Hz.
        }
        
        Console.WriteLine($"  >> Generating sound at frequency {_frequency} Hz.");
        
        var deltaTime = 2 * Math.PI / SamplesPerSecond;
        
        for (var i = 0; i < soundData.Length; ++i)
        {
            soundData[i] = (short)(Amplitude * short.MaxValue * Math.Sin(i * deltaTime * _frequency));
        }
        
        // Increase frequency for the next sound generation.
        _frequency += 100;
        
        return true; // Continue generating sound.
    }
}