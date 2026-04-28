using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Drag
{
    public class Wire : GameObject
    {
        public bool InMove = false;
        public bool IsConnected = false;
        private ScaleTransform scaleTransform;
        private RotateTransform rotateTransform;
        private Connecter _begin;
        private Connecter _end;

        public Connecter Begin => _begin;
        public Connecter End => _end;
        public Wire(Connecter begin,Connecter end,VectorPoint point) : base(point)
        {
            rotateTransform = new RotateTransform();
            scaleTransform = new ScaleTransform();

            TransformGroup group = new TransformGroup();
            group.Children.Add(scaleTransform);
            group.Children.Add(rotateTransform);

            _begin = begin;
            _end = end;

            Object.Margin = new Thickness(Begin.Object.Width / 2, Begin.Object.Height / 4, 0, 0);
            Object.Width = 50;
            Object.Height = 50;
            Object.Fill = begin.Object.Fill;
            Object.RenderTransformOrigin = new Point(0, 0.5);
            Object.VerticalAlignment = VerticalAlignment.Top;
            Object.HorizontalAlignment = HorizontalAlignment.Left;
            Object.RenderTransform = group;

            Canvas.SetZIndex(Object, -1);
        }

        public void Rotate(double angle)
        {
            rotateTransform.Angle = angle;
        }
        public void ScaleX(double value)
        {
            scaleTransform.ScaleX = value;
        }

        public void Reset()
        {
            Object.Width = 50;
            Object.Height = 50;
            scaleTransform.ScaleX = 1;
            rotateTransform.Angle = 0;
        }
    }
}
