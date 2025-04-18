/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Demos.Audio.AmigaModPlayerDemo.Models;

public class Instrument
{
    public string Name { get; set; } = string.Empty;
    public short Length { get; set; }
    public int  FineTune { get; set; }
    public int Volume { get; set; }
    public short RepeatOffsetStart { get; set; }
    public short RepeatLength { get; set; }
    public float[] AudioData { get; set; } = [];
}
