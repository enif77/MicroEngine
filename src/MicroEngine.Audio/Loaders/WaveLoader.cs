/* Copyright (C) Premysl Fara and Contributors */

using System;
using System.Buffers.Binary; // For potential future use with BinaryPrimitives
using System.IO;
using System.Runtime.InteropServices; // Required for MemoryMarshal
using System.Text;

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
/// Supports uncompressed PCM format, 16-bit samples, Mono or Stereo.
/// </summary>
public static class WaveLoader
{
    private const int ChunkSignatureLength = 4;
    private const short PcmAudioFormat = 1;
    private const short SupportedBitsPerSample = 16;
    private const int PcmFormatChunkMinimalSize = 16; // For audioFormat to bitsPerSample

    // Precompute signature bytes for efficient comparison
    private static readonly byte[] RiffSignatureBytes = Encoding.ASCII.GetBytes("RIFF");
    private static readonly byte[] WaveFormatSignatureBytes = Encoding.ASCII.GetBytes("WAVE");
    private static readonly byte[] FmtChunkSignatureBytes = Encoding.ASCII.GetBytes("fmt ");
    private static readonly byte[] DataChunkSignatureBytes = Encoding.ASCII.GetBytes("data");

    /// <summary>
    /// Loads audio data from a WAVE format stream.
    /// </summary>
    /// <param name="stream">The stream containing the WAVE file data. Must be readable.</param>
    /// <param name="leaveOpen">If true, the stream is left open after reading; otherwise, it's disposed.</param>
    /// <returns>A Sound instance containing the loaded audio data.</returns>
    /// <exception cref="ArgumentNullException">Thrown if stream is null.</exception>
    /// <exception cref="ArgumentException">Thrown if the stream is not readable.</exception>
    /// <exception cref="WaveLoadException">Thrown if the stream is not a valid or supported WAVE file, or if required chunks are missing.</exception>
    /// <exception cref="IOException">Thrown if an I/O error occurs while reading the stream.</exception>
    /// <exception cref="EndOfStreamException">Thrown if the end of the stream is reached unexpectedly.</exception>
    /// <exception cref="PlatformNotSupportedException">Thrown if the efficient MemoryMarshal conversion cannot be used on a big-endian system.</exception>
    public static Sound Load(Stream stream, bool leaveOpen = false)
    {
        ArgumentNullException.ThrowIfNull(stream);

        if (!stream.CanRead)
        {
            throw new ArgumentException("Stream must be readable.", nameof(stream));
        }

        // Use BinaryReader, ensure correct encoding for signatures, and control stream disposal
        using (var reader = new BinaryReader(stream, Encoding.ASCII, leaveOpen))
        {
            // --- RIFF Header ---
            byte[] signatureBytes = reader.ReadBytes(ChunkSignatureLength);
            if (signatureBytes.Length < ChunkSignatureLength || !signatureBytes.AsSpan().SequenceEqual(RiffSignatureBytes))
            {
                throw new WaveLoadException($"Invalid RIFF signature. Expected '{Encoding.ASCII.GetString(RiffSignatureBytes)}'.");
            }

            int riffChunkSize = reader.ReadInt32(); // Size of the rest of the file (from 'WAVE' onwards)
                                                     // We could add validation: stream.Length should approx = riffChunkSize + 8

            byte[] formatBytes = reader.ReadBytes(ChunkSignatureLength);
            if (formatBytes.Length < ChunkSignatureLength || !formatBytes.AsSpan().SequenceEqual(WaveFormatSignatureBytes))
            {
                throw new WaveLoadException($"Invalid WAVE format signature. Expected '{Encoding.ASCII.GetString(WaveFormatSignatureBytes)}'.");
            }

            // --- Find and Process 'fmt ' and 'data' chunks ---
            bool fmtChunkRead = false;
            bool dataChunkFound = false; // We find it first, read later
            long dataChunkPosition = -1;
            int dataChunkSize = 0;

            // Format details - initialized to invalid values
            short audioFormat = -1;
            short numChannels = -1;
            int sampleRate = -1;
            short bitsPerSample = -1;
            // int byteRate = -1; // Calculated or read, often unused directly if PCM
            // short blockAlign = -1; // Calculated or read, often unused directly if PCM

            while (stream.Position < stream.Length)
            {
                byte[] chunkSignature = reader.ReadBytes(ChunkSignatureLength);
                if (chunkSignature.Length < ChunkSignatureLength)
                {
                    // Avoid exception if we are at the very end perhaps due to padding.
                    if (stream.Position >= stream.Length) break;
                    throw new EndOfStreamException("Unexpected end of stream while reading chunk signature.");
                }

                int chunkSize = reader.ReadInt32();
                if (chunkSize < 0)
                {
                     throw new WaveLoadException($"Invalid negative chunk size encountered: {chunkSize}.");
                }

                long nextChunkPosition = stream.Position + chunkSize;
                 // RIFF chunks are often padded to an even number of bytes, handle this when skipping
                if (chunkSize % 2 != 0)
                {
                    nextChunkPosition++;
                }


                if (chunkSignature.AsSpan().SequenceEqual(FmtChunkSignatureBytes))
                {
                    if (fmtChunkRead)
                    {
                        throw new WaveLoadException("Duplicate 'fmt ' chunk found.");
                    }

                    // --- Format ('fmt ') Chunk ---
                    if (chunkSize < PcmFormatChunkMinimalSize)
                    {
                        throw new WaveLoadException($"Format chunk size is too small ({chunkSize} bytes). Standard PCM requires at least {PcmFormatChunkMinimalSize}.");
                    }

                    audioFormat = reader.ReadInt16();
                    numChannels = reader.ReadInt16();
                    sampleRate = reader.ReadInt32();
                    /* byteRate = */ reader.ReadInt32(); // Read but ignore for basic PCM validation
                    /* blockAlign = */ reader.ReadInt16(); // Read but ignore for basic PCM validation
                    bitsPerSample = reader.ReadInt16();

                    // Skip any extra format bytes (e.g., for non-PCM formats or extensions)
                    int bytesRead = PcmFormatChunkMinimalSize;
                    if (chunkSize > bytesRead)
                    {
                        int remainingBytes = chunkSize - bytesRead;
                         if (stream.Position + remainingBytes > nextChunkPosition) // Sanity check
                        {
                             throw new WaveLoadException("Error reading or skipping extra format chunk bytes.");
                        }
                        reader.ReadBytes(remainingBytes); // Skip extra data
                    }
                    fmtChunkRead = true;

                    // --- Basic Format Validation ---
                    if (audioFormat != PcmAudioFormat)
                    {
                        throw new WaveLoadException($"Unsupported audio format. Expected PCM ({PcmAudioFormat}), found {audioFormat}.");
                    }
                    if (numChannels != 1 && numChannels != 2)
                    {
                        throw new WaveLoadException($"Unsupported number of channels: {numChannels}. Only Mono (1) or Stereo (2) are supported.");
                    }
                    if (bitsPerSample != SupportedBitsPerSample)
                    {
                        throw new WaveLoadException($"Unsupported bits per sample: {bitsPerSample}. Only {SupportedBitsPerSample}-bit is supported.");
                    }
                     if (sampleRate <= 0)
                    {
                        throw new WaveLoadException($"Invalid sample rate: {sampleRate}. Must be positive.");
                    }
                }
                else if (chunkSignature.AsSpan().SequenceEqual(DataChunkSignatureBytes))
                {
                     if (dataChunkFound)
                    {
                        throw new WaveLoadException("Duplicate 'data' chunk found."); // Or potentially allow multiple? Standard WAVE has one.
                    }
                    // --- Data ('data') Chunk ---
                    dataChunkSize = chunkSize;
                    dataChunkPosition = stream.Position; // Remember where data starts
                    dataChunkFound = true;

                     // Skip the data chunk for now; we'll read it after finding 'fmt '
                    stream.Position = nextChunkPosition; // Use calculated next position including padding

                }
                else // Unknown chunk type
                {
                    // Skip the chunk data safely
                     string unknownSig = Encoding.ASCII.GetString(chunkSignature);
                     System.Diagnostics.Debug.WriteLine($"Skipping unknown chunk '{unknownSig}' of size {chunkSize}"); // Optional logging
                     stream.Position = nextChunkPosition; // Use calculated next position including padding
                }

                 // Ensure we don't read past the calculated end of the chunk
                 if(stream.Position > nextChunkPosition && nextChunkPosition >= 0) {
                      // This case should ideally not happen if chunkSize is correct and seeking works.
                      // If it does, it indicates a problem with the file or skipping logic.
                       throw new WaveLoadException($"Error skipping chunk. Read past expected end. Current Pos: {stream.Position}, Expected End: {nextChunkPosition}");
                 }
                 // If skipping failed to advance, manually advance if needed (e.g., non-seekable stream and read failed)
                 // This basic skipping relies on stream.Position being updated correctly by ReadBytes or Seek.

                 // Optimization: If we have found both, we can potentially break early
                 // However, parsing all chunks ensures we handle files where 'data' might appear before 'fmt ' (unlikely but possible)
                 // if (fmtChunkRead && dataChunkFound) break;
            }

            // --- Validation After Parsing Chunks ---
            if (!fmtChunkRead)
            {
                throw new WaveLoadException($"Required '{Encoding.ASCII.GetString(FmtChunkSignatureBytes)}' chunk not found.");
            }
            if (!dataChunkFound)
            {
                throw new WaveLoadException($"Required '{Encoding.ASCII.GetString(DataChunkSignatureBytes)}' chunk not found.");
            }
            if (dataChunkPosition < 0)
            {
                throw new WaveLoadException("Internal error: Data chunk position not recorded."); // Should not happen if dataChunkFound is true
            }
             if (dataChunkSize <= 0)
            {
                // Allow zero-size data chunk? Technically valid but results in an empty sound.
                 System.Diagnostics.Debug.WriteLine($"Warning: Data chunk size is {dataChunkSize}. Resulting sound will be empty.");
                 // Let's allow it and return an empty sound object. If this is undesirable, throw here:
                 // throw new WaveLoadException($"Invalid data chunk size: {dataChunkSize}. Must be positive.");
            }


            // --- Read Audio Data ---
            stream.Position = dataChunkPosition; // Seek back to the start of the data

            // Check if the declared data chunk size exceeds stream bounds
            if (dataChunkPosition + dataChunkSize > stream.Length)
            {
                 throw new WaveLoadException($"Data chunk size ({dataChunkSize} bytes) exceeds the available stream data ({(stream.Length - dataChunkPosition)} bytes remaining). File may be truncated or header incorrect.");
            }

            byte[] rawData = Array.Empty<byte>();
            if (dataChunkSize > 0) // Only read if there's data to read
            {
                 rawData = reader.ReadBytes(dataChunkSize);
                 if (rawData.Length != dataChunkSize)
                 {
                    // This might indicate an issue with the stream itself if the length check passed earlier.
                    throw new EndOfStreamException($"Expected {dataChunkSize} bytes for data chunk, but only read {rawData.Length}. Stream reading failed unexpectedly.");
                 }
            }


            // --- Create Sound Object and Convert Samples ---
            int bytesPerSample = bitsPerSample / 8; // Should be 2 for 16-bit
            int numSamplesTotal = (dataChunkSize > 0) ? dataChunkSize / bytesPerSample : 0;

            // **Important**: Verify how your `Sound` class expects stereo data.
            // Assumption: The `Sound` class expects the *total number of short values* in its Samples array (L, R, L, R...).
            // If it expects the number of *sample frames* (pairs), the creation logic needs adjustment.
            Sound sound;
            if (numChannels == 1)
            {
                sound = Sound.Create16BitMonoSound(numSamplesTotal, sampleRate);
                if (numSamplesTotal != sound.Samples.Length)
                {
                    throw new WaveLoadException($"Mismatch creating Mono Sound: Expected {numSamplesTotal} samples, Sound object holds {sound.Samples.Length}. Check Sound.Create16BitMonoSound.");
                }
            }
            else // numChannels == 2
            {
                // Assuming CreateStereoSound expects the total number of shorts (L+R samples)
                sound = Sound.Create16BitStereoSound(numSamplesTotal, sampleRate);
                 if (numSamplesTotal != sound.Samples.Length)
                {
                    throw new WaveLoadException($"Mismatch creating Stereo Sound: Expected {numSamplesTotal} samples (L+R), Sound object holds {sound.Samples.Length}. Check Sound.Create16BitStereoSound.");
                }
                // If Create16BitStereoSound expected *sample frames* (pairs), it should be called with numSamplesTotal / 2
                // and the Samples array size check should reflect that.
            }

            // --- Convert Bytes to Shorts ---
             if (rawData.Length > 0) // Only convert if there is data
             {
                ConvertBytesToShortsMarshal(rawData, sound.Samples);
                // Or use the loop version if needed:
                // ConvertBytesToShortsLoop(rawData, sound.Samples);
             }

            return sound;
        }
    }

