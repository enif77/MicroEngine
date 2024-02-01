/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngineDemoApp.Games;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

using MicroEngine;
using MicroEngine.Core;
using MicroEngine.Extensions;
using MicroEngine.Lights;
using MicroEngine.Managers;
using MicroEngine.Materials;
using MicroEngine.SceneObjects;
using MicroEngine.Shaders;

public class ManySimpleCubesDemo : IGame
{
    private Scene? _scene;
    
    public string Name => "many-simple-cubes-demo";

    
    public bool Initialize(int width, int height)
    {
        var scene = new Scene(
            CreateCamera(width, height));
        
        //scene.AddSkybox(CreateSkybox());
        scene.AddSkybox(GenerateStarsSkybox());

        var cubeMaterial = new Material(
            Texture.LoadFromFile("Resources/Textures/container2.png"),
            Texture.LoadFromFile("Resources/Textures/container2_specular.png"),
            new DefaultShader());
        
        // Generates 1000 cubes in a 10x10x10 grid.
        for (var x = -5; x < 5; x++)
        {
            for (var y = -5; y < 5; y++)
            {
                for (var z = -5; z < 5; z++)
                {
                    scene.AddChild(CreateCube(cubeMaterial, new Vector3(x, y, z)));
                }
            }
        }
        
        scene.AddLight(new DirectionalLight(scene.Lights.Count)
        {
            Ambient = new Vector3(0.7f, 0.7f, 0.7f),
        });
       
        _scene = scene;
        
        Renderer.EnableFaceCulling();
        
        return true;
    }

    
    private bool _firstMove = true;
    private Vector2 _lastPos;
    
    public bool Update(float deltaTime)
    {
        var keyboardState = InputManager.Instance.KeyboardState;
        var mouseState = InputManager.Instance.MouseState;
        
        if (_scene == null)
        {
            throw new InvalidOperationException("The scene is not initialized.");
        }
     
        if (keyboardState.IsKeyDown(Keys.Escape))
        {
            return false;
        }
        
        
        const float cameraSpeed = 1.5f;
        const float sensitivity = 0.2f;
        
        if (keyboardState.IsKeyDown(Keys.W))
        {
            _scene.Camera.Position += _scene.Camera.Front * cameraSpeed * deltaTime; // Forward
        }
        if (keyboardState.IsKeyDown(Keys.S))
        {
            _scene.Camera.Position -= _scene.Camera.Front * cameraSpeed * deltaTime; // Backwards
        }
        if (keyboardState.IsKeyDown(Keys.A))
        {
            _scene.Camera.Position -= _scene.Camera.Right * cameraSpeed * deltaTime; // Left
        }
        if (keyboardState.IsKeyDown(Keys.D))
        {
            _scene.Camera.Position += _scene.Camera.Right * cameraSpeed * deltaTime; // Right
        }
        if (keyboardState.IsKeyDown(Keys.Space))
        {
            _scene.Camera.Position += _scene.Camera.Up * cameraSpeed * deltaTime; // Up
        }
        if (keyboardState.IsKeyDown(Keys.LeftShift))
        {
            _scene.Camera.Position -= _scene.Camera.Up * cameraSpeed * deltaTime; // Down
        }

        var mouse = mouseState;

        if (_firstMove)
        {
            _lastPos = new Vector2(mouse.X, mouse.Y);
            _firstMove = false;
        }
        else
        {
            var deltaX = mouse.X - _lastPos.X;
            var deltaY = mouse.Y - _lastPos.Y;
            _lastPos = new Vector2(mouse.X, mouse.Y);

            _scene.Camera.Yaw += deltaX * sensitivity;
            _scene.Camera.Pitch -= deltaY * sensitivity;
        }

        _scene.Update(deltaTime);
        
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

    
    public void SetCameraAspectRatio(float aspectRatio)
    {
        if (_scene == null)
        {
            throw new InvalidOperationException("The scene is not initialized.");
        }
        
        _scene.Camera.AspectRatio = aspectRatio;
    }
    
    
    #region creators and generators

    private Camera CreateCamera(int windowWidth, int windowHeight)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(windowWidth);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(windowHeight);

        return new Camera(Vector3.UnitZ * 5, windowWidth / (float)windowHeight);
    }
    
    
    // private ISceneObject CreateSkybox()
    // {
    //     var skybox = new MultiTextureSkyboxWithIndices(new MultiTextureMaterial(
    //         [
    //             Texture.LoadFromFile($"Resources/Textures/Skyboxes/TestSkybox/pz.jpg", TextureWrapMode.ClampToEdge),
    //             Texture.LoadFromFile($"Resources/Textures/Skyboxes/TestSkybox/px.jpg", TextureWrapMode.ClampToEdge),
    //             Texture.LoadFromFile($"Resources/Textures/Skyboxes/TestSkybox/nz.jpg", TextureWrapMode.ClampToEdge),
    //             Texture.LoadFromFile($"Resources/Textures/Skyboxes/TestSkybox/nx.jpg", TextureWrapMode.ClampToEdge),
    //             Texture.LoadFromFile($"Resources/Textures/Skyboxes/TestSkybox/py.jpg", TextureWrapMode.ClampToEdge),
    //             Texture.LoadFromFile($"Resources/Textures/Skyboxes/TestSkybox/ny.jpg", TextureWrapMode.ClampToEdge)
    //         ],
    //         new MultiTextureShader()));
    //     
    //     skybox.GenerateGeometry();
    //     
    //     return skybox;
    // }


