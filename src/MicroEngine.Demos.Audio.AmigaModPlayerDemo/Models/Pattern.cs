/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Demos.Audio.AmigaModPlayerDemo.Models;

public class Pattern
{
    public int PatternNumber { get; set; }
    public List<Channel> Channels { get; } = new();
}