/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Demos.Audio.AmigaModPlayerDemo.Pcm;

using MicroEngine.Demos.Audio.AmigaModPlayerDemo.Models;

/**
 * AudioGenerator - manages audio generation for the entire module
 *
 * Contains a list of channel audio generators, one for each channel
 *
 * Maintains the current position of the module and retrieves and mixes channel audio
 */
public class AudioGenerator
{
    private readonly ProTrackerModule _module;
    private readonly List<int> _soloChannels;
    private int _ticksPerRow = 6;
    private int _beatsPerMinute = 125;
    private readonly List<int> _orderList;
    private double _samplesPerTick;

    private readonly List<ChannelAudioGenerator> _channelAudioGenerators = new();

    private int _orderListPosition = 0;
    private int _samplePosition = 0;
    private int _tickPosition = 0;
    private int _rowPosition = 0;
    private int _currentlyPlayingPatternNumber;
    private int _nextRowNumber = 0;
    private int _nextRowPatternNumber;
    private int _nextRowOrderListPosition = 0;

    
    public AudioGenerator(ProTrackerModule module, List<int>? replacementOrderList = null, List<int>? soloChannels = null)
    {
        _module = module;
        var replacementOrderList1 = replacementOrderList ?? new List<int>();
        _soloChannels = soloChannels ?? new List<int>();

        _orderList = replacementOrderList1.Any()
            ? replacementOrderList1
            : _module.OrderList.GetRange(0, _module.NumberOfSongPositions);

        _samplesPerTick = GetSamplesPerTick();

        _currentlyPlayingPatternNumber = _orderList[_orderListPosition];
        _nextRowPatternNumber = _currentlyPlayingPatternNumber;
        
        // Generate channel audio generators for each channel in the module.
        foreach (var channel in _module.Patterns[_currentlyPlayingPatternNumber].Channels)
        {
            _channelAudioGenerators.Add(new ChannelAudioGenerator(channel.PanningPosition, _beatsPerMinute, _ticksPerRow));
        }

        DetermineNextRow();
    }
    

    public bool SongStillActive() =>
        _rowPosition != -1;

    /**
     * Retrieves the next set of samples in the song, mixing the results from each channel audio generator
     */
    public IList<short> GenerateNextSamples(int numberOfSamples = 0)
    {
        if (!SongStillActive())
        {
            return [0, 0];
        }

        // If numberOfSamples is zero, generate the number of samples remaining for the current tick.
        var samplesToGenerate = (numberOfSamples == 0)
            ? (_samplesPerTick - _samplePosition)
            : numberOfSamples;
        var samplesToReturn = new List<short>();

        for (var i = 0;  i <= samplesToGenerate; i++)
        {
            RecalculateSongPosition();
            ApplyNewRowData();
            ApplyPerTickEffects();

            var leftSample = 0.0F;
            var rightSample = 0.0F;

            for (var j = 0; j < _channelAudioGenerators.Count; j++)
            {
                if (CurrentChannelIsPlaying(j))
                {
                    var nextSample = _channelAudioGenerators[j].GetNextSample();
                    leftSample += nextSample.Item1;
                    rightSample += nextSample.Item2;
                }
            }

            samplesToReturn.Add(
                ConvertTo16Bit(leftSample)
            );
            
            samplesToReturn.Add(
                ConvertTo16Bit(rightSample)
            );

            _samplePosition++;
        }

        return samplesToReturn;
    }
    
    /**
     * If we are at the start of a new row, update the channel audio generators with new row data, and apply start of
     * row effects, if present
     */
    private void ApplyNewRowData()
    {
        if (!IsStartOfNewRow())
        {
            return;
        }

        SendRowDataToChannels(_rowPosition);

        for (var i = 0; i < _channelAudioGenerators.Count; i++)
        {
            if (CurrentChannelIsPlaying(i))
            {
                _channelAudioGenerators[i].ApplyStartOfRowEffects();
            }
        }
    }

