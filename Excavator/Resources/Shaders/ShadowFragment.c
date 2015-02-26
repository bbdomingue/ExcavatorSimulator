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
		if ((distanceFromLight < shadowCoordinateWdivide.z))
			gl_FragColor = clamp((ambColor + (difColor * 0.15)), 0.0, 1.0);
		else
			gl_FragColor = clamp(ambColor + difColor * clamp(0.15 + shadow_intensity * 0.85, 0.0, 1.0), 0.0, 1.0);
	}
	else
		gl_FragColor = clamp(ambColor + difColor * clamp(0.15 + shadow_intensity * 0.85, 0.0, 1.0), 0.0, 1.0);
}
