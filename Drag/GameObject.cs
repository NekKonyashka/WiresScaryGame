using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Drag
{
    public abstract class GameObject
    {
        private VectorPoint _position;

        public VectorPoint Position => _position;

        private Rectangle _object;
        public Rectangle Object => _object;

        public GameObject(VectorPoint point)
        {
            _position = point;
            _object = new Rectangle() { SnapsToDevicePixels = true };

            Grid.SetColumn(_object,point.X);
            Grid.SetRow(_object,point.Y);
        }
    }
}
