using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamSeifert.GLE
{
    public class HeightMap
    {
        public virtual void GLDelete()
        {
        }

        public virtual void GLDraw()
        {
        }

        public virtual void GLDraw(float dir, int ang)
        {
            this.GLDraw();
        }
    }
}
