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

public class Game5 : IGame
{
    private readonly ResourcesManager _resourcesManager;
    
    private Scene? _scene;
    private readonly SceneObjectController _cubeController = new();

    public string Name => "game-with-cubes5";

    
    public Game5(ResourcesManager resourcesManager)
    {
        _resourcesManager = resourcesManager ?? throw new ArgumentNullException(nameof(resourcesManager));
    }
    
    
    public bool Initialize(int width, int height)
    {
        var scene = new Scene(
            CreateCamera(width, height));
        
        #region Skybox
        
        scene.AddSkybox(CreateSkybox());
        
        #endregion
        
        
        #region Cubes
        
        scene.AddChild(_cubeController);
        
        _cubeController.AddChild(scene.Camera);
        
        var cubeMaterial = new Material(
            _resourcesManager.LoadTexture("Resources/Textures/container2.png"),
            _resourcesManager.LoadTexture("Resources/Textures/container2_specular.png"),
            new DefaultShader());

        var cube1 = CreateCube(cubeMaterial, new Vector3(0.0f, 0.0f, 0.0f));
        
        _cubeController.AddChild(cube1);

        var cube2 = CreateCube(cubeMaterial, new Vector3(0.0f, 0.0f, -2.0f));
        cube2.Scale = 0.5f;
        
        cube1.AddChild(cube2);
        
        #endregion
        
        
        scene.AddLight(new DirectionalLight(scene.Lights.Count)
        {
            Ambient = new Vector3(0.65f)
        });
        
        
        #region plane
       
        var plane = TexturedPlaneGenerator.Generate(
            cubeMaterial,
            10.0f);
        
        plane.BuildGeometry();
        
        plane.Position = new Vector3(0.0f, -3.0f, 0.0f);
        plane.Scale = 50.0f;
        
        scene.AddChild(plane);
        
        #endregion
        
        
        _scene = scene;
        
        Renderer.EnableFaceCulling();
        
        return true;
    }

    
    private const float RotationSpeed = 60.0f;
    private const float MovementSpeed = 1.0f;
    
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
        
        
        if (keyboardState.IsKeyDown(Keys.Space))
        {
            _cubeController.Position = Vector3.Zero;
            _cubeController.Rotation = Vector3.Zero;
            
            _cubeController.YawAbs(0);
            _cubeController.PitchAbs(0);
            _cubeController.RollAbs(0);
        }
        
        
        if (keyboardState.IsKeyDown(Keys.R))
        {
            _cubeController.Position = Vector3.Zero;
            _cubeController.Rotation = Vector3.Zero;
            
            _cubeController.YawAbs(45);
            _cubeController.PitchAbs(45);
            _cubeController.RollAbs(0);
        }
        
        
        // Forward/backward movement.
        if (keyboardState.IsKeyDown(Keys.W))
        {
            _cubeController.Advance(MovementSpeed * deltaTime);    
        }
        
        if (keyboardState.IsKeyDown(Keys.S))
        {
            _cubeController.Advance(-MovementSpeed * deltaTime);    
        }
       
        // Left/right movement.
        if (keyboardState.IsKeyDown(Keys.A))
        {
            _cubeController.Strafe(MovementSpeed * deltaTime);    
        }
        
        if (keyboardState.IsKeyDown(Keys.D))
        {
            _cubeController.Strafe(-MovementSpeed * deltaTime);    
        }
       
        // Up/down movement.
        if (keyboardState.IsKeyDown(Keys.LeftControl))
        {
            _cubeController.Ascend(MovementSpeed * deltaTime);    
        }
        
        if (keyboardState.IsKeyDown(Keys.LeftShift))
        {
            _cubeController.Ascend(-MovementSpeed * deltaTime);    
        }

        
        // Yaw rotation.
        if (keyboardState.IsKeyDown(Keys.Q))
        {
            _cubeController.Yaw(RotationSpeed * deltaTime);    
        }
        
        if (keyboardState.IsKeyDown(Keys.E))
        {
            _cubeController.Yaw(-RotationSpeed * deltaTime);    
        }
        
        // Roll rotation.
        if (keyboardState.IsKeyDown(Keys.Left))
        {
            _cubeController.Roll(RotationSpeed * deltaTime);    
        }
        
        if (keyboardState.IsKeyDown(Keys.Right))
        {
            _cubeController.Roll(-RotationSpeed * deltaTime);    
        }
        
        // Pitch rotation.
        if (keyboardState.IsKeyDown(Keys.Up))
        {
            _cubeController.Pitch(RotationSpeed * deltaTime);    
        }
        
        if (keyboardState.IsKeyDown(Keys.Down))
        {
            _cubeController.Pitch(-RotationSpeed * deltaTime);    
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

    private ICamera CreateCamera(int windowWidth, int windowHeight)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(windowWidth);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(windowHeight);

        return new FlyByCamera(Vector3.UnitZ * 3, windowWidth / (float)windowHeight);
    }
    
    
    private ISceneObject CreateSkybox()
    {
        return SimpleStarsSkyboxGenerator.Generate();
    }

    
    private ISceneObject CreateCube(IMaterial material, Vector3 position)
    {
        var cube = TexturedCubeGenerator.Generate(material);

        cube.Position = position;
       
        cube.BuildGeometry();
       
        return cube;
    }

    #endregion
}