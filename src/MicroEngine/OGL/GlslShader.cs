/* Copyright (C) Premysl Fara and Contributors */

namespace MicroEngine.OGL;

using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

/// <summary>
/// Class representing a GLSL shader program.
/// </summary>
public sealed class GlslShader : IDisposable
{
    private readonly Dictionary<string, int> _uniformLocations = new();
    
    private int _handle;
    private bool _wasBuild;
    
    /// <summary>
    /// Builds the shader program.
    /// </summary>
    /// <param name="vertexShaderSource">The source code of the vertex shader.</param>
    /// <param name="fragmentShaderSource">The source code of the fragment shader.</param>
    /// <exception cref="ArgumentException">Thrown if the vertex or fragment shader source is null or whitespace.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the shader has already been built.</exception>
    public void Build(string vertexShaderSource, string fragmentShaderSource)
    {
        if (string.IsNullOrWhiteSpace(vertexShaderSource))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(vertexShaderSource));
        }

        if (string.IsNullOrWhiteSpace(fragmentShaderSource))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(fragmentShaderSource));
        }
        
        if (_wasBuild)
        {
            throw new InvalidOperationException("Shader has already been built.");
        }
        
        // The first thing we need to do is create the shader program.
        // A shader program is a collection of shaders that can be used together.
        // In OpenGL, you can have multiple shaders in a single program, but for now, we'll just use one vertex and one fragment shader.
         // There are several different types of shaders, but the only two you need for basic rendering are the vertex and fragment shaders.
        // The vertex shader is responsible for moving around vertices, and uploading that data to the fragment shader.
        //   The vertex shader won't be too important here, but they'll be more important later.
        // The fragment shader is responsible for then converting the vertices to "fragments", which represent all the data OpenGL needs to draw a pixel.
        //   The fragment shader is what we'll be using the most here.

        // Load vertex shader and compile

        // GL.CreateShader will create an empty shader (obviously). The ShaderType enum denotes which type of shader will be created.
        var vertexShader = GL.CreateShader(ShaderType.VertexShader);

        // Now, bind the GLSL source code
        GL.ShaderSource(vertexShader, vertexShaderSource);

        // And then compile
        CompileShader(vertexShader);

        // We do the same for the fragment shader.
        var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fragmentShaderSource);
        CompileShader(fragmentShader);

        // These two shaders must then be merged into a shader program, which can then be used by OpenGL.
        // To do this, create a program...
        _handle = GL.CreateProgram();

        // Attach both shaders...
        GL.AttachShader(_handle, vertexShader);
        GL.AttachShader(_handle, fragmentShader);

        // And then link them together.
        LinkProgram(_handle);

        // When the shader program is linked, it no longer needs the individual shaders attached to it; the compiled code is copied into the shader program.
        // Detach them, and then delete them.
        GL.DetachShader(_handle, vertexShader);
        GL.DetachShader(_handle, fragmentShader);
        GL.DeleteShader(fragmentShader);
        GL.DeleteShader(vertexShader);

        // The shader is now ready to go, but first, we're going to cache all the shader uniform locations.
        // Querying this from the shader is very slow, so we do it once on initialization and reuse those values
        // later.

        // First, we have to get the number of active uniforms in the shader.
        GL.GetProgrami(_handle, ProgramProperty.ActiveUniforms, out var numberOfUniforms);
        
        // Loop over all the uniforms,
        for (var i = 0; i < numberOfUniforms; i++)
        {
            // get the name of this uniform,
            //var key = GL.GetActiveUniform(_handle, i, out _, out _);

            // public static unsafe string GetActiveUniform(int program, uint index, int bufSize, out int length, out int size, out UniformType type)
            var key = GL.GetActiveUniform(_handle, (uint)i, 1024, out _, out _, out _);
            
            // get the location,
            var location = GL.GetUniformLocation(_handle, key);

            // and then add it to the dictionary.
            _uniformLocations.Add(key, location);
        }
        
        _wasBuild = true;
    }
    
    /// <summary>
    /// A wrapper function that enables the shader program.
    /// </summary>
    public void Use()
    {
        CheckWasBuilt();
        
        GL.UseProgram(_handle);
    }
    
    /// <summary>
    /// The shader sources provided with this project use hardcoded layout(location)-s. If you want to do it dynamically,
    /// you can omit the layout(location=X) lines in the vertex shader and use this in VertexAttribPointer instead of the hardcoded values.
    /// </summary>
    /// <param name="attribName">An attribute name.</param>
    /// <returns>An attribute location.</returns>
    public int GetAttribLocation(string attribName)
    {
        CheckWasBuilt();
        
        return GL.GetAttribLocation(_handle, attribName);
    }

    // Uniform setters
    // Uniforms are variables that can be set by user code, instead of reading them from the VBO.
    // You use VBOs for vertex-related data, and uniforms for almost everything else.

    // Setting a uniform is almost always the exact same, so I'll explain it here once, instead of in every method:
    //     1. Bind the program you want to set the uniform on
    //     2. Get a handle to the location of the uniform with GL.GetUniformLocation.
    //     3. Use the appropriate GL.Uniform* function to set the uniform.

    /// <summary>
    /// Set a uniform int on this shader.
    /// </summary>
    /// <param name="name">The name of the uniform</param>
    /// <param name="data">The data to set</param>
    public void SetInt(string name, int data)
    {
        CheckWasBuilt();
        
        GL.UseProgram(_handle);
        GL.Uniform1i(_uniformLocations[name], data);
    }

    /// <summary>
    /// Set a uniform float on this shader.
    /// </summary>
    /// <param name="name">The name of the uniform</param>
    /// <param name="data">The data to set</param>
    public void SetFloat(string name, float data)
    {
        CheckWasBuilt();
        
        GL.UseProgram(_handle);
        GL.Uniform1f(_uniformLocations[name], data);
    }

    /// <summary>
    /// Set a uniform Matrix4 on this shader
    /// </summary>
    /// <param name="name">The name of the uniform</param>
    /// <param name="data">The data to set</param>
    /// <remarks>
    ///   <para>
    ///   The matrix is transposed before being sent to the shader.
    ///   </para>
    /// </remarks>
    public void SetMatrix4(string name, Matrix4 data)
    {
        CheckWasBuilt();
        
        //GL.UseProgram(_handle);
        //GL.UniformMatrix4(_uniformLocations[name], true, ref data);
        GL.ProgramUniformMatrix4f(_handle, _uniformLocations[name], 1, true, ref data);
    }

    /// <summary>
    /// Set a uniform Vector2 on this shader.
    /// </summary>
    /// <param name="name">The name of the uniform</param>
    /// <param name="data">The data to set</param>
    public void SetVector2(string name, Vector2 data)
    {
        CheckWasBuilt();
        
        //GL.UseProgram(_handle);
        //GL.Uniform2(_uniformLocations[name], data);
        GL.ProgramUniform2f(_handle, _uniformLocations[name], 1, ref data);
    }
    
    /// <summary>
    /// Set a uniform Vector3 on this shader.
    /// </summary>
    /// <param name="name">The name of the uniform</param>
    /// <param name="data">The data to set</param>
    public void SetVector3(string name, Vector3 data)
    {
        CheckWasBuilt();
        
        //GL.UseProgram(_handle);
        //GL.Uniform3(_uniformLocations[name], data);
        GL.ProgramUniform3f(_handle, _uniformLocations[name], 1, ref data);
    }
    
    
    private void CheckWasBuilt()
    {
        if (!_wasBuild || _handle == 0)
        {
            throw new InvalidOperationException("GlslShader: Shader has not been built yet. Call Build() before using the shader.");
        }
    }
    
    
    private static void CompileShader(int shader)
    {
        // Try to compile the shader
        GL.CompileShader(shader);

        // Check for compilation errors
        GL.GetShaderi(shader, ShaderParameterName.CompileStatus, out var code);
        if (code != (int)All.True)
        {
            GL.GetShaderInfoLog(shader, out var glShaderErrorMessage);
            throw new Exception($"GlslShader: Error occurred whilst compiling Shader({shader}), error({glShaderErrorMessage})");
        }
    }

    
    private static void LinkProgram(int program)
    {
        // We link the program
        GL.LinkProgram(program);

        // Check for linking errors
        GL.GetProgrami(program, ProgramProperty.LinkStatus, out var code);
        if (code != (int)All.True)
        {
            GL.GetProgramInfoLog(program, out var glProgramErrorMessage);
            throw new Exception($"GlslShader: Error occurred whilst linking Program({program}), error({glProgramErrorMessage})");
        }
    }
    
    
    private bool _disposedValue;

    private void Dispose(bool disposing)
    {
        if (_disposedValue)
        {
            return;
        }

        GL.DeleteProgram(_handle);

        _disposedValue = true;
    }

    
    ~GlslShader()
    {
        if (_disposedValue == false)
        {
            Console.WriteLine("GlslShader: GPU Resource leak! Did you forget to call Dispose()?");
        }
    }


    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
