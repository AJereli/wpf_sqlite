using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPF_SQL_App.VIews;

namespace WPF_SQL_App
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IDBProvider _dbProvider;
        public MainWindow()
        {
            InitializeComponent();

            _dbProvider = new DBProvider();

            loginBtn.Click += LoginBtn_Click;
        }

        private async void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(uiLogin.Text))
            {
                MessageBox.Show("Login is required");
                return;
            }
            if (string.IsNullOrEmpty(uiPass.Password))
            {
                MessageBox.Show("Password is required");
                return;
            }

            var user = await _dbProvider.Login(uiLogin.Text, uiPass.Password);
            if (user != null)
            {
                AppContext.Current.CurrentUser = user;
                var boxesWIns = new BoxesWindow();
                boxesWIns.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Username or password is wrong");
            }

        }
    }
}
