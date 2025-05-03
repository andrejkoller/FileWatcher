using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace FileWatcher.Models
{
    public class Logger
    {
        public static void LogStatus(Panel statusPanel, string message, Brush? foregroundColor = null)
        {
            if (statusPanel is not null)
            {
                var textBlock = new TextBlock
                {
                    Text = $"{DateTime.Now:T} - {message}",
                    Foreground = foregroundColor ?? Brushes.Black
                };

                statusPanel.Children.Add(textBlock);

                if (VisualTreeHelper.GetParent(statusPanel) is ScrollViewer scrollViewer)
                {
                    scrollViewer.ScrollToEnd();
                }
            }
            else
            {
                throw new InvalidOperationException("StatusPanel is not initialized.");
            }
        }
    }
}
