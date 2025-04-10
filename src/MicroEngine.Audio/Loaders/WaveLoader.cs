/* Copyright (C) Premysl Fara and Contributors */

using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices; // Required for MemoryMarshal if using optimization

namespace MicroEngine.Audio.Loaders;

/// <summary>
/// Represents errors that occur during WAVE file parsing.
/// </summary>
public class WaveLoadException : Exception
{
    public WaveLoadException(string message) : base(message) { }
    public WaveLoadException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Loads a WAVE audio file into a Sound instance.
/// Currently supports uncompressed PCM format, 16-bit samples, Mono or Stereo.
/// </summary>
public static class WaveLoader // Made static as it only contains static methods
{
    private const string RiffSignature = "RIFF";
    private const string WaveFormatSignature = "WAVE";
    private const string FmtChunkSignature = "fmt ";
    private const string DataChunkSignature = "data";
    private const short PcmAudioFormat = 1; // Uncompressed PCM
    private const short SupportedBitsPerSample = 16;

    /// <summary>
    /// Loads audio data from a WAVE format stream.
    /// </summary>
    /// <param name="stream">The stream containing the WAVE file data. The stream must be readable and seekable.</param>
    /// <param name="leaveOpen">If true, the stream is left open after reading; otherwise, it's disposed.</param>
    /// <returns>A Sound instance containing the loaded audio data.</returns>
    /// <exception cref="ArgumentNullException">Thrown if stream is null.</exception>
    /// <exception cref="ArgumentException">Thrown if the stream is not readable or seekable.</exception>
    /// <exception cref="WaveLoadException">Thrown if the stream is not a valid or supported WAVE file.</exception>
    /// <exception cref="IOException">Thrown if an I/O error occurs while reading the stream.</exception>
    /// <exception cref="EndOfStreamException">Thrown if the end of the stream is reached unexpectedly.</exception>
    public static Sound Load(Stream stream, bool leaveOpen = false)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }
        if (!stream.CanRead)
        {
            throw new ArgumentException("Stream must be readable.", nameof(stream));
        }
        // Seeking might be needed if we implement skipping unknown chunks robustly later
        // if (!stream.CanSeek)
        // {
        //     throw new ArgumentException("Stream must be seekable.", nameof(stream));
        // }

