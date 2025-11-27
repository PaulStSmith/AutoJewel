using System.Diagnostics;

namespace AutoJewelModern;

#pragma warning disable IDE1006 // Naming Styles

/// <summary>
/// Main application form for AutoJewel Modern.
/// Handles UI interactions, hotkey registration, game process attachment,
/// periodic capture and matching via <see cref="JewelMatcher"/>, and simulating mouse moves.
/// </summary>
public partial class MainForm : Form
{
    /// <summary>
    /// Hotkey identifier used for registering the global hotkey.
    /// </summary>
    private const int HOTKEY_ID = 0x1000;

    /// <summary>
    /// Standard Bejeweled 3 process name.
    /// </summary>
    private const string PROCESS_NAME_STANDARD = "Bejeweled3";

    /// <summary>
    /// Trial/demo Bejeweled 3 process name.
    /// </summary>
    private const string PROCESS_NAME_TRIAL = "popcapgame1";

    /// <summary>
    /// Per-game-mode settings mapping to timer interval and simulated move time (ms).
    /// Key: <see cref="JewelMatcher.GameMode"/>, Value: tuple of (Interval, MoveTime).
    /// </summary>
    private static readonly Dictionary<JewelMatcher.GameMode, (int Interval, int MoveTime)> GameSettings = new()
    {
        { JewelMatcher.GameMode.Classic, (1500, 100) },
        { JewelMatcher.GameMode.Zen, (500, 50) },
        { JewelMatcher.GameMode.Lightning, (300, 50) },
        { JewelMatcher.GameMode.IceStorm, (300, 50) },
        { JewelMatcher.GameMode.Balance, (300, 50) }
    };

    /// <summary>
    /// Timer used to periodically capture the game window and attempt to find moves.
    /// </summary>
    private readonly System.Windows.Forms.Timer _gameTimer = new();

    /// <summary>
    /// Time in milliseconds to hold the mouse down when making a simulated move.
    /// </summary>
    private int _moveTime = 100;

    /// <summary>
    /// The pattern matcher responsible for analyzing screenshots and producing move suggestions.
    /// </summary>
    private readonly JewelMatcher _matcher = new();

    /// <summary>
    /// Currently selected process name to attach to (standard or trial).
    /// </summary>
    private string _processName = PROCESS_NAME_STANDARD;

    /// <summary>
    /// Handle to the game window process when attached.
    /// </summary>
    private IntPtr _processHandle = IntPtr.Zero;

    /// <summary>
    /// Flag indicating whether automation is currently running.
    /// </summary>
    private bool _running = false;

