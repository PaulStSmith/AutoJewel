using System.Drawing;
using System.Runtime.InteropServices;

namespace AutoJewelModern;

/// <summary>
/// Provides a set of Win32 interop helpers for registering global hotkeys, retrieving window bounds,
/// synthesizing mouse input and capturing window contents as bitmaps.
/// </summary>
public static partial class Win32Helper
{
    /// <summary>
    /// The Windows message ID sent when a registered hotkey is pressed.
    /// </summary>
    public const int WM_HOTKEY = 0x0312;

    /// <summary>
    /// Modifier keys that can be combined when registering a hotkey.
    /// Values match the modifiers accepted by the Win32 RegisterHotKey API.
    /// </summary>
    [Flags]
    public enum HotkeyMods : uint
    {
        /// <summary>
        /// The ALT key.
        /// </summary>
        Alt = 0x0001,

        /// <summary>
        /// The CTRL key.
        /// </summary>
        Control = 0x0002,

        /// <summary>
        /// The SHIFT key.
        /// </summary>
        Shift = 0x0004,

        /// <summary>
        /// The Windows key (WIN).
        /// </summary>
        Win = 0x0008
    }

    /// <summary>
    /// Virtual-key codes used for registering hotkeys in this application.
    /// Extend as needed for additional keys.
    /// </summary>
    public enum HotkeyVk : uint
    {
        /// <summary>
        /// Function key F8 (virtual key code 0x77).
        /// </summary>
        F8 = 0x77
    }

    /// <summary>
    /// Represents the rectangle of a window in screen coordinates as returned by the Win32 GetWindowRect API.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WindowRect
    {
        /// <summary>
        /// The x-coordinate of the upper-left corner of the window.
        /// </summary>
        public int Left;

        /// <summary>
        /// The y-coordinate of the upper-left corner of the window.
        /// </summary>
        public int Top;

        /// <summary>
        /// The x-coordinate of the lower-right corner of the window.
        /// </summary>
        public int Right;

        /// <summary>
        /// The y-coordinate of the lower-right corner of the window.
        /// </summary>
        public int Bottom;
    }

    /// <summary>
    /// Win32 mouse event flag for a left button press.
    /// </summary>
    private const int MOUSEEVENTF_LEFTDOWN = 0x0002;

    /// <summary>
    /// Win32 mouse event flag for a left button release.
    /// </summary>
    private const int MOUSEEVENTF_LEFTUP = 0x0004;

    /// <summary>
    /// Registers a system-wide hotkey. This is a low-level P/Invoke declaration for the Win32 RegisterHotKey function.
    /// </summary>
    /// <param name="hWnd">Handle to the window that will receive WM_HOTKEY messages. Use <see cref="IntPtr.Zero"/> to associate with the current thread.</param>
    /// <param name="id">Application-defined identifier of the hotkey.</param>
    /// <param name="fsModifiers">Modifier keys (Alt, Control, Shift, Win) as a bitwise combination.</param>
    /// <param name="vk">Virtual-key code of the hotkey.</param>
    /// <returns>True if the hotkey was registered successfully; otherwise false. Call <see cref="Marshal.GetLastWin32Error"/> for extended error information when false.</returns>
    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    /// <summary>
    /// Unregisters a system-wide hotkey. P/Invoke declaration for the Win32 UnregisterHotKey function.
    /// </summary>
    /// <param name="hWnd">Handle to the window associated with the hotkey. Use the same handle that was passed to <see cref="RegisterHotKey(IntPtr,int,uint,uint)"/>.</param>
    /// <param name="id">Identifier of the hotkey to unregister.</param>
    /// <returns>True if successful; otherwise false. Call <see cref="Marshal.GetLastWin32Error"/> for details.</returns>
    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool UnregisterHotKey(IntPtr hWnd, int id);

    /// <summary>
    /// Retrieves the dimensions of the bounding rectangle of the specified window. P/Invoke declaration for GetWindowRect.
    /// </summary>
    /// <param name="hWnd">Handle to the window.</param>
    /// <param name="lpRect">When this method returns, contains a <see cref="WindowRect"/> structure with the window coordinates in screen space.</param>
    /// <returns>True on success; otherwise false. Call <see cref="Marshal.GetLastWin32Error"/> for failure details.</returns>
    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetWindowRect(IntPtr hWnd, out WindowRect lpRect);

    /// <summary>
    /// Brings the specified window to the top of the Z order. P/Invoke declaration for BringWindowToTop.
    /// </summary>
    /// <param name="hWnd">Handle to the window to bring to top.</param>
    /// <returns>True if successful; otherwise false.</returns>
    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool BringWindowToTop(IntPtr hWnd);

    /// <summary>
    /// Sets the specified window as the foreground window. P/Invoke declaration for SetForegroundWindow.
    /// </summary>
    /// <param name="hWnd">Handle to the window to set to foreground.</param>
    /// <returns>True if the window was brought to the foreground; otherwise false.</returns>
    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool SetForegroundWindow(IntPtr hWnd);