    // Conversion using MemoryMarshal (Generally faster on Little-Endian systems)
    private static void ConvertBytesToShortsMarshal(byte[] sourceBytes, short[] destinationShorts)
    {
        if (!BitConverter.IsLittleEndian)
        {
            // Option 1: Throw - Simplest if big-endian isn't a target.
            throw new PlatformNotSupportedException("Optimized WAVE loading using MemoryMarshal requires a little-endian system.");

            // Option 2: Fallback to loop (if ConvertBytesToShortsLoop is kept and handles endianness correctly - current loop assumes LE)
            // Console.WriteLine("Warning: Falling back to slower byte conversion loop on Big-Endian system.");
            // ConvertBytesToShortsLoop(sourceBytes, destinationShorts); // Ensure loop handles BE if needed
            // return;
        }

        // Ensure the source data length is compatible with short conversion.
        if (sourceBytes.Length % sizeof(short) != 0)
        {
            throw new WaveLoadException($"Audio data size ({sourceBytes.Length} bytes) must be an even number for 16-bit samples conversion.");
        }

        int shortsCount = sourceBytes.Length / sizeof(short);
        if (shortsCount != destinationShorts.Length)
        {
             throw new WaveLoadException($"Data size mismatch: Source data translates to {shortsCount} shorts, but destination array expects {destinationShorts.Length} shorts.");
        }
        if (shortsCount == 0) return; // Nothing to copy


        // Create a ReadOnlySpan<short> view over the byte array's memory.
        ReadOnlySpan<short> sourceShorts = MemoryMarshal.Cast<byte, short>(sourceBytes);

        // Copy the data efficiently.
        sourceShorts.CopyTo(destinationShorts);
    }

