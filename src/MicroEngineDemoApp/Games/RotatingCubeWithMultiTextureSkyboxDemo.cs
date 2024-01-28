/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngineDemoApp.Games;

using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

using MicroEngine;
using MicroEngine.Core;
using MicroEngine.Extensions;
using MicroEngine.Lights;
using MicroEngine.Managers;
using MicroEngine.Materials;
using MicroEngine.SceneObjects;
using MicroEngine.Shaders;

public class RotatingCubeWithMultiTextureSkyboxDemo : IGame
{
    private Scene? _scene;
    private Cube? _cube;
    
    public string Name => "rotating-cube-demo-multitex-skybox";
    
    public Camera Camera => _scene?.Camera ?? throw new InvalidOperationException("The scene is not initialized.");
    
    
    public bool Initialize(int width, int height)
    {
        var scene = new Scene(
            CreateCamera(width, height));
        
        scene.AddSkybox(CreateSkybox());

        scene.AddChild(CreateCube(
            new Material(
                Texture.LoadFromFile("Resources/Textures/container2.png"),
                Texture.LoadFromFile("Resources/Textures/container2_specular.png"),
                new DefaultShader()),
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
    
    
    private ISceneObject CreateSkybox()
    {
        var skyboxName = "TestSkybox";
        //var skyboxName = "Pond";
        //var skyboxName = "Rocky";
        var material = new MultiTextureMaterial(
            new ITexture[]
            {
                Texture.LoadFromFile($"Resources/Textures/Skyboxes/{skyboxName}/pz.jpg"),
                Texture.LoadFromFile($"Resources/Textures/Skyboxes/{skyboxName}/px.jpg"),
                Texture.LoadFromFile($"Resources/Textures/Skyboxes/{skyboxName}/nz.jpg"),
                Texture.LoadFromFile($"Resources/Textures/Skyboxes/{skyboxName}/nx.jpg"),
                Texture.LoadFromFile($"Resources/Textures/Skyboxes/{skyboxName}/py.jpg"),
                Texture.LoadFromFile($"Resources/Textures/Skyboxes/{skyboxName}/ny.jpg")
            },
            new MultiTextureShader());
        
        var skybox = new MultiTextureSkybox(material);
        
        skybox.GenerateGeometry();
        
        return skybox;
    }

    
    private Cube CreateCube(IMaterial material, Vector3 position)
    {
        var cube = new Cube()
        {
            Material = material,
            Position = position
        };
        
        cube.GenerateGeometry();
        
        _cube = cube;
        
        return cube;
    }

    #endregion
}