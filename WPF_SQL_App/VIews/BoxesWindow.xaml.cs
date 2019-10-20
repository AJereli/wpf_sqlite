using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using WPF_SQL_App.Models;

namespace WPF_SQL_App.VIews
{
    /// <summary>
    /// Логика взаимодействия для BoxesWindow.xaml
    /// </summary>
    public partial class BoxesWindow : Window
    {
        private readonly IDBProvider _dbProvider;

        public BoxesWindow()
        {
            InitializeComponent();

            _dbProvider = new DBProvider();

            var l = GetAllBoxes();
            uiAddBoxBtn.Click += UiAddBoxBtn_Click;
            boxesList.ItemsSource = new ObservableCollection<BoxModel>(l);

        }

        private void UiAddBoxBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(uiBoxName.Text))
            {
                MessageBox.Show("Box name is required");
                return;
            }


        }

        private IEnumerable<BoxModel> GetAllBoxes()
        {
            var results = _dbProvider.GetAllBoxes(AppContext.Current.CurrentUser.Id);
            return results;
        }
    }
}
