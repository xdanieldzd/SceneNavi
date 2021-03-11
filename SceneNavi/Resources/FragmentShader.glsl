#version 120

// Inputs
varying vec3 in_position;
varying vec3 in_normal;

// Combiner parameters
uniform bool combinerIsTwoCycleMode;
uniform int combinerColorParam[8];
uniform int combinerAlphaParam[8];

// Combiner inputs
uniform sampler2D combinerInputTexel[2];
uniform vec4 combinerInputPrimitive;
uniform float combinerInputPrimitiveLODFrac;
uniform vec4 combinerInputEnvironment;

// Combiner outputs & temporary variables
vec4 texelColors[2];
vec4 combinerCombined;

// Combiner function definitions
vec4 runCombiner(void);
vec3 combinerGetColorA(int);
vec3 combinerGetColorB(int);
vec3 combinerGetColorC(int);
vec3 combinerGetColorD(int);
float combinerGetAlphaABD(int);
float combinerGetAlphaC(int);

// Lighting function definitions
vec4 applyLighting(vec4);

// Main function
void main(void)
{
	vec4 outputColor = runCombiner();
	//outputColor = applyLighting(outputColor);		// TODO fixme
	gl_FragColor = outputColor;
}

// Functions
vec4 runCombiner(void)
{
	combinerCombined = vec4(0.0);

	texelColors[0] = texture2D(combinerInputTexel[0], gl_TexCoord[0].st);
	texelColors[1] = texture2D(combinerInputTexel[1], gl_TexCoord[1].st);

	combinerCombined.rgb = (combinerGetColorA(combinerColorParam[0]) - combinerGetColorB(combinerColorParam[1])) * combinerGetColorC(combinerColorParam[2]) + combinerGetColorD(combinerColorParam[3]);
	combinerCombined.a = (combinerGetAlphaABD(combinerAlphaParam[0]) - combinerGetAlphaABD(combinerAlphaParam[1])) * combinerGetAlphaC(combinerAlphaParam[2]) + combinerGetAlphaABD(combinerAlphaParam[3]);

	if (combinerIsTwoCycleMode == true)
	{
		combinerCombined.rgb = (combinerGetColorA(combinerColorParam[4]) - combinerGetColorB(combinerColorParam[5])) * combinerGetColorC(combinerColorParam[6]) + combinerGetColorD(combinerColorParam[7]);
		combinerCombined.a = (combinerGetAlphaABD(combinerAlphaParam[4]) - combinerGetAlphaABD(combinerAlphaParam[5])) * combinerGetAlphaC(combinerAlphaParam[6]) + combinerGetAlphaABD(combinerAlphaParam[7]);
	}

	return combinerCombined;
}

vec3 combinerGetColorA(int param)
{
	switch (param)
	{
		case 0: return combinerCombined.rgb;						// G_CCMUX_COMBINED
		case 1: return texelColors[0].rgb;							// G_CCMUX_TEXEL0
		case 2: return texelColors[1].rgb;							// G_CCMUX_TEXEL1
		case 3: return combinerInputPrimitive.rgb;					// G_CCMUX_PRIMITIVE
		case 4: return gl_Color.rgb;								// G_CCMUX_SHADE
		case 5: return combinerInputEnvironment.rgb;				// G_CCMUX_ENVIRONMENT
		case 6: return vec3(1.0);									// G_CCMUX_1
		case 7: return noise3(gl_FragCoord.x * gl_FragCoord.y);		// G_CCMUX_NOISE
		default: return vec3(0.0);									// G_CCMUX_0
    }
}

vec3 combinerGetColorB(int param)
{
	switch (param)
	{
		case 0: return combinerCombined.rgb;						// G_CCMUX_COMBINED
		case 1: return texelColors[0].rgb;							// G_CCMUX_TEXEL0
		case 2: return texelColors[1].rgb;							// G_CCMUX_TEXEL1
		case 3: return combinerInputPrimitive.rgb;					// G_CCMUX_PRIMITIVE
		case 4: return gl_Color.rgb;								// G_CCMUX_SHADE
		case 5: return combinerInputEnvironment.rgb;				// G_CCMUX_ENVIRONMENT
		case 6: return vec3(0.5);									// G_CCMUX_CENTER -- chroma keying center, unemulated
		case 7: return vec3(0.5);									// G_CCMUX_K4 -- YUV-to-RGB constant K4, unemulated
		default: return vec3(0.0);									// G_CCMUX_0
    }
}

