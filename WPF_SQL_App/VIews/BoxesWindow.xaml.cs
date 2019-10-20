using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        private IEnumerable<BoxModel> _currentBoxes;

        private AppContext appContext => AppContext.Current;

        public BoxesWindow()
        {
            InitializeComponent();

            _dbProvider = new DBProvider();

            uiAddBoxBtn.Click += UiAddBoxBtn_Click;

            Refresh();

        }

        private void UiAddBoxBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(uiBoxName.Text))
            {
                MessageBox.Show("Box name is required");
                return;
            }

            if (_currentBoxes.FirstOrDefault(box => box.Title == uiBoxName.Text) != null)
            {
                MessageBox.Show("Box with this name already exists");
                return;
            }

            _dbProvider.AddBox(uiBoxName.Text, appContext.CurrentUser.Id);

            Refresh();

        }

        private void Refresh()
        {
            var count = _dbProvider.CountAvailableForMe(appContext.CurrentUser.Id);
            _currentBoxes = GetAllBoxes();
            boxesList.ItemsSource = new ObservableCollection<BoxModel>(_currentBoxes);
            uiAvailableLable.Content = $"Available for me: {count}";
        }

        private IEnumerable<BoxModel> GetAllBoxes()
        {
            var results = _dbProvider.GetAllBoxes(AppContext.Current.CurrentUser.Id);
            return results;
        }
    }
}
