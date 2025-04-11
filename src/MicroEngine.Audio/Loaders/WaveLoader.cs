/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Audio.Loaders;

/// <summary>
/// Loads a WAVE file into a Sound instance.
/// </summary>
public class WaveLoader : ISoundLoader
{
    public Sound Load(Stream stream)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        using (var reader = new BinaryReader(stream))
        {
            // RIFF header
            var signature = new string(reader.ReadChars(4));
            if (signature != "RIFF")
            {
                throw new NotSupportedException("Specified stream is not a wave file.");
            }

            var riff_chunck_size = reader.ReadInt32();

            var format = new string(reader.ReadChars(4));
            if (format != "WAVE")
            {
                throw new NotSupportedException("Specified stream is not a wave file.");
            }

            // WAVE header
            var format_signature = new string(reader.ReadChars(4));
            if (format_signature != "fmt ")
            {
                throw new NotSupportedException("Specified wave file is not supported.");
            }

            int format_chunk_size = reader.ReadInt32();
            int audio_format = reader.ReadInt16();
            
            int num_channels = reader.ReadInt16();
            if (num_channels != 1 && num_channels != 2)
            {
                throw new NotSupportedException("Specified wave file is not supported.");
            }
            
            int sample_rate = reader.ReadInt32();
            if (sample_rate < 1 || sample_rate > 44100)
            {
                throw new NotSupportedException("Specified wave file is not supported.");
            }
            
            int byte_rate = reader.ReadInt32();
            int block_align = reader.ReadInt16();
            
            int bits_per_sample = reader.ReadInt16(); 
            if (bits_per_sample != 16)
            {
                throw new NotSupportedException("Specified wave file is not supported.");
            }
            
            var data_signature = new string(reader.ReadChars(4));
            if (data_signature != "data")
            {
                throw new NotSupportedException("Specified wave file is not supported.");
            }

            var data_chunk_size = reader.ReadInt32();
            
            // The sound data
            var bytes = reader.ReadBytes(data_chunk_size);
            if (bytes.Length != data_chunk_size)
            {
                throw new NotSupportedException("The number of loaded bytes does not match the data chunk size.");
            }
            
            var sound = (num_channels == 1)
                ? Sound.Create16BitMonoSound(bytes.Length / 2, sample_rate)     // 16 bit mono
                : Sound.Create16BitStereoSound(bytes.Length / 4, sample_rate);  // 16 bit stereo

            if (num_channels == 1)
            {
                for (var i = 0; i < sound.Samples.Length; i++)
                {
                    sound.Samples[i] = (short)(bytes[i * 2] | (bytes[i * 2 + 1] << 8));
                }
            }
            else
            {
                for (var i = 0; i < sound.Samples.Length; i += 2)
                {
                    sound.Samples[i] = (short)(bytes[i * 2] | (bytes[i * 2 + 1] << 8));
                    sound.Samples[i + 1] = (short)(bytes[i * 2 + 2] | (bytes[i * 2 + 3] << 8));
                }
            }
            
            return sound;
        }
    }
}
