/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Demos.Audio.AmigaModPlayerDemo.Pcm;

using MicroEngine.Demos.Audio.AmigaModPlayerDemo.Models;

/**
 * ChannelAudioGenerator - manages the state of an individual channel. Maintains information about the current note,
 * instrument, and effects. Processes effects relevant to the channel and applies them to the active note state.
 *
 * Has a resampler instance, which manages retrieval of instrument audio data.
 */
public class ChannelAudioGenerator
{
    private const float TriamuRatio = 1.007246412F;
    private const float Tolerance = 1e-6F;
    
    private readonly PanningPosition _panningPosition;

    public int BeatsPerMinute = 125;
    public int TicksPerRow = 6;
    
    private readonly Resampler _resampler = new();

    // Instrument state variables
    private int _currentInstrumentNumber = 0;
    private Instrument? _activeInstrument;

    // Note state variables
    private float _actualPitch = 0.0F;
    private float _specifiedPitch = 0.0F;
    private bool _isInstrumentPlaying = false;
    private int _fineTune = 0;

    // Effect state variables
    private int _currentVolume = 0;
    private EffectType _currentEffect = EffectType.UNKNOWN_EFFECT;
    private int _effectXValue = 0;
    private int _effectYValue = 0;

    // State variables for specific effect types
    private int _slideToNotePitchShift = 0;
    private float _vibratoCyclesPerRow = 0.0F;
    private int _vibratoDepth = 0;
    private float _vibratoSamplesPerCyclePosition = 0.0F;
    private float _vibratoSamplesPerCycle = 0.0F;
    private int _vibratoSamplesElapsed = 0;
    
    
    // This is used to calculate a vibrato in a sine waveform. While I could do the sine math myself, this is
    // how ProTracker implemented it - and therefore how I will implement it.
    private readonly int[] _sineTable =
    [
        0, 24, 49, 74, 97, 120, 141, 161, 180, 197, 212, 224, 235, 244, 250, 253, 255, 253, 250, 244, 235, 224, 212,
        197, 180, 161, 141, 120, 97, 74, 49, 24, 0, -24, -49, -74, -97, -120, -141, -161, -180, -197, -212, -224, -235,
        -244, -250, -253, -255, -253, -250, -244, -235, -224, -212, -197, -180, -161, -141, -120, -97, -74, -49, -24
    ];

    
    public ChannelAudioGenerator(PanningPosition panningPosition, int beatsPerMinute = 125, int ticksPerRow = 6)
    {
        _panningPosition = panningPosition;
        BeatsPerMinute = beatsPerMinute;
        TicksPerRow = ticksPerRow;
    }
    
    
    /**
    * Retrieves the next sample from the resampler, applies volume and panning adjustment, and returns it.
    *
    * Vibrato effects are applied in here as well, since those need to be handled per-sample rather than per-tick.
    */
    public (float, float) GetNextSample()
    {
        if (!_isInstrumentPlaying)
        {
            return (0.0F, 0.0F);
        }

        var vibratoPitchAdjustment = GetVibratoPitchAdjustment();
        if (vibratoPitchAdjustment != 0 && Math.Abs(_actualPitch - (_specifiedPitch + vibratoPitchAdjustment)) > Tolerance)
        {
            _resampler.RecalculateStep(_actualPitch + vibratoPitchAdjustment, Constants.SAMPLING_RATE);
        }

        var actualSample = _resampler.GetInterpolatedSample();
        var volumeAdjustedSample = GetSampleWithVolumeApplied(actualSample, _currentVolume);

        if (_currentEffect is EffectType.VIBRATO or EffectType.VIBRATO_WITH_VOLUME_SLIDE)
        {
            _vibratoSamplesElapsed = GetUpdatedVibratoSamplesElapsed();
        }

        return GetStereoSample(volumeAdjustedSample);
    }

    /**
     * Handles new row data during playback - when a new row is provided, it updates the instrument, pitch, and effect
     */
    public void SetRowData(Row row, IList<Instrument> instruments)
    {
        UpdateInstrument(row, instruments);
        UpdatePitch(row);
        UpdateEffect(row.Effect, row.EffectXValue, row.EffectYValue);
    }