        // Use BinaryReader, ensuring UTF8 isn't used for single-byte chars, and control stream disposal
        // Note: WAVE strings are typically ASCII, ReadChars might be okay, but reading bytes and checking is safer.
        using (var reader = new BinaryReader(stream, Encoding.ASCII, leaveOpen))
        {
            // --- RIFF Header ---
            byte[] riffBytes = reader.ReadBytes(4);
            int riffChunkSize = reader.ReadInt32(); // Size of the rest of the file
            byte[] waveBytes = reader.ReadBytes(4);

            if (Encoding.ASCII.GetString(riffBytes) != RiffSignature)
            {
                throw new WaveLoadException($"Invalid RIFF signature. Expected '{RiffSignature}', found '{Encoding.ASCII.GetString(riffBytes)}'.");
            }
            if (Encoding.ASCII.GetString(waveBytes) != WaveFormatSignature)
            {
                throw new WaveLoadException($"Invalid WAVE format signature. Expected '{WaveFormatSignature}', found '{Encoding.ASCII.GetString(waveBytes)}'.");
            }

            // --- Find 'fmt ' and 'data' chunks ---
            // More robust implementations would loop through chunks until 'fmt ' and 'data' are found,
            // skipping unknown ones. This version assumes 'fmt ' directly follows 'WAVE'.
            string chunkSignature;
            int chunkSize;

            // --- Format ('fmt ') Chunk ---
            chunkSignature = ReadChunkSignature(reader);
            chunkSize = reader.ReadInt32();

            if (chunkSignature != FmtChunkSignature)
            {
                throw new WaveLoadException($"Expected '{FmtChunkSignature}' chunk, found '{chunkSignature}'.");
            }

            // Read fmt chunk data
            short audioFormat = reader.ReadInt16();
            short numChannels = reader.ReadInt16();
            int sampleRate = reader.ReadInt32();
            int byteRate = reader.ReadInt32();     // Often ignored, can be calculated: SampleRate * NumChannels * BitsPerSample/8
            short blockAlign = reader.ReadInt16(); // Often ignored, can be calculated: NumChannels * BitsPerSample/8
            short bitsPerSample = reader.ReadInt16();

            // Skip any extra format bytes (e.g., for non-PCM formats or extensions)
            int expectedFmtChunkSize = 16; // Standard size for PCM
            if (chunkSize > expectedFmtChunkSize)
            {
                reader.ReadBytes(chunkSize - expectedFmtChunkSize);
            }
            else if (chunkSize < expectedFmtChunkSize)
            {
                 // This could indicate a truncated or non-standard PCM fmt chunk
                 throw new WaveLoadException($"Format chunk size is unexpectedly small ({chunkSize} bytes). Standard PCM requires at least {expectedFmtChunkSize}.");
            }

            // --- Format Validation ---
            if (audioFormat != PcmAudioFormat)
            {
                throw new WaveLoadException($"Unsupported audio format. Expected PCM (1), found {audioFormat}.");
            }
            if (numChannels != 1 && numChannels != 2)
            {
                throw new WaveLoadException($"Unsupported number of channels: {numChannels}. Only Mono (1) or Stereo (2) are supported.");
            }
            if (bitsPerSample != SupportedBitsPerSample)
            {
                throw new WaveLoadException($"Unsupported bits per sample: {bitsPerSample}. Only {SupportedBitsPerSample}-bit is supported.");
            }
            if (sampleRate <= 0) // Basic sanity check
            {
                throw new WaveLoadException($"Invalid sample rate: {sampleRate}. Must be positive.");
            }

            // --- Data ('data') Chunk ---
            // Find the data chunk, skipping others if necessary
            chunkSignature = ReadChunkSignature(reader);
            while (chunkSignature != DataChunkSignature && stream.Position < stream.Length)
            {
                chunkSize = reader.ReadInt32();
                // Skip the chunk data safely
                if (stream.CanSeek)
                {
                    stream.Seek(chunkSize, SeekOrigin.Current);
                }
                else
                {
                     // Cannot seek, must read bytes to skip
                     reader.ReadBytes(chunkSize); // This is less efficient and can fail on huge chunks
                }

                if (stream.Position >= stream.Length)
                {
                     throw new WaveLoadException($"Reached end of stream while searching for '{DataChunkSignature}' chunk.");
                }
                chunkSignature = ReadChunkSignature(reader);
            }


            if (chunkSignature != DataChunkSignature)
            {
                throw new WaveLoadException($"Could not find the '{DataChunkSignature}' chunk.");
            }

            int dataChunkSize = reader.ReadInt32();
            if (dataChunkSize <= 0)
            {
                throw new WaveLoadException($"Invalid data chunk size: {dataChunkSize}. Must be positive.");
            }


            // --- Read Audio Data ---
            byte[] rawData = reader.ReadBytes(dataChunkSize);
            if (rawData.Length != dataChunkSize)
            {
                throw new EndOfStreamException($"Expected {dataChunkSize} bytes for data chunk, but only read {rawData.Length}. Stream may be truncated.");
            }

            // --- Create Sound Object and Convert Samples ---
            int bytesPerSample = bitsPerSample / 8;
            int numSamplesTotal = dataChunkSize / bytesPerSample;

            Sound sound;
            if (numChannels == 1)
            {
                sound = Sound.Create16BitMonoSound(numSamplesTotal, sampleRate);
            }
            else // numChannels == 2
            {
                // For stereo, numSamplesTotal includes both L and R samples.
                // The Sound class likely expects the total number of *individual* samples (L+R).
                sound = Sound.Create16BitStereoSound(numSamplesTotal / 2, sampleRate); // Assuming CreateStereo expects number of *sample pairs* or *frames*
                // *Correction*: Based on the original code's loop, it seems the Sound class expects the *total number of short values* in its Samples array.
                // Let's adjust the creation call to match the original intent.
                sound = Sound.Create16BitStereoSound(numSamplesTotal, sampleRate);
                // *If* Sound.Create16BitStereoSound expected sample *frames* (pairs), it should be numSamplesTotal / 2. Clarify Sound class behavior.
                // Assuming total shorts:
                 if (numSamplesTotal != sound.Samples.Length) { /* Error or adjust logic */ }
            }


            // --- Convert Bytes to Shorts ---
            // Option 1: Original Loop (Clear and Portable)
            ConvertBytesToShortsLoop(rawData, sound.Samples, numChannels);

            // Option 2: Using MemoryMarshal (Potentially Faster, Requires Little-Endian System)
            // Check endianness if using this method for portability.
            // if (!BitConverter.IsLittleEndian) {
            //     throw new PlatformNotSupportedException("Optimized WAVE loading requires a little-endian system.");
            // }
            // ConvertBytesToShortsMarshal(rawData, sound.Samples);


            return sound;
        }
    }

    private static string ReadChunkSignature(BinaryReader reader)
    {
        byte[] signatureBytes = reader.ReadBytes(4);
        if (signatureBytes.Length < 4)
        {
            throw new EndOfStreamException("Unexpected end of stream while reading chunk signature.");
        }
        return Encoding.ASCII.GetString(signatureBytes);
    }

    // Option 1: Conversion using a loop
    private static void ConvertBytesToShortsLoop(byte[] sourceBytes, short[] destinationShorts, int numChannels)
    {
        int bytesPerSample = 2; // Assuming 16-bit
        int frameSize = bytesPerSample * numChannels;
        int sampleIndex = 0;

        for (int i = 0; i < sourceBytes.Length; i += bytesPerSample)
        {
            if (sampleIndex >= destinationShorts.Length)
            {
                 throw new WaveLoadException($"Data chunk size ({sourceBytes.Length} bytes) results in more samples than expected by the Sound object ({destinationShorts.Length} shorts). Check Sound creation logic.");
            }
            // Assumes Little-Endian format (least significant byte first)
            destinationShorts[sampleIndex++] = (short)(sourceBytes[i] | (sourceBytes[i + 1] << 8));
        }

        if (sampleIndex != destinationShorts.Length) {
             throw new WaveLoadException($"Mismatch between converted samples ({sampleIndex}) and expected samples ({destinationShorts.Length}). Data chunk size might be inconsistent with header info.");
        }
    }

    // Option 2: Conversion using MemoryMarshal (Potentially faster, needs testing)
    // Make sure the destination buffer size matches exactly.
    private static void ConvertBytesToShortsMarshal(byte[] sourceBytes, short[] destinationShorts)
    {
        if (!BitConverter.IsLittleEndian)
        {
            // Fallback to loop or throw exception if big-endian support needed
             throw new PlatformNotSupportedException("MemoryMarshal-based conversion requires a little-endian system.");
        }

        // Create a span of shorts overlaying the byte array's memory.
        // This is unsafe if the byte array length is not a multiple of sizeof(short).
        if (sourceBytes.Length % 2 != 0)
        {
            throw new WaveLoadException("Audio data size must be an even number of bytes for 16-bit samples.");
        }

        ReadOnlySpan<short> sourceShorts = MemoryMarshal.Cast<byte, short>(sourceBytes);

        if (sourceShorts.Length != destinationShorts.Length)
        {
             throw new WaveLoadException($"Mismatch between source byte data size ({sourceBytes.Length} bytes -> {sourceShorts.Length} shorts) and destination array size ({destinationShorts.Length} shorts).");
        }

        // Copy the data efficiently.
        sourceShorts.CopyTo(destinationShorts);
    }

    // TODO: Implement LoadAsync for non-blocking I/O
    // public static async Task<Sound> LoadAsync(Stream stream, bool leaveOpen = false, CancellationToken cancellationToken = default)
    // {
    //     // Similar logic using ReadAsync methods
    //     await Task.Yield(); // Placeholder
    //     throw new NotImplementedException();
    // }
}

