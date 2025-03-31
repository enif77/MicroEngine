/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Demos.Audio.MixerDemo;

using MicroEngine.Audio;

internal static class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("MicroEngine Audio Mixer Demo");
        
        Console.WriteLine();
        
        Console.WriteLine("Audio Context Info before the Mixer is created and initialized: ");
        Console.WriteLine(Mixer.GetAudioContextInfoJson());
        
        Console.WriteLine();
        
        Console.WriteLine("Creating the audio mixer...");
        
        // Create the audio mixer.
        // This will not initialize the audio mixer.
        // The audio mixer will be initialized when the Initialize method is called.
        // The audio mixer should be shut down when it is no longer needed.
        // The audio mixer should be shut down before the application exits.
        var mixer = new Mixer();

        try
        {
            Console.WriteLine("Initializing the audio mixer...");
            
            // Initialize the audio mixer before using it.
            // This will create the audio device and the audio context.
            // The audio context will be made current.
            // The audio context will be used for all audio operations.
            // If the mixer is not initialized, the audio operations will fail.
            // If the mixer.Initialize() method is called multiple times, subsequent calls will be ignored.
            mixer.Initialize();
            
            Console.WriteLine("Audio mixer initialized.");
            
            Console.WriteLine();
            
            Console.WriteLine("Audio Context Info after the Mixer is created: ");
            Console.WriteLine(Mixer.GetAudioContextInfoJson());
            
            Console.WriteLine();
            
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(true);
        
            Console.WriteLine();
            
            Console.WriteLine("Exiting...");
            
            Console.WriteLine();
        }
        finally
        {
            Console.WriteLine("Shutting down the audio mixer...");
            
            // Shutdown the audio mixer after using it.
            // This will destroy the audio context and close the audio device.
            // The Mixer class is IDisposable, so it can be used in a using statement.
            // The Dispose method will be called by the destructor if not called explicitly.
            mixer.Shutdown();
            
            Console.WriteLine("Mixer shut down.");
        }
        
        Console.WriteLine();
        
        Console.WriteLine("DONE");
    }
}
