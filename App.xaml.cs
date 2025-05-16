using System.Windows;
using System.Windows.Forms;

namespace Renvert
{
    public partial class App : Application
    {
        private NotifyIcon trayIcon;
        private FileWatcher watcher;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            trayIcon = new NotifyIcon
            {
                Icon = System.Drawing.SystemIcons.Information,
                Visible = true,
                Text = "Renvert"
            };

            var menu = new ContextMenuStrip();
            menu.Items.Add("Open", null, (s, args) => new MainWindow().Show());
            menu.Items.Add("Exit", null, (s, args) => ExitApp());
            trayIcon.ContextMenuStrip = menu;

            trayIcon.MouseClick += (s, args) =>
            {
                if (args.Button == MouseButtons.Left)
                {
                    new MainWindow().Show();
                }
            };

            watcher = new FileWatcher(@"C:\Users\Huzaim\Desktop\Convert");
            watcher.Start();
        }

        private void ExitApp()
        {
            trayIcon.Visible = false;
            watcher.Stop();
            Shutdown();
        }
    }
}
