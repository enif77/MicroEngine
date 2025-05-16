# Micro Engine

The `MicroEngine` project is a lightweight game engine written in C# using OpenTK. 
It provides a modular structure for creating 3D applications, with components for rendering, 
scene management, and more. Below is an overview of the key classes and interfaces:

![Main build workflow status](https://github.com/enif77/MicroEngine/actions/workflows/dotnet.yml/badge.svg)

## Features

- [x] Scene graph.
- [x] Lights.
- [x] Materials.
- [x] Textures.
- [x] Meshes.
- [x] Cameras.
- [x] Skybox.
- [x] Audio.
- [ ] Shadows.
- [ ] Physics.
- [ ] Scripting.

## Todo

- Skybox should use a cube map.
- Replace IGame.SetCameraAspectRatio().
- Logging using ILogger.
- Write documentation. :-)

---

## **Core Classes and Interfaces**

### **`Scene`**
- Represents the main container for all objects in the 3D world.
- Manages a hierarchy of scene objects (`ISceneObject`).
- Handles updates and rendering of the scene.

### **`ISceneObject`**

`ISceneObject` is an interface for all objects in a scene. It defines hierarchy, representation, 
and transformations of objects. Below is a concise description of its properties and methods:

#### **Hierarchy**
- **`Parent`**: The parent of this object.
- **`Children`**: A list of child objects of this object.

#### **Representation**
- **`Geometry`**: The geometry defining the shape of the object.
- **`Material`**: The material defining the appearance of the object.
- **`IsVisible`**: Indicates whether the object and its children are visible.

#### **Transformations (Position, Rotation, Scale)**
- **`Position`**: The position of the object relative to its parent.
- **`Rotation`**: The rotation of the object (in radians).
- **`Scale`**: The scale of the object relative to its parent.
- **`SetRotationX/Y/Z(float angle)`**: Sets the rotation around the X/Y/Z axis.

#### **Model Matrix**
- **`ModelMatrix`**: The model matrix of the object (updated before rendering).
- **`NeedsModelMatrixUpdate`**: Indicates whether the model matrix needs to be updated.

#### **Other Functions**
- **`GetScene()`**: Returns the `Scene` instance to which the object belongs (throws an exception if not found).


### **`IMaterial`**
- Interface for materials that define how objects appear when rendered.
- Properties:
    - `Shader`: The shader program used for rendering.
    - `Textures`: A collection of textures applied to the material.
- Example implementation:
    - `Material`: A simple material with diffuse and specular properties.

### **`IGeometry`**
- Interface for defining the shape of a 3D object.
- Examples:
    - `DefaultGeometry`: Default geometry with normals and a single texture.
    - `MultiTextureGeometry`: A geometry with multiple textures. No lights.
    - `SingleTextureGeometry`: A geometry with a single texture. No lights.

### **`IShader`**
- Interface for shader programs used in rendering.
- Manages vertex and fragment shaders.
- Example implementation:
    - `DefaultShader`: Default shader with textures and lights.
    - `SkyboxShader`: Shader for rendering sky boxes.
    - `MultiTextureShader`: Shader for rendering objects with multiple textures.
    - `SingleTextureShader`: Shader for rendering objects with a single texture.


### **`ICamera`**

`ICamera` is an interface for all camera objects in a scene. It extends `ISceneObject`
and defines properties and methods for managing the camera's view and projection.

#### **Camera Properties**
- **`Fov`**: The field of view (FOV) in degrees, representing the vertical angle of the camera's view.
- **`Direction`**: A vector indicating the direction the camera is looking.

#### **View and Projection**
- **`GetViewMatrix()`**: Returns the view matrix, which defines the camera's position and orientation in the scene.
- **`GetProjectionMatrix()`**: Returns the projection matrix, which defines how the 3D scene is projected onto a 2D screen.

### **`IGame`**
- Interface for the main game loop.
- Responsibilities:
    - Initializing the game.
    - Updating the game state.
    - Rendering the scene.

---

### **How to Use MicroEngine**

1. **Create a Scene**: Instantiate a `Scene` and populate it with objects implementing `ISceneObject`.
2. **Add Geometry and Materials**: Use `IGeometry` and `IMaterial` to define the appearance of objects.
3. **Set Up a Camera**: Add an `ICamera` to the scene to define the view.
4. **Implement a Game Loop**: Use `IGame` to manage updates and rendering.

This modular structure allows you to extend and customize the engine for your specific needs.

## Links

Camera:

- https://gamedev.stackexchange.com/questions/183748/3d-camera-rotation-unwanted-roll-space-flight-cam
- https://stackoverflow.com/questions/64953941/how-to-implement-the-roll-angle-together-with-yaw-and-pitch-in-glmlookat-funct
- https://learnopengl.com/Getting-started/Camera 
- https://kengine.sourceforge.net/tutorial/vc/camera-eng.htm
- https://cboard.cprogramming.com/game-programming/135390-how-properly-move-strafe-yaw-pitch-camera-opengl-glut-using-glulookat.html
- https://tuttlem.github.io/2013/12/30/a-camera-implementation-in-c.html
- https://gamedev.stackexchange.com/questions/136174/im-rotating-an-object-on-two-axes-so-why-does-it-keep-twisting-around-the-thir
- https://gamedev.stackexchange.com/questions/183748/3d-camera-rotation-unwanted-roll-space-flight-cam  
- https://swiftgl.github.io/learn/01-camera.html
 
Skybox:

- https://learnopengl.com/Advanced-OpenGL/Cubemaps
