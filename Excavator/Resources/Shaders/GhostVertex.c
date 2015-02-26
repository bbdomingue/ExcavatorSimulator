varying vec2 texCoords;
void main()
{
	gl_Position = ftransform();
	texCoords = gl_MultiTexCoord0.st;
}
