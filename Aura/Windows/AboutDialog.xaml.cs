using Aura.Utils;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace Aura.Windows
{
    public partial class AboutDialog : ContentDialog
    {
        private readonly AutoUpdater _autoUpdater;

        public AboutDialog(AutoUpdater autoUpdater)
        {
            InitializeComponent();
            _autoUpdater = autoUpdater;

            VersionText.Text = autoUpdater.LocalVersion.ToString();
            LastCheckText.Text = autoUpdater.Model.LastCheck.ToString();
        }

        private async void ButtonCheck_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();

            _ = _autoUpdater.CheckForUpdates(true);

            var dialog = new UpdateDialog(_autoUpdater)
            {
                XamlRoot = this.XamlRoot
            };
            await dialog.ShowAsync();
        }
    }
}

