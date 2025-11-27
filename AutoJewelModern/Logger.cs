namespace AutoJewelModern;

/// <summary>
/// Static logging utility that provides centralized logging functionality with different levels.
/// Logs are displayed in a dedicated <see cref="LogWindow"/>.
/// </summary>
public static class Logger
{
    private static LogWindow? _logWindow;
    private static bool _isEnabled = false;

    /// <summary>
    /// Gets or sets whether logging is currently enabled.
    /// </summary>
    public static bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            _isEnabled = value;
            if (value && _logWindow == null)
            {
                InitializeLogWindow();
            }
        }
    }

    /// <summary>
    /// Initializes or shows the log window.
    /// </summary>
    public static void ShowLogWindow()
    {
        if (_logWindow == null)
        {
            InitializeLogWindow();
        }
        
        _logWindow?.Show();
        _logWindow?.BringToFront();
    }

    /// <summary>
    /// Hides the log window.
    /// </summary>
    public static void HideLogWindow()
    {
        _logWindow?.Hide();
    }

    /// <summary>
    /// Initializes the log window if not already created.
    /// </summary>
    private static void InitializeLogWindow()
    {
        if (_logWindow == null)
        {
            _logWindow = new LogWindow();
        }
    }

    /// <summary>
    /// Logs an informational message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public static void Info(string message)
    {
        Log(message, "INFO");
    }

    /// <summary>
    /// Logs a warning message.
    /// </summary>
    /// <param name="message">The warning message to log.</param>
    public static void Warn(string message)
    {
        Log(message, "WARN");
    }

    /// <summary>
    /// Logs an error message.
    /// </summary>
    /// <param name="message">The error message to log.</param>
    public static void Error(string message)
    {
        Log(message, "ERROR");
    }

    /// <summary>
    /// Logs an error message with exception details.
    /// </summary>
    /// <param name="message">The error message to log.</param>
    /// <param name="ex">The exception that occurred.</param>
    public static void Error(string message, Exception ex)
    {
        Log($"{message}: {ex.Message}", "ERROR");
        Log($"Stack trace: {ex.StackTrace}", "ERROR");
    }

    /// <summary>
    /// Logs a debug message.
    /// </summary>
    /// <param name="message">The debug message to log.</param>
    public static void Debug(string message)
    {
        Log(message, "DEBUG");
    }

    /// <summary>
    /// Logs a success/operation message.
    /// </summary>
    /// <param name="message">The success message to log.</param>
    public static void Success(string message)
    {
        Log(message, "SUCCESS");
    }

    /// <summary>
    /// Core logging method that handles the actual log output.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="level">The log level.</param>
    private static void Log(string message, string level)
    {
        if (!_isEnabled) return;

        try
        {
            if (_logWindow == null)
            {
                InitializeLogWindow();
            }

            _logWindow?.AppendLog(message, level);
        }
        catch
        {
            // Silently ignore logging errors to avoid infinite loops
        }
    }

    /// <summary>
    /// Clears all log entries.
    /// </summary>
    public static void Clear()
    {
        _logWindow?.ClearLog(null, EventArgs.Empty);
    }

    public static void SaveLogToFile(string filePath)
    {
        _logWindow?.SaveLogToFile(filePath);
    }

    /// <summary>
    /// Logs pattern matching details for debugging.
    /// </summary>
    /// <param name="color">The jewel color being processed.</param>
    /// <param name="patternCount">Number of patterns found.</param>
    /// <param name="bestPriority">Priority of the best match found.</param>
    public static void LogPatternMatch(string color, int patternCount, int bestPriority)
    {
        Debug($"Pattern match for {color}: {patternCount} patterns found, best priority: {bestPriority}");
    }

    /// <summary>
    /// Logs mouse move details for debugging.
    /// </summary>
    /// <param name="srcX">Source X coordinate.</param>
    /// <param name="srcY">Source Y coordinate.</param>
    /// <param name="dstX">Destination X coordinate.</param>
    /// <param name="dstY">Destination Y coordinate.</param>
    /// <param name="moveTime">Time spent on move in milliseconds.</param>
    public static void LogMouseMove(int srcX, int srcY, int dstX, int dstY, int moveTime)
    {
        Info($"Mouse move: ({srcX},{srcY}) → ({dstX},{dstY}) in {moveTime}ms");
    }

    /// <summary>
    /// Logs board analysis results.
    /// </summary>
    /// <param name="mode">Current game mode.</param>
    /// <param name="windowWidth">Captured window width.</param>
    /// <param name="windowHeight">Captured window height.</param>
    /// <param name="scaleFactor">Computed scale factor.</param>
    public static void LogBoardAnalysis(string mode, int windowWidth, int windowHeight, float scaleFactor)
    {
        Debug($"Board analysis - Mode: {mode}, Window: {windowWidth}x{windowHeight}, Scale: {scaleFactor:F3}");
    }

    /// <summary>
    /// Logs jewel color detection results for a specific cell.
    /// </summary>
    /// <param name="row">Board row.</param>
    /// <param name="col">Board column.</param>
    /// <param name="color">Detected color.</param>
    /// <param name="hue">Hue value.</param>
    /// <param name="saturation">Saturation value.</param>
    public static void LogColorDetection(int row, int col, string color, int hue, int saturation)
    {
        Debug($"Color at ({row},{col}): {color} (H:{hue}, S:{saturation})");
    }
}