    // Kept for reference or fallback: Conversion using a simple loop (Assumes Little-Endian)
    private static void ConvertBytesToShortsLoop(byte[] sourceBytes, short[] destinationShorts)
    {
        int bytesPerSample = 2; // 16-bit
        int sampleIndex = 0;

        if (sourceBytes.Length % bytesPerSample != 0)
        {
             throw new WaveLoadException($"Audio data size ({sourceBytes.Length} bytes) must be an even number for 16-bit samples loop conversion.");
        }
         if ((sourceBytes.Length / bytesPerSample) != destinationShorts.Length)
        {
             throw new WaveLoadException($"Data size mismatch during loop conversion: Source implies {sourceBytes.Length / bytesPerSample} shorts, destination expects {destinationShorts.Length}.");
        }
        if (destinationShorts.Length == 0) return; // Nothing to convert

        for (int i = 0; i < sourceBytes.Length; i += bytesPerSample)
        {
            // Assumes Little-Endian format (least significant byte first)
            destinationShorts[sampleIndex++] = (short)(sourceBytes[i] | (sourceBytes[i + 1] << 8));
        }
    }

    // TODO: Implement LoadAsync for non-blocking I/O using ReadAsync methods
    // public static async Task<Sound> LoadAsync(Stream stream, bool leaveOpen = false, CancellationToken cancellationToken = default)
    // {
    //     ArgumentNullException.ThrowIfNull(stream);
    //     // ... Check CanRead etc ...
    //     // Use await reader.ReadBytesAsync(...) / stream.ReadAsync(...)
    //     // Ensure cancellationToken is passed and checked appropriately.
    //     await Task.Yield(); // Placeholder
    //     throw new NotImplementedException();
    // }
}


