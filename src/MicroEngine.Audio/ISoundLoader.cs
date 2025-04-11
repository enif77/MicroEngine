namespace MicroEngine.Audio;

/// <summary>
/// Defines a contract for loading sounds from a stream.
/// </summary>
public interface ISoundLoader
{
    /// <summary>
    /// Loads a sound from the specified stream.
    /// </summary>
    /// <param name="stream">A stream containing a sound or music.</param>
    /// <returns>A Sound instance containing a sound or music from the stream.</returns>
    Sound Load(Stream stream);
}
