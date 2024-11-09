using System;
using System.IO;
using System.Windows.Forms;
using System.Threading;

class Program
{
    static readonly string rootFolder = @"C:\Users\wania\my-first-app"; // Update this to your folder path
    static System.Threading.Timer clipboardTimer;

    static void Main()
    {
        // Initialize FileSystemWatcher for monitoring the folder (optional, as it's for observing changes)
        FileSystemWatcher watcher = new FileSystemWatcher
        {
            Path = rootFolder,
            IncludeSubdirectories = true,
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite
        };

        // Set event handlers
        watcher.Created += OnChanged;
        watcher.Deleted += OnChanged;
        watcher.Renamed += OnRenamed;
        watcher.EnableRaisingEvents = true;

        Console.WriteLine("Monitoring folder and clipboard for restricted copy-paste actions...");

        // Set up clipboard monitoring timer to check every second
        clipboardTimer = new System.Threading.Timer(CheckClipboard, null, 0, 1000);

        Console.WriteLine("Press 'q' to quit.");
        while (Console.Read() != 'q') ;
    }

    // File change handler (optional)
    private static void OnChanged(object source, FileSystemEventArgs e)
    {
        Console.WriteLine($"File: {e.FullPath} {e.ChangeType}");
    }

    // File renamed handler (optional)
    private static void OnRenamed(object source, RenamedEventArgs e)
    {
        Console.WriteLine($"File: {e.OldFullPath} renamed to {e.FullPath}");
    }

    // Monitor clipboard content every second
    private static void CheckClipboard(object state)
    {
        try
        {
            // Check if clipboard contains file paths
            if (Clipboard.ContainsFileDropList())
            {
                var files = Clipboard.GetFileDropList();
                foreach (string file in files)
                {
                    if (file.StartsWith(rootFolder, StringComparison.OrdinalIgnoreCase))
                    {
                        // Clear the clipboard if a file from the root folder is detected
                        Console.WriteLine("Copying from this folder is restricted. Clipboard cleared.");
                        Clipboard.Clear();
                        MessageBox.Show("Copying files from this folder is restricted.", "Restricted Action", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking clipboard: {ex.Message}");
        }
    }
}