    /// <summary>
    /// Flag indicating whether debug mode (logging and bitmap saving) is enabled.
    /// </summary>
    private bool _debugEnabled = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainForm"/> class.
    /// </summary>
    public MainForm()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Handles the form load event.
    /// Loads pattern definitions, configures the game timer and registers the global hotkey.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    /// <remarks>
    /// Displays an error message if initialization fails (pattern load or hotkey registration).
    /// </remarks>
    private async void MainForm_Load(object sender, EventArgs e)
    {
        Logger.Info("AutoJewel starting up...");
        try
        {
            Logger.Info("Loading pattern definitions from patterns.txt");
            await _matcher.LoadPatternsAsync("patterns.txt");
            Logger.Success("Pattern definitions loaded successfully");
            
            _gameTimer.Stop();
            _gameTimer.Interval = 300;
            _gameTimer.Tick += GameTimer_Tick;
            Logger.Debug($"Game timer configured with {_gameTimer.Interval}ms interval");

            Logger.Debug("Registering global hotkey Ctrl+F8");
            var hotkeyRegistered = Win32Helper.RegisterHotKey(Handle, HOTKEY_ID,
                Win32Helper.HotkeyMods.Control, Win32Helper.HotkeyVk.F8);
                
            if (!hotkeyRegistered)
            {
                Logger.Error("Failed to register global hotkey Ctrl+F8");
                ShowErrorMessage("Failed to register the global hotkey Ctrl+F8.");
            }
            else
            {
                Logger.Success("Global hotkey Ctrl+F8 registered successfully");
            }

            statusLabel.Text = "Ready - Press Ctrl+F8 to start/stop automation";
            Logger.Info("AutoJewel initialization completed successfully");
        }
        catch (Exception ex)
        {
            Logger.Error("Initialization failed", ex);
            ShowErrorMessage($"Initialization failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Tick handler invoked by the <see cref="_gameTimer"/>.
    /// Captures the game window, asks the matcher for a solution and executes the simulated mouse move when found.
    /// </summary>
    /// <param name="sender">Event sender (timer).</param>
    /// <param name="e">Event arguments.</param>
    /// <remarks>
    /// This method silently ignores exceptions to avoid interrupting gameplay. If capturing fails or coordinates are invalid,
    /// automation will stop and an error will be shown.
    /// </remarks>
    private void GameTimer_Tick(object? sender, EventArgs e)
    {
        if (!_running) return;

        try
        {
            Logger.Debug("Timer tick - capturing game window");
            var windowRect = Win32Helper.GetWindowRectangle(_processHandle);
            Logger.Debug($"Window rectangle: {windowRect.Left},{windowRect.Top} {windowRect.Width}x{windowRect.Height}");
            
            using var bitmap = Win32Helper.CaptureWindow(_processHandle);
            
            if (bitmap == null)
            {
                Logger.Error("Failed to capture game window - stopping automation");
                StopRunning();
                ShowErrorMessage("Failed to capture the game window.");
                return;
            }

            Logger.Debug($"Captured bitmap: {bitmap.Width}x{bitmap.Height}");
            var solution = _matcher.GetSolution(bitmap);
            if (solution == null)
            {
                Logger.Debug("No solution found - continuing");
                return;
            }

            Logger.Info($"Solution found: pattern priority {solution.Method.Pattern.Priority}");
            
            var pointSrc = new Point(
                solution.PointSrc.X + windowRect.Left,
                solution.PointSrc.Y + windowRect.Top
            );

            var pointDst = new Point(
                solution.PointDst.X + windowRect.Left,
                solution.PointDst.Y + windowRect.Top
            );

            Logger.Debug($"Screen coordinates - Source: ({pointSrc.X},{pointSrc.Y}), Destination: ({pointDst.X},{pointDst.Y})");

            // Validate points are within window bounds
            if (!IsPointInWindow(pointSrc, windowRect) || !IsPointInWindow(pointDst, windowRect))
            {
                Logger.Error($"Move coordinates outside window bounds - Source: ({pointSrc.X},{pointSrc.Y}), Dest: ({pointDst.X},{pointDst.Y}), Window: {windowRect}");
                StopRunning();
                ShowErrorMessage("Move coordinates are outside game window.");
                return;
            }

            // Execute the move
            Logger.Debug($"Executing move: ({pointSrc.X},{pointSrc.Y}) → ({pointDst.X},{pointDst.Y}) with {_moveTime}ms delay");
            Cursor.Position = pointSrc;
            Win32Helper.MouseDown();
            Thread.Sleep(_moveTime);
            Cursor.Position = pointDst;
            Win32Helper.MouseUp();
            Logger.Success("Move executed successfully");
        }
        catch (Exception ex)
        {
            Logger.Error("Exception in timer tick", ex);
        }
    }

    /// <summary>
    /// Validates whether a given point lies strictly inside the specified window rectangle.
    /// </summary>
    /// <param name="point">Point in screen coordinates to test.</param>
    /// <param name="window">Window rectangle in screen coordinates.</param>
    /// <returns>True if the point is inside the window; otherwise false.</returns>
    private static bool IsPointInWindow(Point point, Rectangle window)
    {
        return point.X > window.Left && point.X < window.Right &&
               point.Y > window.Top && point.Y < window.Bottom;
    }

    /// <summary>
    /// Applies UI-selected parameters to the matcher and internal timers.
    /// </summary>
    /// <returns>True if parameters were set successfully; false if a required selection is missing.</returns>
    private bool SetParameters()
    {
        var selectedMode = GetSelectedGameMode();
        if (!selectedMode.HasValue)
        {
            Logger.Warn("No game mode selected");
            return false;
        }

        _matcher.Mode = selectedMode.Value;
        Logger.Info($"Game mode set to: {selectedMode.Value}");
        
        var (interval, moveTime) = GameSettings[selectedMode.Value];
        _gameTimer.Interval = interval;
        _moveTime = moveTime;
        Logger.Debug($"Timer settings - Interval: {interval}ms, Move time: {moveTime}ms");

        _processName = standardRadioButton.Checked ? PROCESS_NAME_STANDARD : PROCESS_NAME_TRIAL;
        Logger.Info($"Process name set to: {_processName}");
        return true;
    }

    /// <summary>
    /// Reads the selected game mode from the UI radio buttons.
    /// </summary>
    /// <returns>The selected <see cref="JewelMatcher.GameMode"/> or null if none selected.</returns>
    private JewelMatcher.GameMode? GetSelectedGameMode()
    {
        if (classicRadioButton.Checked) return JewelMatcher.GameMode.Classic;
        if (zenRadioButton.Checked) return JewelMatcher.GameMode.Zen;
        if (lightningRadioButton.Checked) return JewelMatcher.GameMode.Lightning;
        if (iceStormRadioButton.Checked) return JewelMatcher.GameMode.IceStorm;
        if (balanceRadioButton.Checked) return JewelMatcher.GameMode.Balance;
        return null;
    }

    /// <summary>
    /// Attempts to start automation by validating parameters, attaching to the game process and starting the timer.
    /// </summary>
    /// <remarks>
    /// On success, updates UI state and status label. On failure, shows an appropriate error message and does not start automation.
    /// </remarks>
    private void StartRunning()
    {
        Logger.Info("Starting automation...");
        
        if (!SetParameters())
        {
            Logger.Error("Failed to set parameters - missing game mode or process type selection");
            ShowErrorMessage("Please select a game mode and process type.");
            return;
        }

        try
        {
            Logger.Info($"Looking for game process: {_processName}");
            var processes = Process.GetProcessesByName(_processName);
            if (processes.Length == 0)
            {
                Logger.Error($"Game process '{_processName}' not found");
                ShowErrorMessage($"Game process '{_processName}' not found. Make sure Bejeweled 3 is running.");
                return;
            }

            Logger.Success($"Found {processes.Length} instance(s) of {_processName}");
            _processHandle = processes[0].MainWindowHandle;
            if (_processHandle == IntPtr.Zero)
            {
                Logger.Error("Could not get main window handle from process");
                ShowErrorMessage("Could not get game window handle.");
                return;
            }
            
            Logger.Success($"Attached to game window handle: 0x{_processHandle:X}");
        }
        catch (Exception ex)
        {
            Logger.Error("Failed to connect to game process", ex);
            ShowErrorMessage($"Failed to connect to game process: {ex.Message}");
            return;
        }

        // Enable logging and bitmap saving if debug mode is enabled
        if (_debugEnabled)
        {
            Logger.IsEnabled = true;
            Logger.ShowLogWindow();
            BitmapSaver.IsEnabled = true;
            
            // Clear old bitmaps to keep directory clean
            _ = BitmapSaver.ClearBitmaps();
            
            Logger.Info($"Logging and bitmap saving enabled - Mode: {_matcher.Mode}, Interval: {_gameTimer.Interval}ms, Move time: {_moveTime}ms");
        }

        Win32Helper.BringWindowToTop(Handle);

        _running = true;
        _gameTimer.Start();
        statusLabel.Text = $"Running in {_matcher.Mode} mode - Press Ctrl+F8 to stop";
        startButton.Enabled = false;
        stopButton.Enabled = true;
        
        Logger.Success("Automation started successfully");
    }

    /// <summary>
    /// Stops automation gracefully, stops the timer and updates UI state.
    /// </summary>
    private void StopRunning()
    {
        Logger.Info("Stopping automation...");
        _running = false;
        _gameTimer.Stop();
        statusLabel.Text = "Stopped - Press Ctrl+F8 to start";
        startButton.Enabled = true;
        stopButton.Enabled = false;
        Logger.Success("Automation stopped");
        Logger.SaveLogToFile("bitmaps\\autojewel.log");
    }

    /// <summary>
    /// Displays an error message and brings the main window to the foreground before showing it.
    /// </summary>
    /// <param name="message">The error message to display.</param>
    private void ShowErrorMessage(string message)
    {
        Win32Helper.BringWindowToTop(Handle);
        Win32Helper.SetForegroundWindow(Handle);
        MessageBox.Show(message, "AutoJewel Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    /// <summary>
    /// Processes Windows messages to intercept the registered hotkey.
    /// </summary>
    /// <param name="m">Windows message to process.</param>
    /// <remarks>
    /// When the hotkey registered with id <see cref="HOTKEY_ID"/> is pressed, toggles automation start/stop.
    /// Other messages are forwarded to the base implementation.
    /// </remarks>
    protected override void WndProc(ref Message m)
    {
        if (m.Msg == Win32Helper.WM_HOTKEY && (int)m.WParam == HOTKEY_ID)
        {
            if (_running)
                StopRunning();
            else
                StartRunning();
        }
        else
        {
            base.WndProc(ref m);
        }
    }

    /// <summary>
    /// Handler for the Start button click event. Starts automation.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    private void startButton_Click(object sender, EventArgs e)
    {
        StartRunning();
    }

    /// <summary>
    /// Handler for the Stop button click event. Stops automation.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    private void stopButton_Click(object sender, EventArgs e)
    {
        StopRunning();
    }

    /// <summary>
    /// Handler for the About button click.
    /// Displays information about the application and usage instructions.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    private void aboutButton_Click(object sender, EventArgs e)
    {
        var aboutText = """
            AutoJewel - Modernized
            
            An automated Bejeweled 3 player using pattern recognition.
            
            Original Author: Wudi <wudicgi@gmail.com>
            Original License: GPL v2
            Modernized for .NET 9 by Claude Code
            
            Usage:
            1. Start Bejeweled 3
            2. Select game mode and process type
            3. Press Ctrl+F8 or click Start to begin automation
            4. Press Ctrl+F8 or click Stop to end automation
            
            The program works best in windowed mode at 1024x768 resolution.
            """;

        MessageBox.Show(aboutText, "About AutoJewel", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    /// <summary>
    /// Handler for the Exit button click. Exits the application.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    private void exitButton_Click(object sender, EventArgs e)
    {
        Application.Exit();
    }

    /// <summary>
    /// Handler for the debug checkbox state change.
    /// Controls whether logging and bitmap saving are enabled.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    private void debugCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        _debugEnabled = debugCheckBox.Checked;
        
        if (_debugEnabled)
        {
            Logger.IsEnabled = true;
            BitmapSaver.IsEnabled = true;
            Logger.Info("Debug mode enabled - logging and bitmap saving activated");
        }
        else
        {
            Logger.Info("Debug mode disabled - logging and bitmap saving deactivated");
            Logger.IsEnabled = false;
            BitmapSaver.IsEnabled = false;
            Logger.HideLogWindow();
        }
    }

    /// <summary>
    /// Cleans up resources when the form is closed.
    /// Ensures the registered hotkey is unregistered.
    /// </summary>
    /// <param name="e">Form closed event arguments.</param>
    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        Win32Helper.UnregisterHotKey(Handle, HOTKEY_ID);
        base.OnFormClosed(e);
    }
}