    private ISceneObject GenerateStarsSkybox()
    {
        const int textureSize = 1024;
        const int nStars = 256;
        // const int starSize = 5;
        
        // RGBA texture buffer
        var texture = new byte[textureSize * textureSize * 4];

        // Clear the texture.
        for (var i = 0; i < texture.Length; i++)
        {
            texture[i] = 0;
        }

        //var rand = new Random(5728);
        var rand = new Random();
        var textures = new ITexture[6];
        for (var t = 0; t < textures.Length; t++)
        {
            for (var s = 0; s < nStars; s++)
            {
                // 4 and textureSize - 5 to avoid stars on the edges.
                var x = rand.Next(4, textureSize - 5);
                var y = rand.Next(4, textureSize - 5);
                var starSize = rand.Next(3, 7);
                var c = (byte)rand.Next(64, 256);
                // var r = (byte)rand.Next(64, 256);
                // var g = (byte)rand.Next(64, 256);
                // var b = (byte)rand.Next(64, 256);
                
                // Draw the star as a square
                for (var dx = -starSize / 2; dx < starSize / 2 + 1; dx++)
                {
                    for (var dy = -starSize / 2; dy < starSize / 2 + 1; dy++)
                    {
                        // Check if the coordinates are within the image bounds
                        if (0 <= x + dx && x + dx < textureSize && 0 <= y + dy && y + dy < textureSize)
                        {
                            var abs = (Math.Abs(dx) + Math.Abs(dy)) / 2.0f;
                            var cc = (abs == 0) 
                                ? c
                                : (byte)(c * (1.0f / abs));
                            
                            PutPixel(texture, textureSize, x + dx, y + dy, cc, cc, cc);
                        }
                    }
                }
            }

            textures[t] = Texture.LoadFromRgbaBytes(texture, textureSize, textureSize, TextureWrapMode.ClampToEdge);
            
            for (var i = 0; i < texture.Length; i++)
            {
                texture[i] = 0;
            }
        }
        
        var skybox = new MultiTextureSkyboxWithIndices(new MultiTextureMaterial(
            textures,
            new MultiTextureShader()));
        
        skybox.GenerateGeometry();
        
        return skybox;
    }

    
    private static void PutPixel(byte[] texture, int textureSize, int x, int y, byte r, byte g, byte b, byte a = 255)
    {
        var index = (x + y * textureSize) * 4;
        texture[index] = r;
        texture[index + 1] = g;
        texture[index + 2] = b;
        texture[index + 3] = a;
    }
    

    private Cube CreateCube(IMaterial material, Vector3 position)
    {
        var cube = new Cube()
        {
            Material = material,
            Position = position,
            Scale = 0.5f
        };
        
        cube.GenerateGeometry();
        
        return cube;
    }

    #endregion
}
