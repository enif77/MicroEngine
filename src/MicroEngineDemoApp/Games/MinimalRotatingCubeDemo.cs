/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngineDemoApp.Games;

using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

using MicroEngine;
using MicroEngine.Core;
using MicroEngine.Extensions;
using MicroEngine.Materials;
using MicroEngine.SceneObjects;
using MicroEngine.Shaders;

public class MinimalRotatingCubeDemo : IGame
{
    private Scene? _scene;
    private Cube? _cube;
    
    public Camera Camera => _scene?.Camera ?? throw new InvalidOperationException("The scene is not initialized.");
    
    
    public bool Initialize(int width, int height)
    {
        var scene = new Scene(
            CreateCamera(width, height));
        
        scene.AddChild(CreateCube(
            new SimpleTextureMaterial(
                Texture.LoadFromFile("Resources/Textures/container2.png"),
                new SimpleTextureShader()),
            new Vector3(0.0f, 0.0f, 0.0f)));
        
        _scene = scene;
        
        return true;
    }

    
    private float _angleX = 0.0f;
    private float _angleY = 0.0f;
    private float _angleZ = 0.0f;
    
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
        
        _cube!.Rotation = new Vector3(MathHelper.DegreesToRadians(_angleX), MathHelper.DegreesToRadians(_angleY), MathHelper.DegreesToRadians(_angleZ));
        
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
    }

    public void SetCameraAspectRatio(float aspectRatio)
    {
    }
    
    
    #region creators and generators

    private Camera CreateCamera(int windowWidth, int windowHeight)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(windowWidth);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(windowHeight);

        return new Camera(Vector3.UnitZ * 1.5f, windowWidth / (float)windowHeight);
    }
    
    
    private Cube CreateCube(IMaterial material, Vector3 position)
    {
        var cube = new Cube()
        {
            Material = material,
            Position = position
        };
        
        cube.GenerateVertexObjectBuffer();
        cube.GenerateVertexArrayObjectForPosNormTexVbo();
        
        _cube = cube;
        
        return cube;
    }

    #endregion
}