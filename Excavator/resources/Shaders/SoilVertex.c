// Used for shadow lookup
varying vec4 ShadowCoord;
varying vec2 texCoords;

varying vec3 vertex_light_position;
varying vec3 vertex_light_half_vector;
varying vec3 vertex_normal;

varying vec4 ambColor;
varying vec4 difColor;

void main()
{
	gl_Position = ftransform();

	texCoords = vec2(gl_Vertex[0] / 40, gl_Vertex[2] / 40);

	ShadowCoord = gl_TextureMatrix[7] * gl_Vertex;  

    vertex_light_half_vector = normalize(gl_LightSource[0].halfVector.xyz);
    vertex_normal = normalize(gl_NormalMatrix * gl_Normal);
    vertex_light_position = normalize(gl_LightSource[0].position.xyz);

	ambColor = gl_FrontMaterial.ambient * (gl_LightSource[0].ambient +  gl_LightModel.ambient);

    difColor = gl_FrontMaterial.diffuse * gl_LightSource[0].diffuse;
    difColor *= max(dot(vertex_normal, vertex_light_position), 0.0);

//    vec4 specular_color = gl_FrontMaterial.specular * gl_LightSource[0].specular * pow(max(dot(vertex_normal, vertex_light_half_vector), 0.0) , gl_FrontMaterial.shininess);
}
