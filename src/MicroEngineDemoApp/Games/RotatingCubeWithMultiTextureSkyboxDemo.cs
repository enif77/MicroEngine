/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngineDemoApp.Games;

using OpenTK.Graphics.OpenGL4;
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
        
        scene.SetCamera(new FpsCamera()
        {
            Position = new Vector3(0.0f, 0.0f, 3.0f),
            Yaw = -90.0f,
        });
        
        scene.AddSkybox(CreateSkybox());

        var cubeShader = new DefaultShader();
        cubeShader.Build();
        
        scene.AddChild(CreateCube(
            new Material(
                _resourcesManager.LoadTexture("Textures/container2.png"),
                _resourcesManager.LoadTexture("Textures/container2_specular.png"),
                cubeShader),
            new Vector3(0.0f, 0.0f, 0.0f)));
        
        scene.AddLight(new DirectionalLight(scene.Lights.Count)
        {
            Ambient = new Vector3(0.7f, 0.7f, 0.7f),
        });
       
        _scene = scene;
        
        GlRenderer.EnableFaceCulling();
        
        return true;
    }

    
    private bool _firstMove = true;
    private Vector2 _lastPos;
    
    private float _angleX;
    private float _angleY;
    
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
        
        
        const float cameraMovementSpeed = 2.0f;
        const float cameraRotationSpeed = 90f;
        const float sensitivity = 0.2f;
        
        var camera = ((FpsCamera)_scene.Camera);
        
        if (keyboardState.IsKeyDown(Keys.W))
        {
            _scene.Camera.Position += camera.FrontVector * cameraMovementSpeed * deltaTime; // Forward
        }
        if (keyboardState.IsKeyDown(Keys.S))
        {
            _scene.Camera.Position -= camera.FrontVector * cameraMovementSpeed * deltaTime; // Backwards
        }
        if (keyboardState.IsKeyDown(Keys.A))
        {
            _scene.Camera.Position -= camera.RightVector * cameraMovementSpeed * deltaTime; // Left
        }
        if (keyboardState.IsKeyDown(Keys.D))
        {
            _scene.Camera.Position += camera.RightVector * cameraMovementSpeed * deltaTime; // Right
        }
        if (keyboardState.IsKeyDown(Keys.Q))
        {
            _scene.Camera.Position += camera.UpVector * cameraMovementSpeed * deltaTime; // Up
        }
        if (keyboardState.IsKeyDown(Keys.E))
        {
            _scene.Camera.Position -= camera.UpVector * cameraMovementSpeed * deltaTime; // Down
        }
        
        if (keyboardState.IsKeyDown(Keys.Left))
        {
            camera.Yaw -= cameraRotationSpeed * deltaTime;      // Turn left
        }
        if (keyboardState.IsKeyDown(Keys.Right))
        {
            camera.Yaw += cameraRotationSpeed * deltaTime;      // Turn right
        }
        if (keyboardState.IsKeyDown(Keys.Up))
        {
            camera.Pitch -= cameraRotationSpeed * deltaTime;    // Turn down
        }
        if (keyboardState.IsKeyDown(Keys.Down))
        {
            camera.Pitch += cameraRotationSpeed * deltaTime;    // Turn up
        }
        
        
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
    
    private ISceneObject CreateSkybox()
    {
        var skyboxShader = new MultiTextureSkyboxShader();
        skyboxShader.Build();
        
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
            skyboxShader);
        
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