/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngineDemoApp.Games;

using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

using MicroEngine;
using MicroEngine.Extensions;
using MicroEngine.Extensions.Generators.SceneObjects;
using MicroEngine.Managers;
using MicroEngine.Materials;
using MicroEngine.OGL;
using MicroEngine.SceneObjects;
using MicroEngine.SceneObjects.Cameras;
using MicroEngine.SceneObjects.Lights;
using MicroEngine.Shaders;

public class Game3 : IGame
{
    private readonly ResourcesManager _resourcesManager;
    
    private Scene? _scene;
    private readonly List<ISceneObject> _cubes = new();
    private readonly SceneObjectController _cameraController = new();

    public string Name => "game-with-cubes3";

    public ICamera Camera => _scene?.Camera ?? throw new InvalidOperationException("The scene is not initialized.");

    
    public Game3(ResourcesManager resourcesManager)
    {
        _resourcesManager = resourcesManager ?? throw new ArgumentNullException(nameof(resourcesManager));
    }
    
    
    public bool Initialize()
    {
        var scene = new Scene();
        
        scene.SetCamera(new FlyByCamera());
        
        #region Skybox
        
        scene.AddSkybox(CreateSkybox());
        
        #endregion
        
        
        #region Cubes
        
        // TODO: Create the first cube and then clone it with setting position to cube clones.
        
        var cubeShader = new DefaultShader();
        cubeShader.Build();
        
        var cubeMaterial = new Material(
            _resourcesManager.LoadTexture("Textures/container2.png"),
            _resourcesManager.LoadTexture("Textures/container2_specular.png"),
            cubeShader);
        
        scene.AddChild(CreateCube(cubeMaterial, new Vector3(0.0f, 0.0f, 0.0f)));
        scene.AddChild(CreateCube(cubeMaterial, new Vector3(2.0f, 5.0f, -15.0f)));
        scene.AddChild(CreateCube(cubeMaterial, new Vector3(-1.5f, -2.2f, -2.5f)));
        scene.AddChild(CreateCube(cubeMaterial, new Vector3(-3.8f, -2.0f, -12.3f)));
        scene.AddChild(CreateCube(cubeMaterial, new Vector3(2.4f, -0.4f, -3.5f)));
        scene.AddChild(CreateCube(cubeMaterial, new Vector3(-1.7f, 3.0f, -7.5f)));
        scene.AddChild(CreateCube(cubeMaterial, new Vector3(1.3f, -2.0f, -2.5f)));
        scene.AddChild(CreateCube(cubeMaterial, new Vector3(1.5f, 2.0f, -2.5f)));
        scene.AddChild(CreateCube(cubeMaterial, new Vector3(1.5f, 0.2f, -1.5f)));
        scene.AddChild(CreateCube(cubeMaterial, new Vector3(-1.3f, 1.0f, -1.5f)));
        
        _cubes[1].Scale = 2.5f;
        
        #endregion
        
        
        scene.AddLight(new DirectionalLight(scene.Lights.Count));
        
        
        #region Lamps
        
        CreateLamps(scene);
        
        #endregion
        
        
        #region spot light
        
        var spotLight = CreateSpotLight(scene, new Vector3(0.7f, 0.2f, 2.0f));
        scene.AddLight(spotLight, scene.Camera);
        
        #endregion
        
        
        #region sub-cubes
       
        //CreateSubCubes(_cubes[1]);
        CreateSubCubes2(_cubes[1]);
        
        #endregion

        
        #region plane

        // var plane = TexturedPlaneGenerator.Generate(
        //     new SimpleTextureMaterial(
        //         Texture.LoadFromFile("Textures/container2.png"),
        //         new SimpleTextureShader()),
        //     10.0f);
        
        var plane = TexturedPlaneGenerator.Generate(
            cubeMaterial,
            10.0f);
        
        plane.BuildGeometry();
        
        plane.Position = new Vector3(0.0f, -3.0f, 0.0f);
        plane.Scale = 50.0f;
        
        scene.AddChild(plane);
        
        
        _cameraController.Position = Vector3.UnitZ * 3.0f;
        scene.AddChild(_cameraController);
        
        _cameraController.AddChild(scene.Camera);
        
        #endregion
        
        
        _scene = scene;
        
        GlRenderer.EnableFaceCulling();
        
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
    
    //private Vector3 _cameraMovementVector = Vector3.Zero;
    
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
            
            //_cameraMovementVector = Vector3.Zero;
        }

        
        var camera = (FlyByCamera)_scene.Camera;
        
        
        // Forward/backward movement.
        _cameraForwardSpeed = UpdateSpeed(_cameraForwardSpeed, keyboardState.IsKeyDown(Keys.W), -0.2f, 0.01f, 3.5f, deltaTime);
        _cameraForwardSpeed = UpdateSpeed(_cameraForwardSpeed, keyboardState.IsKeyDown(Keys.S), 0.2f, 0.01f, 3.5f, deltaTime);
        _cameraController.Advance(-_cameraForwardSpeed * deltaTime);
        
        // Left/right movement.
        _cameraSideSpeed = UpdateSpeed(_cameraSideSpeed, keyboardState.IsKeyDown(Keys.A), -0.2f, 0.01f, 3.5f, deltaTime);
        _cameraSideSpeed = UpdateSpeed(_cameraSideSpeed, keyboardState.IsKeyDown(Keys.D), 0.2f, 0.01f, 3.5f, deltaTime);
        _cameraController.Strafe(-_cameraSideSpeed * deltaTime);
        
