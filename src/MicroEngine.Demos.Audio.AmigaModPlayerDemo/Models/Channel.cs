/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Demos.Audio.AmigaModPlayerDemo.Models;

public class Channel
{
    public List<Row> Rows { get; } = new();
    public PanningPosition PanningPosition { get; set; }
}