// --- Placeholder for the Sound class ---
// This needs to exist for the code to compile.
// Adapt the Create methods and Samples property based on your actual Sound class.
public class Sound
{
    public short[] Samples { get; private set; }
    public int SampleRate { get; private set; }
    public int Channels { get; private set; }

    private Sound(int numSamples, int sampleRate, int channels)
    {
        Samples = new short[numSamples];
        SampleRate = sampleRate;
        Channels = channels;
    }

    // Assumes numSamplesTotal is the total number of shorts needed.
    public static Sound Create16BitMonoSound(int numSamplesTotal, int sampleRate)
    {
        if(numSamplesTotal <= 0 || sampleRate <= 0) throw new ArgumentOutOfRangeException("Invalid size or rate");
        return new Sound(numSamplesTotal, sampleRate, 1);
    }

    // Assumes numSamplesTotal is the total number of shorts needed (LRLRLR...).
    public static Sound Create16BitStereoSound(int numSamplesTotal, int sampleRate)
    {
         if(numSamplesTotal <= 0 || sampleRate <= 0) throw new ArgumentOutOfRangeException("Invalid size or rate");
         if (numSamplesTotal % 2 != 0) throw new ArgumentException("Total samples for stereo must be even.");
        // Example: numSamplesTotal = 10 means 5 Left + 5 Right samples.
        return new Sound(numSamplesTotal, sampleRate, 2);
    }
}
