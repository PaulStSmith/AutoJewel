namespace AutoJewelModern;

#pragma warning disable IDE1006 // Naming Styles

/// <summary>
/// A dedicated window for displaying application debug and operation logs.
/// </summary>
public partial class LogWindow : Form
{
    private TextBox _logTextBox;
    private CheckBox _autoScrollCheckBox;

#pragma warning disable CS8618
    // Non-nullable field must contain a non-null value when exiting constructor.
    // Consider adding the 'required' modifier or declaring as nullable.
    // Fields are initialized in InitializeComponent method.

    /// <summary>
    /// Initializes a new instance of the <see cref="LogWindow"/> class.
    /// </summary>
    public LogWindow()
    {
        InitializeComponent();
    }
#pragma warning restore CS8618 

    private void InitializeComponent()
    {
        Text = "AutoJewel Debug Log";
        Size = new Size(800, 600);
        StartPosition = FormStartPosition.CenterScreen;

        // Create main layout
        var tableLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 2,
            ColumnCount = 1
        };

        // Add control panel at top
        var controlPanel = new Panel
        {
            Height = 40,
            Dock = DockStyle.Top,
            Padding = new Padding(8)
        };

        _autoScrollCheckBox = new CheckBox
        {
            Text = "Auto-scroll",
            Checked = true,
            Location = new Point(8, 10)
        };

        var clearButton = new Button
        {
            Text = "Clear",
            Size = new Size(60, 23),
            Location = new Point(120, 8)
        };
        clearButton.Click += ClearLog;

        controlPanel.Controls.Add(_autoScrollCheckBox);
        controlPanel.Controls.Add(clearButton);// .AddRange([_autoScrollCheckBox, clearButton]);

        // Create log text box
        _logTextBox = new TextBox
        {
            Multiline = true,
            ReadOnly = true,
            Dock = DockStyle.Fill,
            ScrollBars = ScrollBars.Vertical,
            Font = new Font("Consolas", 9),
            BackColor = Color.Black,
            ForeColor = Color.LightGreen
        };

        tableLayout.Controls.Add(controlPanel, 0, 0);
        tableLayout.Controls.Add(_logTextBox, 0, 1);

        tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        Controls.Add(tableLayout);

        // Prevent closing, just hide instead
        FormClosing += formClosingHandler;
    }
    private void formClosingHandler(object? s, FormClosingEventArgs e)
    {
        e.Cancel = true;
        Hide();
    }

    public void SaveLogToFile(string filePath)
    {
        if (InvokeRequired)
        {
            Invoke(() => SaveLogToFile(filePath));
            return;
        }
        try
        {
            System.IO.File.WriteAllText(filePath, _logTextBox.Text);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to save log to file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>
    /// Appends a new log entry with timestamp.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="level">The log level (INFO, WARN, ERROR, DEBUG).</param>
    public void AppendLog(string message, string level = "INFO")
    {
        if (InvokeRequired)
        {
            Invoke(() => AppendLog(message, level));
            return;
        }

        var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
        var logEntry = $"[{timestamp}] [{level}] {message}";

        _logTextBox.AppendText(logEntry + Environment.NewLine);

        if (_autoScrollCheckBox.Checked)
        {
            _logTextBox.SelectionStart = _logTextBox.Text.Length;
            _logTextBox.ScrollToCaret();
        }
    }

    /// <summary>
    /// Clears all log entries.
    /// </summary>
    public void ClearLog(object? sender, EventArgs e)
    {
        if (InvokeRequired)
        {
            Invoke(ClearLog, sender, e);
            return;
        }

        _logTextBox.Clear();
    }
}