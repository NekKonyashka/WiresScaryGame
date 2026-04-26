using System.Diagnostics;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Drag
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _frameIndex = 0;
        private TaskCompletionSource<bool> _task;
        private BitmapImage _bitmap;
        private DateTime _lastFrame;
        private List<Rectangle> _items;
        private Image _currentSprite;
        public Manager Manager;
        private List<string> tittles = new List<string>()
        {
            "Красава!","У тебя золотые руки!","Вай, красота","Просто мастер!","Смотри не ударься током","Балдеж!","67","Ты ювелир!","Ты просто босс, просто начальник"
        };
        public MainWindow()
        {
            InitializeComponent();
            _items = new List<Rectangle>();
            _bitmap = new BitmapImage(new Uri("./res/Sprites.png", UriKind.Relative));
            ResizeMode = ResizeMode.NoResize;
            Manager = new Manager(this);
            Manager.AllConnected += Manager_AllConnected;
            Manager.OnConnecting += Manager_OnConnecting;
            Manager.OnFail += Manager_OnFail;
            Loaded += MainWindow_Loaded;
        }

        public void AddItem(Rectangle item)
        {
            _items.Add(item);
            Window.Children.Add(item);
        }

        private void Manager_OnFail(object? sender, EventArgs e)
        {
            Tittle.Foreground = Brushes.Red;
            Tittle.Text = "ОСТОРОЖНО!!!";
        }

        private void Manager_OnConnecting(object? sender, OnConnectedEventArgs e)
        {
            Tittle.Foreground = Brushes.White;
            Tittle.Text = tittles[new Random().Next(tittles.Count)];
            Image sprite = new Image()
            {
                Width = _bitmap.Width / 2,
                Height = _bitmap.Height / 2,
                Stretch = Stretch.Fill,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            _currentSprite = sprite;
            Grid.SetColumn(sprite, e.End_pos.X);
            Grid.SetRow(sprite, e.End_pos.Y);
            Canvas.SetZIndex(sprite, 10);
            Window.Children.Add(sprite);
            _lastFrame = DateTime.Now;

            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void CompositionTarget_Rendering(object? sender, EventArgs e)
        {
            int x = (int)((_frameIndex % 2 == 0 ? 0 : 1) * (_bitmap.Width / 2));
            int y = (int)((_frameIndex / 2) * (_bitmap.Height / 2));
            if ((DateTime.Now - _lastFrame).TotalMilliseconds > 100)
            {
                CroppedBitmap cropped = new CroppedBitmap(_bitmap,
                    new Int32Rect(x, y, (int)(_bitmap.Width / 2), (int)(_bitmap.Height / 2)));

                _currentSprite.Source = cropped;

                _lastFrame = DateTime.Now;
                _frameIndex++;
            }
            if(_frameIndex == 4)
            {
                _frameIndex = 0;
                _task?.SetResult(true);

                Window.Children.Remove(_currentSprite);
                CompositionTarget.Rendering -= CompositionTarget_Rendering;
            }
        }
        public Task StartAnim()
        {
            _task = new TaskCompletionSource<bool>();
            return _task.Task;
        }
        private async void Manager_AllConnected(object? sender, EventArgs e)
        {
            await _task.Task;
            Restart.Visibility = Visibility.Visible;
            RestartBAck.Visibility = Visibility.Visible;
            Tittle.Text = "";
        }

        private void AddRow()
        {
            Window.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1,GridUnitType.Star)});
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Manager.Initialize();
        }

        private void Restart_Click(object sender, RoutedEventArgs e)
        {
            _task = null;
            RestartBAck.Visibility = Visibility.Hidden;
            Restart.Visibility = Visibility.Hidden;
            foreach(var item in _items)
            {
                Window.Children.Remove(item);
            }
            if (Manager.Amount % 4 == 0 && Window.RowDefinitions.Count < 6)
            {
                AddRow();
                Manager.Upgrade();
                Grid.SetRowSpan(Restart,Window.RowDefinitions.Count);
                Grid.SetRowSpan(RestartBAck, Window.RowDefinitions.Count);
                Grid.SetRowSpan(Screamer, Window.RowDefinitions.Count);
                Height += 120;
            }
            Manager.Reset();
        }
    }
}