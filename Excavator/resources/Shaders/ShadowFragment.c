varying vec4 ShadowCoord;
uniform sampler2D ShadowMap; // Shadow

varying vec2 texCoords;
uniform sampler2D tex0; // Grass

varying vec4 ambColor;
varying vec4 difColor;

void main()
{		
 	float shadow = 1.0;

	vec4 shadowCoordinateWdivide = ShadowCoord / ShadowCoord.w ;

	bool b1 = shadowCoordinateWdivide.x > 0 && shadowCoordinateWdivide.x < 1;
	bool b2 = shadowCoordinateWdivide.y > 0 && shadowCoordinateWdivide.y < 1;

	if (b1 && b2)
	{
		shadowCoordinateWdivide.z -= 0.0001;

		float distanceFromLight = texture(ShadowMap, shadowCoordinateWdivide.st).z;
		if ((distanceFromLight < shadowCoordinateWdivide.z)) shadow = 0.5;

/*		gl_FragColor = clamp(ambColor + shadow * difColor, 0.0, 1.0);
	}
	else
	{
		gl_FragColor.r = b1 ? 1 : 0;
		gl_FragColor.g = b2 ? 1 : 0;
		gl_FragColor.b = 0;*/
	}

    vec4 tex = texture2D(tex0, texCoords);

	float add = tex.r + tex.b + tex.g;

	gl_FragColor = clamp(
		(add == 0) ? (ambColor + shadow * difColor) : (shadow * tex), 
		0.0, 
		1.0); 		
}
