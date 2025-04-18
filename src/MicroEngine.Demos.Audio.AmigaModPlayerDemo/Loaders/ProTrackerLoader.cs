/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Demos.Audio.AmigaModPlayerDemo.Loaders;

using MicroEngine.Demos.Audio.AmigaModPlayerDemo.Models;

public class ProTrackerLoader
{
    private const int TITLE_LENGTH = 20;
    private const int INSTRUMENT_NAME_LENGTH = 22;
    private const int NUMBER_OF_INSTRUMENTS = 31;
    private const int ORDER_LIST_MAX_LENGTH = 128;
    private const string PROTRACKER_IDENTIFIER = "M.K.";


    private int _modulePosition = 0;
    private byte[] _moduleData = [];
    
    
    /// <summary>
    /// Loads a ProTracker module from the specified path.
    /// </summary>
    /// <param name="path">A path to a module.</param>
    /// <returns>A ProTrackerModule instance.</returns>
    /// <exception cref="ArgumentException">If no path was specified.</exception>
    /// <exception cref="FileNotFoundException">If the specified file was not found.</exception>
    /// <exception cref="InvalidDataException">If the specified file is not a ProTracker file format.</exception>
    public ProTrackerModule? Load(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentException("Path cannot be null or empty.", nameof(path));
        }
        
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("The specified file does not exist.", path);
        }
        
        // Let's start reading the module.
        _modulePosition = 0;
        
        // Load the file into a byte array.
        _moduleData = File.ReadAllBytes(path);

        if (IsProTrackerModule(_moduleData) == false)
        {
            return null;
        }

        var module = new ProTrackerModule();
        
        // Get the module title.
        module.Title = GetString(TITLE_LENGTH, true);
        
        // Get instruments.
        for (var i = 0; i < NUMBER_OF_INSTRUMENTS; i++)
        {
            var instrument = GetInstrument();
            
            module.Instruments.Add(instrument);
        }

        // Get the song info.
        module.NumberOfSongPositions = GetByte();
        module.NoiseTrackerRestartPosition = GetByte();

        // Get instruments info.
        for (var i = 0; i < ORDER_LIST_MAX_LENGTH; i++)
        {
            module.OrderList.Add(GetByte());
        }

        // Get the module identifier.
        var identifier = GetString(4);
        if (PROTRACKER_IDENTIFIER != identifier)
        {
            throw new InvalidDataException(
                $"Could not validate file as a ProTracker module. Expected identifier {PROTRACKER_IDENTIFIER} but was {identifier}. Terminating.");
        }
        module.Identifier = identifier;
        
        // Get the number of patterns in the module.
        var numberOfPatterns = 0;
        foreach (var moduleId in module.OrderList)
        {
            if (moduleId > numberOfPatterns)
            {
                numberOfPatterns = moduleId;
            }
        }
        
        // Get the patterns.
        for (var i = 0; i <= numberOfPatterns; i++)
        {
            var pattern = new Pattern()
            {
                PatternNumber = i
            };
            
            // Panning positions are hard-coded LRRL in 4-channel ProTracker format
            pattern.Channels.Add(new Channel() { PanningPosition = PanningPosition.LEFT });
            pattern.Channels.Add(new Channel() { PanningPosition = PanningPosition.RIGHT });
            pattern.Channels.Add(new Channel() { PanningPosition = PanningPosition.RIGHT });
            pattern.Channels.Add(new Channel() { PanningPosition = PanningPosition.LEFT });

            // Get the pattern data.
            var channels = pattern.Channels;
            for (var rowNumber = 0; rowNumber < 64; rowNumber++)
            {
                for (var channelNumber = 0; channelNumber < 4; channelNumber++)
                {
                    channels[channelNumber].Rows.Add(GetRow());
                }
            }
            
            module.Patterns.Add(pattern);
        }
        
        // Get the instrument samples.
        foreach (var instrument in module.Instruments)
        {
            if (instrument.Length > 0)
            {
                instrument.AudioData = new float[instrument.Length * 2];
                
                var byteAudioData = GetBytes(instrument.Length * 2);
                for (var i = 0; i < byteAudioData.Length; i++)
                {
                    var b = byteAudioData[i];
                    var s = (b > 127)
                        ? b - 256
                        : b; 
                    instrument.AudioData[i] = s / (float)sbyte.MaxValue;
                }
            }
        }
        
        return module;
    }
    
    
    private static bool IsProTrackerModule(byte[] fileData)
    {
        // Check if the file is a ProTracker module by looking for the "M.K." identifier.
        for (var i = 0; i < fileData.Length - 3; i++)
        {
            if (fileData[i] == 'M' && fileData[i + 1] == '.' && fileData[i + 2] == 'K' && fileData[i + 3] == '.')
            {
                return true;
            }
        }

        return false;
    }
    
    /// <summary>
    /// Gets an instrument from the byte array.
    /// </summary>
    private Instrument GetInstrument()
    {
        var instrumentName = GetString(INSTRUMENT_NAME_LENGTH, true);
        var instrumentLength = GetShort();
        var fineTune = GetSignedNibble();
        var volume = GetByte();
        var repeatOffsetStart = GetShort();
        var repeatLength = GetShort();

        return new Instrument()
        {
            Name = instrumentName,
            Length = instrumentLength,
            FineTune = fineTune,
            Volume = volume,
            RepeatOffsetStart = repeatOffsetStart,
            RepeatLength = repeatLength
        };
    } 
    
    /// <summary>
    /// Extracts a pattern channel row from the byte array.
    /// </summary>
    /// <returns></returns>
    private Row GetRow()
    {
        var byte0 = GetByte();
        var byte1 = GetByte();
        var byte2 = GetByte();
        var byte3 = GetByte();
        
        //val instrumentNumber = bufferData[0].toInt().and(240) + bufferData[2].toInt().and(240).shr(4)
        var instrumentNumber = (byte0 & 240) + ((byte2 & 240) >> 4);
        
        //val pitch = bufferData[0].toInt().and(15).shl(8).or(bufferData[1].toInt().and(255)).toFloat()
        var pitch = (float)(((byte0 & 15) << 8) | (byte1 & 255));
        
        //val effect = bufferData[2].toInt().and(15).shl(8).or(bufferData[3].toInt().and(255))
        var effect = ((byte2 & 15) << 8) | (byte3 & 255);
        
        //val effectNumber = effect.and(3840).shr(8)
        var effectNumber = (effect & 3840) >> 8;
        
        //val xValue = effect.and(240).shr(4)
        var xValue = (effect & 240) >> 4;
        
        //val yValue = effect.and(15)
        var yValue = effect & 15;

        EffectType effectType;
        switch (effectNumber)
        {
            case 0: 
                {
                    if (xValue == 0 && yValue == 0)
                    {
                        effectType = EffectType.NONE;
                    } else
                    {
                        effectType = EffectType.ARPEGGIO;
                    }
                }
                break;
            
            case 1: effectType = EffectType.PITCH_SLIDE_UP; break;
            case 2: effectType = EffectType.PITCH_SLIDE_DOWN; break;
            case 3: effectType = EffectType.SLIDE_TO_NOTE; break;
            case 4: effectType = EffectType.VIBRATO; break;
            case 5: effectType = EffectType.SLIDE_TO_NOTE_WITH_VOLUME_SLIDE; break;
            case 6: effectType = EffectType.VIBRATO_WITH_VOLUME_SLIDE; break;
            case 9: effectType = EffectType.INSTRUMENT_OFFSET; break;
            case 10: effectType = EffectType.VOLUME_SLIDE; break;
            case 11: effectType = EffectType.POSITION_JUMP; break;
            case 12: effectType = EffectType.SET_VOLUME; break;
            case 13: effectType = EffectType.PATTERN_BREAK; break;
            case 14: 
                {
                    switch (xValue)
                    {
                        case 5: effectType = EffectType.SET_FINE_TUNE; break;
                        case 10: effectType = EffectType.FINE_VOLUME_SLIDE_UP; break;
                        case 11: effectType = EffectType.FINE_VOLUME_SLIDE_DOWN; break;
                        
                        default:
                            effectType = EffectType.UNKNOWN_EFFECT;
                            break;
                    }
                }
                break;
            
            case 15: effectType = EffectType.CHANGE_SPEED; break;
            
            default:
                effectType = EffectType.UNKNOWN_EFFECT;
                break;
        }
        
        return new Row()
        {
            InstrumentNumber = instrumentNumber,
            Pitch = pitch,
            Effect = effectType,
            EffectXValue = xValue,
            EffectYValue = yValue
        };
    }  
    
    /// <summary>
    /// Get a string from the byte array.
    /// </summary>
    private string GetString(int length, bool trimEnd = false)
    {
        // Extract a string from the byte array.
        var str = GetBytes(length);
        
        // Convert the byte array to a string and trim any null characters if requested.
        return trimEnd
            ? System.Text.Encoding.ASCII.GetString(str).TrimEnd('\0')
            : System.Text.Encoding.ASCII.GetString(str);
    }  
    
    /// <summary>
    /// Get N bytes from the byte array.
    /// </summary>
    private byte[] GetBytes(int length)
    {
        var bytes = new byte[length];
        
        Array.Copy(_moduleData, _modulePosition, bytes, 0, length);
        _modulePosition += length;
        
        return bytes;
    }  
    
    /// <summary>
    /// Extract a short from the byte array.
    /// </summary>
    private int GetByte()
    {
        return _moduleData[_modulePosition++];
    }
    
    /// <summary>
    /// Extract a short from the byte array.
    /// </summary>
    private short GetShort()
    {
        var result = (short)((_moduleData[_modulePosition] << 8) | _moduleData[_modulePosition + 1]);
        _modulePosition += 2;
        
        return result;
    }
    
    /// <summary>
    /// Gets the lower 4 bits of a byte as an signed nibble.
    /// </summary>
    private int GetSignedNibble()
    {
        // Get rid of the upper 4 bits.
        var nibble = GetByte() & 15;

        // If first bit is 1, it's a negative number.
        return ((nibble & 8) != 0)
            ? nibble - 16
            : nibble;
    }
}
