using System.Collections.ObjectModel;
using System.ComponentModel;
using FileWatcher.Models;
using System.Windows.Media;
using FileWatcher.Services;
using System.Windows;

namespace FileWatcher.ViewModels
{
    public class LogViewModel : INotifyPropertyChanged
    {
        private readonly WatcherService _watcherService;

        public ObservableCollection<LogMessageModel> LogMessages { get; } = new();

        public LogViewModel(IEnumerable<WatcherModel> watchers)
        {
            _watcherService = new WatcherService();
            _watcherService.FileChanged += (_, message) => AddLog(message, Brushes.White);
            _watcherService.FileRenamed += (_, message) => AddLog(message, Brushes.DarkCyan);

            _watcherService.StartWatching(watchers);

            foreach (var watcher in watchers)
            {
                AddLog($"{watcher.Name}: Watching {watcher.Path} for changes...", Brushes.White);
            }
        }

        private void AddLog(string message, Brush color)
        {
            Console.WriteLine($"Adding log: {message}");
            Application.Current.Dispatcher.Invoke(() =>
            {
                LogMessages.Add(new LogMessageModel
                {
                    Message = message,
                    ForegroundColor = color
                });
            });
        }

        public void Stop() => _watcherService.StopWatching();

        public event PropertyChangedEventHandler? PropertyChanged;
    }

}
