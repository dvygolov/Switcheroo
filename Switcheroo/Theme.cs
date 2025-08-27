using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Microsoft.Win32;
using Switcheroo.Properties;

namespace Switcheroo
{
    public static class Theme
    {
        private static SolidColorBrush Background;
        private static SolidColorBrush Foreground;
        private static MainWindow mainWindow;

        public enum Mode
        {
            Light,
            Dark,
            System
        }

        public static void SuscribeWindow(MainWindow main)
        {
            mainWindow = main;
        }

        public static void LoadTheme()
        {
            Mode mode;

            mode = GetThemeModeFromSettings();

            switch (mode)
            {
                case Mode.Light:
                    SetLightTheme();
                    break;
                case Mode.Dark:
                    SetDarkTheme();
                    break;
                case Mode.System:
                    if (IsSystemInDarkMode())
                    {
                        SetDarkTheme();
                    }
                    else
                    {
                        SetLightTheme();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }

            SetUpTheme();
        }

        private static void SetDarkTheme()
        {
            Background = new SolidColorBrush(Color.FromRgb(30, 30, 30));
            Foreground = new SolidColorBrush(Color.FromRgb(240, 240, 240));
        }

        private static void SetLightTheme()
        {
            Background = new SolidColorBrush(Color.FromRgb(248, 248, 248));
            Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        }

        private static Mode GetThemeModeFromSettings()
        {
            Mode mode;
            if (!Enum.TryParse(Settings.Default.Theme, out mode))
            {
                mode = Mode.Light;
            }

            return mode;
        }

        public static bool IsSystemInDarkMode()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(
                    "Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize"))
                {
                    if (key != null)
                    {
                        object value = key.GetValue("AppsUseLightTheme");
                        if (value != null && value is int)
                        {
                            return ((int)value) == 0; // 0 = dark, 1 = light
                        }
                    }
                }
            }
            catch { }
            return false;
        }

        private static void SetUpTheme()
        {
            mainWindow.Border.Background =
                mainWindow.txtSearch.Background = mainWindow.lblProgramName.Background
                = mainWindow.Border.BorderBrush = Background;
            mainWindow.txtSearch.Foreground = mainWindow.lblProgramName.Foreground = Foreground;
        }
    }
}
