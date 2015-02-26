varying vec4 vertex_shadow;
varying vec2 texCoords;

varying vec4 ambColor;
varying vec4 difColor;
varying float shadow_intensity;

uniform sampler2D ShadowMap; // Shadow
uniform sampler2D tex0; // Soil

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
			gl_FragColor = clamp((ambColor + (difColor * 0.3)), 0.0, 1.0);
		else
			gl_FragColor = clamp(ambColor + difColor * clamp(0.3 + shadow_intensity * 0.7, 0.0, 1.0), 0.0, 1.0);
	}
	else 
		gl_FragColor = clamp(ambColor + difColor * clamp(0.3 + shadow_intensity * 0.7, 0.0, 1.0), 0.0, 1.0);

    vec4 tex = texture2D(tex0, texCoords);

	gl_FragColor = gl_FragColor * tex;
}

