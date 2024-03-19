/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngineDemoApp.Games;

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

public class Game5 : IGame
{
    private readonly ResourcesManager _resourcesManager;
    
    private Scene? _scene;
    private readonly SceneObjectController _cubeController = new();
    private readonly SceneObjectController _cameraController = new();

    public string Name => "game-with-cubes5";

    
    public Game5(ResourcesManager resourcesManager)
    {
        _resourcesManager = resourcesManager ?? throw new ArgumentNullException(nameof(resourcesManager));
    }
    
    
    public bool Initialize()
    {
        CreateScene(Program.Settings.WindowWidth, Program.Settings.WindowHeight);
        
        Renderer.EnableFaceCulling();
        
        return true;
    }

    
    private const float RotationSpeed = 60.0f;
    private const float MovementSpeed = 1.0f;

    private const float MouseSensitivity = 0.2f;
    private bool _firstMove = true;
    private Vector2 _lastPos;
    
    public bool Update(float deltaTime)
    {
        if (_scene == null)
        {
            throw new InvalidOperationException("The scene is not initialized.");
        }
        
        var keyboardState = InputManager.Instance.KeyboardState;
        
        if (keyboardState.IsKeyDown(Keys.Escape))
        {
            return false;
        }
        
        
        if (keyboardState.IsKeyDown(Keys.Space))
        {
            _cubeController.Position = Vector3.Zero;
            _cubeController.Rotation = Vector3.Zero;
            
            _cubeController.UpdateAxes();
            
            var camera = (FpsCamera)_scene.Camera;
            
            camera.Yaw = -90.0f;  // TODO: Yaw 0 should be forward.
            camera.Pitch = 0.0f;
        }
        
        
        // Forward/backward movement.
        if (keyboardState.IsKeyDown(Keys.W))
        {
            _cubeController.Advance(MovementSpeed * deltaTime);    
        }
        
        if (keyboardState.IsKeyDown(Keys.S))
        {
            _cubeController.Advance(-MovementSpeed * deltaTime);    
        }
       
        // Left/right movement.
        if (keyboardState.IsKeyDown(Keys.A))
        {
            _cubeController.Strafe(MovementSpeed * deltaTime);    
        }
        
        if (keyboardState.IsKeyDown(Keys.D))
        {
            _cubeController.Strafe(-MovementSpeed * deltaTime);    
        }
       
        // Up/down movement.
        if (keyboardState.IsKeyDown(Keys.LeftControl))
        {
            _cubeController.Ascend(MovementSpeed * deltaTime);    
        }
        
        if (keyboardState.IsKeyDown(Keys.LeftShift))
        {
            _cubeController.Ascend(-MovementSpeed * deltaTime);    
        }

        
        // Yaw rotation.
        if (keyboardState.IsKeyDown(Keys.Q))
        {
            _cubeController.Yaw(MathHelper.DegreesToRadians(RotationSpeed * deltaTime));    
        }
        
        if (keyboardState.IsKeyDown(Keys.E))
        {
            _cubeController.Yaw(MathHelper.DegreesToRadians(-RotationSpeed * deltaTime));    
        }
        
        // Roll rotation.
        if (keyboardState.IsKeyDown(Keys.Left))
        {
            _cubeController.Roll(MathHelper.DegreesToRadians(-RotationSpeed * deltaTime));    
        }
        
        if (keyboardState.IsKeyDown(Keys.Right))
        {
            _cubeController.Roll(MathHelper.DegreesToRadians(RotationSpeed * deltaTime));    
        }
        
        // Pitch rotation.
        if (keyboardState.IsKeyDown(Keys.Up))
        {
            _cubeController.Pitch(MathHelper.DegreesToRadians(-RotationSpeed * deltaTime));    
        }
        
        if (keyboardState.IsKeyDown(Keys.Down))
        {
            _cubeController.Pitch(MathHelper.DegreesToRadians(RotationSpeed * deltaTime));    
        }
        
        
        if (keyboardState.IsKeyDown(Keys.I))
        {
            _cubeController.SetRotationX( _cubeController.Rotation.X + MathHelper.DegreesToRadians(-RotationSpeed * deltaTime));
            _cubeController.UpdateAxes();
        }
        
        if (keyboardState.IsKeyDown(Keys.K))
        {
            _cubeController.SetRotationX(_cubeController.Rotation.X + MathHelper.DegreesToRadians(RotationSpeed * deltaTime));
            _cubeController.UpdateAxes();
        }
        
        if (keyboardState.IsKeyDown(Keys.J))
        {
            _cubeController.SetRotationY(_cubeController.Rotation.Y + MathHelper.DegreesToRadians(RotationSpeed * deltaTime));
            _cubeController.UpdateAxes();
        }
        
        if (keyboardState.IsKeyDown(Keys.L))
        {
            _cubeController.SetRotationY(_cubeController.Rotation.Y + MathHelper.DegreesToRadians(-RotationSpeed * deltaTime));
            _cubeController.UpdateAxes();
        }
        
        if (keyboardState.IsKeyDown(Keys.U))
        {
            _cubeController.SetRotationZ(_cubeController.Rotation.Z + MathHelper.DegreesToRadians(-RotationSpeed * deltaTime));
            _cubeController.UpdateAxes();
        }
        
        if (keyboardState.IsKeyDown(Keys.O))
        {
            _cubeController.SetRotationZ(_cubeController.Rotation.Z + MathHelper.DegreesToRadians(RotationSpeed * deltaTime));
            _cubeController.UpdateAxes();
        }
        
        
        if (keyboardState.IsKeyDown(Keys.T))
        {
            var cameraPitch = ((FpsCamera)_scene.Camera).Pitch + -RotationSpeed * deltaTime;
            if (cameraPitch < -45.0f)
            {
                cameraPitch = -45.0f;
            }
            
            ((FpsCamera)_scene.Camera).Pitch = cameraPitch;
        }
        
        if (keyboardState.IsKeyDown(Keys.G))
        {
            var cameraPitch = ((FpsCamera)_scene.Camera).Pitch + RotationSpeed * deltaTime;
            if (cameraPitch > 45.0f)
            {
                cameraPitch = 45.0f;
            }
            
            ((FpsCamera)_scene.Camera).Pitch = cameraPitch;
        }
        
        if (keyboardState.IsKeyDown(Keys.F))
        {
            var cameraYaw = ((FpsCamera)_scene.Camera).Yaw + RotationSpeed * deltaTime;
            if (cameraYaw > 25.0f)  // 0 = left
            {
                cameraYaw = 25.0f;
            }
            
            ((FpsCamera)_scene.Camera).Yaw = cameraYaw; 
        }
        
        if (keyboardState.IsKeyDown(Keys.H))
        {
            var cameraYaw = ((FpsCamera)_scene.Camera).Yaw + -RotationSpeed * deltaTime;
            if (cameraYaw < -205.0f)  // 180 = right
            {
                cameraYaw = -205.0f;
            }
            
            ((FpsCamera)_scene.Camera).Yaw = cameraYaw; 
        }
        
        
        
        var mouseState = InputManager.Instance.MouseState;
        
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
            
            var cameraYaw = ((FpsCamera)_scene.Camera).Yaw + -deltaX * MouseSensitivity;
            if (cameraYaw < -205.0f)
            {
                cameraYaw = -205.0f;
            }
            else if (cameraYaw > 25.0f)
            {
                cameraYaw = 25.0f;
            }
            
            ((FpsCamera)_scene.Camera).Yaw = cameraYaw;
            
            
            var cameraPitch = ((FpsCamera)_scene.Camera).Pitch + deltaY * MouseSensitivity;
            if (cameraPitch < -45.0f)
            {
                cameraPitch = -45.0f;
            }
            else if (cameraPitch > 45.0f)
            {
                cameraPitch = 45.0f;
            }
            
            ((FpsCamera)_scene.Camera).Pitch = cameraPitch;
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

    public void SetCameraAspectRatio(float aspectRatio)
    {
        if (_scene == null)
        {
            throw new InvalidOperationException("The scene is not initialized.");
        }
        
        _scene.Camera.AspectRatio = aspectRatio;
    }
    
    
    #region creators and generators
    
    private void CreateScene(int width, int height)
    {
        //var scene = new Scene(new FlyByCamera(width / (float)height));
        var scene = new Scene();
        
        scene.SetCamera(new FpsCamera()
        {
            Position = new Vector3(0f, -1f, 1f),
            AspectRatio = width / (float)height
        });
        
        // Skybox
        
        scene.AddSkybox(SimpleStarsSkyboxGenerator.Generate(_resourcesManager));
        
        // Cubes
        
        scene.AddChild(_cubeController);
        
        var cubeMaterial = new Material(
            _resourcesManager.LoadTexture("Textures/container2.png"),
            _resourcesManager.LoadTexture("Textures/container2_specular.png"),
            new DefaultShader(_resourcesManager));

        var cube1 = CreateCube(cubeMaterial, new Vector3(0.0f, 0.0f, 0.0f));
        
        _cubeController.AddChild(cube1);

        var cube2 = CreateCube(cubeMaterial, new Vector3(0.0f, 0.0f, -2.0f));
        cube2.Scale = 0.5f;
        
        cube1.AddChild(cube2);
        
        var cube3 = CreateCube(cubeMaterial, new Vector3(2.0f, 0.0f, 0f));
        cube3.Scale = 0.5f;
        
        cube1.AddChild(cube3);
        
        var cube4 = CreateCube(cubeMaterial, new Vector3(-2.0f, 0.0f, 0f));
        cube4.Scale = 0.5f;
        
        cube1.AddChild(cube4);
       
        
        // Generates 1000 cubes in a 10x10x10 grid.
        for (var x = -2; x < 2; x++)
        {
            for (var y = -2; y < 2; y++)
            {
                for (var z = -2; z < 2; z++)
                {
                    var cube = CreateCube(cubeMaterial, new Vector3(x - 5, y, z - 5));
                    cube.Scale = 0.5f;
                    
                    scene.AddChild(cube);
                }
            }
        }


        //_cameraController.Position = new Vector3(0, -1.0f, 1.1f);
        //cube1.AddChild(_cameraController);
        //
        //scene.RemoveChild(scene.Camera);
        //_cameraController.AddChild(scene.Camera);
        
        scene.RemoveChild(scene.Camera);
        cube1.AddChild(scene.Camera);
        
        
        scene.AddChild(CreateCube(cubeMaterial, new Vector3(10.0f, 10.0f, 0.0f)));
        scene.AddChild(CreateCube(cubeMaterial, new Vector3(5.0f, 10.0f, 5.0f)));
        scene.AddChild(CreateCube(cubeMaterial, new Vector3(10.0f, 5.0f, 0.0f)));
       
        // Lights
        
        scene.AddLight(new DirectionalLight(scene.Lights.Count)
        {
            Ambient = new Vector3(0.65f)
        });
        
        // Plane
       
        var plane = TexturedPlaneGenerator.Generate(
            cubeMaterial,
            10.0f);
        
        plane.Position = new Vector3(0.0f, -3.0f, 0.0f);
        plane.Scale = 50.0f;
        
        plane.BuildGeometry();
        
        scene.AddChild(plane);
       
        _scene = scene;
    }

    
    private ISceneObject CreateCube(IMaterial material, Vector3 position)
    {
        var cube = TexturedCubeGenerator.Generate(material);

        cube.Position = position;
       
        cube.BuildGeometry();
       
        return cube;
    }

    #endregion
}