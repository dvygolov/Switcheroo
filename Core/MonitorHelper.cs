/*
 * Switcheroo - The incremental-search task switcher for Windows.
 * http://www.switcheroo.io/
 * Copyright 2009, 2010 James Sulak
 * Copyright 2014 Regin Larsen
 * 
 * Switcheroo is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Switcheroo is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with Switcheroo.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Runtime.InteropServices;

namespace Switcheroo.Core
{
    public static class MonitorHelper
    {
        /// <summary>
        /// Gets the DPI scaling factor for the system
        /// </summary>
        private static double GetDpiScale()
        {
            IntPtr hdc = WinApi.GetDC(IntPtr.Zero);
            if (hdc != IntPtr.Zero)
            {
                int dpiX = WinApi.GetDeviceCaps(hdc, WinApi.LOGPIXELSX);
                WinApi.ReleaseDC(IntPtr.Zero, hdc);
                return dpiX / 96.0; // 96 DPI is 100% scaling
            }
            return 1.0; // Default to no scaling
        }

        /// <summary>
        /// Gets the monitor information for the monitor that contains the mouse cursor
        /// </summary>
        /// <returns>Monitor info or null if failed</returns>
        public static MonitorInfo GetMonitorFromCursor()
        {
            WinApi.POINT cursorPos;
            if (!WinApi.GetCursorPos(out cursorPos))
                return null;

            IntPtr hMonitor = WinApi.MonitorFromPoint(cursorPos, WinApi.MonitorOptions.MONITOR_DEFAULTTONEAREST);
            if (hMonitor == IntPtr.Zero)
                return null;

            WinApi.MONITORINFO monitorInfo = new WinApi.MONITORINFO();
            monitorInfo.cbSize = Marshal.SizeOf(monitorInfo);

            if (!WinApi.GetMonitorInfo(hMonitor, ref monitorInfo))
                return null;

            double dpiScale = GetDpiScale();

            return new MonitorInfo
            {
                WorkArea = new MonitorRectangle
                {
                    Left = monitorInfo.rcWork.Left,
                    Top = monitorInfo.rcWork.Top,
                    Right = monitorInfo.rcWork.Right,
                    Bottom = monitorInfo.rcWork.Bottom
                },
                MonitorArea = new MonitorRectangle
                {
                    Left = monitorInfo.rcMonitor.Left,
                    Top = monitorInfo.rcMonitor.Top,
                    Right = monitorInfo.rcMonitor.Right,
                    Bottom = monitorInfo.rcMonitor.Bottom
                },
                IsPrimary = (monitorInfo.dwFlags & 1) != 0,
                DpiScale = dpiScale
            };
        }
    }

    public class MonitorInfo
    {
        public MonitorRectangle WorkArea { get; set; }
        public MonitorRectangle MonitorArea { get; set; }
        public bool IsPrimary { get; set; }
        public double DpiScale { get; set; } = 1.0;

        public int Width => MonitorArea.Right - MonitorArea.Left;
        public int Height => MonitorArea.Bottom - MonitorArea.Top;
        public int WorkAreaWidth => WorkArea.Right - WorkArea.Left;
        public int WorkAreaHeight => WorkArea.Bottom - WorkArea.Top;

        // WPF device-independent pixel coordinates (scaled for DPI)
        public double WpfWorkAreaLeft => WorkArea.Left / DpiScale;
        public double WpfWorkAreaTop => WorkArea.Top / DpiScale;
        public double WpfWorkAreaWidth => WorkAreaWidth / DpiScale;
        public double WpfWorkAreaHeight => WorkAreaHeight / DpiScale;
    }

    public class MonitorRectangle
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Right { get; set; }
        public int Bottom { get; set; }
    }
}
