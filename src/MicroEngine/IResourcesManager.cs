/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine;

using OpenTK.Graphics.OpenGL4;

/// <summary>
/// Manages resources.
/// </summary>
public interface IResourcesManager
{
    /// <summary>
    /// An optional root directory path.
    /// </summary>
    string RootPath { get; set; }
    
    
    #region general files

    /// <summary>
    /// Determines whether the specified file exists.
    /// </summary>
    /// <param name="path">The file to check.</param>
    /// <returns>True if the file exists.</returns>
    bool FileExists(string path);
    
    /// <summary>
    /// Loads a text file from the given path.
    /// </summary>
    /// <param name="path">A file path.</param>
    /// <returns>A string representing the contents of the requested file.</returns>
    string LoadTextFile(string path);
    
    /// <summary>
    /// Loads a binary file from the given path.
    /// </summary>
    /// <param name="path">A file path.</param>
    /// <returns>An array of bytes representing the contents of the requested file.</returns>
    byte[] LoadBinaryFile(string path);
    
    #endregion


    #region textures

    /// <summary>
    /// Checks whether a texture with the given name exists.
    /// </summary>
    /// <param name="name">A texture name.</param>
    /// <returns>True if a texture with the given name exists.</returns>
    bool HasTexture(string name);
    
    /// <summary>
    /// Gets a texture by its name.
    /// </summary>
    /// <param name="name">A texture name.</param>
    /// <returns>A texture with a given name or the NullTexture instance.</returns>
    ITexture GetTexture(string name);
    
    /// <summary>
    /// Loads a texture from the given path.
    /// </summary>
    /// <param name="path">A path to a texture. Will be used as an unique name for this texture.</param>
    /// <param name="wrapMode">Texture wrap mode. TextureWrapMode.Repeat by default.</param>
    /// <returns>A loaded texture.</returns>
    ITexture LoadTexture(string path, TextureWrapMode wrapMode = TextureWrapMode.Repeat);

    /// <summary>
    /// Loads a texture from the given RGBA bytes.
    /// </summary>
    /// <param name="name">An unique name of this texture.</param>
    /// <param name="pixels">An array of RGBA pixels.</param>
    /// <param name="width">A width of this texture.</param>
    /// <param name="height">A height of this texture.</param>
    /// <param name="wrapMode">Texture wrap mode. TextureWrapMode.Repeat by default.</param>
    /// <returns>A loaded texture.</returns>
    ITexture LoadTextureFromRgbaBytes(
        string name, byte[] pixels, int width, int height,
        TextureWrapMode wrapMode = TextureWrapMode.Repeat);

    #endregion
    
    
    #region materials
    
    /// <summary>
    /// Checks whether a material with the given name exists.
    /// </summary>
    /// <param name="name">A material name.</param>
    /// <returns>True if a material with the given name exists.</returns>
    bool HasMaterial(string name);
    
    /// <summary>
    /// Gets a material by its name.
    /// </summary>
    /// <param name="name">A material name.</param>
    /// <returns>A material with a given name or the NullMaterial instance.</returns>
    IMaterial GetMaterial(string name);
    
    /// <summary>
    /// Loads a material to the resources manager.
    /// If such a material already exists, it won't be replaced.
    /// </summary>
    /// <param name="name">A material name.</param>
    /// <param name="material">A material.</param>
    /// <returns>True if the material was successfully added.</returns>
    bool LoadMaterial(string name, IMaterial material);
    
    #endregion
    
    
    #region shaders
    
    /// <summary>
    /// Checks whether a shader with the given name exists.
    /// </summary>
    /// <param name="name">A shader name.</param>
    /// <returns>True if a shader with the given name exists.</returns>
    bool HasShader(string name);
    
    /// <summary>
    /// Gets a shader by its name.
    /// </summary>
    /// <param name="name">A shader name.</param>
    /// <returns>A shader with a given name or the NullShader instance.</returns>
    IShader GetShader(string name);
    
    /// <summary>
    /// Loads and builds a shader to this resource manager.
    /// </summary>
    /// <param name="name">A shader name.</param>
    /// <param name="shader">A shader.</param>
    /// <returns>True if the shader was successfully added.</returns>
    bool LoadShader(string name, IShader shader);
    
    #endregion
    
    
    #region geometries
    
    /// <summary>
    /// Checks whether a geometry with the given name exists.
    /// </summary>
    /// <param name="name">A geometry name.</param>
    /// <returns>True if a geometry with the given name exists.</returns>
    bool HasGeometry(string name);
    
    /// <summary>
    /// Gets a geometry by its name.
    /// </summary>
    /// <param name="name">A geometry name.</param>
    /// <returns>A geometry with a given name or the NullGeometry instance.</returns>
    IGeometry GetGeometry(string name);
    
    /// <summary>
    /// Loads a geometry to the resources manager.
    /// </summary>
    /// <param name="name">A geometry name.</param>
    /// <param name="geometry">A geometry.</param>
    /// <returns>True if the geometry was successfully added.</returns>
    bool LoadGeometry(string name, IGeometry geometry);
    
    #endregion
}