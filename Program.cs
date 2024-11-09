using System;
using System.IO;
using System.Windows.Forms;
using System.Threading;

class Program
{
    static readonly string rootFolder = @"C:\Users\wania\my-first-app"; 
    static System.Threading.Timer clipboardTimer;

    static void Main()
    {
        FileSystemWatcher watcher = new FileSystemWatcher
        {
            Path = rootFolder,
            IncludeSubdirectories = true,
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite
        };

        watcher.Created += OnChanged;
        watcher.Deleted += OnChanged;
        watcher.Renamed += OnRenamed;
        watcher.EnableRaisingEvents = true;

        Console.WriteLine("Monitoring folder and clipboard for restricted copy-paste actions...");

        clipboardTimer = new System.Threading.Timer(CheckClipboard, null, 0, 1000);

        Console.WriteLine("Press 'q' to quit.");
        while (Console.Read() != 'q') ;
    }

    private static void OnChanged(object source, FileSystemEventArgs e)
    {
        Console.WriteLine($"File: {e.FullPath} {e.ChangeType}");
    }

    private static void OnRenamed(object source, RenamedEventArgs e)
    {
        Console.WriteLine($"File: {e.OldFullPath} renamed to {e.FullPath}");
    }

    private static void CheckClipboard(object state)
    {
        try
        {
            if (Clipboard.ContainsFileDropList())
            {
                var files = Clipboard.GetFileDropList();
                foreach (string file in files)
                {
                    if (file.StartsWith(rootFolder, StringComparison.OrdinalIgnoreCase))
                    {
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
