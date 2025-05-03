using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileWatcher.Models
{
    public class WatcherOptions
    {
        public string Name { get; set; } = string.Empty;
        public string Folder { get; set; } = string.Empty;
        public string Filter { get; set; } = "*.*";
        public bool IncludeSubdirectories { get; set; } = false;

        public Dictionary<string, FileSystemWatcher> Watchers { get; set; } = new Dictionary<string, FileSystemWatcher>();

        public string[] SupportedExtensions { get; set; } = [];

        public void AddWatcher(string name, FileSystemWatcher watcher)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Watcher name cannot be null or empty.", nameof(name));

            if (Watchers.ContainsKey(name))
                throw new InvalidOperationException($"A watcher with the name '{name}' already exists.");

            Watchers[name] = watcher;
        }

        public void RemoveWatcher(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Watcher name cannot be null or empty.", nameof(name));

            if (!Watchers.ContainsKey(name))
                throw new KeyNotFoundException($"No watcher found with the name '{name}'.");

            Watchers[name].Dispose();

            Watchers.Remove(name);
        }
    }
}
