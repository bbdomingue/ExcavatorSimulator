using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

using SamSeifert.ComplexFileParser;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SamSeifert.GLE
{
    public class SkyBox
    {
        private String _StringPath;
        private String _StringExt = ".jpg";
        private bool _BoolFull = true;
        private int _Index;
        private float[] _LightPos = new float[] { 0, 0, 0, 1.0f };
        private float[] _LightDir = new float[] { 0, 0, 0, 1.0f };
        private int[] _Textures = new int[6];
        private bool _Setup = false;

        public SkyBox(String file, int dex)
        {
            this._Index = dex;
            this._StringPath = Directory.GetParent(file).FullName;

            String kPos = "Pos";
            String kDir = "Dir";
            String kExt = "Ext";
            String kX = "X";
            String kY = "Y";
            String kZ = "Z";
            String kType = "Type";
            String kHalf = "Half";

            String contents = File.ReadAllText(file);
            TagFile f = TagFile.parseText(contents);

            String output = null;
            Double outputD = 0.0;

            if (f != null)
            {
                foreach (var search in f.getMatches(ref kPos))
                {
                    if (search._Params.TryGetValue(kX, out output))
                        if (Double.TryParse(output, out outputD))
                            this._LightPos[0] = (float) outputD;
                    if (search._Params.TryGetValue(kY, out output))
                        if (Double.TryParse(output, out outputD))
                            this._LightPos[1] = (float)outputD;
                    if (search._Params.TryGetValue(kZ, out output))
                        if (Double.TryParse(output, out outputD))
                            this._LightPos[2] = (float)outputD;
                }

                foreach (var search in f.getMatches(ref kDir))
                {
                    if (search._Params.TryGetValue(kX, out output))
                        if (Double.TryParse(output, out outputD))
                            this._LightDir[0] = (float)outputD;
                    if (search._Params.TryGetValue(kY, out output))
                        if (Double.TryParse(output, out outputD))
                            this._LightDir[1] = (float)outputD;
                    if (search._Params.TryGetValue(kZ, out output))
                        if (Double.TryParse(output, out outputD))
                            this._LightDir[2] = (float)outputD;
                }

                foreach (var search in f.getMatches(ref kExt)) search._Params.TryGetValue(kType, out this._StringExt);
                foreach (var search in f.getMatches(ref kHalf)) this._BoolFull = false;
            }
        }




        private void getImages()
        {
            for (int i = 0; i < this._Textures.Length; i++)
            {
                String n;                
                switch (i)
                {
                    case 0: n = "Top"; break;
                    case 1: n = "Bottom"; break;
                    case 2: n = "Right"; break;
                    case 3: n = "Left"; break;
                    case 4: n = "Front"; break;
                    case 5: n = "Back"; break;
                    default: n = "BLANK"; break;
                }

                var pt = Path.Combine(this._StringPath, n + this._StringExt);
                if (File.Exists(pt))
                {
                    var im = Image.FromFile(pt);
                    this._Textures[i] = SkyBox.getGLTexture(im);
                }
                else this._Textures[i] = 0;
            }
        }

        public override string ToString()
        {
            return "Sky Box " + this._Index;
        }













        public void GLDelete()
        {
            if (this._Setup)
            {
                for (int i = 0; i < this._Textures.Length; i++)
                {
                    if (this._Textures[i] != 0) GL.DeleteTexture(this._Textures[i]);
                    this._Textures[i] = 0;
                }
                this._Setup = false;
                this.resetDistance();
            }
        }

        public void resetDistance()
        {
            if (this._List != 0)
            {
                GL.DeleteLists(this._List, 1);
                this._List = 0;
            }
        }

        private int _List = 0;
        public void Draw(float distance)
        {
            if (!this._Setup)
            {
                this._Setup = true;
                this.getImages();
            }
            else if (this._List == 0)
            {
                this._List = GL.GenLists(1);

                GL.NewList(this._List, ListMode.Compile);
                {
                    int tex = 0;
                    int scale = this._BoolFull ? 2 : 1;

                    GL.Color3(Color.White);
                    GL.Enable(EnableCap.Texture2D);
                    GL.Enable(EnableCap.TextureCubeMapSeamless);

                    if (this._BoolFull) GL.Translate(0, -distance, 0);

                    // Top
                    tex = this._Textures[0];
                    if (tex != 0)
                    {
                        GL.BindTexture(TextureTarget.Texture2D, tex);
                        GL.Begin(BeginMode.Quads);
                        {
                            GL.TexCoord2(0, 0);
                            GL.Vertex3(-distance, distance * scale, -distance);
                            GL.TexCoord2(1, 0);
                            GL.Vertex3(distance, distance * scale, -distance);
                            GL.TexCoord2(1, 1);
                            GL.Vertex3(distance, distance * scale, distance);
                            GL.TexCoord2(0, 1);
                            GL.Vertex3(-distance, distance * scale, distance);
                        }
                        GL.End();
                    }

                    // Bottom
                    tex = this._Textures[1];
                    if (tex != 0)
                    {
                        GL.BindTexture(TextureTarget.Texture2D, tex);
                        GL.Begin(BeginMode.Quads);
                        {
                            GL.TexCoord2(0, 0); GL.Vertex3(-distance, 0, distance);
                            GL.TexCoord2(1, 0); GL.Vertex3(distance, 0, distance);
                            GL.TexCoord2(1, 1); GL.Vertex3(distance, 0, -distance);
                            GL.TexCoord2(0, 1); GL.Vertex3(-distance, 0, -distance);
                        }
                        GL.End();
                    }

                    // Right
                    tex = this._Textures[2];
                    if (tex != 0)
                    {
                        GL.BindTexture(TextureTarget.Texture2D, tex);
                        GL.Begin(BeginMode.Quads);
                        {
                            GL.TexCoord2(0, 0);
                            GL.Vertex3(distance, 0, -distance);
                            GL.TexCoord2(1, 0);
                            GL.Vertex3(distance, 0, distance);
                            GL.TexCoord2(1, 1);
                            GL.Vertex3(distance, distance * scale, distance);
                            GL.TexCoord2(0, 1);
                            GL.Vertex3(distance, distance * scale, -distance);
                        }
                        GL.End();
                    }

                    // Left
                    tex = this._Textures[3];
                    if (tex != 0)
                    {
                        GL.BindTexture(TextureTarget.Texture2D, tex);
                        GL.Begin(BeginMode.Quads);
                        {
                            GL.TexCoord2(0, 0);
                            GL.Vertex3(-distance, 0, distance);
                            GL.TexCoord2(1, 0);
                            GL.Vertex3(-distance, 0, -distance);
                            GL.TexCoord2(1, 1);
                            GL.Vertex3(-distance, distance * scale, -distance);
                            GL.TexCoord2(0, 1);
                            GL.Vertex3(-distance, distance * scale, distance);
                        }
                        GL.End();
                    }

                    // Front
                    tex = this._Textures[4];
                    if (tex != 0)
                    {
                        GL.BindTexture(TextureTarget.Texture2D, tex);
                        GL.Begin(BeginMode.Quads);
                        {
                            GL.TexCoord2(0, 0);
                            GL.Vertex3(-distance, 0, -distance);
                            GL.TexCoord2(1, 0);
                            GL.Vertex3(distance, 0, -distance);
                            GL.TexCoord2(1, 1);
                            GL.Vertex3(distance, distance * scale, -distance);
                            GL.TexCoord2(0, 1);
                            GL.Vertex3(-distance, distance * scale, -distance);
                        }
                        GL.End();
                    }

                    // Back
                    tex = this._Textures[5];
                    if (tex != 0)
                    {
                        GL.BindTexture(TextureTarget.Texture2D, tex);
                        GL.Begin(BeginMode.Quads);
                        {
                            GL.TexCoord2(0, 0);
                            GL.Vertex3(distance, 0, distance);
                            GL.TexCoord2(1, 0);
                            GL.Vertex3(-distance, 0, distance);
                            GL.TexCoord2(1, 1);
                            GL.Vertex3(-distance, distance * scale, distance);
                            GL.TexCoord2(0, 1);
                            GL.Vertex3(distance, distance * scale, distance);
                        }
                        GL.End();
                    }

                    if (this._BoolFull) GL.Translate(0, distance, 0);

                    GL.Disable(EnableCap.TextureCubeMapSeamless);
                    GL.BindTexture(TextureTarget.Texture2D, 0);
                    GL.Disable(EnableCap.Texture2D);
                }
                GL.EndList();
            }
            else GL.CallList(this._List);
        }















        private static int getGLTexture(Image im)
        {
            im.RotateFlip(RotateFlipType.RotateNoneFlipY);

            int w = im.Width, h = im.Height;

            Bitmap TextureBitmap = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(TextureBitmap)) g.DrawImage(im, 0, 0, w, h);
            im.Dispose();

            System.Drawing.Imaging.BitmapData TextureData = TextureBitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, w, h),
                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                TextureBitmap.PixelFormat);

            int output;

            GL.GenTextures(1, out output);
            GL.BindTexture(TextureTarget.Texture2D, output);

            // Get Rid of Texture Edges on Sky Dome
            int param;

            param = (int)TextureWrapMode.ClampToEdge;
            GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, ref param);
            GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, ref param);

            param = (int)All.Nearest;
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, param);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, param);

            //the following code sets certian parameters for the texture
            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (float)TextureEnvMode.Modulate);

            // tell OpenGL to build mipmaps out of the bitmap data
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.GenerateMipmap, (float)1.0f);

            // load the texture
            GL.TexImage2D(
                TextureTarget.Texture2D,
                0, // level
                PixelInternalFormat.Three,
                TextureBitmap.Width, TextureBitmap.Height,
                0, // border
                OpenTK.Graphics.OpenGL.PixelFormat.Bgr,
                PixelType.UnsignedByte,
                TextureData.Scan0
                );

            //free the bitmap data (we dont need it anymore because it has been passed to the OpenGL driver
            TextureBitmap.UnlockBits(TextureData);
            TextureBitmap.Dispose();

            return output;
        }
    }
}
