/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngineDemoApp.Games;

using MicroEngine;
using MicroEngine.Core;
using MicroEngine.Extensions;
using MicroEngine.Lights;
using MicroEngine.Materials;
using MicroEngine.SceneObjects;
using MicroEngine.Shaders;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

public class Game : IGame
{
    private Scene? _scene;
    private readonly List<Cube> _cubes = new();
    
    public Camera Camera => _scene?.Camera ?? throw new InvalidOperationException("The scene is not initialized.");
    
    
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
            Texture.LoadFromFile("Resources/Textures/container2.png"),
            Texture.LoadFromFile("Resources/Textures/container2_specular.png"),
            new DefaultShader());
        
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
       
        CreateSubCubes(_cubes[1]);
        
        #endregion
        
        
        _scene = scene;
        
        return true;
    }

    
    private bool _firstMove = true;
    private Vector2 _lastPos;
    
    public bool Update(float deltaTime, KeyboardState keyboardState, MouseState mouseState)
    {
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

    
    public void UpdateCameraFov(float fovChange)
    {
        if (_scene == null)
        {
            throw new InvalidOperationException("The scene is not initialized.");
        }
        
        _scene.Camera.Fov += fovChange;
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

        return new Camera(Vector3.UnitZ * 3, windowWidth / (float)windowHeight);
    }
    
    
    private Skybox CreateSkybox()
    {
        var skybox = new Skybox(
            new SimpleTextureMaterial(
                Texture.LoadFromFile("Resources/Textures/SKYBOX.jpg"),
                new SkyboxShader()));
        
        skybox.GenerateVertexObjectBuffer();
        skybox.GenerateVertexArrayObjectForPosTexVbo();
        
        return skybox;
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
        var lampShader = new LampShader();
        
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


    private void CreateSubCubes(ISceneObject parentCube)
    {
        var shader = new LampShader();
        
        var subCube = new Cube()
        {
            Material = new SimpleColorMaterial(
                new Vector3(1.0f, 1.0f, 0.0f),
                shader),
            Position = new Vector3(2, 0, 0),
            Scale = 0.5f
        };
        
        subCube.GenerateVertexObjectBuffer();
        subCube.GenerateVertexArrayObjectForPosNormTexVbo();
            
        _cubes.Add(subCube);
        
        subCube.Parent = parentCube;
        parentCube.AddChild(subCube);
        
        
        var subCube2 = new Cube()
        {
            Material = new SimpleColorMaterial(
                new Vector3(1.0f, 0.0f, 1.0f),
                shader),
            Position = new Vector3(-2, 0, 0),
            Scale = 0.5f
        };
        
        subCube2.SetRotationX(MathHelper.DegreesToRadians(45));
        subCube2.SetRotationZ(MathHelper.DegreesToRadians(45));
        
        subCube2.GenerateVertexObjectBuffer();
        subCube2.GenerateVertexArrayObjectForPosNormTexVbo();
            
        _cubes.Add(subCube2);
        
        subCube2.Parent = _cubes[1];
        _cubes[1].AddChild(subCube2);
    }
    
    
    private Cube CreateCube(IMaterial material, Vector3 position)
    {
        var angle = 20.0f * _cubes.Count;
        
        var cube = new Cube()
        {
            Material = material,
            Position = position,
            Rotation = new Vector3(1.0f * angle, 0.3f * angle, 0.5f * angle)
        };
        
        cube.GenerateVertexObjectBuffer();
        cube.GenerateVertexArrayObjectForPosNormTexVbo();
        
        _cubes.Add(cube);
        
        return cube;
    }

    
    private Cube CreateLamp(IMaterial material, Vector3 position)
    {
        var lamp = new Cube()
        {
            Material = material,
            Position = position,
            Scale = 0.2f
        };
        
        lamp.GenerateVertexObjectBuffer();
        lamp.GenerateVertexArrayObjectForPosNormTexVbo();
            
        _cubes.Add(lamp);

        return lamp;
    }

    #endregion
}