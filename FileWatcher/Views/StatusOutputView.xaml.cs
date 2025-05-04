using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FileWatcher.Models;
using FileWatcher.Services;
using Path = System.IO.Path;

namespace FileWatcher.Views
{
    /// <summary>
    /// Interaction logic for StatusOutputView.xaml
    /// </summary>
    public partial class StatusOutputView : UserControl
    {
        private ObservableCollection<Watcher> _watchersList;
        private readonly WatcherManager _watcherManager;
        private bool _isWatching = false;

        public StatusOutputView(WatcherManager watcherManager)
        {
            _watcherManager = watcherManager;
            InitializeComponent();
            _watchersList = new ObservableCollection<Watcher>(_watcherManager.Watchers);
        }

        private void InitializeFileWatchers()
        {
            try
            {
                foreach (var watcher in _watchersList)
                {
                    RegisterWatcherEvents(watcher);
                }
            }
            catch (Exception ex)
            {
                Logger.LogStatus(StatusPanel, $"Error initializing watcher: {ex.Message}", Brushes.Red);
            }
        }

        private void DisposeFileWatchers()
        {
            if (_watchersList.Count > 0)
            {
                foreach (var watcher in _watchersList)
                {
                    watcher.FileSystemWatcher.Changed -= OnFileChanged;
                    watcher.FileSystemWatcher.Created -= OnFileChanged;
                    watcher.FileSystemWatcher.Deleted -= OnFileChanged;
                    watcher.FileSystemWatcher.Renamed -= OnFileRenamed;

                    watcher.FileSystemWatcher.EnableRaisingEvents = false;

                    watcher.FileSystemWatcher.Dispose();
                    Logger.LogStatus(StatusPanel, $"Stopped watching: {watcher.FileSystemWatcher.Path}", Brushes.Green);
                }
                _watchersList.Clear();
            }
        }

        private void RegisterWatcherEvents(Watcher watcher)
        {
            if (Directory.Exists(watcher.FileSystemWatcher.Path))
            {
                watcher.FileSystemWatcher.Changed += OnFileChanged;
                watcher.FileSystemWatcher.Created += OnFileChanged;
                watcher.FileSystemWatcher.Deleted += OnFileChanged;
                watcher.FileSystemWatcher.Renamed += OnFileRenamed;

                watcher.FileSystemWatcher.EnableRaisingEvents = true;

                Logger.LogStatus(StatusPanel, $"Started watching: {watcher.FileSystemWatcher.Path}", Brushes.Green);
            }
            else
            {
                Logger.LogStatus(StatusPanel, $"Directory does not exist: {watcher.FileSystemWatcher.Path}", Brushes.Red);
            }
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            try
            {
                var fileSystemWatcher = sender as FileSystemWatcher;
                var watcher = _watchersList.FirstOrDefault(w => w.FileSystemWatcher == fileSystemWatcher);
                if (watcher != null)
                {
                    Dispatcher.Invoke(() =>
                    {
                        var message = $"{e.ChangeType} file: {e.FullPath}";
                        Logger.LogStatus(StatusPanel, message, Brushes.Black);
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.LogStatus(StatusPanel, $"Error processing file change: {ex.Message}", Brushes.Red);
            }
        }

        private void OnFileRenamed(object sender, RenamedEventArgs e)
        {
            try
            {
                var fileSystemWatcher = sender as FileSystemWatcher;
                var watcher = _watchersList.FirstOrDefault(w => w.FileSystemWatcher == fileSystemWatcher);
                if (watcher != null)
                {
                    Dispatcher.Invoke(() =>
                    {
                        var message = $"Renamed file: {e.OldFullPath} to {e.FullPath}";
                        Logger.LogStatus(StatusPanel, message, Brushes.Black);
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.LogStatus(StatusPanel, $"Error processing file rename: {ex.Message}", Brushes.Red);
            }
        }
    }
}
