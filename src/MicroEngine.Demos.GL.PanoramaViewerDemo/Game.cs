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
    
    public string Name => "panorama-viewer";
   
    
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
        scene.AddSkybox(LoadSkybox("Textures/Skyboxes/Tecnam"));
        //scene.AddSkybox(LoadSkybox("Textures/Skyboxes/Tecnam/pano.bmp"));
        
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
        
        if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
        {
            // Turn left
            camera.Yaw -= cameraRotationSpeed * deltaTime;
        }
        
        if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
        {
            // Turn right
            camera.Yaw += cameraRotationSpeed * deltaTime;
        }
        
        if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W))
        {
            // Turn down
            camera.Pitch -= cameraRotationSpeed * deltaTime;
        }
        
        if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S))
        {
            // Turn up
            camera.Pitch += cameraRotationSpeed * deltaTime;
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
            material = LoadSkyboxMaterialFromCubeTextures(fullSkyboxPath);
        }
        else if (File.Exists(fullSkyboxPath))
        {
            material = LoadSkyboxMaterialFromEquirectangularProjectionImage(fullSkyboxPath);
        }
        else
        {
            Console.Error.WriteLine($"Skybox path '{fullSkyboxPath}' not found.");
            
            material = LoadDefaultSkyboxMaterial();
        }
        
        var skybox = MultiTextureSkybox.Create(material);
        skybox.BuildGeometry();
        
        return skybox;
    }


    private IMaterial LoadDefaultSkyboxMaterial()
    {
        var skyboxShader = new MultiTextureSkyboxShader();
        skyboxShader.Build();
        
        var skyboxTexture = GenerateCheckerboardTexture();
        
        return Material.Create(
            [
                skyboxTexture,
                skyboxTexture,
                skyboxTexture,
                skyboxTexture,
                skyboxTexture,
                skyboxTexture
            ],
            skyboxShader);
    }


    private IMaterial LoadSkyboxMaterialFromCubeTextures(string skyboxPath)
    {
        var skyboxShader = new MultiTextureSkyboxShader();
        skyboxShader.Build();
        
        // If the path is a directory, we expect the 6-texture format.
        return Material.Create(
            [
                LoadTexture(FrontFaceName, skyboxPath),
                LoadTexture(RightFaceName, skyboxPath),
                LoadTexture(BackFaceName, skyboxPath),
                LoadTexture(LeftFaceName, skyboxPath),
                LoadTexture(TopFaceName, skyboxPath),
                LoadTexture(BottomFaceName, skyboxPath)
            ],
            skyboxShader);
    }


    private ITexture LoadTexture(string textureName, string path)
    {
        var textureFullPath = Path.Combine(path, $"{textureName}.bmp");
        
        return File.Exists(textureFullPath)
            ? _resourcesManager.LoadTexture(textureName, textureFullPath, TextureWrapMode.ClampToEdge)
            : GenerateCheckerboardTexture();
    }

    
    private IMaterial LoadSkyboxMaterialFromEquirectangularProjectionImage(string skyboxImagePath)
    {
        var skyboxShader = new MultiTextureSkyboxShader();
        skyboxShader.Build();
        
        // If the path is an image, we expect a single-file texture format.
        var panoImage = _resourcesManager.LoadBmpImage(skyboxImagePath);
            
        var convertor = new EquirectangularProjectionToCubeMapConverter();
           
        const int maxWidth = 1024;
        const double rotation = 0;
            
        return Material.Create(
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


    /// <summary>
    /// Generates an 8x8 checkerboard texture.
    /// </summary>
    /// <returns>An 8x8 checkerboard texture.</returns>
    private ITexture GenerateCheckerboardTexture()
    {
        if (_resourcesManager.HasTexture("Checkerboard"))
        {
            return _resourcesManager.GetTexture("Checkerboard");
        }

        const int textureSize = 256;
        const int tileSize = textureSize / 8;
        
        var image = new Graphics.Image(textureSize, textureSize);
        
        var pixelIndex = 0;
        for (var y = 0; y < image.Height; y++)
        {
            var tileY = y / tileSize;
            
            for (var x = 0; x < image.Width; x++)
            {
                var tileX = x / tileSize;
                
                var c = (byte)(((tileX + tileY) & 1) == 0 ? 200 : 100);
                
                image.Pixels[pixelIndex++] = c;
                image.Pixels[pixelIndex++] = c;
                image.Pixels[pixelIndex++] = c;
                image.Pixels[pixelIndex++] = 255;
            }
        }

        return _resourcesManager.LoadTexture("Checkerboard", image, TextureWrapMode.ClampToEdge);
    }

    #endregion
}