// --- Placeholder for the Sound class ---
// Adapt this based on your actual Sound class implementation.
// Pay close attention to how stereo samples are stored and counted.
public class Sound
{
    public short[] Samples { get; private set; }
    public int SampleRate { get; private set; }
    public int Channels { get; private set; }

    // Private constructor ensures creation via static methods
    private Sound(int numTotalSamples, int sampleRate, int channels)
    {
        // Basic validation inside constructor
        if (numTotalSamples < 0) throw new ArgumentOutOfRangeException(nameof(numTotalSamples), "Number of samples cannot be negative.");
        if (sampleRate <= 0) throw new ArgumentOutOfRangeException(nameof(sampleRate), "Sample rate must be positive.");
        if (channels < 1) throw new ArgumentOutOfRangeException(nameof(channels), "Number of channels must be at least 1.");
        if (channels == 2 && numTotalSamples % 2 != 0) throw new ArgumentException("Total samples for stereo sound must be an even number.", nameof(numTotalSamples));


        Samples = new short[numTotalSamples];
        SampleRate = sampleRate;
        Channels = channels;
    }

    // Creates a Mono sound.
    // numSamplesTotal: The exact number of individual short values required for the sample data.
    public static Sound Create16BitMonoSound(int numSamplesTotal, int sampleRate)
    {
        return new Sound(numSamplesTotal, sampleRate, 1);
    }

    // Creates a Stereo sound.
    // numSamplesTotal: The exact total number of individual short values required for the *interleaved* sample data (L, R, L, R...).
    // Example: A 1-second, 44100Hz stereo sound would have numSamplesTotal = 44100 * 2 = 88200.
    public static Sound Create16BitStereoSound(int numSamplesTotal, int sampleRate)
    {
         // Constructor now handles the validation for even number of samples.
        return new Sound(numSamplesTotal, sampleRate, 2);
    }
}
