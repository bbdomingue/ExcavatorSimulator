varying vec2 texCoords;
uniform sampler2D tex0;
uniform sampler2D tex1;

void main()
{	
	gl_FragColor = texture2D(tex0, texCoords);
	gl_FragColor.a = 0.4f;
	gl_FragDepth = texture2D(tex1, texCoords);
}
