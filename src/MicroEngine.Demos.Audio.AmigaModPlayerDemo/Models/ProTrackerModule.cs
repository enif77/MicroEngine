/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Demos.Audio.AmigaModPlayerDemo.Models;

public class ProTrackerModule
{
    public string Title { get; set; } = string.Empty;
    public List<int> OrderList { get; } = new();
    public List<Pattern> Patterns { get; } = new();
    public List<Instrument> Instruments { get; } = new();
    public int NumberOfSongPositions { get; set; }
    public int NoiseTrackerRestartPosition { get; set; }
    public string Identifier { get; set; } = string.Empty;
}
