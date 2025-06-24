/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.Demos.GL.RotatingCubeDemo;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

using MicroEngine;
using MicroEngine.Extensions;
using MicroEngine.Extensions.Generators.Textures;
using MicroEngine.Extensions.Generators.SceneObjects;
using MicroEngine.Managers;
using MicroEngine.Materials;
using MicroEngine.OGL;
using MicroEngine.SceneObjects.Cameras;
using MicroEngine.Shaders;

public class Game : IGame
{
    private Scene? _scene;
    private ISceneObject? _cube;
    
    
    public string Name => "minimal-rotating-cube-demo";

    
    public bool Initialize()
    {
        // We do not want to see back faces of the cube.
        GlRenderer.EnableFaceCulling();
        
        // The scene is the root of the scene graph.
        var scene = new Scene();
        
        // Each scene has a camera.
        scene.SetCamera(new FpsCamera()
        {
            Position = Vector3.UnitZ * 1.5f
        });

        // Generate a white texture.
        var texture = BasicColorTexturesGenerator.GenerateWhiteTexture(256, 256);
        
        // Generate 8x8 blocks black and white checkers pattern into the texture.
        var blockSize = texture.Width / 8;
        for (var y = 0; y < texture.Height; y++)
        {
            for (var x = 0; x < texture.Width; x++)
            {
                var i = (x / blockSize) % 2;
                var j = (y / blockSize) % 2;
                
                if ((i + j) % 2 == 0)
                {
                    texture.PutPixel(x, y, 0, 0, 0);
                }
            }
        }
        
        // We are using the resources manager to keep track of the resources (the texture and the shader).
        var resourcesManager = ResourcesManager.Instance;
        
        // Create a shader for rendering the cube with the texture.
        var shader = new SimpleTextureShader();
        
        // Build the shader, so it is ready to be used.
        shader.Build();
        
        // Generate a cube with the texture.
        var cube = TexturedCubeGenerator.Generate(
            new SimpleTextureMaterial(
                resourcesManager.LoadTexture(
                    "gray",
                    texture,
                    TextureWrapMode.ClampToEdge),
                shader));
        
        // Set the cube's position and build its geometry.
        cube.Position = new Vector3(0.0f, 0.0f, 0.0f);
        cube.BuildGeometry();
        
        // Add the cube to the scene.
        scene.AddChild(cube);
        
        // Remember the scene and the cube.
        _cube = cube;
        _scene = scene;
        
        return true;
    }

    
    private float _angleX;
    private float _angleY;
    private float _angleZ;
    
    public bool Update(float deltaTime)
    {
        if (_scene == null)
        {
            throw new InvalidOperationException("The scene is not initialized.");
        }
     
        // Get the actual keyboard state.
        var keyboardState = InputManager.Instance.KeyboardState;
        
        // If the user presses the Escape key, we will exit the application.
        if (keyboardState.IsKeyDown(Keys.Escape))
        {
            // We will exit the application.
            return false;
        }

        // Calculate the cube rotation angles.
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
        
        // Set the cube's rotation angles.
        _cube!.Rotation = new Vector3(
            MathHelper.DegreesToRadians(_angleX),
            MathHelper.DegreesToRadians(_angleY),
            MathHelper.DegreesToRadians(_angleZ));
        
        // Update the scene.
        _scene.Update(deltaTime);
        
        // We are not exiting the application yet.
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
}
