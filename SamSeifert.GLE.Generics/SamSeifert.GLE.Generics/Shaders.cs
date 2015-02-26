using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SamSeifert.GLE
{
    public class Shaders
    {
        public static bool CreateShaders(
            String vertexShader, 
            String fragmentShader,
            out int program,
            out int vshader,
            out int fshader)
        {
            int vertShader = createVertShader(vertexShader);
            int fragShader = createFragShader(fragmentShader);

            if (vertShader != 0 && fragShader != 0)
            {
                int shader = GL.CreateProgram();

                GL.AttachShader(shader, vertShader);
                GL.AttachShader(shader, fragShader);

                int param;

                GL.LinkProgram(shader);
                GL.GetProgram(shader, ProgramParameter.LinkStatus, out param);
                if (param == 0)
                {
                    Console.WriteLine("Linkage error");
                    program = 0; vshader = 0; fshader = 0;
                    GL.DeleteShader(vertShader);
                    GL.DeleteShader(fragShader);
                    return false;
                }

                GL.ValidateProgram(shader);
                GL.GetProgram(shader, ProgramParameter.ValidateStatus, out param);
                if (param == 0)
                {
                    Console.WriteLine("Validate error");
                    program = 0; vshader = 0; fshader = 0;
                    GL.DeleteShader(vertShader);
                    GL.DeleteShader(fragShader);
                    return false;
                }

                vshader = vertShader;
                fshader = fragShader;
                program = shader;

                return true;
            }
            else
            {
                program = 0; vshader = 0; fshader = 0;
                if (vertShader != 0) GL.DeleteShader(vertShader);
                if (fragShader != 0) GL.DeleteShader(fragShader);
                return false;
            }
        }

        private static int createVertShader(String vertexCode)
        {
            int vertShader = GL.CreateShader(ShaderType.VertexShader);

            if (vertShader == 0) return 0;

            GL.ShaderSource(vertShader, vertexCode);
            GL.CompileShader(vertShader);

            int param;
            GL.GetShader(vertShader, ShaderParameter.CompileStatus, out param);
            if (param == 0)
            {
                Console.WriteLine("Vert Shader Compile Error");
                return 0;
            }
            return vertShader;
        }

        private static int createFragShader(String fragCode)
        {
            int fragShader = GL.CreateShader(ShaderType.FragmentShader);
            if (fragShader == 0) return 0;

            GL.ShaderSource(fragShader, fragCode);
            GL.CompileShader(fragShader);

            int param;
            GL.GetShader(fragShader, ShaderParameter.CompileStatus, out param);

            if (param == 0)
            {
                Console.WriteLine("Frag Shader Compile Error");
                return 0;
            }
            return fragShader;
        }
    }
}
