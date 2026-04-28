using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Drag
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _frameIndex = 0;
        private TaskCompletionSource<bool> _endAnimTask;
        private BitmapImage _bitmap;
        private DateTime _lastFrame;
        private List<Rectangle> _items;
        private Image _currentSprite;

        public Manager Manager;
        public Task<bool> CurrentTask => _endAnimTask.Task;
        private string[] tittles =
        {
            "Красава!","У тебя золотые руки!","Вай, красота",
            "Просто мастер!","Смотри не ударься током","Балдеж!",
            "67","Ты ювелир!","Ты просто босс, просто начальник",
            "Повышаю твою зп", "Волшебно","Я поражен!","Вау!",
            "Как ты это делаешь?","Повелитель проводов","Спасите помогите"
        };
        public MainWindow()
        {
            InitializeComponent();
            _items = new List<Rectangle>();
            _bitmap = new BitmapImage(new Uri("./res/Images/Sprites.png", UriKind.Relative));
            ResizeMode = ResizeMode.NoResize;
            Manager = new Manager(this);

            Manager.AllConnected += Manager_AllConnected;
            Manager.OnConnecting += Manager_OnConnecting;
            Manager.OnFail += Manager_OnFail;
            Manager.OnGameObjectCreated += AddItem;
            Manager.AnimationRequested += EndAnim;

            Loaded += (s, e) =>
            {
                Manager.Initialize(Window.ColumnDefinitions.Count, Window.RowDefinitions.Count);
            };
        }

        private void AddItem(object? sender,ObjectInfoEventArgs e)
        {
            _items.Add(e.Object);
            Window.Children.Add(e.Object);
        }

        private void Manager_OnFail(object? sender, EventArgs e)
        {
            ShakeShakeMilkShakeкокакола67();
            Tittle.Foreground = Brushes.Red;
            Tittle.Text = "ОСТОРОЖНО!!!";
        }

        private void Manager_OnConnecting(object? sender, OnConnectedEventArgs e)
        {
            StartAnim();
            Tittle.Foreground = Brushes.White;
            Tittle.Text = tittles[new Random().Next(tittles.Length)];
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
            EndAnim();
        }

        private async Task EndAnim()
        {
            await _endAnimTask.Task;
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
            Window.Children.Remove(_currentSprite);
        }

        private void CompositionTarget_Rendering(object? sender, EventArgs e)
        {
            if ((DateTime.Now - _lastFrame).TotalMilliseconds > 100)
            {
                if (_frameIndex == 4)
                {
                    _frameIndex = 0;
                    _endAnimTask?.SetResult(true);
                }
                int x = (int)((_frameIndex % 2 == 0 ? 0 : 1) * (_bitmap.Width / 2));
                int y = (int)((_frameIndex / 2) * (_bitmap.Height / 2));

                CroppedBitmap cropped = new CroppedBitmap(_bitmap,
                    new Int32Rect(x, y, (int)(_bitmap.Width / 2), (int)(_bitmap.Height / 2)));

                _currentSprite.Source = cropped;

                _lastFrame = DateTime.Now;
                _frameIndex++;
            }
        }
        public void StartAnim()
        {
            _endAnimTask = new TaskCompletionSource<bool>();
        }
        private async void Manager_AllConnected(object? sender, EventArgs e)
        {
            await _endAnimTask.Task;
            await Task.Delay(150);
            Restart.Visibility = Visibility.Visible;
            RestartBAck.Visibility = Visibility.Visible;
            Tittle.Text = "";
        }

        private async void ShakeShakeMilkShakeкокакола67()
        {
            var current = Screamer.Fill;
            Screamer.Visibility = Visibility.Visible;
            var random = new Random();
            for (int i = 0; i < 70; i++)
            {
                int x_shift = random.Next(-5, 6);
                int y_shift = random.Next(-5, 6);

                Screamer.Fill = i % 2 == 0 ? Brushes.Black : current;


                Left += x_shift;
                Top += y_shift;

                await Task.Delay(1);
            }
            Screamer.Visibility = Visibility.Hidden;
        }

        private void AddRow()
        {
            Window.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1,GridUnitType.Star)});
        }

        private void Restart_Click(object sender, RoutedEventArgs e)
        {
            _endAnimTask = null;

            RestartBAck.Visibility = Visibility.Hidden;
            Restart.Visibility = Visibility.Hidden;
            foreach(var item in _items)
            {
                Window.Children.Remove(item);
            }
            if (Manager.Amount % 4 == 0 && Window.RowDefinitions.Count < 6)
            {
                AddRow();
                Manager.PlaySound("upgrade");
                Grid.SetRowSpan(Restart,Window.RowDefinitions.Count);
                Grid.SetRowSpan(RestartBAck, Window.RowDefinitions.Count);
                Grid.SetRowSpan(Screamer, Window.RowDefinitions.Count);
                Height += 120;
            }
            Manager.Reset(Window.ColumnDefinitions.Count,Window.RowDefinitions.Count);
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Manager.ConnectionCheck(sender);
        }
    }
}