    /**
     * Keep track of our current song position. Called once per sample generated. The samplePosition counter is increased every
     * time we generate a new sample. In this function, if we have exceeded samples per tick, increase the tick number.
     * If we have exceeded the number of ticks per row, advance to the next row and calculate what the new next row
     * will be.
     */
    private void RecalculateSongPosition()
    {
        // Update tick position, if needed.
        if (_samplePosition >= _samplesPerTick)
        {
            _samplePosition = 0;
            _tickPosition++;
        }

        // Update row position, order list position, and current pattern number, if needed
        if (_tickPosition >= _ticksPerRow)
        {
            _tickPosition = 0;
            _rowPosition = _nextRowNumber;
            _currentlyPlayingPatternNumber = _nextRowPatternNumber;
            _orderListPosition = _nextRowOrderListPosition;
            
            DetermineNextRow();
        }
    }
    
    /**
    * Calculate the next row of the song. If we are at the last row of the song, set to -1. If we are at the last row of
    * the pattern but not the last row of the song, the next row will either be the first row of the next pattern in the
    * order list, or whatever the pattern break specifies (if the effect is present in the row). In all other cases, the
    * next row position is just the current row position + 1.
    */
    private void DetermineNextRow()
    {
        if (!SongStillActive())
        {
            return;
        }

        var isLastRowOfPattern = 
            _rowPosition == 63 ||
            RowHasGlobalEffect(EffectType.PATTERN_BREAK, _module, _currentlyPlayingPatternNumber,_rowPosition) ||
            RowHasGlobalEffect(EffectType.POSITION_JUMP, _module, _currentlyPlayingPatternNumber, _rowPosition);
        var isLastRowOfSong = isLastRowOfPattern && (_orderListPosition + 1 >= _orderList.Count);

        if (isLastRowOfSong)
        {
            _nextRowNumber = -1;
            _nextRowPatternNumber = -1;
        }
        else if (isLastRowOfPattern)
        {
            var patternBreakRow = GetRowWithGlobalEffect(_rowPosition, EffectType.PATTERN_BREAK, _currentlyPlayingPatternNumber, _module);
            var positionJumpRow = GetRowWithGlobalEffect(_rowPosition, EffectType.POSITION_JUMP, _currentlyPlayingPatternNumber, _module);

            _nextRowNumber = Utils.CoerceAtMost((patternBreakRow == null)
                ? 0
                : patternBreakRow.EffectXValue * 10 + patternBreakRow.EffectYValue,63);
            _nextRowOrderListPosition = GetNextOrderListPosition(positionJumpRow, _orderListPosition);
            _nextRowPatternNumber = _orderList[_nextRowOrderListPosition];
        }
        else
        {
            _nextRowNumber = _rowPosition + 1;
        }
    }

    /**
     * Apply per tick effects. Most of the work is done by the channel audio generators.
     */
    private void ApplyPerTickEffects()
    {
        if (!IsStartOfNewTick())
        {
            return;
        }

        for (var i = 0; i < _channelAudioGenerators.Count; i++)
        {
            if (CurrentChannelIsPlaying(i))
            {
                _channelAudioGenerators[i].ApplyPerTickEffects(_tickPosition);
            }
        }
    }

    /**
     * Calculate how many samples should be generated per tick
     */
    private double GetSamplesPerTick()
    {
        var beatsPerSecond = _beatsPerMinute / 60.0;
        var samplesPerBeat = Constants.SAMPLING_RATE / beatsPerSecond;
        var samplesPerRow = samplesPerBeat / 4;

        return samplesPerRow / _ticksPerRow;
    }
    
    /**
     * Determines whether the current position is the start of a new tick
     */
    private bool IsStartOfNewTick() =>
        SongStillActive() && _samplePosition == 0;

    /**
     * Determines whether the current position is the start of a new row.
     */
    private bool IsStartOfNewRow() =>
        SongStillActive() && _tickPosition == 0 && _samplePosition == 0;

