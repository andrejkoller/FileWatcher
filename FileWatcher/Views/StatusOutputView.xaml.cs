using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FileWatcher.Models;
using Path = System.IO.Path;

namespace FileWatcher.Views
{
    /// <summary>
    /// Interaction logic for StatusOutputView.xaml
    /// </summary>
    public partial class StatusOutputView : UserControl
    {
        private readonly WatcherOptions _options = new();
        public StatusOutputView(WatcherOptions options)
        {
            _options = options;
            InitializeComponent();
        }

        private void InitializeFileWatchers()
        {
            try
            {
                foreach (var watcher in _options.Watchers)
                {
                    watcher.Value.Changed += OnFileChanged;
                    watcher.Value.Created += OnFileChanged;
                    watcher.Value.Deleted += OnFileChanged;
                    watcher.Value.Renamed += OnFileRenamed;

                    watcher.Value.EnableRaisingEvents = true;

                    Logger.LogStatus(StatusPanel, $"Started watching: {watcher.Value.Path}", Brushes.Green);
                }
            }
            catch (Exception ex)
            {
                Logger.LogStatus(StatusPanel, $"Error initializing watcher: {ex.Message}", Brushes.Red);
                DisposeFileWatchers();
            }
        }

        private void DisposeFileWatchers()
        {
            if (_options.Watchers.Count > 0)
            {
                foreach (var watcher in _options.Watchers)
                {
                    watcher.Value.EnableRaisingEvents = false;
                    watcher.Value.Dispose();
                    Logger.LogStatus(StatusPanel, $"Stopped watching: {watcher.Value.Path}", Brushes.Green);
                }
                _options.Watchers.Clear();
            }
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            if (IsFileTypeSupported(e.FullPath, _options.SupportedExtensions))
            {
                Dispatcher.Invoke(() =>
                {
                    var message = $"{e.ChangeType} file: {e.FullPath}";
                    Logger.LogStatus(StatusPanel, message, Brushes.Black);
                });
            }
        }

        private void OnFileRenamed(object sender, RenamedEventArgs e)
        {
            if (IsFileTypeSupported(e.FullPath, _options.SupportedExtensions))
            {
                Dispatcher.Invoke(() =>
                {
                    var message = $"Renamed file: {e.OldFullPath} to {e.FullPath}";
                    Logger.LogStatus(StatusPanel, message, Brushes.Black);
                });
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            SetUIState(isWatching: true);
            InitializeFileWatchers();

            Logger.LogStatus(StatusPanel, $"Watcher(s) started successfully.\nListening for changes...", Brushes.Green);
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            SetUIState(isWatching: false);
            DisposeFileWatchers();

            Logger.LogStatus(StatusPanel, "Watcher stopped.", Brushes.Green);
        }

        private void SetUIState(bool isWatching)
        {
            StartButton.IsEnabled = !isWatching;
            StopButton.IsEnabled = isWatching;
        }

        private static bool IsFileTypeSupported(string filePath, string[] extensions)
        {
            var fileExtension = Path.GetExtension(filePath)?.ToLowerInvariant();
            return extensions.Contains(fileExtension);
        }
    }
}