        // Up/down movement.
        _cameraVerticalSpeed = UpdateSpeed(_cameraVerticalSpeed, keyboardState.IsKeyDown(Keys.LeftControl), -0.2f, 0.01f, 3.5f, deltaTime);
        _cameraVerticalSpeed = UpdateSpeed(_cameraVerticalSpeed, keyboardState.IsKeyDown(Keys.LeftShift), 0.2f, 0.01f, 3.5f, deltaTime);
        _cameraController.Ascend(-_cameraVerticalSpeed * deltaTime);

        
        // Yaw rotation.
        _cameraYawSpeed = UpdateSpeed(_cameraYawSpeed, keyboardState.IsKeyDown(Keys.Q), 40f, 4.0f, 120f, deltaTime);
        _cameraYawSpeed = UpdateSpeed(_cameraYawSpeed, keyboardState.IsKeyDown(Keys.E), -40f, 4.0f, 120f, deltaTime);
        _cameraController.Yaw(MathHelper.DegreesToRadians(_cameraYawSpeed * deltaTime));
        
        // Roll rotation.
        _cameraRollSpeed = UpdateSpeed(_cameraRollSpeed, keyboardState.IsKeyDown(Keys.Left), 40f, 4.0f, 120f, deltaTime);
        _cameraRollSpeed = UpdateSpeed(_cameraRollSpeed, keyboardState.IsKeyDown(Keys.Right), -40f, 4.0f, 120f, deltaTime);
        _cameraController.Roll(MathHelper.DegreesToRadians(-_cameraRollSpeed * deltaTime));
        
        // Pitch rotation.
        _cameraPitchSpeed = UpdateSpeed(_cameraPitchSpeed, keyboardState.IsKeyDown(Keys.Up), 40f, 4.0f, 120f, deltaTime);
        _cameraPitchSpeed = UpdateSpeed(_cameraPitchSpeed, keyboardState.IsKeyDown(Keys.Down), -40f, 4.0f, 120f, deltaTime);
        _cameraController.Pitch(MathHelper.DegreesToRadians(-_cameraPitchSpeed * deltaTime));
        
        
        //_scene.Camera.Position += _cameraMovementVector;
        
        
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
            
            _cameraController.Roll(MathHelper.DegreesToRadians(deltaX * sensitivity));
            //_cameraController.Yaw(deltaX * sensitivity);
            _cameraController.Pitch(MathHelper.DegreesToRadians(deltaY * sensitivity));
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
    
    private ISceneObject CreateSkybox()
    {
        return SimpleStarsSkyboxGenerator.Generate(_resourcesManager);
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


    private readonly Vector3[] _lampPositions = new[]
    {
        new Vector3(0.7f, 0.2f, 2.0f),
        new Vector3(2.3f, -3.3f, -4.0f),
        new Vector3(-4.0f, 2.0f, -12.0f),
        new Vector3(0.0f, 0.0f, -3.0f)
    };
    
    private void CreateLamps(Scene scene)
    {
        var lampShader = new SimpleColorShader();
        lampShader.Build();
        
        var lampMaterial = new SimpleColorMaterial(
            new Vector3(1.0f, 1.0f, 1.0f),
            lampShader);
        
        var redLampMaterial = new SimpleColorMaterial(
            new Vector3(1.0f, 0.0f, 0.0f),
            lampShader);
        
        var lampId = 0;
        foreach (var lampPosition in _lampPositions)
        {
            var lamp = CreateLamp((lampId == 3)
                ? redLampMaterial
                : lampMaterial,
                lampPosition);
            
            scene.AddChild(lamp);
            
            var lampLight = new PointLight(scene.Lights.Count)
            {
                Position = lampPosition
            };
            if (lampId == 3)
            {
                lampLight.Diffuse = new Vector3(1.0f, 0.0f, 0.0f);
                //lampLight.Range = 3.0f;
                lampLight.SetLightAttenuationConstants(20.0f);
            }
            
            scene.AddLight(lampLight, lamp);
            
            lampId++;
        }
    }

    
    private void CreateSubCubes2(ISceneObject parentCube)
    {
        var shader = new SimpleColorShader();
        shader.Build();
        
        var subCube = SimpleCubeGenerator.Generate(new SimpleColorMaterial(
            new Vector3(1.0f, 1.0f, 0.0f),
            shader));
        subCube.Position = new Vector3(2, 0, 0);
        subCube.Scale = 0.5f;
        
        subCube.BuildGeometry();

        subCube.Parent = parentCube;
        parentCube.AddChild(subCube);
        
        
        var subCube2 = SimpleCubeGenerator.Generate(new SimpleColorMaterial(
                new Vector3(1.0f, 0.0f, 1.0f),
                shader)); 
        subCube2.Position = new Vector3(-2, 0, 0);
        subCube2.Scale = 0.5f;
        
        subCube2.BuildGeometry();
        
        subCube2.SetRotationX(MathHelper.DegreesToRadians(45));
        subCube2.SetRotationZ(MathHelper.DegreesToRadians(45));
        
        subCube2.Parent = parentCube;
        parentCube.AddChild(subCube2);
        
        
        // Cube with the camera as its parent.
        var subCube3 = SimpleCubeGenerator.Generate(new SimpleColorMaterial(
            new Vector3(0.75f, 0.25f, 0.50f),
            shader)); 
        subCube3.Position = new Vector3(-2, 0, 0);
        subCube3.Scale = 0.25f;
        
        subCube3.BuildGeometry();
        
        subCube3.Parent = parentCube.GetScene().Camera;
        parentCube.GetScene().Camera.AddChild(subCube3);
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

    
    private ISceneObject CreateLamp(IMaterial material, Vector3 position)
    {
        var lamp = TexturedCubeGenerator.Generate(material);
        lamp.Position = position;
        lamp.Scale = 0.2f;
        
        lamp.BuildGeometry();
            
        _cubes.Add(lamp);

        return lamp;
    }

    #endregion
}