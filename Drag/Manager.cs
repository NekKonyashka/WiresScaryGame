using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Drag
{
    public class Manager
    {
        private Random random;
        private MainWindow _window;
        private Grid _grid;
        private List<Wire> _wires;
        private Dictionary<Brush, ConnecterPairs> _connecterPairs;
        private Wire _currentWire;
        private static int _connectedWires = 0;
        private SoundPlayer _soundPlayer;
        private SoundPlayer _winPlayer;

        public event EventHandler AllConnected;
        public event EventHandler OnConnecting;
        public event EventHandler OnFail;

        private Dictionary<int, Brush> _brushes = new()
        {
            [0] = Brushes.Yellow,
            [1] = Brushes.Blue,
            [2] = Brushes.Green,
            [3] = Brushes.Red,
        };

        public Manager(MainWindow mainWindow)
        {
            _soundPlayer = new SoundPlayer("./res/zvuk-jelektroshoker.wav");
            _winPlayer = new SoundPlayer("./res/with-applause.wav");

            _connecterPairs = new Dictionary<Brush, ConnecterPairs>();

            _window = mainWindow;
            _grid = mainWindow.Window;
            _window.MouseUp += _window_MouseUp;

            _wires = new List<Wire>();
            random = new Random();
        }

        private void _window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if(_currentWire != null)
            {
                var begin = _currentWire.Begin.Object;
                var end = _currentWire.End.Object;

                Point end_pos = end.TranslatePoint(new Point(), _window);
                Point begin_pos = begin.TranslatePoint(new Point(), _window);
                Point mouse_pos = e.GetPosition(_window);
                _currentWire.InMove = false;
                if (mouse_pos.X - end_pos.X > end.Width / 8 &&
                    mouse_pos.X - end_pos.X < end.Width / 2 + end.Width / 4 &&
                    mouse_pos.Y - end_pos.Y > end.Height / 8 &&
                    mouse_pos.Y - end_pos.Y < end.Height / 2 + end.Width / 4)
                {
                    _soundPlayer.SoundLocation = "./res/zvuk-jelektroshoker.wav";
                    _soundPlayer.Play();

                    double length = end_pos.X - begin_pos.X;
                    double height = end_pos.Y - begin_pos.Y;
                    _currentWire.Rotate(Math.Atan2(height, length) * 180 / Math.PI);
                    _connectedWires++;
                    _currentWire.IsConnected = true;
                    OnConnecting?.Invoke(this, null);

                    Canvas.SetZIndex(_currentWire.Object, _connectedWires);
                    _window.UpdateLayout();

                    if (IsAllConnected())
                    {
                        _winPlayer.Play();
                        AllConnected?.Invoke(this, null);
                    }
                }
                else
                {
                    OnFail?.Invoke(this, null);
                    ShakeShakeMilkShakeкокакола67();

                    _soundPlayer.SoundLocation = "./res/zvuk_-_muzhskoj_krik.wav";
                    _soundPlayer.Play();

                    _currentWire.Reset();
                }
                _currentWire = null;
            }
        }

        private async void ShakeShakeMilkShakeкокакола67()
        {
            Brush color;
            for (int i = 0; i < 50; i++)
            {
                int x_shift = random.Next(-5, 5);
                int y_shift = random.Next(-5, 5);

                color = x_shift > 0 ? color = Brushes.Black : new ImageBrush(new BitmapImage(new Uri("./res/screamer.jpg",UriKind.Relative)));

                _window.Background = color;
                _window.Left += x_shift;
                _window.Top += y_shift;

                await Task.Delay(1);
            }
            _window.Background = Brushes.White;
        }

        public void Reset()
        {
            _wires.Clear();
            _connecterPairs.Clear();
            _connectedWires = 0;
            Initialize();
        }

        private bool IsAllConnected()
        {
            return _connectedWires == 4;
        }

        public void Initialize()
        {
            InitilizeBegins();
            InitializeEnds();
            InitializeWires();
        }

        private void InitilizeBegins()
        {
            var shuffled = _brushes.OrderBy(x => random.Next()).ToList();
            for (int i = 0; i < _brushes.Count; i++)
            {
                Connecter begin = new Connecter(new VectorPoint(0, i), shuffled[i].Value);
                begin.Object.MouseDown += Object_MouseDown;
                _grid.Children.Add(begin.Object);

                _connecterPairs[shuffled[i].Value] = new ConnecterPairs() { Begin = begin };
            }
        }

        private void InitializeEnds()
        {
            var shuffled = _brushes.OrderBy(x => random.Next()).ToList();
            for (int i = 0; i < _brushes.Count; i++)
            {
                Connecter end = new Connecter(new VectorPoint(_grid.ColumnDefinitions.Count - 1, i), shuffled[i].Value);
                _grid.Children.Add(end.Object);

                _connecterPairs[shuffled[i].Value].End = end;
            }
        }

        private void InitializeWires()
        {
            for(int i = 0; i < _brushes.Count; i++)
            {
                var begin = _connecterPairs[_brushes[i]].Begin;
                var end = _connecterPairs[_brushes[i]].End;

                Wire wire = new Wire(begin, end,new VectorPoint(0,begin.Position.Y));
                _wires.Add(wire);

                Grid.SetColumnSpan(wire.Object, _grid.ColumnDefinitions.Count);
                Grid.SetRowSpan(wire.Object, _grid.RowDefinitions.Count);

                var begin_pos = begin.Object.TranslatePoint(new Point(), _window);
                wire.SetMargin(begin_pos.X, begin_pos.Y);
                
                _grid.Children.Add(wire.Object);
            }
        }
        private void Object_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Rectangle current = sender as Rectangle;
            _currentWire = GetWire(current);
            if (_currentWire.IsConnected == true)
            {
                _currentWire = null;
                return;
            }
            _currentWire.InMove = true;
            Drag(_currentWire);
        }
        public Wire GetWire(Rectangle rectangle)
        {
            foreach(var wire in _wires)
            {
                if(wire.Object.Fill == rectangle.Fill)
                {
                    return wire;
                }
            }
            return null;
        }
        private async void Drag(Wire wire)
        {
            if (!wire.InMove) return;
            Canvas.SetZIndex(wire.Object, _connectedWires);
            double length = 0;
            double height = 0;
            double distance = 0;
            double rows = wire.Position.Y * _grid.RowDefinitions[0].ActualHeight;
            Point wire_pos = wire.Object.TranslatePoint(new Point(), _window);
            Point mouse_pos;
            while (wire.InMove)
            {
                mouse_pos = Mouse.GetPosition(_window);

                length = mouse_pos.X - wire.Object.Margin.Left;
                height = mouse_pos.Y - rows - wire.Object.Margin.Top - 25;

                distance = Math.Sqrt(Math.Pow(Math.Abs(length), 2) + Math.Pow(Math.Abs(height), 2));

                wire.Rotate(Math.Atan2(height, length) * 180 / Math.PI);
                wire.Object.Width = Math.Max(distance + 25, 1);
                await Task.Delay(16);
            }
            if (!wire.IsConnected)
            {
                Canvas.SetZIndex(wire.Object, -1);
            }
        }
    }
}