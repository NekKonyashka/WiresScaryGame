using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Drag
{
    public class Wire : GameObject
    {
        public bool InMove = false;
        public bool IsConnected = false;
        private RotateTransform rotateTransform;
        private Connecter _begin;
        private Connecter _end;

        public Connecter Begin => _begin;
        public Connecter End => _end;
        public Wire(Connecter begin,Connecter end,VectorPoint point) : base(point)
        {
            Canvas.SetZIndex(Object, -1);
            Object.Width = 50;
            Object.Height = 50;
            Object.Fill = begin.Object.Fill;
            Object.RenderTransformOrigin = new Point(0, 0.5);
            Object.VerticalAlignment = VerticalAlignment.Top;
            Object.HorizontalAlignment = HorizontalAlignment.Left;
            rotateTransform = new RotateTransform();
            Object.RenderTransform = rotateTransform;

            _begin = begin;
            _end = end;
        }

        public void Rotate(double angle)
        {
            rotateTransform.Angle = angle;
            Debug.WriteLine(rotateTransform.Angle);
        }

        public void SetMargin(double x, double y)
        {
            Object.Margin = new Thickness(x + Begin.Object.Width / 2, y + Begin.Object.Height / 4, 0, 0);
        }

        public void Reset()
        {
            Object.Width = 50;
            Object.Height = 50;
            rotateTransform.Angle = 0;
        }
    }
}