    /**
     * Applies effects that take place only once at the start of a row, before any pcm data has been generated for that row
     */
    public void ApplyStartOfRowEffects()
    {
        _currentVolume = (_currentEffect) switch
        {
            EffectType.FINE_VOLUME_SLIDE_UP => Utils.CoerceAtMost(_currentVolume + _effectYValue, 64),
            EffectType.FINE_VOLUME_SLIDE_DOWN => Utils.CoerceAtLeast(_currentVolume - _effectYValue, 0),
            EffectType.SET_VOLUME => Utils.CoerceAtMost(_effectXValue * 16 + _effectYValue, 64),
            _ => _currentVolume
        };

        if (EffectType.INSTRUMENT_OFFSET == _currentEffect)
        {
            _resampler.AudioDataReference = _effectXValue * 4096 + _effectYValue * 256;
        }
    }

    /**
     * Applies effects that should be applied once per tick
     */
    public void ApplyPerTickEffects(int tickPosition)
    {
        if (tickPosition == 0)
        {
            return;
        }

        ApplyVolumeAdjustmentEffect();
        ApplyPitchAdjustmentEffect(tickPosition);
    }
    
    
    private void ApplyVolumeAdjustmentEffect()
    {
        _currentVolume = (_currentEffect) switch
        {
            EffectType.SLIDE_TO_NOTE_WITH_VOLUME_SLIDE => ApplyVolumeSlide(_effectXValue, _effectYValue, _currentVolume),
            EffectType.VOLUME_SLIDE => ApplyVolumeSlide(_effectXValue, _effectYValue, _currentVolume),
            EffectType.VIBRATO_WITH_VOLUME_SLIDE => ApplyVolumeSlide(_effectXValue, _effectYValue, _currentVolume),
            _ => _currentVolume
        };
    }

    
    private void ApplyPitchAdjustmentEffect(int tickPosition)
    {
        switch (_currentEffect)
        {
            case EffectType.SLIDE_TO_NOTE:
            case EffectType.SLIDE_TO_NOTE_WITH_VOLUME_SLIDE:
                ApplySlideToNoteAdjustment();
                break;
                
            case EffectType.PITCH_SLIDE_UP:
            case EffectType.PITCH_SLIDE_DOWN:
                _actualPitch = GetPitchSlideAdjustment(_effectXValue, _effectYValue, _currentEffect, _actualPitch);
                _specifiedPitch = _actualPitch;
                _resampler.RecalculateStep(_actualPitch, Constants.SAMPLING_RATE);
                break;
                
            case EffectType.ARPEGGIO:
            {
                // Get the new pitch based on our tick position.
                var numberOfSemitones = (tickPosition % 3) switch
                {
                    1 => _effectXValue,
                    2 => _effectYValue,
                    _ => 0
                };

                // Calculate the pitch adjustment, factoring in number of semitones - each semitone is the equivalent of 8 finetunes.
                _actualPitch = GetFineTuneAdjustedPitch(_specifiedPitch, 8 * numberOfSemitones);
                _resampler.RecalculateStep(_actualPitch, Constants.SAMPLING_RATE);
            }
                break;
                
            default:
                return;
        }
    }
    
    /**
     * Updates the instrument for a new row. If no new instrument is specified, retain current instrument data.
     *
     * If a new instrument number is specified, replace the current instrument with the new instrument.
     *
     * In either case, if any instrument number is specified, set the current volume to the active instrument's volume.
     */
    private void UpdateInstrument(Row row, IList<Instrument> instruments)
    {
        if (row.InstrumentNumber == 0)
        {
            return;
        }

        if (row.InstrumentNumber != _currentInstrumentNumber)
        {
            _currentInstrumentNumber = row.InstrumentNumber;
            _activeInstrument = instruments[_currentInstrumentNumber - 1];
            _resampler.Instrument = instruments[_currentInstrumentNumber - 1];

            // In rare circumstances, an instrument number might be provided without an accompanying note - if this is
            // a different instrument than the one currently playing, stop playing it.
            if (row.Pitch == 0.0F)
            {
                _isInstrumentPlaying = false;
            }

            // If the instrument is changing, we definitely want to reset the audio data reference, unless effect is 3xx
            if (EffectType.SLIDE_TO_NOTE != row.Effect)
            {
                _resampler.AudioDataReference = Constants.INSTRUMENT_STARTING_REFERENCE;
            }
        }

        // Specifying instrument number always updates the volume, even if no pitch is specified
        _currentVolume = _activeInstrument!.Volume;
    }

