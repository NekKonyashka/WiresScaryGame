using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Drag
{
    public class Connecter : GameObject
    {
        public Connecter(VectorPoint point,Brush color) : base(point)
        {
            Object.Width = 100;
            Object.Height = 100;
            Object.Fill = color;
        }
    }
}
