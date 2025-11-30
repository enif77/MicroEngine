/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Demos.Audio.SoundBufferWithChangingDataDemo;

using OpenTK.Audio.OpenAL;

using MicroEngine.Audio;
using MicroEngine.Extensions.Audio.Generators;

/// <summary>
/// Demonstrates that data of a sound buffer can be changed over time.
/// </summary>
internal static class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("MicroEngine SoundBuffer with Changing Data Demo");
        
        Console.WriteLine();
        
        // Create the audio mixer.
        using (var mixer = new Mixer())
        {
            // Initialize the audio mixer before using it.
            mixer.Initialize();
            
            // From where the sound goes.
            var source = mixer.CreateSoundSource();
            
            // Audio data buffer used for the sound.
            var buffer = mixer.CreateSoundBuffer();

            for (int freq = 440; freq < 2000; freq += 100)
            {
                // One second of sound (44100 samples, 44100 samples per second, some frequency, amplitude/volume 0.5).
                var sound = SoundsGenerator.Generate16BitSineWaveMonoSound(44100, 44100, freq, 0.5);
                
                // Load the sound data into the buffer.
                buffer.LoadData(sound);    
                
                // Attach the buffer to the source.
                source.AttachSoundBuffer(buffer);
                
                // Play the sound using the sound source.
                source.Play();
                
                // Query the source to find out when it stops playing.
                var count = 0;
                do
                {
                    count++;
                    if (count > 4)
                    {
                        // We play a sound for a half of a second max.
                        Console.WriteLine("Frequency {0} stopped.", freq);

                        break;
                    }

                    Thread.Sleep(125);
                } while (source.State == SourceState.Playing);
                
                // Stop the source.
                source.Stop();
                
                // Detach the buffer from the source so it can be used again.
                source.DetachSoundBuffer();
            }

            Console.WriteLine("Playing done, disposing...");
            
            source.Stop();
            source.Destroy();
            
            buffer.Destroy();
        }

        Console.WriteLine();
        
        Console.WriteLine("DONE");
    }
}
