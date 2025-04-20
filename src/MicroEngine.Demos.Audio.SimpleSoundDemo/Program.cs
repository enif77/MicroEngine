/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Demos.Audio.SimpleSoundDemo;

using OpenTK.Audio.OpenAL;

using MicroEngine.Audio;
using MicroEngine.Extensions.Audio.Generators;

/// <summary>
/// Demonstrates the use of the MicroEngine Audio library.
/// </summary>
internal static class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("MicroEngine Audio Mixer Demo");
        
        Console.WriteLine();

        // Create a sound.
        // This sound is a 1 second long sine wave at 440 Hz.
        // Sounds are simple objects that can be deleted anytime once they are no more needed.
        var sound440 = SoundsGenerator.Generate16BitSineWaveMonoSound(44100, 44100, 440, 0.5);
        var whiteNoiseSound = SoundsGenerator.Generate16BitWhiteNoiseMonoSound(
            44100, 
            44100,
            0.7,
            100,
            1200.0,
            0.7,
            false,
            0);
        
        // Create the audio mixer.
        // This will not initialize the audio mixer.
        // The audio mixer will be initialized when the Initialize method is called.
        // The audio mixer should be shut down when it is no longer needed.
        // The mixer will dispose all the sound buffers and sources when it is disposed.
        using (var mixer = new Mixer())
        {
            // Initialize the audio mixer before using it.
            mixer.Initialize();
            
            // Create a sound buffer and attach the sound to it.
            var buffer = mixer.CreateSoundBuffer();
            buffer.LoadData(sound440);
            //buffer.LoadData(whiteNoiseSound);
            
            // Create a sound source and attach the sound buffer to it.
            var source = mixer.CreateSoundSource();
            source.AttachSoundBuffer(buffer);
            
            // Play the sound using the sound source.
            source.Play();
            
            // Wait for the sound to finish playing.
            while (source.State == ALSourceState.Playing)
            {
                Thread.Sleep(100);
            }
        }

        Console.WriteLine();
        
        Console.WriteLine("DONE");
    }
}