vec3 combinerGetColorC(int param)
{
	switch (param)
	{
		case 0: return combinerCombined.rgb;						// G_CCMUX_COMBINED
		case 1: return texelColors[0].rgb;							// G_CCMUX_TEXEL0
		case 2: return texelColors[1].rgb;							// G_CCMUX_TEXEL1
		case 3: return combinerInputPrimitive.rgb;					// G_CCMUX_PRIMITIVE
		case 4: return gl_Color.rgb;								// G_CCMUX_SHADE
		case 5: return combinerInputEnvironment.rgb;				// G_CCMUX_ENVIRONMENT
		case 6: return vec3(0.5);									// G_CCMUX_SCALE -- chroma keying scale, unemulated
		case 7: return vec3(combinerCombined.a);					// G_CCMUX_COMBINED_ALPHA
		case 8: return vec3(texelColors[0].a);						// G_CCMUX_TEXEL0_ALPHA
		case 9: return vec3(texelColors[1].a);						// G_CCMUX_TEXEL1_ALPHA
		case 10: return vec3(combinerInputPrimitive.a);				// G_CCMUX_PRIMITIVE_ALPHA
		case 11: return vec3(gl_Color.a);							// G_CCMUX_SHADE_ALPHA
		case 12: return vec3(combinerInputEnvironment.a);			// G_CCMUX_ENV_ALPHA
		case 13: return vec3(combinerInputPrimitiveLODFrac);		// G_CCMUX_LOD_FRACTION
		case 14: return vec3(0.5);									// G_CCMUX_PRIM_LOD_FRAC -- unclear, unemulated
		case 15: return vec3(0.5);									// G_CCMUX_K5 -- YUV-to-RGB constant K5, unemulated
		default: return vec3(0.0);									// G_CCMUX_0
    }
}

vec3 combinerGetColorD(int param)
{
	switch (param)
	{
		case 0: return combinerCombined.rgb;						// G_CCMUX_COMBINED
		case 1: return texelColors[0].rgb;							// G_CCMUX_TEXEL0
		case 2: return texelColors[1].rgb;							// G_CCMUX_TEXEL1
		case 3: return combinerInputPrimitive.rgb;					// G_CCMUX_PRIMITIVE
		case 4: return gl_Color.rgb;								// G_CCMUX_SHADE
		case 5: return combinerInputEnvironment.rgb;				// G_CCMUX_ENVIRONMENT
		case 6: return vec3(1.0);									// G_CCMUX_1
		default: return vec3(0.0);									// G_CCMUX_0
    }
}

float combinerGetAlphaABD(int param)
{
	switch (param)
	{
		case 0: return combinerCombined.a;							// G_ACMUX_COMBINED
		case 1: return texelColors[0].a;							// G_ACMUX_TEXEL0
		case 2: return texelColors[1].a;							// G_ACMUX_TEXEL1
		case 3: return combinerInputPrimitive.a;					// G_ACMUX_PRIMITIVE
		case 4: return gl_Color.a;									// G_ACMUX_SHADE
		case 5: return combinerInputEnvironment.a;					// G_ACMUX_ENVIRONMENT
		case 6: return 1.0;											// G_ACMUX_1
		default: return 0.0;										// G_ACMUX_0
    }
}

float combinerGetAlphaC(int param)
{
	switch (param)
	{
		case 0: return combinerInputPrimitiveLODFrac;				// G_ACMUX_LOD_FRACTION
		case 1: return texelColors[0].a;							// G_ACMUX_TEXEL0
		case 2: return texelColors[1].a;							// G_ACMUX_TEXEL1
		case 3: return combinerInputPrimitive.a;					// G_ACMUX_PRIMITIVE
		case 4: return gl_Color.a;									// G_ACMUX_SHADE
		case 5: return combinerInputEnvironment.a;					// G_ACMUX_ENVIRONMENT
		case 6: return 0.5;											// G_ACMUX_PRIM_LOD_FRAC -- unclear, unemulated
		default: return 0.0;										// G_ACMUX_0
    }
}

vec4 applyLighting(vec4 input)
{
	vec3 lightDirection = normalize(gl_LightSource[0].position.xyz - in_position);
	float diff = max(dot(in_normal, lightDirection), 0.0);
	vec3 diffuse = diff * gl_FrontLightProduct[1].diffuse.rgb;

	vec3 result = (gl_FrontLightProduct[0].ambient.rgb + diffuse) * input.rgb;
	return vec4(result, input.a);
}