    /**
     * Updates the channel audio generators with new row information - note, effects, and audio data
     */
    private void SendRowDataToChannels(int rowNumber)
    {
        // Channel audio generators need to be aware of speed changes for vibrato calculating purposes
        if (RowHasGlobalEffect(EffectType.CHANGE_SPEED, _module, _currentlyPlayingPatternNumber, rowNumber))
        {
            ApplySpeedChange(rowNumber);
        }

        for (var i = 0; i < _channelAudioGenerators.Count; i++)
        {
            if (CurrentChannelIsPlaying(i))
            {
                var channel = _module.Patterns[_currentlyPlayingPatternNumber].Channels[i];
                var row = channel.Rows[rowNumber];
                
                _channelAudioGenerators[i].SetRowData(row, _module.Instruments);
            }
        }
    }
    
    /**
     * When a speed change effect is specified, change the speed of the song and calculate new ticksPerRow and beatsPerMinute
     * Channel audio generators must be aware of the speed change since that is used in vibrato effects
     */
    private void ApplySpeedChange(int rowNumber)
    {
        var speedChangeRow = GetRowWithGlobalEffect(rowNumber, EffectType.CHANGE_SPEED, _currentlyPlayingPatternNumber, _module);
        if (speedChangeRow == null)
        {
            return;
        }

        var speedChange = speedChangeRow.EffectXValue * 16 + speedChangeRow.EffectYValue;
        if (speedChange < 32)
        {
            _ticksPerRow = speedChange;
            foreach (var channelAudioGenerator in _channelAudioGenerators)
            {
                channelAudioGenerator.TicksPerRow = speedChange;
            }    
        }
        else
        {
            _beatsPerMinute = speedChange;
            foreach (var channelAudioGenerator in _channelAudioGenerators)
            {
                channelAudioGenerator.BeatsPerMinute = speedChange;
            }  
        }
    }

    /**
     * Returns true if the row has the specified global effect
     */
    private bool RowHasGlobalEffect(EffectType effect, ProTrackerModule module, int patternNumber, int rowNumber)
    {
        return module.Patterns[patternNumber].Channels
            .Any(channel => channel.Rows[rowNumber].Effect == effect);
    }

    /**
     * Returns a row at the specified song position that has a specified effect. If more than one row has the specified
     * effect, it will return the last one in the list. If no row has the effect, it will return null.
     */
    private Row? GetRowWithGlobalEffect(int rowNumber, EffectType effect, int patternNumber, ProTrackerModule ptmodule)
    {
        for (var i = ptmodule.Patterns[patternNumber].Channels.Count - 1; i >= 0; i--)
        {
            var channel = ptmodule.Patterns[patternNumber].Channels[i];
            if (channel.Rows[rowNumber].Effect == effect)
            {
                return channel.Rows[rowNumber];
            }
        }
        
        return null;
    }

    /**
     * Determines whether a specified channel number should be playing. If no channels are soloed, returns true.
     */
    private bool CurrentChannelIsPlaying(int channelNumber) =>
        _soloChannels.Count == 0 || _soloChannels.Contains(channelNumber);

    /**
     * Converts the incoming sample, as a float, into a 16-bit short. This is the final step of our audio generation -
     * the calling function receives a collection of 16-bit values for the PCM audio data, which is then sent to the
     * output device.
     */
    private short ConvertTo16Bit(float sample) =>
        (short)Utils.Coerce((int)(sample * short.MaxValue), short.MinValue, short.MaxValue);

    /**
     * Determines the next position in the order list. If there is a non-null row with a position jump effect, the next
     * order list position is determined by the position jump effect in that row. Otherwise, it's just one higher than
     * the current order list position
     */
    private int GetNextOrderListPosition(Row? rowWithPositionJump, int currentOrderListPosition)
    {
        return (rowWithPositionJump != null)
            ? Utils.Coerce(rowWithPositionJump.EffectXValue * 16 + rowWithPositionJump.EffectYValue, 0, 127)
            : currentOrderListPosition + 1;
    }
}
