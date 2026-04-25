using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
namespace Drag
{
    public struct VectorPoint
    {
        public int X;
        public int Y;
        public VectorPoint(int X,int Y)
        {
            this.X = X;
            this.Y = Y;
        }
    }
}