    /**
     * Changes the pitch to the pitch of the given row. Immediately change the pitch to the
     * given row's pitch, unless a slide to note effect is specified.
     */
    private void UpdatePitch(Row row)
    {
        if (row.Pitch == 0.0F)
        {
            return;
        }

        _fineTune = GetFineTuneValue(_activeInstrument!, row);
        var fineTuneAdjustedPitch = GetFineTuneAdjustedPitch(row.Pitch, _fineTune);

        _specifiedPitch = fineTuneAdjustedPitch;

        // A slide to note effect will not reset the position of the audio data reference or cause us to immediately change the pitch.
        if (row.Effect != EffectType.SLIDE_TO_NOTE && row.Effect != EffectType.SLIDE_TO_NOTE_WITH_VOLUME_SLIDE)
        {
            _actualPitch = fineTuneAdjustedPitch;
            _resampler.AudioDataReference = Constants.INSTRUMENT_STARTING_REFERENCE;
        }

        _isInstrumentPlaying = true;
        _resampler.RecalculateStep(_actualPitch, Constants.SAMPLING_RATE);
    }

    /**
     * Accepts the sample and responds with a volume-adjusted sample. Maximum volume is 64 - at 64, it will simply respond
     * with the sample at the original value that it was already at. For anything below 64, it determines the volume ratio
     * and multiplies the sample by that. A volume value of zero will result in a sample value of zero. Note that the
     * actual range of samples values is -1.0F to 1.0F, so the actual volume multiplier will be within that range
     */
    private float GetSampleWithVolumeApplied(float actualSample, int volume)
    {
        return (volume == 64)
            ? actualSample
            : actualSample * (volume / 64.0F);
    }

    /**
     * Accepts a sample float and responds with a pair of floats adjusted for stereo panning
     *
     * Protracker panning is very simple: it's either left channel or right channel. All we need to do is respond with the
     * sample in the pair's first field for left panning, or second field for right panning.
     */
    private (float, float) GetStereoSample(float sample)
    {
        return (PanningPosition.LEFT == _panningPosition)
            ? (sample, 0.0F)
            : (0.0F, sample);
    }
    
    /**
     * Apply volume slide effect. Volume increase/decrease are the x/y values of the effect. They cannot both be non-
     * zero, but this function assumes that has already been taken care of.
     */
    private int ApplyVolumeSlide(int volumeIncrease, int volumeDecrease, int volume)
    {
        return (volumeIncrease > 0)
            ? Utils.CoerceAtMost(volumeIncrease + volume, 64)
            : Utils.CoerceAtLeast(volume - volumeDecrease, 0);
    }

    /**
     * Apply a slide to note effect. If the actual pitch is at the specified pitch, do nothing (we have already reached
     * the desired pitch). Otherwise, either add or subtract the pitch shift (depending on where the actual pitch is
     * relative to the specified pitch), making sure we do not exceed the specified pitch.
     */
    private void ApplySlideToNoteAdjustment()
    {
        // If we are already at the specified pitch, do nothing.
        if (Math.Abs(_actualPitch - _specifiedPitch) < Tolerance)
        {
            return;
        }

        if (_actualPitch > _specifiedPitch)
        {
            _actualPitch = Utils.CoerceAtLeast(_actualPitch - _slideToNotePitchShift, _specifiedPitch);
        }
        else if (_actualPitch < _specifiedPitch)
        {
            _actualPitch = Utils.CoerceAtMost(_actualPitch + _slideToNotePitchShift, _specifiedPitch);
        }

        _resampler.RecalculateStep(_actualPitch, Constants.SAMPLING_RATE);
    }

    /**
     * When a new effect is given, update the effect. In some cases, we want to set some state variables because we may
     * need to "remember" the effect parameters later.
     */
    private void UpdateEffect(EffectType effectType, int xValue, int yValue)
    {
        switch (effectType)
        {
            case EffectType.SLIDE_TO_NOTE:
                _slideToNotePitchShift = GetSlideToNotePitchShift(xValue, yValue);
                break;
                
            case EffectType.VIBRATO: 
            case EffectType.VIBRATO_WITH_VOLUME_SLIDE:
                UpdateVibratoState(xValue, yValue, true, effectType);
                break;
                
            default:
                // When there is no effect, make sure to recalculate to the correct actual pitch so that a vibrato
                // effect does not leave the pitch in a bad state
                _resampler.RecalculateStep(_actualPitch, Constants.SAMPLING_RATE);
                break;
        }

        _currentEffect = effectType;
        _effectXValue = xValue;
        _effectYValue = yValue;
    }
    
