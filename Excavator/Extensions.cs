using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Excavator
{
    static class Extensions
    {
        public static PhysX.Math.Vector3 xyz(this PhysX.Math.Vector4 str)
        {
            return new PhysX.Math.Vector3(str.X, str.Y, str.Z);
        }

        public static PhysX.Math.Vector3 toPhysX(this OpenTK.Vector3 str)
        {
            return new PhysX.Math.Vector3(str.X, str.Y, str.Z);
        }
    }
}
