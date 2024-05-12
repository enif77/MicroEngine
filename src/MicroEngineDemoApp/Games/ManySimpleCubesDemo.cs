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

public class ManySimpleCubesDemo : IGame
{
    private readonly ResourcesManager _resourcesManager;
    private Scene? _scene;
    
    public string Name => "many-simple-cubes-demo";

    
    public ManySimpleCubesDemo(ResourcesManager resourcesManager)
    {
        _resourcesManager = resourcesManager ?? throw new ArgumentNullException(nameof(resourcesManager));
    }
    
    
    public bool Initialize()
    {
        var scene = new Scene();
        
        scene.SetCamera(CreateCamera(Program.Settings.WindowWidth, Program.Settings.WindowHeight));
        
        scene.AddSkybox(SimpleStarsSkyboxGenerator.Generate(_resourcesManager));

        // var cubeMaterial = new Material(
        //     _resourcesManager.LoadTexture("Textures/container2.png"),
        //     _resourcesManager.LoadTexture("Textures/container2_specular.png"),
        //     new DefaultShader(_resourcesManager))
        // {
        //     OpacityLevel = 2
        // };
        
        var cubeMaterial2 = new Material(
            _resourcesManager.LoadTexture("Textures/container2.png"),
            _resourcesManager.LoadTexture("Textures/container2_specular.png"),
            new DefaultShader(_resourcesManager));
        
        // Generates 1000 cubes in a 10x10x10 grid.
        var opacityBias = 0;
        for (var x = -5; x < 5; x++)
        {
            for (var y = -5; y < 5; y++)
            {
                for (var z = -5; z < 5; z++)
                {
                    if ((x + y + z) % 2 == 0)
                    {
                        scene.AddChild(CreateCube(cubeMaterial2, new Vector3(x, y, z)));
                        
                        continue;
                    }
                    
                    scene.AddChild(CreateCube(new Material(
                        _resourcesManager.LoadTexture("Textures/container2.png"),
                        _resourcesManager.LoadTexture("Textures/container2_specular.png"),
                        new DefaultShader(_resourcesManager))
                    {
                        OpacityLevel = 2,
                        OpacityBias = opacityBias++
                    }, new Vector3(x, y, z)));
                }
            }
        }
        
        
        // for (var x = -5; x < 5; x++)
        // {
        //     var cubeMaterial = new Material(
        //         _resourcesManager.LoadTexture("Textures/container2.png"),
        //         _resourcesManager.LoadTexture("Textures/container2_specular.png"),
        //         new DefaultShader(_resourcesManager))
        //     {
        //         OpacityLevel = 2,
        //         OpacityBias = x * 5
        //     };
        //     
        //     for (var y = -5; y < 5; y++)
        //     {
        //         for (var z = -5; z < 5; z++)
        //         {
        //             if ((x + y + z) % 2 == 0)
        //             {
        //                 scene.AddChild(CreateCube(cubeMaterial2, new Vector3(x, y, z)));
        //                 
        //                 continue;
        //             }
        //             
        //             scene.AddChild(CreateCube(cubeMaterial, new Vector3(x, y, z)));
        //         }
        //     }
        // }
        
        
        scene.AddLight(new DirectionalLight(scene.Lights.Count)
        {
            Ambient = new Vector3(0.7f, 0.7f, 0.7f),
        });
       
        _scene = scene;
        
        //Renderer.EnableFaceCulling();
        
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
        
        var camera = ((FpsCamera)_scene.Camera);
        
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
            _scene.Camera.Position += camera.RightVector * cameraSpeed * deltaTime; // Left
        }
        if (keyboardState.IsKeyDown(Keys.D))
        {
            _scene.Camera.Position -= camera.RightVector * cameraSpeed * deltaTime; // Right
        }
        if (keyboardState.IsKeyDown(Keys.Space))
        {
            _scene.Camera.Position += camera.UpVector * cameraSpeed * deltaTime; // Up
        }
        if (keyboardState.IsKeyDown(Keys.LeftShift))
        {
            _scene.Camera.Position -= camera.UpVector * cameraSpeed * deltaTime; // Down
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

            camera.Yaw -= deltaX * sensitivity;
            camera.Pitch += deltaY * sensitivity;
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
    
    
    #region creators and generators

    private FpsCamera CreateCamera(int windowWidth, int windowHeight)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(windowWidth);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(windowHeight);

        return new FpsCamera()
        {
            Position = Vector3.UnitZ * 5
        };
    }

    
    private ISceneObject CreateCube(IMaterial material, Vector3 position)
    {
        var cube = TexturedCubeGenerator.Generate(material);
        cube.Position = position;
        cube.Scale = 0.25f;
        
        cube.BuildGeometry();
        
        return cube;
    }

    #endregion
}
