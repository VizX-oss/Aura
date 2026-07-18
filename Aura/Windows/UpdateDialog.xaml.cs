using Aura.Utils;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Aura.Windows
{
    public partial class UpdateDialog : ContentDialog
    {
        public AutoUpdater AutoUpdater { get; private set; }

        public UpdateDialog(AutoUpdater autoUpdater)
        {
            InitializeComponent();

            AutoUpdater = autoUpdater;
            this.DataContext = autoUpdater.Model;

            if (autoUpdater.Model.Status == Models.UpdateStatus.None)
            {
                _ = autoUpdater.CheckForUpdates(true);
            }
        }

        private void CheckButton_Click(object sender, RoutedEventArgs e)
        {
            _ = AutoUpdater.CheckForUpdates(true);
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            _ = AutoUpdater.DownloadUpdate();
        }

        private void InstallButton_Click(object sender, RoutedEventArgs e)
        {
            AutoUpdater.InstallUpdate();
        }
    }
}

