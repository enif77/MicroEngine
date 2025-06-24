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
using MicroEngine.SceneObjects.Cameras;
using MicroEngine.SceneObjects.Lights;
using MicroEngine.Shaders;

public class Game : IGame
{
    private readonly ResourcesManager _resourcesManager;
    
    private Scene? _scene;
    private readonly List<ISceneObject> _cubes = new();

    public string Name => "game-with-cubes";

    //public ICamera Camera => _scene?.Camera ?? throw new InvalidOperationException("The scene is not initialized.");

    
    public Game(ResourcesManager resourcesManager)
    {
        _resourcesManager = resourcesManager ?? throw new ArgumentNullException(nameof(resourcesManager));
    }
    
    
    public bool Initialize()
    {
        var scene = new Scene();
        
        scene.SetCamera(new FpsCamera()
            {
                Position = Vector3.UnitZ * 3,
            });
        
        scene.AddSkybox(CreateSkybox());
        
        #region Cubes
        
        // TODO: Create the first cube and then clone it with setting position to cube clones.
        
        var cubeShader = new DefaultShader();
        cubeShader.Build();
        
        var cubeMaterial = Material.Create(
            _resourcesManager.LoadTexture("container2", "Textures/container2.bmp"),
            _resourcesManager.LoadTexture("container2_specular", "Textures/container2_specular.bmp"),
            cubeShader);
        
        scene.AddChild(CreateCube(cubeMaterial, new Vector3( 0.0f,  0.0f,   0.0f)));
        scene.AddChild(CreateCube(cubeMaterial, new Vector3( 2.0f,  5.0f, -15.0f)));
        scene.AddChild(CreateCube(cubeMaterial, new Vector3(-1.5f, -2.2f,  -2.5f)));
        scene.AddChild(CreateCube(cubeMaterial, new Vector3(-3.8f, -2.0f, -12.3f)));
        scene.AddChild(CreateCube(cubeMaterial, new Vector3( 2.4f, -0.4f,  -3.5f)));
        scene.AddChild(CreateCube(cubeMaterial, new Vector3(-1.7f,  3.0f,  -7.5f)));
        scene.AddChild(CreateCube(cubeMaterial, new Vector3( 1.3f, -2.0f,  -2.5f)));
        scene.AddChild(CreateCube(cubeMaterial, new Vector3( 1.5f,  2.0f,  -2.5f)));
        scene.AddChild(CreateCube(cubeMaterial, new Vector3( 1.5f,  0.2f,  -1.5f)));
        scene.AddChild(CreateCube(cubeMaterial, new Vector3(-1.3f,  1.0f,  -1.5f)));
        
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

        var plane = TexturedPlaneGenerator.Generate(
            cubeMaterial,
            10.0f);
        plane.Position = new Vector3(0.0f, -3.0f, 0.0f);
        plane.Scale = 50.0f;
        plane.BuildGeometry();
        
        scene.AddChild(plane);
        
        #endregion
        
        
        _scene = scene;
        
        InputManager.Instance.MouseWheel += e => ((FpsCamera)_scene.Camera).Fov -= e.OffsetY;
        
        GlRenderer.EnableFaceCulling();
        
        return true;
    }

    
    private bool _firstMove = true;
    private Vector2 _lastPos;
    
    private float _cameraForwardSpeed = 0.0f;
    private float _cameraSideSpeed = 0.0f;
    private float _cameraVerticalSpeed = 0.0f;
    
    private Vector3 _cameraMovementVector = Vector3.Zero;
    
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
            
