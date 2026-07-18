using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Aura.Windows;
using Aura.Utils;
using Aura.Models;
using Aura.Utils.Handlers;
using Aura.Utils.Logger;

namespace Aura
{
    public partial class App : Application
    {
        public static Window MainWindow { get; private set; }
        private static readonly ILogger Logger = AppLogger.GetLoggerForCurrentClass();

        public App()
        {
            this.InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            string[] cmdArgs = Environment.GetCommandLineArgs();
            
            if (cmdArgs.Length > 1)
            {
                string[] appArgs = new string[cmdArgs.Length - 1];
                Array.Copy(cmdArgs, 1, appArgs, 0, appArgs.Length);
                
                HandleCommandLine(appArgs);
                return;
            }

            MainWindow = new SettingsWindow();
            MainWindow.Activate();
        }
        
        private void HandleCommandLine(string[] args)
        {
            Logger.Info("Starting app with command line arguments: {0}", string.Join(", ", args));

            AutoFileSaver<SettingsModel> autoFileSaver = new AutoFileSaver<SettingsModel>("settings.xml", true);
            AppearanceHandler handler = new AppearanceHandler(autoFileSaver.Model);

            foreach (string arg in args)
            {
                switch (arg.ToLowerInvariant())
                {
                    case "/light":
                        handler.SwitchToLightTheme();
                        break;

                    case "/dark":
                        handler.SwitchToDarkTheme();
                        break;

                    case "/change":
                        DateTime now = DateTime.Now;

                        DateTime t1 = DateTime.Today.AddHours(autoFileSaver.Model.LightThemeTime.Hour).AddMinutes(autoFileSaver.Model.LightThemeTime.Minute);
                        DateTime t2 = DateTime.Today.AddHours(autoFileSaver.Model.DarkThemeTime.Hour).AddMinutes(autoFileSaver.Model.DarkThemeTime.Minute);

                        if (now > t1 && now < t2)
                        {
                            handler.SwitchToLightTheme();
                        }
                        else
                        {
                            handler.SwitchToDarkTheme();
                        }

                        break;

                    case "/update":
                        AutoUpdater autoUpdater = new AutoUpdater(true, true);
                        autoUpdater.CheckForUpdates(true).Wait();
                        break;

                    case "/clean":
                        TaskSchedulerHandler.DeleteAllTasks();
                        break;

                    default:
                        Logger.Error("Command line argument is not accepted: {0}", arg);
                        break;
                }
            }
            
            Environment.Exit(0);
        }
    }
}

