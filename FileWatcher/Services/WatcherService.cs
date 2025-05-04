using System.IO;
using FileWatcher.Models;

namespace FileWatcher.Services
{
    public class WatcherService
    {
        public List<WatcherModel> Watchers { get; set; } = [];

        private readonly List<FileSystemWatcher> _activeWatchers = new();

        public event EventHandler<string>? FileChanged;
        public event EventHandler<string>? FileRenamed;

        public void AddWatcher(string name, FileSystemWatcher watcher)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Watcher name cannot be null or empty.", nameof(name));

            if (Watchers.Any(w => w.Name == name))
                throw new InvalidOperationException($"A watcher with the name '{name}' already exists.");

            Watchers.Add(new WatcherModel
            {
                Id = Watchers.Count + 1,
                Name = name,
                Path = watcher.Path,
                Filter = watcher.Filter,
                IncludeSubdirectories = watcher.IncludeSubdirectories,
                FileSystemWatcher = watcher
            });
        }

        public void RemoveWatcher(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Watcher name cannot be null or empty.", nameof(name));

            var watcher = Watchers.FirstOrDefault(w => w.Name == name);
            if (watcher == null)
                throw new KeyNotFoundException($"No watcher found with the name '{name}'.");

            watcher.FileSystemWatcher.Dispose();
            Watchers.Remove(watcher);
        }

        public void StartWatching(IEnumerable<WatcherModel> watchers)
        {
            StopWatching();

            foreach (var watcher in watchers)
            {
                var fsw = new FileSystemWatcher
                {
                    Path = watcher.Path,
                    IncludeSubdirectories = watcher.IncludeSubdirectories,
                    NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite
                };

                fsw.Changed += OnChanged;
                fsw.Created += OnChanged;
                fsw.Deleted += OnChanged;
                fsw.Renamed += OnRenamed;

                fsw.EnableRaisingEvents = true;
                _activeWatchers.Add(fsw);
            }
        }

        public void StopWatching()
        {
            foreach (var watcher in _activeWatchers)
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
            }
            _activeWatchers.Clear();
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            FileChanged?.Invoke(this, $"{e.ChangeType}: {e.FullPath}");
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            FileRenamed?.Invoke(this, $"Renamed from {e.OldFullPath} to {e.FullPath}");
        }
    }
}