            _cameraMovementVector = Vector3.Zero;
        }

        
        // Forward/backward movement.
        if (keyboardState.IsKeyDown(Keys.W))
        {
            _cameraForwardSpeed += 0.01f * deltaTime; // Forward
            if (_cameraForwardSpeed > 0.5f)
            {
                _cameraForwardSpeed = 0.5f;
            }
            
            _cameraMovementVector += ((FpsCamera)_scene.Camera).FrontVector * (_cameraForwardSpeed * deltaTime);
        }
        else
        {
            if (_cameraForwardSpeed > 0.0f)
            {
                _cameraForwardSpeed -= 0.01f * deltaTime;
                if (_cameraForwardSpeed < 0.0f)
                {
                    _cameraForwardSpeed = 0.0f;
                }    
            }
        }
        
        if (keyboardState.IsKeyDown(Keys.S))
        {
            _cameraForwardSpeed -= 0.01f * deltaTime; // Backwards
            if (_cameraForwardSpeed < -0.5f)
            {
                _cameraForwardSpeed = -0.5f;
            }
            
            _cameraMovementVector += ((FpsCamera)_scene.Camera).FrontVector * (_cameraForwardSpeed * deltaTime);
        }
        else
        {
            if (_cameraForwardSpeed < 0.0f)
            {
                _cameraForwardSpeed += 0.01f * deltaTime;
                if (_cameraForwardSpeed > 0.0f)
                {
                    _cameraForwardSpeed = 0.0f;
                }    
            }
        }
        
        
        // Left/right movement.
        if (keyboardState.IsKeyDown(Keys.A))
        {
            _cameraSideSpeed -= 0.01f * deltaTime;
            if (_cameraSideSpeed < -0.5f)
            {
                _cameraSideSpeed = -0.5f;
            }
            
            _cameraMovementVector += ((FpsCamera)_scene.Camera).RightVector * (_cameraSideSpeed * deltaTime);
        }
        else
        {
            if (_cameraSideSpeed < 0.0f)
            {
                _cameraSideSpeed += 0.01f * deltaTime;
                if (_cameraSideSpeed > 0.0f)
                {
                    _cameraSideSpeed = 0.0f;
                }    
            }
        }
        
        if (keyboardState.IsKeyDown(Keys.D))
        {
            _cameraSideSpeed += 0.01f * deltaTime;
            if (_cameraSideSpeed > 0.5f)
            {
                _cameraSideSpeed = 0.5f;
            }
            
            _cameraMovementVector += ((FpsCamera)_scene.Camera).RightVector * (_cameraSideSpeed * deltaTime);
        }
        else
        {
            if (_cameraSideSpeed > 0.0f)
            {
                _cameraSideSpeed -= 0.01f * deltaTime;
                if (_cameraSideSpeed < 0.0f)
                {
                    _cameraSideSpeed = 0.0f;
                }    
            }
        }
        
        
        // Up/down movement.
        if (keyboardState.IsKeyDown(Keys.LeftControl))
        {
            _cameraVerticalSpeed -= 0.01f * deltaTime;
            if (_cameraVerticalSpeed < -0.5f)
            {
                _cameraVerticalSpeed = -0.5f;
            }
            
            _cameraMovementVector += ((FpsCamera)_scene.Camera).UpVector * (_cameraVerticalSpeed * deltaTime);
        }
        else
        {
            if (_cameraVerticalSpeed < 0.0f)
            {
                _cameraVerticalSpeed += 0.01f * deltaTime;
                if (_cameraVerticalSpeed > 0.0f)
                {
                    _cameraVerticalSpeed = 0.0f;
                }    
            }
        }
        
        if (keyboardState.IsKeyDown(Keys.LeftShift))
        {
            _cameraVerticalSpeed += 0.01f * deltaTime;
            if (_cameraVerticalSpeed > 0.5f)
            {
                _cameraVerticalSpeed = 0.5f;
            }
            
            _cameraMovementVector += ((FpsCamera)_scene.Camera).UpVector * (_cameraVerticalSpeed * deltaTime);
        }
        else
        {
            if (_cameraVerticalSpeed > 0.0f)
            {
                _cameraVerticalSpeed -= 0.01f * deltaTime;
                if (_cameraVerticalSpeed < 0.0f)
                {
                    _cameraVerticalSpeed = 0.0f;
                }    
            }
        }

        
        _scene.Camera.Position += _cameraMovementVector;
        
        
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

            ((FpsCamera)_scene.Camera).Yaw += deltaX * sensitivity;
            ((FpsCamera)_scene.Camera).Pitch -= deltaY * sensitivity;
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

    // private FpsCamera CreateCamera(int windowWidth, int windowHeight)
    // {
    //     ArgumentOutOfRangeException.ThrowIfNegativeOrZero(windowWidth);
    //     ArgumentOutOfRangeException.ThrowIfNegativeOrZero(windowHeight);
    //
    //     return new FpsCamera()
    //     {
    //         Position = Vector3.UnitZ * 3,
    //     };
    // }
    
    
    private ISceneObject CreateSkybox()
    {
        // var skybox = new MultiTextureSkyboxWithIndices(new MultiTextureMaterial(
        //     [
        //         Texture.LoadFromFile($"Textures/Skyboxes/TestSkybox/pz.jpg", TextureWrapMode.ClampToEdge),
        //         Texture.LoadFromFile($"Textures/Skyboxes/TestSkybox/px.jpg", TextureWrapMode.ClampToEdge),
        //         Texture.LoadFromFile($"Textures/Skyboxes/TestSkybox/nz.jpg", TextureWrapMode.ClampToEdge),
        //         Texture.LoadFromFile($"Textures/Skyboxes/TestSkybox/nx.jpg", TextureWrapMode.ClampToEdge),
        //         Texture.LoadFromFile($"Textures/Skyboxes/TestSkybox/py.jpg", TextureWrapMode.ClampToEdge),
        //         Texture.LoadFromFile($"Textures/Skyboxes/TestSkybox/ny.jpg", TextureWrapMode.ClampToEdge)
        //     ],
        //     new MultiTextureShader()));
        //
        // skybox.GenerateGeometry();
        //
        // return skybox;
        
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
        subCube3.Position = new Vector3(0, 1, -2);
        subCube3.Rotation = new Vector3(45, 45, 0);
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