using System.IO;

namespace FileWatcher.Models
{
    public class WatcherModel
    {
        public int Id { get; set; } = 1;
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string Filter { get; set; } = "*.*";
        public bool IncludeSubdirectories { get; set; } = false;
        public FileSystemWatcher FileSystemWatcher { get; set; } = new();
    }
}
