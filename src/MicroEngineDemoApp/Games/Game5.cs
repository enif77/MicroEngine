/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngineDemoApp.Games;

using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

using MicroEngine;
using MicroEngine.Extensions;
using MicroEngine.Extensions.Generators.SceneObjects;
using MicroEngine.Managers;
using MicroEngine.Materials;
using MicroEngine.OGL;
using MicroEngine.SceneObjects;
using MicroEngine.SceneObjects.Cameras;
using MicroEngine.SceneObjects.Lights;
using MicroEngine.Shaders;

public class Game5 : IGame
{
    private readonly ResourcesManager _resourcesManager;
    
    private Scene? _scene;
    private readonly SceneObjectController _rocketController = new();

    public string Name => "game-with-cubes5";

    
    public Game5(ResourcesManager resourcesManager)
    {
        _resourcesManager = resourcesManager ?? throw new ArgumentNullException(nameof(resourcesManager));
    }
    
    
    public bool Initialize()
    {
        CreateScene();
        
        GlRenderer.EnableFaceCulling();
        
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
            _rocketController.Position = Vector3.Zero;
            _rocketController.Rotation = new Vector3(0f, 0f, 0f);
            
            _rocketController.UpdateAxes();
            
            var camera = (FpsCamera)_scene.Camera;
            
            camera.Yaw = 90.0f;  // TODO: Yaw 0 should be forward.
            camera.Pitch = 0.0f;
        }
        
        
        // Forward/backward movement.
        if (keyboardState.IsKeyDown(Keys.W))
        {
            _rocketController.Advance(MovementSpeed * deltaTime);    
        }
        
        if (keyboardState.IsKeyDown(Keys.S))
        {
            _rocketController.Advance(-MovementSpeed * deltaTime);    
        }
       
        // Left/right movement.
        if (keyboardState.IsKeyDown(Keys.A))
        {
            _rocketController.Strafe(-MovementSpeed * deltaTime);    
        }
        
        if (keyboardState.IsKeyDown(Keys.D))
        {
            _rocketController.Strafe(MovementSpeed * deltaTime);    
        }
       
        // Up/down movement.
        if (keyboardState.IsKeyDown(Keys.LeftControl))
        {
            _rocketController.Ascend(-MovementSpeed * deltaTime);    
        }
        
        if (keyboardState.IsKeyDown(Keys.LeftShift))
        {
            _rocketController.Ascend(MovementSpeed * deltaTime);    
        }

        
        // Yaw rotation.
        if (keyboardState.IsKeyDown(Keys.Q))
        {
            _rocketController.Yaw(MathHelper.DegreesToRadians(-RotationSpeed * deltaTime));    
        }
        
        if (keyboardState.IsKeyDown(Keys.E))
        {
            _rocketController.Yaw(MathHelper.DegreesToRadians(RotationSpeed * deltaTime));
        }
        
        // Roll rotation.
        if (keyboardState.IsKeyDown(Keys.Left))
        {
            _rocketController.Roll(MathHelper.DegreesToRadians(-RotationSpeed * deltaTime));    
        }
        
        if (keyboardState.IsKeyDown(Keys.Right))
        {
            _rocketController.Roll(MathHelper.DegreesToRadians(RotationSpeed * deltaTime));    
        }
        
        // Pitch rotation.
        if (keyboardState.IsKeyDown(Keys.Up))
        {
            _rocketController.Pitch(MathHelper.DegreesToRadians(-RotationSpeed * deltaTime));    
        }
        
        if (keyboardState.IsKeyDown(Keys.Down))
        {
            _rocketController.Pitch(MathHelper.DegreesToRadians(RotationSpeed * deltaTime));    
        }
        
        
        if (keyboardState.IsKeyDown(Keys.I))
        {
            _rocketController.SetRotationX( _rocketController.Rotation.X + MathHelper.DegreesToRadians(-RotationSpeed * deltaTime));
            _rocketController.UpdateAxes();
        }
        
        if (keyboardState.IsKeyDown(Keys.K))
        {
            _rocketController.SetRotationX(_rocketController.Rotation.X + MathHelper.DegreesToRadians(RotationSpeed * deltaTime));
            _rocketController.UpdateAxes();
        }
        
        if (keyboardState.IsKeyDown(Keys.J))
        {
            _rocketController.SetRotationY(_rocketController.Rotation.Y + MathHelper.DegreesToRadians(RotationSpeed * deltaTime));
            _rocketController.UpdateAxes();
        }
        
