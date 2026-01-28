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
    private const string FrontFaceName = "pz";
    private const string BackFaceName = "nz";
    private const string RightFaceName = "px";
    private const string LeftFaceName = "nx";
    private const string TopFaceName = "py";
    private const string BottomFaceName = "ny";
    
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
        
        //scene.AddSkybox(LoadSkybox("Textures/Skyboxes/TestSkybox"));
        //scene.AddSkybox(LoadSkybox("Textures/Skyboxes/Tecnam"));
        scene.AddSkybox(LoadSkybox("Textures/Skyboxes/Tecnam/pano.bmp"));
        
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
        
        var fullSkyboxPath = Path.Combine(_resourcesManager.RootPath, skyboxPath);
        if (Directory.Exists(fullSkyboxPath))
        {
            // If the path is a directory, we expect the 6-texture format.
            material = Material.Create(
                [
                    _resourcesManager.LoadTexture(FrontFaceName, Path.Combine(fullSkyboxPath, "pz.bmp"), TextureWrapMode.ClampToEdge),
                    _resourcesManager.LoadTexture(RightFaceName, Path.Combine(fullSkyboxPath, "px.bmp"), TextureWrapMode.ClampToEdge),
                    _resourcesManager.LoadTexture(BackFaceName, Path.Combine(fullSkyboxPath, "nz.bmp"), TextureWrapMode.ClampToEdge),
                    _resourcesManager.LoadTexture(LeftFaceName, Path.Combine(fullSkyboxPath, "nx.bmp"), TextureWrapMode.ClampToEdge),
                    _resourcesManager.LoadTexture(TopFaceName, Path.Combine(fullSkyboxPath, "py.bmp"), TextureWrapMode.ClampToEdge),
                    _resourcesManager.LoadTexture(BottomFaceName, Path.Combine(fullSkyboxPath, "ny.bmp"), TextureWrapMode.ClampToEdge)
                ],
                skyboxShader);
        }
        else if (File.Exists(fullSkyboxPath))
        {
            // If the path is an image, we expect a single-file texture format.
            var panoImage = _resourcesManager.LoadBmpImage(fullSkyboxPath);
            
            var convertor = new EquirectangularProjectionToCubeMapConverter();
           
            const int maxWidth = 1024;
            const double rotation = 0;
            
            material = Material.Create(
                [
                    _resourcesManager.LoadTexture(FrontFaceName, convertor.RenderFace(panoImage, CubeMapFaceOrientation.PositiveZ, rotation, maxWidth), TextureWrapMode.ClampToEdge),
                    _resourcesManager.LoadTexture(RightFaceName, convertor.RenderFace(panoImage, CubeMapFaceOrientation.PositiveX, rotation, maxWidth), TextureWrapMode.ClampToEdge),
                    _resourcesManager.LoadTexture(BackFaceName, convertor.RenderFace(panoImage, CubeMapFaceOrientation.NegativeZ, rotation, maxWidth), TextureWrapMode.ClampToEdge),
                    _resourcesManager.LoadTexture(LeftFaceName, convertor.RenderFace(panoImage, CubeMapFaceOrientation.NegativeX, rotation, maxWidth), TextureWrapMode.ClampToEdge),
                    _resourcesManager.LoadTexture(TopFaceName, convertor.RenderFace(panoImage, CubeMapFaceOrientation.NegativeY, rotation, maxWidth), TextureWrapMode.ClampToEdge),
                    _resourcesManager.LoadTexture(BottomFaceName, convertor.RenderFace(panoImage, CubeMapFaceOrientation.PositiveY, rotation, maxWidth), TextureWrapMode.ClampToEdge)
                ],
                skyboxShader);
        }
        else
        {
            throw new FileNotFoundException($"Skybox path '{fullSkyboxPath}' not found.");    
        }
        
        var skybox = MultiTextureSkybox.Create(material);
        skybox.BuildGeometry();
        
        return skybox;
    }

    #endregion
}
