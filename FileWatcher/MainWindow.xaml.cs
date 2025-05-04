using System.Windows;
using FileWatcher.Models;
using FileWatcher.Services;
using FileWatcher.ViewModels;

namespace FileWatcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var watcher = new WatcherModel();
            var watcherService = new WatcherService();

            DataContext = new MainWindowViewModel(watcher, watcherService);
        }
    }
}