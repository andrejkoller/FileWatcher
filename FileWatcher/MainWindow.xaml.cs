using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FileWatcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FileSystemWatcher? _watcher;

        public MainWindow()
        {
            DisposeFileWatcher();
            InitializeComponent();
        }

        private void InitializeFileWatcher()
        {
            try
            {
                _watcher = new FileSystemWatcher
                {
                    Path = PathTextBox.Text,
                    EnableRaisingEvents = true,
                    NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite,
                    Filter = "*.*"
                };

                _watcher.Changed += OnFileChanged;
                _watcher.Created += OnFileChanged;
                _watcher.Deleted += OnFileChanged;
                _watcher.Renamed += OnFileRenamed;
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Error initializing watcher: {ex.Message}";
                DisposeFileWatcher();
            }
        }

        private void DisposeFileWatcher()
        {
            if (_watcher != null)
            {
                _watcher.EnableRaisingEvents = false;
                _watcher.Dispose();
                _watcher = null;
            }
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                var message = $"{e.ChangeType} file: {e.FullPath}";
                StatusText.Text = message;
            });
        }

        private void OnFileRenamed(object sender, RenamedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                var message = $"Renamed file: {e.OldFullPath} to {e.FullPath}";
                StatusText.Text = message;
            });
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(PathTextBox.Text))
            {
                StatusText.Text = "Cannot start watcher. Invalid directory path.";
                return;
            }

            InitializeFileWatcher();
            StartButton.IsEnabled = false;
            StopButton.IsEnabled = true;
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            DisposeFileWatcher();
            StopButton.IsEnabled = false;
            StartButton.IsEnabled = true;
            StatusText.Text = "Watcher stopped.";
        }

        private void PathTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PathTextBox.Text))
            {
                StatusText.Text = "Please enter a valid path:";
            }
            else if (!Directory.Exists(PathTextBox.Text))
            {
                StatusText.Text = "Invalid directory path.";
            }
        }
    }
}