/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Audio;

using OpenTK.Audio.OpenAL;

/// <summary>
/// Represents a sound source.
/// </summary>
public interface ISoundSource : IDisposable
{
    /// <summary>
    /// Is the OpenAL source initialized?
    /// </summary>
    bool IsInitialized { get; }

    /// <summary>
    /// The OpenAL source ID.
    /// </summary>
    int ALSourceId { get; }

    /// <summary>
    /// Indicates whether this sound source is looping.
    /// </summary>
    /// <exception cref="InvalidOperationException">When this sound source is not initialized yet.</exception>
    bool IsLooping { get; set; }

    /// <summary>
    /// Gets the state of the OpenAL source.
    /// </summary>
    /// <exception cref="InvalidOperationException">When this sound source is not initialized yet.</exception>
    ALSourceState State { get; }

    /// <summary>
    /// Initializes the OpenAL source.
    /// </summary>
    /// <exception cref="InvalidOperationException">If the AL source cannot be created.</exception>
    void Initialize();

    /// <summary>
    /// Attaches the buffer to the source.
    /// </summary>
    /// <param name="soundBuffer"></param>
    /// <exception cref="InvalidOperationException">When this sound source is not initialized yet.</exception>
    /// <exception cref="ArgumentNullException">If the soundBuffer parameter is null</exception>
    /// <exception cref="InvalidOperationException">When the sound buffer is not initialized.</exception>
    void AttachSoundBuffer(ISoundBuffer soundBuffer);

    /// <summary>
    /// Stops the playback of the sound buffer attached to this source and detaches it.
    /// </summary>
    void DetachSoundBuffer();
    
    /// <summary>
    /// Adds a sound buffer to the queue of this source.
    /// </summary>
    /// <param name="soundBuffer">A sound buffer to be added to this sound source's queue.</param>
    void QueueSoundBuffer(ISoundBuffer soundBuffer);
    
    /// <summary>
    /// Gets the count of sound buffers attached to this source.
    /// </summary>
    /// <returns>The count of sound buffers attached to this source.</returns>
    int GetQueuedSoundBuffersCount();
    
    /// <summary>
    /// Gets the count of sound buffers that have been processed by this source.
    /// </summary>
    /// <returns>The count of sound buffers that have been processed by this source.</returns>
    int GetProcessedSoundBuffersCount();
    
    /// <summary>
    /// Removes the first processed sound buffer from the queue of this source.
    /// </summary>
    /// <returns>The first processed sound buffer from the queue of this source or null.</returns>
    ISoundBuffer? UnqueueSoundBuffer();
    
    /// <summary>
    /// This function plays, replays or resumes a source. The playing source will have it's state changed
    /// to ALSourceState.Playing. When called on a source which is already playing, the source will restart
    /// at the beginning. When the attached buffer(s) are done playing, the source will progress to the
    /// ALSourceState.Stopped state.
    /// </summary>
    /// <exception cref="InvalidOperationException">When this sound source is not initialized.</exception>
    void Play();

    /// <summary>
    /// This function pauses a source. The paused source will have its state changed to ALSourceState.Paused.
    /// </summary>
    /// <exception cref="InvalidOperationException">When this sound source is not initialized yet.</exception>
    void Pause();

    /// <summary>
    /// This function stops a source. The stopped source will have it's state changed to ALSourceState.Stopped.
    /// </summary>
    /// <exception cref="InvalidOperationException">When this sound source is not initialized yet.</exception>
    void Stop();

    /// <summary>
    /// This function stops the source and sets its state to ALSourceState.Initial.
    /// </summary>
    /// <exception cref="InvalidOperationException">When this sound source is not initialized yet.</exception>
    void Rewind();

    /// <summary>
    /// Releases the OpenAL source and stops playback.
    /// This source can be re-initialized later.
    /// This method can be called multiple times.
    /// </summary>
    void Destroy();
}