using System.Drawing.Imaging;

namespace AutoJewelModern;

/// <summary>
/// Utility class for saving captured bitmaps to disk for debugging purposes.
/// </summary>
public static class BitmapSaver
{
    private static readonly object _lock = new object();
    private static bool _isEnabled = false;
    private static string _bitmapFolder = "bitmaps";

    /// <summary>
    /// Gets or sets whether bitmap saving is enabled.
    /// </summary>
    public static bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            _isEnabled = value;
            if (value)
            {
                EnsureBitmapDirectoryExists();
            }
        }
    }

    /// <summary>
    /// Gets or sets the folder where bitmaps will be saved relative to the executable.
    /// </summary>
    public static string BitmapFolder
    {
        get => _bitmapFolder;
        set
        {
            _bitmapFolder = value ?? "bitmaps";
            if (_isEnabled)
            {
                EnsureBitmapDirectoryExists();
            }
        }
    }

    /// <summary>
    /// Saves a bitmap with a timestamped filename if saving is enabled.
    /// </summary>
    /// <param name="bitmap">The bitmap to save.</param>
    /// <param name="prefix">Optional prefix for the filename (e.g., "capture", "pattern").</param>
    /// <returns>The full path where the bitmap was saved, or null if saving is disabled or failed.</returns>
    public static string? SaveBitmap(Bitmap bitmap, string prefix = "capture")
    {
        if (!_isEnabled || bitmap == null)
            return null;

        lock (_lock)
        {
            try
            {
                EnsureBitmapDirectoryExists();

                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
                var filename = $"{prefix}_{timestamp}.png";
                var fullPath = Path.Combine(GetBitmapDirectoryPath(), filename);

                bitmap.Save(fullPath, ImageFormat.Png);
                Logger.Debug($"Saved bitmap: {filename} ({bitmap.Width}x{bitmap.Height})");
                
                return fullPath;
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to save bitmap with prefix '{prefix}'", ex);
                return null;
            }
        }
    }

    /// <summary>
    /// Saves a bitmap with a custom filename if saving is enabled.
    /// </summary>
    /// <param name="bitmap">The bitmap to save.</param>
    /// <param name="filename">The filename to use (without extension).</param>
    /// <returns>The full path where the bitmap was saved, or null if saving is disabled or failed.</returns>
    public static string? SaveBitmapWithName(Bitmap bitmap, string filename)
    {
        if (!_isEnabled || bitmap == null)
            return null;

        lock (_lock)
        {
            try
            {
                EnsureBitmapDirectoryExists();

                var fullFilename = filename.EndsWith(".png") ? filename : $"{filename}.png";
                var fullPath = Path.Combine(GetBitmapDirectoryPath(), fullFilename);

                bitmap.Save(fullPath, ImageFormat.Png);
                Logger.Debug($"Saved bitmap: {fullFilename} ({bitmap.Width}x{bitmap.Height})");
                
                return fullPath;
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to save bitmap with filename '{filename}'", ex);
                return null;
            }
        }
    }

    /// <summary>
    /// Clears all bitmap files from the bitmap directory.
    /// </summary>
    /// <returns>The number of files deleted.</returns>
    public static int ClearBitmaps()
    {
        lock (_lock)
        {
            try
            {
                var bitmapDir = GetBitmapDirectoryPath();
                if (!Directory.Exists(bitmapDir))
                    return 0;

                var pngFiles = Directory.GetFiles(bitmapDir, "*.png");
                var deletedCount = 0;

                foreach (var file in pngFiles)
                {
                    try
                    {
                        File.Delete(file);
                        deletedCount++;
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn($"Failed to delete bitmap file {Path.GetFileName(file)}: {ex.Message}");
                    }
                }

                Logger.Info($"Cleared {deletedCount} bitmap files from {_bitmapFolder} folder");
                return deletedCount;
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to clear bitmap directory", ex);
                return 0;
            }
        }
    }

    /// <summary>
    /// Gets the full path to the bitmap directory.
    /// </summary>
    /// <returns>The full path to the bitmap directory.</returns>
    private static string GetBitmapDirectoryPath()
    {
        var exeDir = Path.GetDirectoryName(AppContext.BaseDirectory) ?? Environment.CurrentDirectory;
        return Path.Combine(exeDir, _bitmapFolder);
    }

    /// <summary>
    /// Ensures the bitmap directory exists, creating it if necessary.
    /// </summary>
    private static void EnsureBitmapDirectoryExists()
    {
        try
        {
            var bitmapDir = GetBitmapDirectoryPath();
            if (!Directory.Exists(bitmapDir))
            {
                Directory.CreateDirectory(bitmapDir);
                Logger.Info($"Created bitmap directory: {bitmapDir}");
            }
        }
        catch (Exception ex)
        {
            Logger.Error("Failed to create bitmap directory", ex);
        }
    }

    /// <summary>
    /// Gets information about the bitmap directory.
    /// </summary>
    /// <returns>A tuple containing directory path, existence status, and file count.</returns>
    public static (string Path, bool Exists, int FileCount) GetDirectoryInfo()
    {
        var path = GetBitmapDirectoryPath();
        var exists = Directory.Exists(path);
        var fileCount = 0;

        if (exists)
        {
            try
            {
                fileCount = Directory.GetFiles(path, "*.png").Length;
            }
            catch
            {
                fileCount = -1; // Indicates error reading directory
            }
        }

        return (path, exists, fileCount);
    }
}