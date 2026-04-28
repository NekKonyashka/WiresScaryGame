using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Drag
{
    public class Manager
    {
        private int _connectedWires = 0;
        private int amountOfУспех = 0;

        private Random random;
        private List<Wire> _wires;
        private Wire _currentWire;

        public int Amount => amountOfУспех;

        public event EventHandler AllConnected;
        public event EventHandler<OnConnectedEventArgs> OnConnecting;
        public event EventHandler OnFail;
        public event EventHandler<ObjectInfoEventArgs> OnGameObjectCreated;
        public event Func<Task> AnimationRequested;

        private Dictionary<Brush, ConnecterPairs> _connecterPairs;
        private Dictionary<string, SoundPlayer> _sounds;
        private Dictionary<int, Brush> _brushes;

        public Manager(MainWindow mainWindow)
        {
            _connecterPairs = new Dictionary<Brush, ConnecterPairs>();

            _wires = new List<Wire>();
            random = new Random();
            Дальтоник();
            LoadSounds();
        }

        private void LoadSounds()
        {
            _sounds = new()
            {
                ["fail"] = new SoundPlayer("./res/Sounds/zvuk_-_muzhskoj_krik.wav"),
                ["connect"] = new SoundPlayer("./res/Sounds/zvuk-jelektroshoker.wav"),
                ["win"] = new SoundPlayer("./res/Sounds/with-applause.wav"),
                ["upgrade"] = new SoundPlayer("./res/Sounds/0984c1d48eb11ef.wav"),
                ["took"] = new SoundPlayer("./res/Sounds/11986c2f439eb45.wav")
            };

            foreach(var sound in _sounds.Values)
            {
                sound.LoadAsync();
            }
        }

        private void Дальтоник()
        {
            _brushes = new();
            for(int i = 0;i < 25; i++)
            {
                _brushes[i] = (Brush)new BrushConverter().ConvertFromString($"#{random.Next(0xFFFFFF + 1):X6}");
            }
        }

        public void ConnectionCheck(object sender)
        {
            if(_currentWire != null)
            {
                MainWindow _window = sender as MainWindow;
                var begin = _currentWire.Begin.Object;
                var end = _currentWire.End.Object;

                Point end_pos = end.TranslatePoint(new Point(), _window);
                Point begin_pos = begin.TranslatePoint(new Point(), _window);
                Point mouse_pos = Mouse.GetPosition(_window);
                _currentWire.InMove = false;
                if (mouse_pos.X - end_pos.X > end.Width / 8 &&
                    mouse_pos.X - end_pos.X < end.Width / 2 + end.Width / 4 &&
                    mouse_pos.Y - end_pos.Y > end.Height / 8 &&
                    mouse_pos.Y - end_pos.Y < end.Height / 2 + end.Width / 4)
                {
                    PlaySound("connect");

                    double length = end_pos.X - begin_pos.X;
                    double height = end_pos.Y - begin_pos.Y;
                    double distance = Math.Sqrt(Math.Pow(Math.Abs(length), 2) + Math.Pow(Math.Abs(height), 2));

                    _currentWire.ScaleX(distance / _currentWire.Object.Width);
                    _currentWire.Rotate(Math.Atan2(height, length) * 180 / Math.PI);
                    _connectedWires++;
                    _currentWire.IsConnected = true;

                    OnConnecting?.Invoke(this, new OnConnectedEventArgs(_currentWire.End.Position));

                    Canvas.SetZIndex(_currentWire.Object, _connectedWires);

                    if (IsAllConnected(_window.Window.RowDefinitions.Count))
                    {
                        amountOfУспех++;
                        Win();
                        AllConnected?.Invoke(this, null);
                    }
                }
                else
                {
                    PlaySound("fail");

                    OnFail?.Invoke(this, null);

                    _currentWire.Reset();
                }
                _currentWire = null;
            }
        }

        public void PlaySound(string key)
        {
            if(_sounds.TryGetValue(key,out var player))
            {
                player.Play();
            }
        }
        private async void Win()
        {
            await AnimationRequested?.Invoke();
            PlaySound("win");
        }


        public void Reset(int columns,int rows)
        {
            _wires.Clear();
            _connecterPairs.Clear();
            _connectedWires = 0;
            Initialize(columns,rows);
        }

        private bool IsAllConnected(int rows)
        {
            return _connectedWires == rows;
        }

        public void Initialize(int columns,int rows)
        {
            InitilizeBegins(rows);
            InitializeEnds(columns,rows);
            InitializeWires(columns,rows);
        }

        private void InitilizeBegins(int rows)
        {
            var shuffled = _brushes.OrderBy(x => random.Next()).ToList();
            for (int i = 0; i < rows; i++)
            {
                Connecter begin = new Connecter(new VectorPoint(0, i), shuffled[i].Value);
                begin.Object.MouseDown += Object_MouseDown;
                OnGameObjectCreated?.Invoke(this, new ObjectInfoEventArgs(begin.Object));

                _connecterPairs[shuffled[i].Value] = new ConnecterPairs() { Begin = begin };
            }
        }

        private void InitializeEnds(int columns,int row)
        {
            var shuffled = _brushes.OrderBy(x => random.Next()).ToList();
            for (int i = 0,count = 0; i < _brushes.Count && count < row; i++)
            {
                if (!_connecterPairs.ContainsKey(shuffled[i].Value))
                {
                    continue;
                }
                Connecter end = new Connecter(new VectorPoint(columns - 1, count), shuffled[i].Value);
                OnGameObjectCreated?.Invoke(this,new ObjectInfoEventArgs(end.Object));

                _connecterPairs[shuffled[i].Value].End = end;
                count++;
            }
        }

        private void InitializeWires(int columns,int row)
        {
            for(int i = 0; i < _brushes.Count; i++)
            {
                if (!_connecterPairs.ContainsKey(_brushes[i]))
                {
                    continue;
                }

                var begin = _connecterPairs[_brushes[i]].Begin;
                var end = _connecterPairs[_brushes[i]].End;

                Wire wire = new Wire(begin, end,new VectorPoint(0,begin.Position.Y));
                _wires.Add(wire);

                Grid.SetColumnSpan(wire.Object, columns);
                Grid.SetRowSpan(wire.Object, row);

                OnGameObjectCreated?.Invoke(this, new ObjectInfoEventArgs(wire.Object));
            }
        }
        private void Object_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            PlaySound("took");

            Rectangle current = sender as Rectangle;
            Grid parent = current.Parent as Grid;

            _currentWire = GetWire(current);
            if (_currentWire.IsConnected == true)
            {
                _currentWire = null;
                return;
            }
            _currentWire.InMove = true;
            Drag(parent, _currentWire);
        }
        private Wire GetWire(Rectangle rectangle)
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
        private async void Drag(Grid _grid,Wire wire)
        {
            if (!wire.InMove)
            {
                return;
            }

            Canvas.SetZIndex(wire.Object, _connectedWires + 1);
            double length = 0;
            double height = 0;
            double distance = 0;
            double rows = wire.Position.Y * _grid.RowDefinitions[0].ActualHeight;
            Point wire_pos = wire.Object.TranslatePoint(new Point(), _grid);
            Point mouse_pos;
            while (wire.InMove)
            {
                mouse_pos = Mouse.GetPosition(_grid);
                length = mouse_pos.X - wire.Object.Margin.Left;
                height = mouse_pos.Y - rows - wire.Object.Margin.Top - wire.Object.Height / 2;

                distance = Math.Sqrt(Math.Pow(Math.Abs(length), 2) + Math.Pow(Math.Abs(height), 2));

                wire.Rotate(Math.Atan2(height, length) * 180 / Math.PI);
                wire.ScaleX(Math.Max(distance + wire.Object.Height / 2, 1) / wire.Object.Width);

                await Task.Delay(16);
            }
            if (!wire.IsConnected)
            {
                Canvas.SetZIndex(wire.Object, -1);
            }
        }
    }
}