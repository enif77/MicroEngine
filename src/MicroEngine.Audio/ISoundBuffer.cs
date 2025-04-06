/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Audio;

/// <summary>
/// Represents a sound buffer.
/// </summary>
public interface ISoundBuffer : IDisposable
{
    /// <summary>
    /// Is the OpenAL buffer initialized?
    /// </summary>
    bool IsInitialized { get; }

    /// <summary>
    /// The OpenAL buffer ID.
    /// </summary>
    int ALBufferId { get; }

    /// <summary>
    /// Initializes the OpenAL buffer.
    /// </summary>
    /// <exception cref="InvalidOperationException">If the AL buffer cannot be created.</exception>
    void Initialize();

    /// <summary>
    /// This function fills this AL buffer with audio data.
    /// </summary>
    /// <param name="sound">A sound.</param>
    /// <exception cref="ArgumentNullException">If the sound parameter is null</exception>
    void LoadData(Sound sound);

    /// <summary>
    /// Releases the OpenAL buffer.
    /// This sound buffer can be re-initialized later.
    /// This method can be called multiple times.
    /// </summary>
    void Destroy();
}
