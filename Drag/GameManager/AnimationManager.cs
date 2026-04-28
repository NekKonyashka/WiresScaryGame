using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Drag
{
    public class AnimationManager
    {
        private int _frameIndex = 0;
        private BitmapImage _image;
        private double _frameWidth;
        private double _frameHeight;
        private DateTime _lastFrame;
        private Image _currentSprite;

        public event EventHandler OnAnimationStart;
        public event EventHandler OnAnimationEnd;

        public Image Image => _currentSprite;

        public AnimationManager()
        {
            _image = new BitmapImage(new Uri("./res/Images/Sprites.png", UriKind.Relative));
            _frameWidth = _image.Width / 2;
            _frameHeight = _image.Height / 2;

            _lastFrame = DateTime.Now;
            _currentSprite = new Image()
            {
                Width = _image.Width / 2,
                Height = _image.Height / 2,
                Stretch = Stretch.Fill,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            Canvas.SetZIndex(_currentSprite, 10);
        }

        public void Start()
        {
            CompositionTarget.Rendering += Animation;
            OnAnimationStart?.Invoke(this,null);
        }

        private void Animation(object? sender,EventArgs e)
        {
            if ((DateTime.Now - _lastFrame).TotalMilliseconds > 100)
            {
                if (_frameIndex == 4)
                {
                    _frameIndex = 0;
                    Stop();
                }

                int x = (int)((_frameIndex % 2 == 0 ? 0 : 1) * _frameWidth);
                int y = (int)((_frameIndex / 2) * _frameHeight);

                CroppedBitmap cropped = new CroppedBitmap(_image,
                    new Int32Rect(x, y, (int)_frameWidth, (int)_frameHeight));

                _currentSprite.Source = cropped;

                _lastFrame = DateTime.Now;
                _frameIndex++;
            }
        }

        private void Stop()
        {
            CompositionTarget.Rendering -= Animation;
            OnAnimationEnd?.Invoke(this, null);
        }
    }
}
