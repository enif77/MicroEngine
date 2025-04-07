/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Demos.Audio.WaveLoaderDemo;

using OpenTK.Audio.OpenAL;

using MicroEngine.Audio;
using MicroEngine.Audio.Loaders;

/// <summary>
/// Demonstrates the use of the WAVE sound file loader.
/// </summary>
internal static class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("MicroEngine Audio WAVE Loader Demo");
        
        Console.WriteLine();

        // Load the WAVE sound file.
        Sound? loadedSound;
        using (var stream = File.OpenRead("Resources/Audio/test_16bit-mono.wav"))
        {
            loadedSound = WaveLoader.Load(stream);
        }
        
        // Create the mixer.
        using (var mixer = new Mixer())
        {
            // Initialize the mixer.
            mixer.Initialize();
            
            // Create a sound buffer and attach the sound to it.
            var buffer = mixer.CreateSoundBuffer();
            buffer.LoadData(loadedSound);
            
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
