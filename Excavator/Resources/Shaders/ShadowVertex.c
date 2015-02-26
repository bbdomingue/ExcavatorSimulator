// Used for shadow lookup
varying vec4 vertex_shadow;
varying vec3 vertex_light_position;
varying vec3 vertex_normal;


varying vec4 ambColor;
varying vec4 difColor;
varying float shadow_intensity;

void main()
{
	ambColor = gl_FrontMaterial.ambient * (gl_LightSource[0].ambient +  gl_LightModel.ambient);
    difColor = gl_FrontMaterial.diffuse * gl_LightSource[0].diffuse;

	gl_Position = gl_ProjectionMatrix * gl_ModelViewMatrix * gl_Vertex;
	vertex_shadow = gl_TextureMatrix[7] * gl_Vertex;  
	vertex_normal = normalize(gl_NormalMatrix * gl_Normal);

	if (0.0 == gl_LightSource[0].position.w) 
		vertex_light_position = normalize(vec3(gl_LightSource[0].position));
	else
		vertex_light_position = normalize(vec3(gl_LightSource[0].position - gl_ModelViewMatrix * gl_Vertex));

	shadow_intensity = max(dot(vertex_normal, vertex_light_position), 0.0);
}
