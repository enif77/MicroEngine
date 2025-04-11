/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Audio.Loaders;

/// <summary>
/// Loads a WAVE file into a Sound instance.
/// 16bit mono or stereo sound with a single data chunk is supported only.
/// </summary>
public class WaveLoader : ISoundLoader
{
    public Sound Load(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        using (var reader = new BinaryReader(stream))
        {
            // [Master RIFF chunk]
            //      FileTypeBlocID  (4 bytes) : Identifier « RIFF »  (0x52, 0x49, 0x46, 0x46)
            //      FileSize        (4 bytes) : Overall file size minus 8 bytes
            //      FileFormatID    (4 bytes) : Format = « WAVE »  (0x57, 0x41, 0x56, 0x45)
            var signature = new string(reader.ReadChars(4));
            if (signature != "RIFF")
            {
                throw new NotSupportedException("Specified stream is not a wave file.");
            }

            // Ignored.
            var fileSize = reader.ReadInt32();
            
            var fileFormatId = new string(reader.ReadChars(4));
            if (fileFormatId != "WAVE")
            {
                throw new NotSupportedException("Specified stream is not a WAVE file.");
            }

            // [Chunk describing the data format]
            //      FormatBlocID    (4 bytes) : Identifier « fmt␣ »  (0x66, 0x6D, 0x74, 0x20)
            //      BlockSize       (4 bytes) : Chunk size minus 8 bytes, which is 16 bytes here  (0x10)
            //      AudioFormat     (2 bytes) : Audio format (1: PCM integer, 3: IEEE 754 float)
            //      NbrChannels     (2 bytes) : Number of channels
            //      Frequency       (4 bytes) : Sample rate (in hertz)
            //      BytesPerSec     (4 bytes) : Number of bytes to read per second (Frequency * BytesPerBloc).
            //      BytesPerBloc    (2 bytes) : Number of bytes per block (NbrChannels * BitsPerSample / 8).
            //      BitsPerSample   (2 bytes) : Number of bits per sample
            var formatBlockId = new string(reader.ReadChars(4));
            if (formatBlockId != "fmt ")
            {
                throw new NotSupportedException("Specified WAVE file format is not supported.");
            }

            var blockSize = reader.ReadInt32();
            if (blockSize != 16)
            {
                throw new NotSupportedException("Specified WAVE file block size is not supported.");
            }
            
            var audioFormat = reader.ReadInt16();
            if (audioFormat != 1)
            {
                throw new NotSupportedException("Specified WAVE file audio format is not supported.");
            }
            
            int nbrChannels = reader.ReadInt16();
            if (nbrChannels != 1 && nbrChannels != 2)
            {
                throw new NotSupportedException("Specified WAVE file number of channels is not supported.");
            }
            
            var frequency = reader.ReadInt32();
            if (frequency is < 1 or > 44100)
            {
                throw new NotSupportedException("Specified wave file is not supported.");
            }
            
            // Ignored.
            var bytesPerSec = reader.ReadInt32();
            var bytesPerBloc = reader.ReadInt16();
            
            int bitsPerSample = reader.ReadInt16(); 
            if (bitsPerSample != 16)
            {
                throw new NotSupportedException("Specified WAVE file bits per sample is not supported.");
            }
            
            // [Chunk containing the sampled data]
            //      DataBlocID      (4 bytes) : Identifier « data »  (0x64, 0x61, 0x74, 0x61)
            //      DataSize        (4 bytes) : SampledData size
            //      SampledData
            var dataSignature = new string(reader.ReadChars(4));
            if (dataSignature != "data")
            {
                throw new NotSupportedException("Specified wave file is not supported.");
            }

            var dataSize = reader.ReadInt32();
            
            // The sound data
            var bytes = reader.ReadBytes(dataSize);
            if (bytes.Length != dataSize)
            {
                throw new NotSupportedException("The number of loaded bytes does not match the data chunk size.");
            }
            
            // Create the sound object.
            var sound = (nbrChannels == 1)
                ? Sound.Create16BitMonoSound(bytes.Length / 2, frequency)     // 16 bit mono
                : Sound.Create16BitStereoSound(bytes.Length / 4, frequency);  // 16 bit stereo

            // Convert the byte array to short array.
            if (nbrChannels == 1)
            {
                // Mono sound
                for (var i = 0; i < sound.Samples.Length; i++)
                {
                    sound.Samples[i] = (short)(bytes[i * 2] | (bytes[i * 2 + 1] << 8));
                }
                
                return sound;
            }
        
            // Stereo sound
            for (var i = 0; i < sound.Samples.Length; i += 2)
            {
                sound.Samples[i] = (short)(bytes[i * 2] | (bytes[i * 2 + 1] << 8));
                sound.Samples[i + 1] = (short)(bytes[i * 2 + 2] | (bytes[i * 2 + 3] << 8));
            }
  
            return sound;
        }
    }
}
