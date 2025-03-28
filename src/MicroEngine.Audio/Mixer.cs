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
    /// <exception cref="InvalidOperationException">Thrown when failed to initialize.</exception>
    public void Initialize()
    {
        if (_isInitialized)    
        {
            return;
        }
        
        _device = ALC.OpenDevice(null);
        if (_device == ALDevice.Null)
        {
            throw new InvalidOperationException("Failed to open audio device.");
        }
        
        _context = ALC.CreateContext(_device, new ALContextAttributes());
        if (_context == ALContext.Null)
        {
            ALC.CloseDevice(_device);
            _device = ALDevice.Null;
            
            throw new InvalidOperationException("Failed to create audio context.");
        }

        var makeContextCurrentResult = ALC.MakeContextCurrent(_context);
        if (!makeContextCurrentResult)
        {
            ALC.DestroyContext(_context);
            _context = ALContext.Null;
            
            ALC.CloseDevice(_device);
            _device = ALDevice.Null;
            
            throw new InvalidOperationException("Failed to make audio context current.");
        }
        
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
        
        ReleaseUnmanagedResources();
        
        _isInitialized = false;
    }
    

    private void ReleaseUnmanagedResources()
    {
        if (!_isInitialized)
        {
            return;
        }
        
        // Close opened audio sources.
        // Close allocated audio buffers.
        // ...
        
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