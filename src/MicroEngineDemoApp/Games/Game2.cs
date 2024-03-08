/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngineDemoApp.Games;

using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

using MicroEngine;
using MicroEngine.Extensions;
using MicroEngine.Extensions.Generators.SceneObjects;
using MicroEngine.Lights;
using MicroEngine.Managers;
using MicroEngine.Materials;
using MicroEngine.SceneObjects;
using MicroEngine.Shaders;

public class Game2 : IGame
{
    private readonly ResourcesManager _resourcesManager;
    
    private Scene? _scene;

    public string Name => "game-with-cubes-2";

    public ICamera Camera => _scene?.Camera ?? throw new InvalidOperationException("The scene is not initialized.");

    
    public Game2(ResourcesManager resourcesManager)
    {
        _resourcesManager = resourcesManager ?? throw new ArgumentNullException(nameof(resourcesManager));
    }
    
    
    public bool Initialize()
    {
        var scene = new Scene(
            CreateCamera(Program.Settings.WindowWidth, Program.Settings.WindowHeight));
        
        #region Skybox
        
        scene.AddSkybox(SimpleStarsSkyboxGenerator.Generate());
        
        #endregion
        
        
        #region Cubes
        
        var map = ParseMap(File.ReadAllText("Resources/Maps/Map10x10.txt"));
        
        var cubeMaterial = new Material(
            _resourcesManager.LoadTexture("Resources/Textures/container2.png"),
            _resourcesManager.LoadTexture("Resources/Textures/container2_specular.png"),
            new DefaultShader());
        
        var lampShader = new SimpleColorShader();
        
        var lampMaterial = new SimpleColorMaterial(
            new Vector3(1.0f, 1.0f, 1.0f),
            lampShader);
        
        var redLampMaterial = new SimpleColorMaterial(
            new Vector3(1.0f, 0.0f, 0.0f),
            lampShader);
        
        for (var z = 0; z < 10; z++)
        {
            for (var x = 0; x < 10; x++)
            {
                var cubeId = map[z * 10 + x];
                
                switch (cubeId)
                {
                    case 1:
                    {
                        var cube = TexturedCubeGenerator.Generate(cubeMaterial);
                        cube.Position = new Vector3(x, 0, z);
                        
                        cube.BuildGeometry();
                        
                        scene.AddChild(cube);
                        break;
                    }
                    case 2:
                    {
                        var lamp = CreateLamp(lampMaterial, new Vector3(x, 2, z));
                        scene.AddChild(lamp);
                        
                        var lampLight = new PointLight(scene.Lights.Count)
                        {
                            Position = new Vector3(x, 2, z)
                        };
                        
                        scene.AddLight(lampLight, lamp);
                        break;
                    }
                    case 3:
                    {
                        var lamp = CreateLamp(redLampMaterial, new Vector3(x, 2, z));
                        scene.AddChild(lamp);
                        
                        var lampLight = new PointLight(scene.Lights.Count)
                        {
                            Position = new Vector3(x, 2, z),
                            Diffuse = new Vector3(1.0f, 0.0f, 0.0f)
                        };
                        
                        scene.AddLight(lampLight, lamp);
                        break;
                    }
                }
            }
        }
        
        #endregion
        
        
        #region Plane
        
        var plane = TexturedPlaneGenerator.Generate(
            cubeMaterial,
            10.0f);
        
        plane.BuildGeometry();
        
        plane.Position = new Vector3(4.5f, -0.5f, 4.5f);
        plane.Scale = 10.0f;
        
        scene.AddChild(plane);
        
        #endregion
        
        
        scene.AddLight(new DirectionalLight(scene.Lights.Count));
        
        
        _scene = scene;
        
        Renderer.EnableFaceCulling();
        
        return true;
    }

    
    private bool _firstMove = true;
    private Vector2 _lastPos;
    
    public bool Update(float deltaTime)
    {
        if (_scene == null)
        {
            throw new InvalidOperationException("The scene is not initialized.");
        }
        
        var keyboardState = InputManager.Instance.KeyboardState;
        
        if (keyboardState.IsKeyDown(Keys.Escape))
        {
            return false;
        }
        
        const float cameraSpeed = 1.5f;
        const float sensitivity = 0.2f;

        var camera = (FpsCamera)_scene.Camera;
        
        if (keyboardState.IsKeyDown(Keys.W))
        {
            _scene.Camera.Position += camera.FrontVector * cameraSpeed * deltaTime; // Forward
        }
        if (keyboardState.IsKeyDown(Keys.S))
        {
            _scene.Camera.Position -= camera.FrontVector * cameraSpeed * deltaTime; // Backwards
        }
        if (keyboardState.IsKeyDown(Keys.A))
        {
            _scene.Camera.Position -= camera.RightVector * cameraSpeed * deltaTime; // Left
        }
        if (keyboardState.IsKeyDown(Keys.D))
        {
            _scene.Camera.Position += camera.RightVector * cameraSpeed * deltaTime; // Right
        }
        if (keyboardState.IsKeyDown(Keys.Space))
        {
            _scene.Camera.Position += camera.UpVector * cameraSpeed * deltaTime; // Up
        }
        if (keyboardState.IsKeyDown(Keys.LeftShift))
        {
            _scene.Camera.Position -= camera.UpVector * cameraSpeed * deltaTime; // Down
        }
        
        var mouseState = InputManager.Instance.MouseState;
        
        if (_firstMove)
        {
            _lastPos = new Vector2(mouseState.X, mouseState.Y);
            _firstMove = false;
        }
        else
        {
            var deltaX = mouseState.X - _lastPos.X;
            var deltaY = mouseState.Y - _lastPos.Y;
            _lastPos = new Vector2(mouseState.X, mouseState.Y);

            camera.Yaw += deltaX * sensitivity;
            camera.Pitch -= deltaY * sensitivity;
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
        
        ((FpsCamera)_scene.Camera).AspectRatio = aspectRatio;
    }
    
    
    #region creators and generators

    private FpsCamera CreateCamera(int windowWidth, int windowHeight)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(windowWidth);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(windowHeight);

        return new FpsCamera(new Vector3(1, 0, 1), windowWidth / (float)windowHeight);
    }
    
    
    private ISceneObject CreateLamp(IMaterial material, Vector3 position)
    {
        var lamp = TexturedCubeGenerator.Generate(material);
        lamp.Position = position;
        lamp.Scale = 0.2f;
        
        lamp.BuildGeometry();

        return lamp;
    }


    private int[] ParseMap(string map)
    {
        var lines = map.Replace("\r", string.Empty).Split('\n');
        var width = lines[0].Length;
        var height = lines.Length;

        var result = new int[width * height];

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var box = 1;
                switch (lines[y][x])
                {
                    default:
                    case '#': box = 1; break;  // wall
                    case '.': box = 0; break;  // floor
                    case 'l': box = 2; break;  // lamp
                    case 'r': box = 3; break;  // red lamp
                }

                result[y * width + x] = box;
            }
        }

        return result;
    }
    
    #endregion
}