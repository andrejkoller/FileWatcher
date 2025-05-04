using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using FileWatcher.Models;
using FileWatcher.Services;

namespace FileWatcher.Views
{
    /// <summary>  
    /// Interaction logic for WatcherInstancesView.xaml  
    /// </summary>  
    public partial class WatcherInstancesView : UserControl
    {
        private ObservableCollection<Watcher> _watchersList;

        private readonly WatcherManager _watcherManager;

        public WatcherInstancesView(WatcherManager watcherManager)
        {
            _watcherManager = watcherManager;
            InitializeComponent();

            _watchersList = new ObservableCollection<Watcher>(_watcherManager.Watchers);
            WatchersListView.ItemsSource = _watchersList;
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Watcher watcher)
            {
                string watcherName = watcher.Name;

                _watcherManager.RemoveWatcher(watcherName);

                var itemToRemove = _watchersList.FirstOrDefault(w => w.Name == watcherName);
                if (itemToRemove != null)
                {
                    _watchersList.Remove(itemToRemove);
                    MessageBox.Show($"Watcher '{watcherName}' removed successfully.\n", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show($"Watcher '{watcherName}' not found in the list.\n", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Cannot remove watcher. No watcher selected.\n", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
