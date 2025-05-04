using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileWatcher.Models;

namespace FileWatcher.Services
{
    public class WatcherManager
    {
        public List<Watcher> Watchers { get; set; } = [];

        public void AddWatcher(string name, FileSystemWatcher watcher)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Watcher name cannot be null or empty.", nameof(name));

            if (Watchers.Any(w => w.Name == name))
                throw new InvalidOperationException($"A watcher with the name '{name}' already exists.");

            Watchers.Add(new Watcher
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
    }
}
