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
        public Manager Manager;
        private List<string> tittles = new List<string>()
        {
            "Красава!","У тебя золотые руки!","Вай, красота","Просто мастер!","Смотри не ударься током","Балдеж!","67"
        };
        public MainWindow()
        {
            InitializeComponent();
            ResizeMode = ResizeMode.NoResize;
            Manager = new Manager(this);
            Manager.AllConnected += Manager_AllConnected;
            Manager.OnConnecting += Manager_OnConnecting;
            Manager.OnFail += Manager_OnFail;
            Loaded += MainWindow_Loaded;
        }

        private void Manager_OnFail(object? sender, EventArgs e)
        {
            Tittle.Foreground = Brushes.Red;
            Tittle.Text = "ОСТОРОЖНО!!!";
        }

        private void Manager_OnConnecting(object? sender, EventArgs e)
        {
            Tittle.Foreground = Brushes.Black;
            Tittle.Text = tittles[new Random().Next(tittles.Count)];
        }

        private void Manager_AllConnected(object? sender, EventArgs e)
        {
            Restart.Visibility = Visibility.Visible;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Manager.Initialize();
        }

        private void Restart_Click(object sender, RoutedEventArgs e)
        {
            Restart.Visibility = Visibility.Hidden;
            var items = Window.Children.OfType<Rectangle>().ToList();
            foreach(var item in items)
            {
                Window.Children.Remove(item);
            }
            Manager.Reset();
        }
    }
}