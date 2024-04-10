/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngineDemoApp.Games;

using OpenTK.Graphics.OpenGL4;
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

public class RotatingCubeWithMultiTextureSkyboxDemo : IGame
{
    private readonly ResourcesManager _resourcesManager;
    private Scene? _scene;
    private ISceneObject? _cube;
    
    public string Name => "rotating-cube-demo-multitex-skybox";
    
    public ICamera Camera => _scene?.Camera ?? throw new InvalidOperationException("The scene is not initialized.");
    
    
    public RotatingCubeWithMultiTextureSkyboxDemo(ResourcesManager resourcesManager)
    {
        _resourcesManager = resourcesManager ?? throw new ArgumentNullException(nameof(resourcesManager));
    }
    
    
    public bool Initialize()
    {
        var scene = new Scene();
        
        scene.SetCamera(CreateCamera(Program.Settings.WindowWidth, Program.Settings.WindowHeight));
        
        scene.AddSkybox(CreateSkybox());

        scene.AddChild(CreateCube(
            new Material(
                _resourcesManager.LoadTexture("Textures/container2.png"),
                _resourcesManager.LoadTexture("Textures/container2_specular.png"),
                new DefaultShader(_resourcesManager)),
            new Vector3(0.0f, 0.0f, 0.0f)));
        
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
    
    private float _angleX = 0.0f;
    private float _angleY = 0.0f;
    
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

        
        _angleX += 20.0f * deltaTime;
        while (_angleX > 360.0f)
        {
            _angleX -= 360.0f;
        }
        
        _angleY += 50.0f * deltaTime;
        while (_angleY > 360.0f)
        {
            _angleY -= 360.0f;
        }
        
        _cube!.Rotation = new Vector3(MathHelper.DegreesToRadians(_angleX), MathHelper.DegreesToRadians(_angleY), 0.0f);
        
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
            Position = Vector3.UnitZ * 3
        };
    }
    
    
    private ISceneObject CreateSkybox()
    {
        var skyboxName = "TestSkybox";
        //var skyboxName = "Pond";
        //var skyboxName = "Rocky";
        var material = new MultiTextureMaterial(
            [
                _resourcesManager.LoadTexture($"Textures/Skyboxes/{skyboxName}/pz.jpg", TextureWrapMode.ClampToEdge),
                _resourcesManager.LoadTexture($"Textures/Skyboxes/{skyboxName}/px.jpg", TextureWrapMode.ClampToEdge),
                _resourcesManager.LoadTexture($"Textures/Skyboxes/{skyboxName}/nz.jpg", TextureWrapMode.ClampToEdge),
                _resourcesManager.LoadTexture($"Textures/Skyboxes/{skyboxName}/nx.jpg", TextureWrapMode.ClampToEdge),
                _resourcesManager.LoadTexture($"Textures/Skyboxes/{skyboxName}/py.jpg", TextureWrapMode.ClampToEdge),
                _resourcesManager.LoadTexture($"Textures/Skyboxes/{skyboxName}/ny.jpg", TextureWrapMode.ClampToEdge)
            ],
            new MultiTextureSkyboxShader(_resourcesManager));
        
        var skybox = SkyboxGenerator.Generate(material);
        
        skybox.BuildGeometry();
        
        return skybox;
    }

    
    private ISceneObject CreateCube(IMaterial material, Vector3 position)
    {
        var cube = TexturedCubeGenerator.Generate(material);
        cube.Position = position;
        
        cube.BuildGeometry();
        
        _cube = cube;
        
        return cube;
    }

    #endregion
}