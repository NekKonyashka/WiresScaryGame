using System.Windows.Shapes;

namespace Drag
{
    public class ObjectInfoEventArgs : EventArgs
    {
        public Rectangle Object { get; set; }

        public ObjectInfoEventArgs(Rectangle Object)
        {
            this.Object = Object;
        }
    }
}