using System.Windows;
using FileWatcher.Models;
using FileWatcher.Services;
using FileWatcher.Views;

namespace FileWatcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Watcher _watcher = new();
        private readonly WatcherManager _watcherManager = new();
        public MainWindow()
        {
            InitializeComponent();
            ContentArea.Content = new OptionsView(_watcher, _watcherManager);
            UpdateButtonStates();
        }

        private void StatusOutputButton_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new StatusOutputView(_watcherManager);
            UpdateButtonStates();
        }

        private void WatcherInstancesButton_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new WatcherInstancesView(_watcherManager);
            UpdateButtonStates();
        }

        private void OptionsButton_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new OptionsView(_watcher, _watcherManager);
            UpdateButtonStates();
        }

        private void UpdateButtonStates()
        {
            StatusOutputButton.IsEnabled = true;
            WatcherInstancesButton.IsEnabled = true;
            OptionsButton.IsEnabled = true;

            if (ContentArea.Content is StatusOutputView)
            {
                StatusOutputButton.IsEnabled = false;
            }
            else if (ContentArea.Content is WatcherInstancesView)
            {
                WatcherInstancesButton.IsEnabled = false;
            }
            else if (ContentArea.Content is OptionsView)
            {
                OptionsButton.IsEnabled = false;
            }
        }
    }
}