/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Demos.Audio.SpatialAudioDemo;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

using MicroEngine;
using MicroEngine.Audio;
using MicroEngine.Extensions;
using MicroEngine.Extensions.Audio.Generators;
using MicroEngine.Extensions.Generators.Textures;
using MicroEngine.Extensions.Generators.SceneObjects;
using MicroEngine.Managers;
using MicroEngine.Materials;
using MicroEngine.OGL;
using MicroEngine.SceneObjects.Cameras;
using MicroEngine.Shaders;

public class Game : IGame, IDisposable
{
    private Scene? _scene;
    private ISceneObject? _listenerCube;
    private ISceneObject? _soundSourceCube;
    private Mixer? _mixer;
    private ISoundSource? _soundSource;
    
    
    public string Name => "spatial-audio-demo";

    
    public bool Initialize()
    {
        // We do not want to see back faces of the cube.
        GlRenderer.EnableFaceCulling();
        
        // The scene is the root of the scene graph.
        var scene = new Scene();
        
        // Each scene has a camera.
        scene.SetCamera(new FpsCamera()
        {
            Position = Vector3.UnitZ * 1.5f
        });

        // Generate a white texture.
        var listenerCubeTexture = BasicColorTexturesGenerator.GenerateWhiteTexture(256, 256);
        
        // Generate 8x8 blocks black and white checkers pattern into the texture.
        var blockSize = listenerCubeTexture.Width / 8;
        for (var y = 0; y < listenerCubeTexture.Height; y++)
        {
            for (var x = 0; x < listenerCubeTexture.Width; x++)
            {
                var i = (x / blockSize) % 2;
                var j = (y / blockSize) % 2;
                
                if ((i + j) % 2 == 0)
                {
                    listenerCubeTexture.PutPixel(x, y, 0, 0, 0);
                }
            }
        }
        
        var soundSourceCubeTexture = BasicColorTexturesGenerator.GenerateRedTexture(256, 256);
        
        // We are using the resources manager to keep track of the resources (the texture and the shader).
        var resourcesManager = ResourcesManager.Instance;
        
        // Create a simple texture shader.
        var simpleTextureShader = new SimpleTextureShader();
        
        // Build the shader.
        simpleTextureShader.Build();
        
        // Generate a cube with the texture.
        var listenerCube = TexturedCubeGenerator.Generate(
            new SimpleTextureMaterial(
                resourcesManager.LoadTextureFromRgbaBytes(
                    "listener-cube-texture",
                    listenerCubeTexture.Pixels,
                    listenerCubeTexture.Width,
                    listenerCubeTexture.Height,
                    TextureWrapMode.ClampToEdge),
                simpleTextureShader));
        
        // Set the listener cube's position and build its geometry.
        listenerCube.Position = new Vector3(0.0f, 0.0f, 0.0f);
        listenerCube.Scale = 0.5f;
        listenerCube.BuildGeometry();
        
        // Generate the sound source cube.
        var soundSourceCube = TexturedCubeGenerator.Generate(
            new SimpleTextureMaterial(
                resourcesManager.LoadTextureFromRgbaBytes(
                    "sound-source-cube-texture",
                    soundSourceCubeTexture.Pixels,
                    soundSourceCubeTexture.Width,
                    soundSourceCubeTexture.Height,
                    TextureWrapMode.ClampToEdge),
                simpleTextureShader));
        
        // Set the sound source cube's position and build its geometry.
        soundSourceCube.Position = new Vector3(0.0f, 0.0f, -5.0f);
        soundSourceCube.Scale = 0.1f;
        soundSourceCube.BuildGeometry();
        
        // The sound source cube is a child of the listener cube.
        // This means that the sound source cube will fly around the listener cube.
        listenerCube.AddChild(soundSourceCube);
        
        // Add the cube to the scene.
        scene.AddChild(listenerCube);
        
        // Remember the scene and the cube.
        _listenerCube = listenerCube;
        _soundSourceCube = soundSourceCube;
        _scene = scene;
        
        
        //  Sound setup.
        _mixer = new Mixer();
        
        _mixer.Initialize();
        
        _mixer.ListenerPosition = _listenerCube.Position;
        _mixer.ListenerVelocity = Vector3.Zero;
        
        // The listener is looking at the negative Z axis.
        // The up vector is the negative Y axis.
        _mixer.ListenerOrientation = (new Vector3(0.0f, 0.0f, -1.0f), new Vector3(0.0f, -1.0f, 0.0f));
        
        var buffer = _mixer.CreateSoundBuffer();
        buffer.LoadData(SoundsGenerator.Generate16BitSineWaveMonoSound(44100, 44100, 440, 0.5));
        
        _soundSource = _mixer.CreateSoundSource();
        _soundSource.AttachSoundBuffer(buffer);
        _soundSource.IsLooping = true;
        _soundSource.Position = _soundSourceCube.Position;
            
        // Play the sound using the sound source.
        _soundSource.Play();
        
        return true;
    }

    
    private float _angleX;
    private float _angleY;
    private float _angleZ;

    private float _passedTime = 0;
    
    public bool Update(float deltaTime)
    {
        if (_scene == null)
        {
            throw new InvalidOperationException("The scene is not initialized.");
        }
     
        // Get the actual keyboard state.
        var keyboardState = InputManager.Instance.KeyboardState;
        
        // If the user presses the Escape key, we will exit the application.
        if (keyboardState.IsKeyDown(Keys.Escape))
        {
            // We will exit the application.
            return false;
        }

        // Calculate the cube rotation angles.
        // _angleX += 20.0f * deltaTime;
        // while (_angleX > 360.0f)
        // {
        //     _angleX -= 360.0f;
        // }
        
        _angleY += 50.0f * deltaTime;
        while (_angleY > 360.0f)
        {
            _angleY -= 360.0f;
        }

        // _angleZ += 10.0f * deltaTime;
        // while (_angleZ > 360.0f)
        // {
        //     _angleZ -= 360.0f;
        // }
        
        // Set the cube's rotation angles.
        _listenerCube!.Rotation = new Vector3(
            MathHelper.DegreesToRadians(_angleX),
            MathHelper.DegreesToRadians(_angleY),
            MathHelper.DegreesToRadians(_angleZ));
        
        // Update the scene.
        _scene.Update(deltaTime);
        
        
        // Update the sound source and the listener positions.
        _mixer!.ListenerPosition = _listenerCube.WorldPosition;
        _soundSource!.Position = _soundSourceCube!.WorldPosition;
        
        
        // Write out the sound source position to the console every 1 second.
        _passedTime += deltaTime;
        if (_passedTime > 1.0f)
        {
            _passedTime = 0;
            
            var soundSourcePosition = _soundSource.Position;
            Console.WriteLine($"Sound source position: {soundSourcePosition.X}, {soundSourcePosition.Y}, {soundSourcePosition.Z}");
            
            var listenerPosition = _mixer.ListenerPosition;
            Console.WriteLine($"Listener position: {listenerPosition.X}, {listenerPosition.Y}, {listenerPosition.Z}");
        }
        
        
        // We are not exiting the application yet.
        return true;
    }
    
    public void Render()
    {
        if (_scene == null)
        {
            throw new InvalidOperationException("The scene is not initialized.");
        }
        
        _scene.Render();
    }

    
    private void ReleaseUnmanagedResources()
    {
        if (_mixer != null)
        {
            _mixer.Dispose();
            _mixer = null;
        }
    }

    
    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~Game()
    {
        ReleaseUnmanagedResources();
    }
}
