using System.Windows.Media;

namespace FileWatcher.Models
{
    public class LogMessageModel
    {
        public string Message { get; set; } = string.Empty;
        public Brush ForegroundColor { get; set; } = Brushes.White;
    }
}
