/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Demos.Audio.AmigaModPlayerDemo.Pcm;

public static class Utils
{
    /// <summary>
    /// Coerces the value to be at least the minimum value and at most the maximum value.
    /// </summary>
    /// <param name="value">A value.</param>
    /// <param name="min">The minimal value.</param>
    /// <param name="max">The maximal value.</param>
    /// <returns></returns>
    public static float Coerce(float value, float min, float max)
    {
        return value < min
            ? min
            : value > max
                ? max
                : value;
    }
    
    /// <summary>
    /// Coerces the value to be at least the minimum value and at most the maximum value.
    /// </summary>
    /// <param name="value">A value.</param>
    /// <param name="min">The minimal value.</param>
    /// <param name="max">The maximal value.</param>
    /// <returns></returns>
    public static int Coerce(int value, int min, int max)
    {
        return value < min
            ? min
            : value > max
                ? max
                : value;
    }
    
    /// <summary>
    /// Coerces the value to be at least the minimum value.
    /// </summary>
    /// <param name="value">A value.</param>
    /// <param name="min">The minimal value.</param>
    /// <returns></returns>
    public static float CoerceAtLeast(float value, float min)
    {
        return value < min
            ? min
            : value;
    }
    
    /// <summary>
    /// Coerces the value to be at least the minimum value.
    /// </summary>
    /// <param name="value">A value.</param>
    /// <param name="min">The minimal value.</param>
    /// <returns></returns>
    public static int CoerceAtLeast(int value, int min)
    {
        return value < min
            ? min
            : value;
    }
    
    /// <summary>
    /// Coerces the value to be at most the maximum value.
    /// </summary>
    /// <param name="value">A value.</param>
    /// <param name="max">The maximal value.</param>
    /// <returns></returns>
    public static float CoerceAtMost(float value, float max)
    {
        return value > max
            ? max
            : value;
    }
    
    /// <summary>
    /// Coerces the value to be at most the maximum value.
    /// </summary>
    /// <param name="value">A value.</param>
    /// <param name="max">The maximal value.</param>
    /// <returns></returns>
    public static int CoerceAtMost(int value, int max)
    {
        return value > max
            ? max
            : value;
    }
}