    /**
     * Vibrato is tracked separately from other effects because it is "special" - it is not neatly applied at the start
     * of a tick or the start of a row like other effects. Instead, we keep track of the vibrato state continuously as
     * we generate samples. So, when a vibrato effect is given, set the variables we need, and remember the past values
     * if we are meant to continue with a currently active vibrato
     */
    private void UpdateVibratoState(int xValue, int yValue, bool continueActiveVibrato, EffectType effectType)
    {
        _vibratoSamplesElapsed = continueActiveVibrato
            ? _vibratoSamplesElapsed
            : 0;

        // Only set these values for a vibrato effect, vibrato+volume slide parameters are used for volume slide only
        if (effectType == EffectType.VIBRATO)
        {
            _vibratoCyclesPerRow = (xValue == 0)
                ? _vibratoCyclesPerRow
                : xValue * TicksPerRow / 64.0F;
            _vibratoDepth = (yValue == 0)
                ? _vibratoDepth
                : yValue;
        }

        var samplesPerRow = (Constants.SAMPLING_RATE / (BeatsPerMinute / 60.0F)) / 4;

        _vibratoSamplesPerCycle = (float)(samplesPerRow * Math.Pow(_vibratoCyclesPerRow, -1));
        _vibratoSamplesPerCyclePosition = _vibratoSamplesPerCycle / 64.0F;
    }
    
    // When a slide to note effect is given, return the new value, or the old value when the params are 0 and 0
    private int GetSlideToNotePitchShift(int xValue, int yValue)
    {
        return (xValue == 0 && yValue == 0)
            ? _slideToNotePitchShift
            : xValue * 16 + yValue;
    }

    // If there is an active vibrato effect, calculate by how much the active note's pitch needs to be adjusted
    private int GetVibratoPitchAdjustment()
    {
        if (_currentEffect != EffectType.VIBRATO && _currentEffect != EffectType.VIBRATO_WITH_VOLUME_SLIDE)
        {
            return 0;
        }
        
        var vibratoCyclePosition = _vibratoSamplesElapsed / _vibratoSamplesPerCyclePosition;
        var sineTableValue = _sineTable[(int)Math.Floor(vibratoCyclePosition)];

        return sineTableValue * _vibratoDepth / 128;
    }
    
    // Recalculate how many samples have elapsed in the vibrato effect. If we have exceeded the number of samples per
    // vibrato cycle, return to zero.
    private int GetUpdatedVibratoSamplesElapsed()
    {
        return (_vibratoSamplesElapsed + 1 >= _vibratoSamplesPerCycle)
            ? 0
            : _vibratoSamplesElapsed + 1;
    }
    
    // If a row has a fine tune value effect, return the value from that effect, otherwise return the instrument's default fine tune value
    private int GetFineTuneValue(Instrument instrument, Row row)
    {
        if (EffectType.SET_FINE_TUNE == row.Effect)
        {
            return (row.EffectYValue >= 8)
                ? row.EffectYValue - 16
                : row.EffectYValue;
        }

        return instrument.FineTune;
    }
    
    /**
     * Each fine tune value represents 1/8th of a semitone - simply divide the pitch by the triamu ratio raised to
     * the power of the number of finetunes (positive or negative).
     */
    private float GetFineTuneAdjustedPitch(float pitch, int fineTune)
    {
        return (fineTune == 0)
            ? pitch
            : pitch / (float)Math.Pow(TriamuRatio, fineTune);
    }
    
    
    private float GetPitchSlideAdjustment(int xValue, int yValue, EffectType effectType, float actualPitch)
    {
        var pitchAdjustment = xValue * 16 + yValue;
        switch (effectType)
        {
            case EffectType.PITCH_SLIDE_UP:
                return Utils.CoerceAtLeast(actualPitch - pitchAdjustment, 113.0F);

            case EffectType.PITCH_SLIDE_DOWN:
                return Utils.CoerceAtMost(actualPitch + pitchAdjustment, 856.0F);

            default:
                return actualPitch;
        }
    }
}
