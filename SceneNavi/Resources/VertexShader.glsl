#version 120

varying vec3 in_position;
varying vec3 in_normal;

void main(void)
{
	gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
	gl_TexCoord[0] = gl_MultiTexCoord0;
	gl_TexCoord[1] = gl_MultiTexCoord1;
	gl_FrontColor = gl_Color;

	in_position = gl_Vertex.xyz;
	in_normal = gl_Normal;
}
