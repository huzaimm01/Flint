using System.IO;

namespace Renvert
{
    public class FileWatcher
    {
        private FileSystemWatcher watcher;
        private string folder;

        public FileWatcher(string folderPath)
        {
            folder = folderPath;
            watcher = new FileSystemWatcher(folder)
            {
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size,
                Filter = "*.*"
            };

            watcher.Renamed += OnRenamed;
        }

        public void Start() => watcher.EnableRaisingEvents = true;
        public void Stop() => watcher.EnableRaisingEvents = false;

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            string srcExt = Path.GetExtension(e.OldFullPath).ToLower();
            string destExt = Path.GetExtension(e.FullPath).ToLower();

            if (srcExt == ".png" && destExt == ".pdf")
            {
                Converter.ImageToPdf(e.OldFullPath, e.FullPath);
            }
        }
    }
}
