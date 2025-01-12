/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngineDemoApp.Games;

using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

using MicroEngine;
using MicroEngine.Extensions;
using MicroEngine.Extensions.Generators.SceneObjects;
using MicroEngine.Managers;
using MicroEngine.Materials;
using MicroEngine.SceneObjects;
using MicroEngine.Shaders;

public class MinimalGameDemo : IGame
{
    private readonly ResourcesManager _resourcesManager;
    private Scene? _scene;
    private ISceneObject? _cube;
    
    public string Name => "game";
    
    public ICamera Camera => _scene?.Camera ?? throw new InvalidOperationException("The scene is not initialized.");
    
    
    public MinimalGameDemo(ResourcesManager resourcesManager)
    {
        _resourcesManager = resourcesManager ?? throw new ArgumentNullException(nameof(resourcesManager));
    }
    
    
    public bool Initialize()
    {
        var scene = new Scene();
        
        scene.SetCamera(new FpsCamera()
        {
            Position = Vector3.UnitZ * 1.5f
        });

        _cube = CreateCube(
            new SimpleTextureMaterial(
                _resourcesManager.LoadTexture("Textures/container2.png"),
                new SimpleTextureShader(_resourcesManager)),
            new Vector3(0.0f, 0.0f, 0.0f));
        
        scene.AddChild(_cube);
        
        _scene = scene;
        
        Renderer.EnableFaceCulling();
        
        return true;
    }

    
    private float _angleX;
    private float _angleY;
    private float _angleZ;
    
    public bool Update(float deltaTime)
    {
        var keyboardState = InputManager.Instance.KeyboardState;
        
        if (_scene == null)
        {
            throw new InvalidOperationException("The scene is not initialized.");
        }
     
        if (keyboardState.IsKeyDown(Keys.Escape))
        {
            return false;
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

        _angleZ += 10.0f * deltaTime;
        while (_angleZ > 360.0f)
        {
            _angleZ -= 360.0f;
        }
        
        _cube!.Rotation = new Vector3(
            MathHelper.DegreesToRadians(_angleX),
            MathHelper.DegreesToRadians(_angleY),
            MathHelper.DegreesToRadians(_angleZ));
        
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
   
    private static ISceneObject CreateCube(IMaterial material, Vector3 position)
    {
        var cube = TexturedCubeGenerator.Generate(material);
        cube.Position = position;
        
        cube.BuildGeometry();
       
        return cube;
    }

    #endregion
}