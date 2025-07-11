/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngineDemoApp.Games;

using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

using MicroEngine;
using MicroEngine.Extensions;
using MicroEngine.Extensions.Generators.SceneObjects;
using MicroEngine.Managers;
using MicroEngine.Materials;
using MicroEngine.SceneObjects.Cameras;
using MicroEngine.SceneObjects.Lights;
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

        // Load and build the default shader.
        _resourcesManager.LoadShader("default", new DefaultShader());
        
        // var cubeMaterial = new Material(
        //     _resourcesManager.LoadTexture("Textures/container2.png"),
        //     _resourcesManager.LoadTexture("Textures/container2_specular.png"),
        //     _resourcesManager.GetShader("default"))
        // {
        //     OpacityLevel = 2
        // };
        
        var cubeMaterialGreen = Material.Create(
            _resourcesManager.LoadTexture("container2_green", "Textures/container2_green.bmp"),
            _resourcesManager.LoadTexture("container2_specular", "Textures/container2_specular.bmp"),
            _resourcesManager.GetShader("default"));
        
        var cubeMaterialRed = Material.Create(
            _resourcesManager.LoadTexture("container2_red", "Textures/container2_red.bmp"),
            _resourcesManager.LoadTexture("container2_specular", "Textures/container2_specular.bmp"),
            _resourcesManager.GetShader("default"));
        
        var cubeMaterialGray = Material.Create(
            _resourcesManager.LoadTexture("container2_gray", "Textures/container2_gray.bmp"),
            _resourcesManager.LoadTexture("container2_specular", "Textures/container2_specular.bmp"),
            _resourcesManager.GetShader("default"));
        
        // Generates 1000 cubes in a 10x10x10 grid.
        // for (var x = -5; x < 5; x++)
        // {
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
        
        var opacityBias = 1;
        for (var x = -5; x < 5; x++)
        {
            for (var y = -5; y < 5; y++)
            {
                for (var z = -5; z < 5; z++)
                {
                    if ((x + y + z) % 8 == 0)
                    {
                        scene.AddChild(CreateCube(cubeMaterialGray, new Vector3(x, y, z)));
                        
                        continue;
                    }
                    
                    if ((x + y + z) % 4 == 0)
                    {
                        scene.AddChild(CreateCube(cubeMaterialRed, new Vector3(x, y, z)));
                        
                        continue;
                    }
                    
                    if ((x + y + z) % 2 == 0)
                    {
                        scene.AddChild(CreateCube(cubeMaterialGreen, new Vector3(x, y, z)));
                        
                        continue;
                    }
                    
                    if ((x + y + z) % 3 == 0)
                    {
                        var pinkMaterial = Material.Create(
                            _resourcesManager.LoadTexture("container2_pink", "Textures/container2_pink.bmp"),
                            _resourcesManager.LoadTexture("container2_specular", "Textures/container2_specular.bmp"),
                            _resourcesManager.GetShader("default"));
                        pinkMaterial.OpacityLevel = 2;
                        pinkMaterial.OpacityBias = opacityBias + 1;
                        
                        scene.AddChild(CreateCube(pinkMaterial, new Vector3(x, y, z)));
                    }
                    else
                    {
                        var otherMaterial = Material.Create(
                            _resourcesManager.LoadTexture("container2", "Textures/container2.bmp"),
                            _resourcesManager.LoadTexture("container2_specular", "Textures/container2_specular.bmp"),
                            _resourcesManager.GetShader("default"));
                        otherMaterial.OpacityLevel = 2;
                        otherMaterial.OpacityBias = opacityBias + 1;
                        
                        scene.AddChild(CreateCube(otherMaterial, new Vector3(x, y, z)));
                    }
                    
                    // This will alternate the opacity bias.
                    // Half of the cubes will have the opacity bias set to 1, the other half to 0.
                    // Cubes with the same bias wont be visible through each other.
                    // Cubes with different bias will be visible through each other.
                    opacityBias = 1 - opacityBias;
                }
            }
        }
        
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
