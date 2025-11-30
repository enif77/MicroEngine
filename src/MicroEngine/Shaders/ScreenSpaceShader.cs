/* Copyright (C) Premysl Fara */

namespace MicroEngine.Shaders;

using OpenTK.Graphics.OpenGL;

using MicroEngine;
using MicroEngine.OGL;

/// <summary>
/// Shader for rendering 2D objects in screen space (text, quads, ...).
/// Each object has its own position in screen space.
/// Overlays the scene with 2D objects in screen space.
/// Overwrites previously rendered 2d and 2D objects.
/// </summary>
public class ScreenSpaceShader : IShader
{
	private readonly GlslShader _glslShader = new();
	private bool _wasBuilt;


	public bool SupportsOpenGLES => true;

	/// <summary>
	/// The default screen space width.
	/// </summary>
	public const int DefaultScreenSpaceWidth = 800;

	/// <summary>
	/// The default screen space height.
	/// </summary>
	public const int DefaultScreenSpaceHeight = 600;

	/// <summary>
	/// The screen space width. The minimal value is 320. The default value is 800.
	/// </summary>
	public int ScreenSpaceWidth { get; init; }

	/// <summary>
	/// The screen space height. The minimal value is 240. The default value is 600.
	/// </summary>
	public int ScreenSpaceHeight { get; init; }


	/// <summary>
	/// Constructor.
	/// </summary>
	/// <param name="screenSpaceWidth">A screen space width.</param>
	/// <param name="screenSpaceHeight">A screen space height.</param>
	/// <exception cref="ArgumentOutOfRangeException">If screen space dimensions are wrong.</exception>
	public ScreenSpaceShader(int screenSpaceWidth = DefaultScreenSpaceWidth,
		int screenSpaceHeight = DefaultScreenSpaceHeight)
	{
		ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(screenSpaceWidth, 320);
		ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(screenSpaceHeight, 240);

		ScreenSpaceWidth = screenSpaceWidth;
		ScreenSpaceHeight = screenSpaceHeight;
	}


	public void Build()
	{
		if (_wasBuilt)
		{
			return;
		}
		
		var vertexShaderSource = VertexShaderSource
			.Replace("320 // $SCREEN_SPACE_WIDTH2", (ScreenSpaceWidth / 2).ToString())
			.Replace("240 // $SCREEN_SPACE_HEIGHT2", (ScreenSpaceHeight / 2).ToString());
		var fragmentShaderSource = FragmentShaderSource;
        
		// Patch the shader to work with OpenGL ES 3.1 and higher.
		if (GlContext.IsGLES)
		{
			vertexShaderSource = vertexShaderSource.Replace(
				"#version 330 core",
				/*language=glsl*/
				"""
				#version 310 es
				precision highp float;
				precision highp int; 
				precision highp sampler2D;
				""");
            
			fragmentShaderSource = fragmentShaderSource.Replace(
				"#version 330 core",
				/*language=glsl*/
				"""
				#version 310 es
				precision highp float;
				precision highp int; 
				precision highp sampler2D;
				""");
		}

		_glslShader.Build(vertexShaderSource, fragmentShaderSource);
		
		_wasBuilt = true;
    }

    
    public int GetAttributeLocation(string name)
    {
        return _glslShader.GetAttribLocation(name);
    }
    
    
    public void Use(Scene scene, ISceneObject sceneObject)
    {
        var material = sceneObject.Material;
        
        material.DiffuseMap.Use(TextureUnit.Texture0);
        
        _glslShader.Use();
        
        _glslShader.SetVector2("screenPosition", sceneObject.Position.Xy);
    }

    private const string VertexShaderSource =
	    /*language=glsl*/
	    """
	    #version 330 core
	    
	    // Screen space coordinates are [0..800][0..600].
	    #define SCREEN_SPACE_WIDTH2 320 // $SCREEN_SPACE_WIDTH2
	    #define SCREEN_SPACE_HEIGHT2 240 // $SCREEN_SPACE_HEIGHT2
	    
	    layout (location = 0) in vec2 aPos;       
	    layout (location = 1) in vec2 aTexCoords;
	    
	    // The position in screen space.
	    uniform vec2 screenPosition;
	    
	    out vec2 TexCoords;
	    
	    void main()
	    {
	    	// [0..W][0..H] -> [-W/2..W/2][-H/2..H/2]
	    	vec2 vertexPosition_homogeneous_space = aPos + screenPosition - vec2(SCREEN_SPACE_WIDTH2, SCREEN_SPACE_HEIGHT2); 
	    	
	    	// map [-W/2..W/2][-H/2..H/2] to [-1..1][-1..1]
	    	vertexPosition_homogeneous_space /= vec2(SCREEN_SPACE_WIDTH2, SCREEN_SPACE_HEIGHT2);  
	    	
	    	// Flip the Y axis.
	    	vertexPosition_homogeneous_space = vec2(vertexPosition_homogeneous_space.x, -vertexPosition_homogeneous_space.y);
	    	
	    	// Output position of the vertex, in clip space
	    	gl_Position = vec4(vertexPosition_homogeneous_space, 0, 1);
	    	
	    	TexCoords = aTexCoords;
	    }
	    """;
    
    private const string FragmentShaderSource =
	    /*language=glsl*/
	    """
	    #version 330 core

	    uniform sampler2D texture0;
	    
	    in vec2 TexCoords;
	    
	    out vec4 FragColor;
	    
	    void main()
	    {
	    	vec4 texColor = texture(texture0, TexCoords);
	    	if (texColor.a < 0.1)
	    	{
	    		discard;
	    	}
	    	
	        FragColor = texColor;
	    }
	    """;
}
	    