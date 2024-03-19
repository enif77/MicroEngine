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

public class Game4 : IGame
{
    private readonly ResourcesManager _resourcesManager;
    
    private Scene? _scene;
    private readonly SceneObjectController _cubeController = new();

    public string Name => "game-with-cubes4";
    
    
    public Game4(ResourcesManager resourcesManager)
    {
        _resourcesManager = resourcesManager ?? throw new ArgumentNullException(nameof(resourcesManager));
    }
    
    
    public bool Initialize()
    {
        var scene = new Scene();
        
        scene.SetCamera(CreateCamera(Program.Settings.WindowWidth, Program.Settings.WindowHeight));
        
        #region Skybox
        
        scene.AddSkybox(CreateSkybox());
        
        #endregion
        
        
        #region Cubes
        
        scene.AddChild(_cubeController);
        
        _cubeController.AddChild(scene.Camera);
        
        var cubeMaterial = new Material(
            _resourcesManager.LoadTexture("Resources/Textures/container2.png"),
            _resourcesManager.LoadTexture("Resources/Textures/container2_specular.png"),
            new DefaultShader(_resourcesManager));

        var cube1 = CreateCube(cubeMaterial, new Vector3(0.0f, 0.0f, 0.0f));
        
        _cubeController.AddChild(cube1);

        var cube2 = CreateCube(cubeMaterial, new Vector3(0.0f, 0.0f, -2.0f));
        cube2.Scale = 0.5f;
        
        cube1.AddChild(cube2);
        
        #endregion
        
        
        scene.AddLight(new DirectionalLight(scene.Lights.Count));
        
        
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

    
    private bool _firstMove = true;
    private Vector2 _lastPos;
    
    private float _forwardSpeed;
    private float _sideSpeed;
    private float _verticalSpeed;
    
    private float _yawSpeed;
    private float _rollSpeed;
    private float _pitchSpeed;
    
   
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
        
        const float sensitivity = 0.2f;

        if (keyboardState.IsKeyDown(Keys.Space))
        {
            _forwardSpeed = 0.0f;
            _sideSpeed = 0.0f;
            _verticalSpeed = 0.0f;
            
            _yawSpeed = 0.0f;
            _rollSpeed = 0.0f;
            _pitchSpeed = 0.0f;
        }
        
        // Forward/backward movement.
        _forwardSpeed = UpdateSpeed(_forwardSpeed, keyboardState.IsKeyDown(Keys.W), 0.2f, 0.01f, 3.5f, deltaTime);
        _forwardSpeed = UpdateSpeed(_forwardSpeed, keyboardState.IsKeyDown(Keys.S), -0.2f, 0.01f, 3.5f, deltaTime);
        _cubeController.Advance(-_forwardSpeed * deltaTime);
        
        // Left/right movement.
        _sideSpeed = UpdateSpeed(_sideSpeed, keyboardState.IsKeyDown(Keys.A), 0.2f, 0.01f, 3.5f, deltaTime);
        _sideSpeed = UpdateSpeed(_sideSpeed, keyboardState.IsKeyDown(Keys.D), -0.2f, 0.01f, 3.5f, deltaTime);
        _cubeController.Strafe(-_sideSpeed * deltaTime);
        
        // Up/down movement.
        _verticalSpeed = UpdateSpeed(_verticalSpeed, keyboardState.IsKeyDown(Keys.LeftControl), 0.2f, 0.01f, 3.5f, deltaTime);
        _verticalSpeed = UpdateSpeed(_verticalSpeed, keyboardState.IsKeyDown(Keys.LeftShift), -0.2f, 0.01f, 3.5f, deltaTime);
        _cubeController.Ascend(-_verticalSpeed * deltaTime);

        
        // Yaw rotation.
        _yawSpeed = UpdateSpeed(_yawSpeed, keyboardState.IsKeyDown(Keys.Q), 40f, 4.0f, 120f, deltaTime);
        _yawSpeed = UpdateSpeed(_yawSpeed, keyboardState.IsKeyDown(Keys.E), -40f, 4.0f, 120f, deltaTime);
        _cubeController.Yaw(_yawSpeed * deltaTime);
        
        // Roll rotation.
        _rollSpeed = UpdateSpeed(_rollSpeed, keyboardState.IsKeyDown(Keys.Left), 40f, 4.0f, 120f, deltaTime);
        _rollSpeed = UpdateSpeed(_rollSpeed, keyboardState.IsKeyDown(Keys.Right), -40f, 4.0f, 120f, deltaTime);
        _cubeController.Roll(-_rollSpeed * deltaTime);
        
        // Pitch rotation.
        _pitchSpeed = UpdateSpeed(_pitchSpeed, keyboardState.IsKeyDown(Keys.Up), 40f, 4.0f, 120f, deltaTime);
        _pitchSpeed = UpdateSpeed(_pitchSpeed, keyboardState.IsKeyDown(Keys.Down), -40f, 4.0f, 120f, deltaTime);
        _cubeController.Pitch(-_pitchSpeed * deltaTime);
        
        
        // var mouseState = InputManager.Instance.MouseState;
        //
        // if (_firstMove)
        // {
        //     _lastPos = new Vector2(mouseState.X, mouseState.Y);
        //     _firstMove = false;
        // }
        // else
        // {
        //     var deltaX = mouseState.X - _lastPos.X;
        //     var deltaY = mouseState.Y - _lastPos.Y;
        //     _lastPos = new Vector2(mouseState.X, mouseState.Y);
        //
        //     ((FpsCamera)_scene.Camera).Yaw += deltaX * sensitivity;
        //     ((FpsCamera)_scene.Camera).Pitch -= deltaY * sensitivity;
        // }
        
        // _scene.Camera.Position = _cubeController.Position;
        // _scene.Camera.Rotation = -_cubeController.Rotation;
        
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
    
    
    #region physics

    private float UpdateSpeed(float currentSpeed, bool accelerate, float acceleration, float decceleration, float maxSpeed, float deltaTime)
    {
        var speed = currentSpeed;
        if (accelerate)
        {
            speed += acceleration * deltaTime;
            if (Math.Abs(speed) > maxSpeed)
            {
                speed = (acceleration < 0)
                    ? -maxSpeed
                    : maxSpeed;
            }

            return speed;
        }
        
        // Flying forward, but not accelerating.
        if (speed > 0.0f && acceleration > 0.0f)
        {
            speed -= decceleration * deltaTime;
            if (speed < 0.0f)
            {
                speed = 0.0f;
            }    
        }
        // Flying backward, but not accelerating.
        else if (speed < 0.0f && acceleration < 0.0f)
        {
            speed += decceleration * deltaTime;
            if (speed > 0.0f)
            {
                speed = 0.0f;
            }    
        }

        return speed;
    }

    #endregion
    
    
    #region creators and generators

    private ICamera CreateCamera(int windowWidth, int windowHeight)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(windowWidth);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(windowHeight);

        // TODO: create and use SOC for camera controls.
        
        //return new FlyByCamera(Vector3.UnitZ * 3, windowWidth / (float)windowHeight);
        return new FlyByCamera(windowWidth / (float)windowHeight);
    }
    
    
    private ISceneObject CreateSkybox()
    {
        return SimpleStarsSkyboxGenerator.Generate(_resourcesManager);
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