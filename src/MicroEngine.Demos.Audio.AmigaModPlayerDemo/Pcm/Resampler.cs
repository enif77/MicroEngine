/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Demos.Audio.AmigaModPlayerDemo.Pcm;

using MicroEngine.Demos.Audio.AmigaModPlayerDemo.Models;

/// <summary>
/// Used to calculate a sample from the given instrument data. Contains the following fields:
/// 
/// audioDataReference: Position in the instrument audio data for the next sample.
///
/// step: Value to add to audioDataReference after retrieving the next sample. If this value decreases, so will the pitch
/// of the instrument. If it increases, the pitch will increase.
///
/// instrument: The instrument that is being resampled. Contains the audio data ByteArray and loop information.
/// </summary>
public class Resampler
{
    private const float PalClockRate = 7093789.2F;

    public float AudioDataReference { get; set; } = Constants.INSTRUMENT_STARTING_REFERENCE;
    public Instrument? Instrument { get; set; } = null;
    
    private float _step = 0.0F;

    
    /**
     * Retrieves a sample and performs linear interpolation. This follows the following steps:
     *
     * 1. Get the current sample and the subsequent sample from the audio data.
     * 2. Determine the rise between the two
     * 3. Determine the run between the two samples: How many samples we need to interpolate before we will switch to
     *    the next pair of samples
     * 4. Calculate the slope
     * 5. Multiply the slope by our current position in the run and add to the sample, which is returned as a byte
     */
    public float GetInterpolatedSample()
    {
        if (Instrument == null)
        {
            return 0.0F;
        }
        
        if (AudioDataReference >= Instrument.AudioData.Length)
        {
            return 0.0F;
        }

        // 1: Get sample and subsequent sample from the audio data
        var flooredReference = (int)Math.Floor(AudioDataReference);
        var sample = Instrument.AudioData[flooredReference];
        var subsequentSample = GetSubsequentSample(Instrument, flooredReference);

        // 2: Determine the rise
        var rise = subsequentSample - sample;

        // 3: Determine the run.
        var stepsSinceFirstStep = (int)Math.Floor((AudioDataReference - flooredReference) / _step);
        var remainingSteps = (int)Math.Floor((flooredReference + 1 - AudioDataReference) / _step);
        var run = remainingSteps + stepsSinceFirstStep + 1;

        // 4: Calculate the slope
        var slope = rise / run;

        // 4.5: Update the audio data reference before we return the sample
        AudioDataReference = GetNextAudioDataReference(AudioDataReference, _step, Instrument);

        // 5: Multiply the slop by our current position in the run and add to the sample, and return
        return sample + slope * stepsSinceFirstStep;
    }
    
    /**
     * Recalculates the step: Since the step is the key for determining the pitch, whenever the pitch changes, we will
     * need to recalculate the step.
     */
    public void RecalculateStep(float pitch, float samplingRate)
    {
        _step = (PalClockRate / (pitch * 2.0F)) / samplingRate;
    }
    
    /**
     * Returns the next sample in the original audio data. If we are at the end of the audio data array, either return 0
     * for an unlooped instrument, or return whatever is at the repeat offset for a looped instrument.
     */
    private float GetSubsequentSample(Instrument? instrument, int currentSamplePosition)
    {
        if (instrument == null)
        {
            return 0.0F;
        }
        
        // If we are at the end of the audio data array, return 0 for an unlooped instrument.
        if (currentSamplePosition + 1 >= instrument.AudioData.Length)
        {
            if (!IsInstrumentLooped(instrument))
            {
                return 0.0F;
            }

            // If the instrument is looped, return whatever value is at the repeat offset start
            return instrument.AudioData[instrument.RepeatOffsetStart * 2];
        }

        // Return the next sample in the audio data array.
        return instrument.AudioData[currentSamplePosition + 1];
    }
    

    private bool IsInstrumentLooped(Instrument? instrument)
    {
        return instrument is { RepeatLength: > 1 };
    }
    
    /**
     * Update the audio data reference by adding the step to it. If our new reference exceeds the length of the audio data,
     * and the instrument is looped, set our new reference to its position at the repeat offset (but keep the remainder)
     *
     * For unlooped instruments, there is no need to calculate this, since the resampler will just return 0 when it has
     * exceeded the length of the audio data
     */
    private float GetNextAudioDataReference(float reference, float step, Instrument? instrument)
    {
        var newReference = reference + step;
        
        if (instrument == null)
        {
            return newReference;
        }

        // If the new reference exceeds the length of the audio data, and the instrument is looped, set our new reference.
        if (IsInstrumentLooped(instrument) && (int)Math.Floor(newReference) >= instrument.AudioData.Length)
        {
            var referenceRemainder = newReference - Math.Floor(newReference);
            newReference = (float)(instrument.RepeatOffsetStart * 2.0 + referenceRemainder);
        }

        return newReference;
    }
}