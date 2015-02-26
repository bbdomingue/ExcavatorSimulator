varying vec4 vertex_shadow;
uniform sampler2D ShadowMap; // Shadow

varying vec4 ambColor;
varying vec4 difColor;
varying float shadow_intensity;

void main()
{		
	vec4 shadowCoordinateWdivide = vertex_shadow / vertex_shadow.w ;

	bool b1 = shadowCoordinateWdivide.x > 0 && shadowCoordinateWdivide.x < 1;
	bool b2 = shadowCoordinateWdivide.y > 0 && shadowCoordinateWdivide.y < 1;

	if (b1 && b2)
	{
		shadowCoordinateWdivide.z -= 0.0001;

		float distanceFromLight = texture(ShadowMap, shadowCoordinateWdivide.st).z;
		if ((distanceFromLight < shadowCoordinateWdivide.z)) shadow_intensity = min(0.3, shadow_intensity);

/*		gl_FragColor = clamp(ambColor + shadow * difColor, 0.0, 1.0);
	}
	else
	{
		gl_FragColor.r = b1 ? 1 : 0;
		gl_FragColor.g = b2 ? 1 : 0;
		gl_FragColor.b = 0;*/
	}

	gl_FragColor = clamp(
		(ambColor + shadow_intensity * difColor), 
		0.0, 
		1.0); 		
}
