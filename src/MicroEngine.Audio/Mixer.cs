using OpenTK.Audio.OpenAL;

namespace MicroEngine.Audio;

/// <summary>
/// The general audio mixer.
/// </summary>
public sealed class Mixer : IDisposable
{
    private bool _isInitialized;
    private ALDevice _device = ALDevice.Null;
    private ALContext _context = ALContext.Null;
    
    public static string GetAudioContextInfoJson()
    {
        var audioContextInfo = new AudioContextInfo
        {
            Version = AL.Get(ALGetString.Version),
            Vendor = AL.Get(ALGetString.Vendor),
            Renderer = AL.Get(ALGetString.Renderer)
        };
        
        return System.Text.Json.JsonSerializer.Serialize(audioContextInfo);
    }

    
    /// <summary>
    /// Initializes the audio mixer.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public void Initialize()
    {
        if (_isInitialized)    
        {
            throw new InvalidOperationException("The audio mixer was already initialized.");
        }
        
        _device = ALC.OpenDevice(null);
        _context = ALC.CreateContext(_device, new ALContextAttributes());

        ALC.MakeContextCurrent(_context);
        
        _isInitialized = true;
    }
    
    
    /// <summary>
    /// Cleans up the audio mixer.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public void Shutdown()
    {
        if (!_isInitialized)
        {
            throw new InvalidOperationException("The audio mixer was not initialized.");
        }
        
        // Close audio sources.
        // Close audio buffers.
        // ...
        
        ReleaseUnmanagedResources();
        
        _isInitialized = false;
    }
    

    private void ReleaseUnmanagedResources()
    {
        if (!_isInitialized)
        {
            return;
        }
        
        if (_context != ALContext.Null) {
            ALC.MakeContextCurrent(ALContext.Null);
            ALC.DestroyContext(_context);
        }
        _context = ALContext.Null;

        if (_device != IntPtr.Zero) {
            ALC.CloseDevice(_device);
        }
        _device = ALDevice.Null;
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~Mixer()
    {
        ReleaseUnmanagedResources();
    }
}