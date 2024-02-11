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
    private readonly List<ISceneObject> _cubes = new();

    public string Name => "game-with-cubes4";

    public ICamera Camera => _scene?.Camera ?? throw new InvalidOperationException("The scene is not initialized.");

    
    public Game4(ResourcesManager resourcesManager)
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
        
        // TODO: Create the first cube and then clone it with setting position to cube clones.
        
        var cubeMaterial = new Material(
            _resourcesManager.LoadTexture("Resources/Textures/container2.png"),
            _resourcesManager.LoadTexture("Resources/Textures/container2_specular.png"),
            new DefaultShader());
        
        scene.AddChild(CreateCube(cubeMaterial, new Vector3(0.0f, 0.0f, 0.0f)));
        
        #endregion
        
        
        scene.AddLight(new DirectionalLight(scene.Lights.Count));
        
        
        #region spot light
        
        var spotLight = CreateSpotLight(scene, new Vector3(0.7f, 0.2f, 2.0f));
        scene.AddLight(spotLight, scene.Camera);
        
        #endregion

        
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
    
    private float _cameraForwardSpeed = 0.0f;
    private float _cameraSideSpeed = 0.0f;
    private float _cameraVerticalSpeed = 0.0f;
    
    private float _cameraYawSpeed = 0.0f;
    private float _cameraRollSpeed = 0.0f;
    private float _cameraPitchSpeed = 0.0f;
    
   
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
            _cameraForwardSpeed = 0.0f;
            _cameraSideSpeed = 0.0f;
            _cameraVerticalSpeed = 0.0f;
            
            _cameraYawSpeed = 0.0f;
            _cameraRollSpeed = 0.0f;
            _cameraPitchSpeed = 0.0f;
        }

        
        var camera = (FlyByCamera)_scene.Camera;
        
        
        // Forward/backward movement.
        _cameraForwardSpeed = UpdateSpeed(_cameraForwardSpeed, keyboardState.IsKeyDown(Keys.W), 0.2f, 0.01f, 3.5f, deltaTime);
        _cameraForwardSpeed = UpdateSpeed(_cameraForwardSpeed, keyboardState.IsKeyDown(Keys.S), -0.2f, 0.01f, 3.5f, deltaTime);
        camera.Advance(-_cameraForwardSpeed * deltaTime);
        
        // Left/right movement.
        _cameraSideSpeed = UpdateSpeed(_cameraSideSpeed, keyboardState.IsKeyDown(Keys.A), 0.2f, 0.01f, 3.5f, deltaTime);
        _cameraSideSpeed = UpdateSpeed(_cameraSideSpeed, keyboardState.IsKeyDown(Keys.D), -0.2f, 0.01f, 3.5f, deltaTime);
        camera.Strafe(-_cameraSideSpeed * deltaTime);
        
        // Up/down movement.
        _cameraVerticalSpeed = UpdateSpeed(_cameraVerticalSpeed, keyboardState.IsKeyDown(Keys.LeftControl), 0.2f, 0.01f, 3.5f, deltaTime);
        _cameraVerticalSpeed = UpdateSpeed(_cameraVerticalSpeed, keyboardState.IsKeyDown(Keys.LeftShift), -0.2f, 0.01f, 3.5f, deltaTime);
        camera.Ascend(-_cameraVerticalSpeed * deltaTime);

        
        // Yaw rotation.
        _cameraYawSpeed = UpdateSpeed(_cameraYawSpeed, keyboardState.IsKeyDown(Keys.Q), 40f, 4.0f, 120f, deltaTime);
        _cameraYawSpeed = UpdateSpeed(_cameraYawSpeed, keyboardState.IsKeyDown(Keys.E), -40f, 4.0f, 120f, deltaTime);
        camera.Yaw(_cameraYawSpeed * deltaTime);
        
        // Roll rotation.
        _cameraRollSpeed = UpdateSpeed(_cameraRollSpeed, keyboardState.IsKeyDown(Keys.Left), 40f, 4.0f, 120f, deltaTime);
        _cameraRollSpeed = UpdateSpeed(_cameraRollSpeed, keyboardState.IsKeyDown(Keys.Right), -40f, 4.0f, 120f, deltaTime);
        camera.Roll(-_cameraRollSpeed * deltaTime);
        
        // Pitch rotation.
        _cameraPitchSpeed = UpdateSpeed(_cameraPitchSpeed, keyboardState.IsKeyDown(Keys.Up), 40f, 4.0f, 120f, deltaTime);
        _cameraPitchSpeed = UpdateSpeed(_cameraPitchSpeed, keyboardState.IsKeyDown(Keys.Down), -40f, 4.0f, 120f, deltaTime);
        camera.Pitch(-_cameraPitchSpeed * deltaTime);
        
        
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
            
            camera.Roll(deltaX * sensitivity);
            //camera.Yaw(deltaX * sensitivity);
            camera.Pitch(deltaY * sensitivity);
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

        return new FlyByCamera(Vector3.UnitZ * 3, windowWidth / (float)windowHeight);
    }
    
    
    private ISceneObject CreateSkybox()
    {
        return SimpleStarsSkyboxGenerator.Generate();
    }
    

    private SpotLight CreateSpotLight(Scene scene, Vector3 position)
    {
        return new SpotLight(scene.Lights.Count)
        {
            Parent = scene.Camera,
            
            Position = position,
            Ambient = new Vector3(0.0f, 0.0f, 0.0f),
            //Diffuse = new Vector3(1.0f, 1.0f, 1.0f),
            Diffuse = new Vector3(0.0f, 1.0f, 0.0f),
            Specular = new Vector3(1.0f, 1.0f, 1.0f),
            Constant = 1.0f,
            Linear = 0.09f,
            Quadratic = 0.032f,
            CutOff = MathF.Cos(MathHelper.DegreesToRadians(12.5f)),
            OuterCutOff = MathF.Cos(MathHelper.DegreesToRadians(17.5f))
        };
    }

    
    private ISceneObject CreateCube(IMaterial material, Vector3 position)
    {
        var angle = 20.0f * _cubes.Count;
        
        var cube = TexturedCubeGenerator.Generate(material);
        cube.Position = position;
        cube.Rotation = new Vector3(1.0f * angle, 0.3f * angle, 0.5f * angle);
        
        cube.BuildGeometry();
        
        _cubes.Add(cube);
        
        return cube;
    }

    #endregion
}