using MicroEngine.Demos.Audio.AmigaModPlayerDemo.Models;
using MicroEngine.Demos.Audio.AmigaModPlayerDemo.Pcm;

namespace MicroEngine.Demos.Audio.AmigaModPlayerDemo.Player;

public class ProTrackerModulePlayer
{
    private ProTrackerModule? _module;
    private AudioGenerator? _audioGenerator;
    
    
    public bool Initialize(string filePath)
    {
        var loader = new Loaders.ProTrackerLoader();

        var module = loader.Load(filePath);
        if (module == null)
        {
            return false;
        }
        
        _module = module;
        _audioGenerator = new AudioGenerator(module);
      
        return true;
    }


    private IList<short>? _nextSoundData;
    private int _nextSoundDataIndex = -1;
    
    public bool GenerateSound(short[] soundData)
    {
        if (_module == null)
        {
            return false;
        }

        if (_audioGenerator == null)
        {
            return false;
        }
        
        if (!_audioGenerator.SongStillActive())
        {
            if (_nextSoundData != null && _nextSoundDataIndex < _nextSoundData.Count)
            {
                // Return the remaining sound data to the output buffer.
                _ = ConsumeRemainingSoundData(soundData);
                
                return true;
            }
            
            return false;
        }

        var soundDataIndex = 0;
        
        if (_nextSoundData != null && _nextSoundDataIndex < _nextSoundData.Count)
        {
            // Return the remaining sound data to the output buffer.
            soundDataIndex = ConsumeRemainingSoundData(soundData);
        }
        
        // Consume data from audio generator till the buffer is full.
        while (_audioGenerator.SongStillActive())
        {
            _nextSoundData = _audioGenerator.GenerateNextSamples();
            _nextSoundDataIndex = 0;
            
            // Copy the sound data to the output buffer.
            for (var i = 0; i < _nextSoundData.Count; i++)
            {
                // Check if the output buffer is full.
                if (soundDataIndex >= soundData.Length)
                {
                    // The output buffer is full, so we need to stop.
                    return true;
                }

                soundData[soundDataIndex] = _nextSoundData[i];
                
                soundDataIndex++;
                _nextSoundDataIndex++;
            }
                
            // One sound data chung is done, continue...        
        }

        return true;
    }

    /// <summary>
    /// Consumes and adds the remaining sound data to the beginning of the output buffer.
    /// </summary>
    /// <param name="soundData">The sound data buffer.</param>
    /// <param name="zeroReminder">If the generated samples remainder is shorter than the output buffer, remaining samples are set to zero.</param>
    /// <returns>The number of consumed samples.</returns>
    private int ConsumeRemainingSoundData(short[] soundData, bool zeroReminder = true)
    {
        var consumedSoundDataCount = 0;
        
        // Consume up to the output buffer size.
        for (var i = 0; i < soundData.Length; i++)
        {
            // Check, if we consumed all the sound data.
            if (_nextSoundDataIndex >= _nextSoundData!.Count)
            {
                // Should we zero the remaining samples?
                if (!zeroReminder)
                {
                    break;
                }
                
                soundData[i] = 0;
            }
            else
            {
                // Copy the sound data to the output buffer.
                soundData[i] = _nextSoundData[_nextSoundDataIndex];

                // Increment the sound data index.
                _nextSoundDataIndex++;
                
                // Increment the consumed sound data count.
                consumedSoundDataCount++;
            }
        }

        // We consumed all the sound data.
        // Here ve assume, that the remainder was shorter, than the output buffer.
        _nextSoundData = null;
        _nextSoundDataIndex = -1;
        
        return consumedSoundDataCount;
    }


    public bool GenerateSoundSimple(short[] soundData)
    {
        if (_module == null)
        {
            return false;
        }
    
        if (_audioGenerator == null)
        {
            return false;
        }
    
        if (!_audioGenerator.SongStillActive())
        {
            return false;
        }
    
        var nextSoundData = _audioGenerator.GenerateNextSamples(soundData.Length);
        
        for (var i = 0; i < soundData.Length; i++)
        {
            soundData[i] = (i >= nextSoundData.Count)
                ? (short)0 
                : nextSoundData[i];
        }
    
        return true;
    }


    public void PrintModuleInfo()
    {
        if (_module == null)
        {
            Console.WriteLine("Module is not loaded.");
            
            return;
        }
        
        var module = _module;
        
        Console.WriteLine($"Module Title: {module.Title}");

        Console.WriteLine($"Instruments:");
        for (int i = 0; i < module.Instruments.Count; i++)
        {
            var instrument = module.Instruments[i];

            if (instrument.Length == 0)
            {
                Console.WriteLine($"  Instrument {i + 1}: {instrument.Name} (Empty)");
            }
            else
            {
                Console.WriteLine(
                    $"  Instrument {i + 1}: {instrument.Name}, Length: {instrument.Length}, Volume: {instrument.Volume}");
            }
        }

        Console.WriteLine($"Number of Song Positions: {module.NumberOfSongPositions}");
        Console.WriteLine($"NoiseTracker Restart Position: {module.NoiseTrackerRestartPosition}");

        // Print out the order list in a readable format 16 numbers per line:
        Console.WriteLine("Order List:");
        for (int i = 0; i < module.OrderList.Count; i++)
        {
            if (i % 16 == 0 && i != 0)
            {
                Console.WriteLine();
            }

            Console.Write($"{module.OrderList[i]:D3} ");
        }

        Console.WriteLine();
        Console.WriteLine("Patterns:");

        for (int i = 0; i < module.Patterns.Count; i++)
        {
            var pattern = module.Patterns[i];
            Console.WriteLine($"  Pattern {i}");

            for (var r = 0; r < 64; r++)
            {
                Console.Write($"    {r:00}: ");

                for (var c = 0; c < pattern.Channels.Count; c++)
                {
                    var row = pattern.Channels[c].Rows[r];

                    Console.Write($"[{row.Pitch:0000} {row.InstrumentNumber:00} {(int)row.Effect:00} {row.EffectXValue:X2} {row.EffectYValue:X2}] ");
                }

                Console.WriteLine();
            }

            Console.WriteLine();
        }
    }
}