/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Demos.GL.PanoramaViewerDemo;

using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

using MicroEngine;
using MicroEngine.Extensions;
using MicroEngine.Managers;
using MicroEngine.Materials;
using MicroEngine.OGL;
using MicroEngine.SceneObjects;
using MicroEngine.SceneObjects.Cameras;
using MicroEngine.Shaders;


public class Game : IGame
{
    private readonly ResourcesManager _resourcesManager;
    private Scene? _scene;
    
    public string Name => "panorama-viewer-demo";
    
    public ICamera Camera => _scene?.Camera ?? throw new InvalidOperationException("The scene is not initialized.");
    
    
    public Game(ResourcesManager resourcesManager)
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
        
        scene.AddSkybox(LoadSkybox("Textures/Skyboxes/TestSkybox"));
       
        _scene = scene;
        
        GlRenderer.EnableFaceCulling();
        
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
        
        
        const float cameraRotationSpeed = 90f;
        const float sensitivity = 0.2f;
        
        var camera = ((FpsCamera)_scene.Camera);
        
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
    
    private ISceneObject LoadSkybox(string skyboxPath)
    {
        if (string.IsNullOrEmpty(skyboxPath))
        {
            throw new ArgumentNullException(nameof(skyboxPath));
        }

        var skyboxShader = new MultiTextureSkyboxShader();
        skyboxShader.Build();
        
        IMaterial material;
        
        if (Directory.Exists(Path.Combine(_resourcesManager.RootPath, skyboxPath)))
        {
            // If the path is a directory we expect the 6 textures format.
            material = Material.Create(
                [
                    _resourcesManager.LoadTexture("pz", Path.Combine(skyboxPath, "pz.bmp"), TextureWrapMode.ClampToEdge),
                    _resourcesManager.LoadTexture("px", Path.Combine(skyboxPath, "px.bmp"), TextureWrapMode.ClampToEdge),
                    _resourcesManager.LoadTexture("nz", Path.Combine(skyboxPath, "nz.bmp"), TextureWrapMode.ClampToEdge),
                    _resourcesManager.LoadTexture("nx", Path.Combine(skyboxPath, "nx.bmp"), TextureWrapMode.ClampToEdge),
                    _resourcesManager.LoadTexture("py", Path.Combine(skyboxPath, "py.bmp"), TextureWrapMode.ClampToEdge),
                    _resourcesManager.LoadTexture("ny", Path.Combine(skyboxPath, "ny.bmp"), TextureWrapMode.ClampToEdge)
                ],
                skyboxShader);
        }
        else if (File.Exists(Path.Combine(_resourcesManager.RootPath, skyboxPath)))
        {
            // If the path is a image, we expect a single-file texture format.
            throw new NotImplementedException("The single file skybox is not yet implemented.");
        }
        else
        {
            throw new FileNotFoundException($"Skybox path '{skyboxPath}' not found.");    
        }
        
        var skybox = MultiTextureSkybox.Create(material);
        skybox.BuildGeometry();
        
        return skybox;
    }

    #endregion
}