    /// <summary>
    /// Synthesizes mouse motion and button clicks. P/Invoke declaration for the legacy mouse_event API.
    /// Note: For more complex scenarios prefer SendInput; mouse_event is sufficient for simple clicks in this project.
    /// </summary>
    /// <param name="dwFlags">Flags that specify various aspects of mouse motion and button press.</param>
    /// <param name="dx">The x-coordinate or amount of motion.</param>
    /// <param name="dy">The y-coordinate or amount of motion.</param>
    /// <param name="dwData">Additional data (e.g., wheel delta).</param>
    /// <param name="dwExtraInfo">An additional value associated with the event.</param>
    [LibraryImport("user32.dll")]
    private static partial void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

    /// <summary>
    /// Registers a system-wide hotkey using strongly-typed modifier and virtual-key enums.
    /// </summary>
    /// <param name="hWnd">Handle to the window that will receive WM_HOTKEY messages.</param>
    /// <param name="id">Application-defined identifier of the hotkey.</param>
    /// <param name="mods">Modifier keys as <see cref="HotkeyMods"/>.</param>
    /// <param name="vk">Virtual key as <see cref="HotkeyVk"/>.</param>
    /// <returns>True if registration succeeded; otherwise false.</returns>
    public static bool RegisterHotKey(IntPtr hWnd, int id, HotkeyMods mods, HotkeyVk vk)
    {
        Logger.Debug($"Registering hotkey - ID: {id}, Mods: {mods}, Key: {vk}");
        var result = RegisterHotKey(hWnd, id, (uint)mods, (uint)vk);
        if (result)
        {
            Logger.Success($"Hotkey registered successfully - ID: {id}");
        }
        else
        {
            var error = Marshal.GetLastWin32Error();
            Logger.Error($"Failed to register hotkey - ID: {id}, Win32 Error: {error}");
        }
        return result;
    }

    /// <summary>
    /// Retrieves the bounding rectangle of the specified window as a <see cref="Rectangle"/> in screen coordinates.
    /// </summary>
    /// <param name="hWnd">Handle to the window to query.</param>
    /// <returns>A <see cref="Rectangle"/> representing the window bounds.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the underlying Win32 call fails.</exception>
    public static Rectangle GetWindowRectangle(IntPtr hWnd)
    {
        Logger.Debug($"Getting window rectangle for handle: 0x{hWnd:X}");
        if (!GetWindowRect(hWnd, out var rect))
        {
            var error = Marshal.GetLastWin32Error();
            Logger.Error($"Failed to get window rectangle - Handle: 0x{hWnd:X}, Win32 Error: {error}");
            throw new InvalidOperationException("Failed to get window rectangle");
        }

        var rectangle = new Rectangle(rect.Left, rect.Top,
            rect.Right - rect.Left, rect.Bottom - rect.Top);
        Logger.Debug($"Window rectangle: {rectangle.X},{rectangle.Y} {rectangle.Width}x{rectangle.Height}");
        return rectangle;
    }

    /// <summary>
    /// Synthesizes a left mouse button press at the current cursor position.
    /// </summary>
    public static void MouseDown()
    {
        Logger.Debug($"Mouse down at current cursor position");
        mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
    }

    /// <summary>
    /// Synthesizes a left mouse button release at the current cursor position.
    /// </summary>
    public static void MouseUp()
    {
        Logger.Debug($"Mouse up at current cursor position");
        mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
    }

    /// <summary>
    /// Attempts to bring the specified window to the top and set it as the foreground window, then captures its image.
    /// </summary>
    /// <param name="hWnd">Handle to the window to capture.</param>
    /// <returns>
    /// A <see cref="Bitmap"/> containing the window image if the window was successfully brought to foreground; otherwise null.
    /// The caller is responsible for disposing the returned <see cref="Bitmap"/>.
    /// </returns>
    public static Bitmap? CaptureWindow(IntPtr hWnd)
    {
        Logger.Debug($"Capturing window - Handle: 0x{hWnd:X}");
        
        var topResult = BringWindowToTop(hWnd);
        var foregroundResult = SetForegroundWindow(hWnd);
        
        if (!topResult || !foregroundResult)
        {
            Logger.Warn($"Failed to bring window to foreground - BringToTop: {topResult}, SetForeground: {foregroundResult}");
            return null;
        }
        
        Logger.Debug("Window brought to foreground, capturing contents");
        return CaptureHandle(hWnd);
    }

    /// <summary>
    /// Captures the client area of the window specified by <paramref name="hWnd"/> and returns it as a <see cref="Bitmap"/>.
    /// </summary>
    /// <param name="hWnd">Handle to the window to capture.</param>
    /// <returns>A <see cref="Bitmap"/> representing the window contents.</returns>
    /// <exception cref="InvalidOperationException">Thrown when capturing fails for any reason. Inner exception contains the original error.</exception>
    public static Bitmap CaptureHandle(IntPtr hWnd)
    {
        try
        {
            Logger.Debug($"Capturing handle contents - Handle: 0x{hWnd:X}");
            var rect = GetWindowRectangle(hWnd);
            using var gHwnd = Graphics.FromHwnd(hWnd);
            var bitmap = new Bitmap(rect.Width, rect.Height, gHwnd);
            using var gBitmap = Graphics.FromImage(bitmap);

            gBitmap.CopyFromScreen(rect.X, rect.Y, 0, 0,
                rect.Size, CopyPixelOperation.SourceCopy);

            Logger.Success($"Captured bitmap: {bitmap.Width}x{bitmap.Height} pixels");
            return bitmap;
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to capture window handle 0x{hWnd:X}", ex);
            throw new InvalidOperationException("Failed to capture window", ex);
        }
    }
}