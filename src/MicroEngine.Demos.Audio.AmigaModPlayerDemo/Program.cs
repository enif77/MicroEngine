/* Copyright (C) Premysl Fara and Contributors */

using MicroEngine.Demos.Audio.AmigaModPlayerDemo.Player;

namespace MicroEngine.Demos.Audio.AmigaModPlayerDemo;

using OpenTK.Audio.OpenAL;

using MicroEngine.Audio;

/// <summary>
/// Demonstrates that sound data can be streamed from a complicated generator ProTracker MOD file.
/// </summary>
internal static class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("MicroEngine Amiga MOD Player Demo");
        
        Console.WriteLine();
        
        // The "music" generator.
        var soundGenerator = new ProTrackerModulePlayer();
            
        // Load the ProTracker module.
        if (!soundGenerator.Initialize("./Resources/space_debris.mod"))
        {
            Console.WriteLine("Failed to load the ProTracker module.");
            
            return;
        }
        
        // Create the audio mixer.
        using (var mixer = new Mixer())
        {
            // Initialize the audio mixer before using it.
            mixer.Initialize();

            // From where the sound goes.
            var source = mixer.CreateSoundSource();
            
            // Audio data buffer used for the sound.
            var sound = Sound.Create16BitStereoSound(44100, 44100);
            
            // Sound buffers used for streaming the sound data.
            var soundBuffers = new ISoundBuffer[4];
            
            // Initial sound buffers and data generation.
            for (var i = 0; i < soundBuffers.Length; i++)
            {
                // Create a sound buffer.
                soundBuffers[i] = mixer.CreateSoundBuffer();

                // Generate initial sound data.
                var haveNextSoundData = soundGenerator.GenerateSound(sound.Samples);
                if (!haveNextSoundData)
                {
                    // The sound generator finished generating the sound.
                    // This should not happen, because we should be able to generate more than 4 segments of music.
                    throw new InvalidOperationException("Failed to generate initial sound data.");
                }
                
                // Load the sound data into the sound buffer.
                soundBuffers[i].LoadData(sound);
                
                // Queue the buffer to the source.
                source.QueueSoundBuffer(soundBuffers[i]);
                
                Console.WriteLine("  >> Buffer {0} queued.", i);
            }
            
            source.Play();
            
            Console.WriteLine("Playing... Press SPACE to stop the sound.");
            
            while (true)
            {
                // Check if the SPACE key is pressed.
                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Spacebar)
                {
                    Console.WriteLine("Stopping...");
                    
                    break;
                }
                
                // Check if the source finished playing a buffer.
                if (source.GetProcessedSoundBuffersCount() == 0)
                {
                    Thread.Sleep(100);
                    
                    continue;
                }
                
                var nextMusicSegmentAvailable = soundGenerator.GenerateSound(sound.Samples);
                if (nextMusicSegmentAvailable == false)
                {
                    Console.WriteLine("  >> Music finished.");
                    
                    // The sound generator finished generating the sound.
                    break;
                }
                
                var soundBuffer = source.UnqueueSoundBuffer();
                if (soundBuffer == null)
                {
                    // We know that there is at least one sound buffer processed.
                    // So this should not happen.
                    throw new InvalidOperationException("Failed to dequeue a sound buffer.");
                }
                
                Console.WriteLine("  >> Buffer {0} dequeued.", soundBuffer.ALBufferId);
                
                // Load the new sound data into the sound buffer.
                soundBuffer.LoadData(sound);
                
                // Queue the buffer to the source.
                source.QueueSoundBuffer(soundBuffer);
                
                Console.WriteLine("  >> Buffer {0} queued.", soundBuffer.ALBufferId);
                
                // Keep source playing.
                if (source.State != ALSourceState.Playing)
                {
                    source.Play();
                }
            }
            
            Console.WriteLine(" >> Waiting for the source to finish playing...");
            
            // Wait for the source to finish playing.
            while (source.State == ALSourceState.Playing)
            {
                Thread.Sleep(100);
            }
            
            Console.WriteLine("  >> Source finished playing.");
            
            // Stop and destroy the source.
            source.Stop();
            source.Destroy();
            
            // Destroy buffers.
            foreach (var soundBuffer in soundBuffers)
            {
                soundBuffer.Destroy();
            }
        }
        
        Console.WriteLine();
        
        Console.WriteLine("DONE");
    }
}
