/***********************************************************************
Author:     Wudi <wudicgi@gmail.com>
License:    GPL (GNU General Public License)
Copyright:  2011 Wudi Labs
Link:       http://blog.wudilabs.org/entry/d26e8ee5/
***********************************************************************/

/***********************************************************************
This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

The GNU General Public License can be found at
http://www.gnu.org/copyleft/gpl.html
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;

namespace WudiLabs.AutoJewel
{
    /// <summary>
    /// Helper methods and P/Invoke signatures for interacting with Win32 APIs used by the application.
    /// </summary>
    public static class Win32Helper
    {
        /// <summary>
        /// WM_HOTKEY message identifier.
        /// </summary>
        public const int MSG_WM_HOTKEY = 0x0312;

        /// <summary>
        /// Modifier flags for registering global hotkeys.
        /// </summary>
        [Flags]
        public enum HotkeyMods : uint
        {
            MOD_ALT = 0x0001,
            MOD_CONTROL = 0x0002,
            MOD_SHIFT = 0x0004,
            MOD_WIN = 0x0008
        }

        /// <summary>
        /// Virtual-key codes used by the application for registering hotkeys.
        /// </summary>
        public enum HotkeyVk : uint
        {
            VK_F8 = 0x77
        }

        /// <summary>
        /// Structure that represents a window rectangle returned by Win32 APIs.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct WindowRect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        const int MOUSEEVENTF_LEFTUP = 0x0004;

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, out WindowRect lpRect);

        [DllImport("user32.dll")]
        public static extern bool BringWindowToTop(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        /// <summary>
        /// Registers a global hotkey using strongly-typed modifiers and virtual key values.
        /// </summary>
        /// <param name="hWnd">Window handle that will receive hotkey messages.</param>
        /// <param name="id">Identifier for the hotkey.</param>
        /// <param name="mods">Modifier keys for the hotkey.</param>
        /// <param name="vk">Virtual key code for the hotkey.</param>
        /// <returns>True if registration succeeded; otherwise false.</returns>
        public static bool RegisterHotKey(IntPtr hWnd, int id, HotkeyMods mods, HotkeyVk vk)
        {
            return RegisterHotKey(hWnd, id, (uint)mods, (uint)vk);
        }

        /// <summary>
        /// Gets the window rectangle as a managed <see cref="Rectangle"/>.
        /// </summary>
        /// <param name="hWnd">Handle of the window.</param>
        /// <returns>A <see cref="Rectangle"/> that represents the window bounds.</returns>
        public static Rectangle GetWindowRect(IntPtr hWnd)
        {
            WindowRect rect = new WindowRect();

            GetWindowRect(hWnd, out rect);

            return new Rectangle(rect.left, rect.top,
                rect.right - rect.left, rect.bottom - rect.top);
        }

        /// <summary>
        /// Simulates a left mouse button down event.
        /// </summary>
        public static void MouseDown()
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
        }

        /// <summary>
        /// Simulates a left mouse button up event.
        /// </summary>
        public static void MouseUp()
        {
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }

        /// <summary>
        /// Captures the specified window to a <see cref="Bitmap"/>. The method brings the window to top and sets it to foreground before capture.
        /// </summary>
        /// <param name="hWnd">Handle of the window to capture.</param>
        /// <returns>A <see cref="Bitmap"/> of the window contents, or null on failure.</returns>
        public static Bitmap CaptureWindow(IntPtr hWnd)
        {
            if (!Win32Helper.BringWindowToTop(hWnd))
            {
                return null;
            }

            if (!Win32Helper.SetForegroundWindow(hWnd))
            {
                return null;
            }

            return CaptureHandle(hWnd);
        }

        /// <summary>
        /// Captures the window identified by the handle into a bitmap.
        /// </summary>
        /// <param name="hWnd">Window handle.</param>
        /// <returns>The captured <see cref="Bitmap"/>.</returns>
        public static Bitmap CaptureHandle(IntPtr hWnd)
        {
            Bitmap bitmap = null;

            try
            {
                Rectangle rect = Win32Helper.GetWindowRect(hWnd);
                Graphics g_hwnd = Graphics.FromHwnd(hWnd);
                bitmap = new Bitmap(rect.Width, rect.Height, g_hwnd);
                Graphics g_bitmap = Graphics.FromImage(bitmap);
                g_bitmap.CopyFromScreen(rect.X, rect.Y, 0, 0,
                    rect.Size, CopyPixelOperation.SourceCopy);
                g_bitmap.Dispose();
                g_hwnd.Dispose();
            }
            catch (Exception exp)
            {
                throw exp;
            }

            return bitmap;
        }
    }
}
