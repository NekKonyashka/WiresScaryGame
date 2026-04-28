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
        private TaskCompletionSource<bool> _endAnimTask;
        private List<Rectangle> _items;


        private Manager _manager;
        private AnimationManager _animationManager;

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
            ResizeMode = ResizeMode.NoResize;
           _manager = new Manager();
            _animationManager = new AnimationManager();

            _animationManager.OnAnimationStart += _animationManager_OnAnimationStart;
            _animationManager.OnAnimationEnd += _animationManager_OnAnimationEnd;

            _manager.AllConnected += Manager_AllConnected;
            _manager.OnConnecting += Manager_OnConnecting;
            _manager.OnFail += Manager_OnFail;
            _manager.OnGameObjectCreated += AddItem;
            _manager.AnimationRequested += EndAnim;

            Loaded += (s, e) =>
            {
                _manager.Initialize(Window.ColumnDefinitions.Count, Window.RowDefinitions.Count);
            };
        }

        private void _animationManager_OnAnimationEnd(object? sender, EventArgs e)
        {
            _endAnimTask.SetResult(true);
            Window.Children.Remove(_animationManager.Image);
        }

        private void _animationManager_OnAnimationStart(object? sender, EventArgs e)
        {
            _endAnimTask = new TaskCompletionSource<bool>();
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
            Tittle.Foreground = Brushes.White;
            Tittle.Text = tittles[new Random().Next(tittles.Length)];

            Grid.SetColumn(_animationManager.Image, e.End_pos.X);
            Grid.SetRow(_animationManager.Image, e.End_pos.Y);
            Window.Children.Add(_animationManager.Image);

            _animationManager.Start();
        }

        private async Task EndAnim()
        {
            await _endAnimTask.Task;
            await Task.Delay(150);
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
            if (_manager.Amount % 4 == 0 && Window.RowDefinitions.Count < 6)
            {
                AddRow();
                _manager.PlaySound("upgrade");
                Grid.SetRowSpan(Restart,Window.RowDefinitions.Count);
                Grid.SetRowSpan(RestartBAck, Window.RowDefinitions.Count);
                Grid.SetRowSpan(Screamer, Window.RowDefinitions.Count);
                Height += 120;
            }
            _manager.Reset(Window.ColumnDefinitions.Count,Window.RowDefinitions.Count);
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _manager.ConnectionCheck(sender);
        }
    }
}