        if (keyboardState.IsKeyDown(Keys.L))
        {
            _rocketController.SetRotationY(_rocketController.Rotation.Y + MathHelper.DegreesToRadians(-RotationSpeed * deltaTime));
            _rocketController.UpdateAxes();
        }
        
        if (keyboardState.IsKeyDown(Keys.U))
        {
            _rocketController.SetRotationZ(_rocketController.Rotation.Z + MathHelper.DegreesToRadians(-RotationSpeed * deltaTime));
            _rocketController.UpdateAxes();
        }
        
        if (keyboardState.IsKeyDown(Keys.O))
        {
            _rocketController.SetRotationZ(_rocketController.Rotation.Z + MathHelper.DegreesToRadians(RotationSpeed * deltaTime));
            _rocketController.UpdateAxes();
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
            var camera = (FpsCamera)_scene.Camera;
            
            var deltaX = mouseState.X - _lastPos.X;
            var deltaY = mouseState.Y - _lastPos.Y;
            _lastPos = new Vector2(mouseState.X, mouseState.Y);
            
            var cameraYaw = camera.Yaw + deltaX * MouseSensitivity;
            if (cameraYaw < -205.0f)
            {
                cameraYaw = -205.0f;
            }
            else if (cameraYaw > 25.0f)
            {
                cameraYaw = 25.0f;
            }
            
            camera.Yaw = cameraYaw;
            
            
            var cameraPitch = camera.Pitch + -deltaY * MouseSensitivity;
            if (cameraPitch < -45.0f)
            {
                cameraPitch = -45.0f;
            }
            else if (cameraPitch > 45.0f)
            {
                cameraPitch = 45.0f;
            }
            
            camera.Pitch = cameraPitch;
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
    
    private void CreateScene()
    {
        var cubeShader = new DefaultShader();
        cubeShader.Build();
        
        // Materials
        var cubeMaterial = Material.Create(
            _resourcesManager.LoadTexture("container2", "Textures/container2.bmp"),
            _resourcesManager.LoadTexture("container2_specular", "Textures/container2_specular.bmp"),
            cubeShader);

        // Scene
        var scene = new Scene();
        
        // Camera. We want to add a camera to the scene, but we will place it as a child of a cube/rocket.
        scene.SetCamera(new FpsCamera(), false);
        
        // Skybox
        scene.AddSkybox(SimpleStarsSkyboxGenerator.Generate(_resourcesManager));
        
        // The rocket made of cubes.
        scene.AddChild(_rocketController);
        
        var rocket = CreateCube(cubeMaterial, new Vector3(0.0f, 0.0f, 0.0f));
        
        rocket.AddChild(CreateCube(cubeMaterial, new Vector3(0.0f, 0.0f, -2.0f), 0.5f));
        rocket.AddChild(CreateCube(cubeMaterial, new Vector3(2.0f, 0.0f, 0f), 0.5f));
        rocket.AddChild(CreateCube(cubeMaterial, new Vector3(-2.0f, 0.0f, 0f), 0.5f));
        
        _rocketController.AddChild(rocket);
        
        // Generates M cubes in a NxNxN grid.
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
       
        // Put the camera on the first cube, so it appears as if the camera is attached to the cube.
        rocket.AddChild(scene.Camera);
        
        // Set the camera position and initial rotation relative to the cube.
        scene.Camera.Position = new Vector3(0f, 1f, 1.0f);
        ((FpsCamera)scene.Camera).Yaw = 90.0f;
        ((FpsCamera)scene.Camera).Pitch = 0.0f;
        
        // Add some more cubes.
        scene.AddChild(CreateCube(cubeMaterial, new Vector3(10.0f, 10.0f, 0.0f)));
        scene.AddChild(CreateCube(cubeMaterial, new Vector3(5.0f, 10.0f, 5.0f)));
        scene.AddChild(CreateCube(cubeMaterial, new Vector3(10.0f, 5.0f, 0.0f)));
        
        // Plane
        var plane = TexturedPlaneGenerator.Generate(
            cubeMaterial,
            10.0f);
        plane.Position = new Vector3(0.0f, -3.0f, 0.0f);
        plane.Scale = 50.0f;
        plane.BuildGeometry();
        scene.AddChild(plane);
       
        // Lights
        scene.AddLight(new DirectionalLight(scene.Lights.Count)
        {
            Ambient = new Vector3(0.65f)
        });
        
        _scene = scene;
    }

    
    private ISceneObject CreateCube(IMaterial material, Vector3 position, float scale = 1.0f)
    {
        var cube = TexturedCubeGenerator.Generate(material);

        cube.Position = position;
        cube.Scale = scale;
       
        cube.BuildGeometry();
       
        return cube;
    }

    #endregion
}