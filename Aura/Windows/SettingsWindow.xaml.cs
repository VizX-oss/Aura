using Aura.Models;
using Aura.Utils;
using Aura.Utils.Handlers;
using Aura.Utils.Logger;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Aura.Windows
{
    public partial class SettingsWindow : Window
    {
        private static readonly ILogger Logger = AppLogger.GetLoggerForCurrentClass();

        private readonly AutoFileSaver<SettingsModel> _autoFileSaver = new AutoFileSaver<SettingsModel>("settings.xml");

        private readonly AutoUpdater _autoUpdater = new AutoUpdater();

        public SettingsWindow()
        {
            InitializeComponent();

            // Center window and set size
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
            if (appWindow != null)
            {
                var displayArea = Microsoft.UI.Windowing.DisplayArea.GetFromWindowId(windowId, Microsoft.UI.Windowing.DisplayAreaFallback.Primary);
                if (displayArea != null)
                {
                    var centricArea = displayArea.WorkArea;
                    var size = new Windows.Graphics.SizeInt32(380, 580);
                    appWindow.Resize(size);
                    var position = new Windows.Graphics.PointInt32(
                        (centricArea.Width - size.Width) / 2,
                        (centricArea.Height - size.Height) / 2
                    );
                    appWindow.Move(position);
                }
            }

            // Check if administrator
            bool isElevated;
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }

            if (!isElevated)
            {
                AdminInfoBar.IsOpen = true;
                ContentPanel.IsEnabled = false;
            }

            // Bind change type combobox items
            ChangeTypeComboBox.ItemsSource = SettingsModel.ChangeTypeValues;

            _autoFileSaver.Model.ShouldChangeProperty += SettingsModel_ShouldChangeProperty;
            _autoFileSaver.Model.PropertyChanged += SettingsModel_PropertyChanged;
            _autoUpdater.Model.PropertyChanged += UpdateModel_PropertyChanged;

            ContentPanel.DataContext = _autoFileSaver.Model;

            _ = _autoUpdater.CheckForUpdates(false);
        }

        private bool SettingsModel_ShouldChangeProperty(object sender, PropertyChangedEventArgs e)
        {
            return true;
        }

        private void SettingsModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SettingsModel model = (SettingsModel)sender;

            List<string> properties = new List<string>() { "Enabled", "LightThemeTime", "DarkThemeTime" };

            if (properties.Contains(e.PropertyName))
            {
                ContentPanel.IsEnabled = false;

                try
                {
                    if (!model.Enabled)
                    {
                        TaskSchedulerHandler.DeleteAllTasks();
                    }
                    else
                    {
                        TaskSchedulerHandler.UpdateAllTasks(model.LightThemeTime, model.DarkThemeTime);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("Task scheduler error: " + ex.Message);
                    ShowErrorMessage();
                }
                finally
                {
                    ContentPanel.IsEnabled = true;
                }
            }
        }

        private async void ShowErrorMessage()
        {
            var dialog = new ContentDialog
            {
                Title = "An error occurred",
                Content = "There was an error while writing to TaskScheduler. Please check logs for more info.",
                CloseButtonText = "Close",
                XamlRoot = this.Content.XamlRoot
            };
            await dialog.ShowAsync();
        }

        private async void UpdateModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateModel model = (UpdateModel)sender;

            if (e.PropertyName == "Status" && model.Status == UpdateStatus.NewUpdate)
            {
                var dialog = new UpdateDialog(_autoUpdater)
                {
                    XamlRoot = this.Content.XamlRoot
                };
                await dialog.ShowAsync();
            }
        }

        private async void BrowseThemeHyperlink_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton hyperlink = (HyperlinkButton)sender;

            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.ComputerFolder;
            picker.FileTypeFilter.Add(".theme");

            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                PropertyInfo propertyInfo = _autoFileSaver.Model.GetType().GetProperty((string)hyperlink.Tag);
                propertyInfo.SetValue(_autoFileSaver.Model, file.Path, null);
            }
        }

        private async void BrowseWallpaperHyperlink_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton hyperlink = (HyperlinkButton)sender;

            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add("*");

            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                PropertyInfo propertyInfo = _autoFileSaver.Model.GetType().GetProperty((string)hyperlink.Tag);
                propertyInfo.SetValue(_autoFileSaver.Model, file.Path, null);
            }
        }

        private void StartLightThemeButton_Click(object sender, RoutedEventArgs e)
        {
            AppearanceHandler handler = new AppearanceHandler(_autoFileSaver.Model);
            handler.SwitchToLightTheme();
        }

        private void StartDarkThemeButton_Click(object sender, RoutedEventArgs e)
        {
            AppearanceHandler handler = new AppearanceHandler(_autoFileSaver.Model);
            handler.SwitchToDarkTheme();
        }

        private async void WindowHeader_OnClickAbout(object sender, RoutedEventArgs e)
        {
            var dialog = new AboutDialog(_autoUpdater)
            {
                XamlRoot = this.Content.XamlRoot
            };
            await dialog.ShowAsync();
        }

        private void RunAsAdmin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = Environment.ProcessPath;
                process.StartInfo.Verb = "runas";
                process.Start();
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to elevate: " + ex.Message);
            }
        }
    }
}

