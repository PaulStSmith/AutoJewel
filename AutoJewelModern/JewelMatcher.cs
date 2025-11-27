using System.Text;

namespace AutoJewelModern;

/// <summary>
/// Provides functionality to detect jewels on a game board from a screenshot, load match patterns,
/// and compute a recommended swap (source/destination points) to produce a match.
/// </summary>
public class JewelMatcher
{
    /// <summary>
    /// Left offset of the game board for Classic mode (reference coordinates).
    /// </summary>
    private const int BOARD_LEFT_CLASSIC = 338;

    /// <summary>
    /// Top offset of the game board for Classic mode (reference coordinates).
    /// </summary>
    private const int BOARD_TOP_CLASSIC = 77;

    /// <summary>
    /// Left offset of the game board for Zen mode (reference coordinates).
    /// </summary>
    private const int BOARD_LEFT_ZEN = 338;

    /// <summary>
    /// Top offset of the game board for Zen mode (reference coordinates).
    /// </summary>
    private const int BOARD_TOP_ZEN = 77;

    /// <summary>
    /// Left offset of the game board for Lightning mode (reference coordinates).
    /// </summary>
    private const int BOARD_LEFT_LIGHTNING = 338;

    /// <summary>
    /// Top offset of the game board for Lightning mode (reference coordinates).
    /// </summary>
    private const int BOARD_TOP_LIGHTNING = 115;

    /// <summary>
    /// Left offset of the game board for Ice Storm mode (reference coordinates).
    /// </summary>
    private const int BOARD_LEFT_ICE_STORM = 338;

    /// <summary>
    /// Top offset of the game board for Ice Storm mode (reference coordinates).
    /// </summary>
    private const int BOARD_TOP_ICE_STORM = 105;

    /// <summary>
    /// Left offset of the game board for Balance mode (reference coordinates).
    /// </summary>
    private const int BOARD_LEFT_BALANCE = 293;

    /// <summary>
    /// Top offset of the game board for Balance mode (reference coordinates).
    /// </summary>
    private const int BOARD_TOP_BALANCE = 103;

    /// <summary>
    /// Number of rows on the game board.
    /// </summary>
    private const int ROW_COUNT = 8;

    /// <summary>
    /// Number of columns on the game board.
    /// </summary>
    private const int COL_COUNT = 8;

    /// <summary>
    /// Reference width of a single jewel (pixels) at reference scaling.
    /// </summary>
    private const int JEWEL_WIDTH = 82;

    /// <summary>
    /// Reference height of a single jewel (pixels) at reference scaling.
    /// </summary>
    private const int JEWEL_HEIGHT = 82;

    /// <summary>
    /// Scaled jewel width in the last analyzed bitmap (updated in GetSolution).
    /// </summary>
    private int _jewelWidth = JEWEL_WIDTH;

    /// <summary>
    /// Scaled jewel height in the last analyzed bitmap (updated in GetSolution).
    /// </summary>
    private int _jewelHeight = JEWEL_HEIGHT;

    /// <summary>
    /// Board left coordinate used as a reference for scaling based on mode.
    /// </summary>
    private int _boardLeftRef = BOARD_LEFT_CLASSIC;

    /// <summary>
    /// Board top coordinate used as a reference for scaling based on mode.
    /// </summary>
    private int _boardTopRef = BOARD_TOP_CLASSIC;

    /// <summary>
    /// Real board left coordinate computed for the last analyzed bitmap.
    /// </summary>
    private int _boardLeftReal = BOARD_LEFT_CLASSIC;

    /// <summary>
    /// Real board top coordinate computed for the last analyzed bitmap.
    /// </summary>
    private int _boardTopReal = BOARD_TOP_CLASSIC;

    /// <summary>
    /// Hue threshold center for red jewels (degrees).
    /// </summary>
    private const int HUE_RED = 350;

    /// <summary>
    /// Hue threshold center for purple jewels (degrees).
    /// </summary>
    private const int HUE_PURPLE = 300;

    /// <summary>
    /// Hue threshold center for blue jewels (degrees).
    /// </summary>
    private const int HUE_BLUE = 210;

    /// <summary>
    /// Hue threshold center for green jewels (degrees).
    /// </summary>
    private const int HUE_GREEN = 130;

    /// <summary>
    /// Hue threshold center for yellow jewels (degrees).
    /// </summary>
    private const int HUE_YELLOW = 55;

    /// <summary>
    /// Hue threshold center for orange jewels (degrees).
    /// </summary>
    private const int HUE_ORANGE = 31;

    /// <summary>
    /// Hue threshold center for white-ish jewels (degrees).
    /// </summary>
    private const int HUE_WHITE = 10;

    /// <summary>
    /// Currently selected game mode which affects board reference offsets and priority heuristics.
    /// </summary>
    private GameMode _mode = GameMode.Classic;

    /// <summary>
    /// Represents the different game modes that affect board placement and heuristics.
    /// </summary>
    public enum GameMode
    {
        /// <summary>
        /// Standard mode / default layout.
        /// </summary>
        Classic,

        /// <summary>
        /// Relaxed layout variant with identical offsets in this implementation.
        /// </summary>
        Zen,

        /// <summary>
        /// Faster/Lightning layout which uses different top offset.
        /// </summary>
        Lightning,

        /// <summary>
        /// Ice Storm layout which uses a different top offset.
        /// </summary>
        IceStorm,

        /// <summary>
        /// Balance layout which may change left/top offsets.
        /// </summary>
        Balance
    }

    /// <summary>
    /// Represents the detectable jewel colors recognized by hue/saturation heuristics.
    /// </summary>
    public enum JewelColor
    {
        /// <summary>Red jewel.</summary>
        Red,

        /// <summary>Purple jewel.</summary>
        Purple,

        /// <summary>Blue jewel.</summary>
        Blue,

        /// <summary>Green jewel.</summary>
        Green,

        /// <summary>Yellow jewel.</summary>
        Yellow,

        /// <summary>Orange jewel.</summary>
        Orange,

        /// <summary>Near-white or low-saturation jewel.</summary>
        White
    }

    /// <summary>
    /// Gets or sets the active <see cref="GameMode"/>. Updating the mode recalculates the
    /// reference board left/top coordinates used for scaling.
    /// </summary>
    public GameMode Mode
    {
        get => _mode;
        set
        {
            _mode = value;
            (_boardLeftRef, _boardTopRef) = value switch
            {
                GameMode.Classic => (BOARD_LEFT_CLASSIC, BOARD_TOP_CLASSIC),
                GameMode.Zen => (BOARD_LEFT_ZEN, BOARD_TOP_ZEN),
                GameMode.Lightning => (BOARD_LEFT_LIGHTNING, BOARD_TOP_LIGHTNING),
                GameMode.IceStorm => (BOARD_LEFT_ICE_STORM, BOARD_TOP_ICE_STORM),
                GameMode.Balance => (BOARD_LEFT_BALANCE, BOARD_TOP_BALANCE),
                _ => (BOARD_LEFT_CLASSIC, BOARD_TOP_CLASSIC)
            };
        }
    }

    /// <summary>
    /// Represents a single match pattern extracted from the pattern file.
    /// </summary>
    /// <param name="SideLength">Side length of the pattern (3 or 4).</param>
    /// <param name="MatchBinary">Binary mask of pattern positions (1 = required cell).</param>
    /// <param name="MoveSrc">Index of the source position ('A') relative to the pattern's flattened array.</param>
    /// <param name="MoveDst">Index of the destination position ('B') relative to the pattern's flattened array.</param>
    /// <param name="Priority">Pattern priority (higher prefers choosing this match).</param>
    public record MatchPattern(int SideLength, int MatchBinary, int MoveSrc, int MoveDst, int Priority);

    /// <summary>
    /// Encapsulates a found match pattern and its top-left location on the board.
    /// </summary>
    /// <param name="Pattern">The matched <see cref="MatchPattern"/> descriptor.</param>
    /// <param name="LeftTopRowIndex">Top row index on the board where the pattern's top-left is placed.</param>
    /// <param name="LeftTopColIndex">Left column index on the board where the pattern's top-left is placed.</param>
    public record MatchMethod(MatchPattern Pattern, int LeftTopRowIndex, int LeftTopColIndex);

    /// <summary>
    /// Represents a proposed swap solution with computed source and destination points and the method used.
    /// </summary>
    /// <param name="PointSrc">Screen coordinate point to click to start the swap.</param>
    /// <param name="PointDst">Screen coordinate point to click to end the swap.</param>
    /// <param name="Method">The <see cref="MatchMethod"/> used to compute this solution.</param>
    public record Solution(Point PointSrc, Point PointDst, MatchMethod Method);

    /// <summary>
    /// Collection of all 3x3 match patterns loaded from the pattern file (after rotation/flip expansion).
    /// </summary>
    private readonly List<MatchPattern> _matchPatterns3x3 = [];

    /// <summary>
    /// Collection of all 4x4 match patterns loaded from the pattern file (after rotation/flip expansion).
    /// </summary>
    private readonly List<MatchPattern> _matchPatterns4x4 = [];

    /// <summary>
    /// Array containing every jewel color; used to iterate over colors when searching matches.
    /// </summary>
    private static readonly JewelColor[] AllColors = [
        JewelColor.Red, JewelColor.Purple, JewelColor.Blue, JewelColor.Green,
        JewelColor.Yellow, JewelColor.Orange, JewelColor.White
    ];

    /// <summary>
    /// Loads pattern definitions from a text file, expands rotations and flips, and populates
    /// the internal 3x3 and 4x4 pattern lists.
    /// </summary>
    /// <param name="filename">Path to the pattern file. Patterns are separated by blank lines and use characters like 'A', 'B', 'X', '-'.</param>
    /// <exception cref="FileNotFoundException">Thrown when the specified <paramref name="filename"/> does not exist.</exception>
    /// <exception cref="InvalidDataException">Thrown when a parsed pattern has an unexpected length (not 9 or 16).</exception>
    public async Task LoadPatternsAsync(string filename)
    {
        Logger.Debug($"Loading patterns from: {filename}");
        
        if (!File.Exists(filename))
        {
            Logger.Error($"Pattern file not found: {filename}");
            throw new FileNotFoundException($"Pattern file not found: {filename}");
        }

        var content = await File.ReadAllTextAsync(filename, Encoding.ASCII);
        var patterns = content.Trim().Replace("\r\n", "\n")
            .Split(new[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);

        Logger.Debug($"Found {patterns.Length} base patterns in file");
        var patternsMet = new HashSet<string>();

        foreach (var pattern in patterns)
        {
            var patternSl = pattern.Replace("\n", "");
            for (int i = 0; i < 3; i++)
            {
                var patternCur = i switch
                {
                    0 => patternSl,
                    1 => FlipPatternH(patternSl),
                    2 => FlipPatternV(patternSl),
                    _ => patternSl
                };

                for (int j = 0; j < 4; j++)
                {
                    if (patternsMet.Add(patternCur))
                    {
                        // Pattern was added (wasn't already present)
                    }
                    patternCur = RotatePattern(patternCur);
                }
            }
        }

        _matchPatterns3x3.Clear();
        _matchPatterns4x4.Clear();

        foreach (var pattern in patternsMet)
        {
            var binaryStr = pattern.Replace('-', '0').Replace('B', '0').Replace('X', '1').Replace('A', '1');
            var binary = Convert.ToInt32(binaryStr, 2);
            var priority = binaryStr.Count(c => c == '1');
            var moveSrc = pattern.IndexOf('A');
            var moveDst = pattern.IndexOf('B');

            var matchPattern = new MatchPattern(
                pattern.Length == 9 ? 3 : 4,
                binary, moveSrc, moveDst, priority);

            if (pattern.Length == 9)
                _matchPatterns3x3.Add(matchPattern);
            else if (pattern.Length == 16)
                _matchPatterns4x4.Add(matchPattern);
            else
            {
                Logger.Error($"Invalid pattern length: {pattern.Length} for pattern: {pattern}");
                throw new InvalidDataException($"Invalid pattern length: {pattern}");
            }
        }
        
        Logger.Success($"Loaded {_matchPatterns3x3.Count} 3x3 patterns and {_matchPatterns4x4.Count} 4x4 patterns");
        
        // Log a few example patterns for debugging
        if (_matchPatterns3x3.Count > 0)
        {
            var examplePattern = _matchPatterns3x3[0];
            Logger.Debug($"Example 3x3 pattern: Binary={Convert.ToString(examplePattern.MatchBinary, 2).PadLeft(9, '0')}, Priority={examplePattern.Priority}, Src={examplePattern.MoveSrc}, Dst={examplePattern.MoveDst}");
        }
    }

    /// <summary>
    /// Rotates a flattened pattern string 90 degrees clockwise.
    /// </summary>
    /// <param name="patternSl">A pattern string of length 9 (3x3) or 16 (4x4) representing cells row-major.</param>
    /// <returns>The rotated pattern string with the same length.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="patternSl"/> has an unexpected length.</exception>
    private static string RotatePattern(string patternSl)
    {
        return patternSl.Length switch
        {
            9 => new string([
                patternSl[2], patternSl[5], patternSl[8],
                patternSl[1], patternSl[4], patternSl[7],
                patternSl[0], patternSl[3], patternSl[6]
            ]),
            16 => new string([
                patternSl[3], patternSl[7], patternSl[11], patternSl[15],
                patternSl[2], patternSl[6], patternSl[10], patternSl[14],
                patternSl[1], patternSl[5], patternSl[9], patternSl[13],
                patternSl[0], patternSl[4], patternSl[8], patternSl[12]
            ]),
            _ => throw new ArgumentException($"Invalid pattern length: {patternSl}")
        };
    }

    /// <summary>
    /// Flips a flattened pattern horizontally (mirror across vertical axis).
    /// </summary>
    /// <param name="patternSl">A pattern string of length 9 (3x3) or 16 (4x4) representing cells row-major.</param>
    /// <returns>The horizontally flipped pattern string.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="patternSl"/> has an unexpected length.</exception>
    private static string FlipPatternH(string patternSl)
    {
        return patternSl.Length switch
        {
            9 => new string([
                patternSl[6], patternSl[7], patternSl[8],
                patternSl[3], patternSl[4], patternSl[5],
                patternSl[0], patternSl[1], patternSl[2]
            ]),
            16 => new string([
                patternSl[12], patternSl[13], patternSl[14], patternSl[15],
                patternSl[8], patternSl[9], patternSl[10], patternSl[11],
                patternSl[4], patternSl[5], patternSl[6], patternSl[7],
                patternSl[0], patternSl[1], patternSl[2], patternSl[3]
            ]),
            _ => throw new ArgumentException($"Invalid pattern length: {patternSl}")
        };
    }

    /// <summary>
    /// Flips a flattened pattern vertically (mirror across horizontal axis).
    /// </summary>
    /// <param name="patternSl">A pattern string of length 9 (3x3) or 16 (4x4) representing cells row-major.</param>
    /// <returns>The vertically flipped pattern string.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="patternSl"/> has an unexpected length.</exception>
    private static string FlipPatternV(string patternSl)
    {
        return patternSl.Length switch
        {
            9 => new string([
                patternSl[2], patternSl[1], patternSl[0],
                patternSl[5], patternSl[4], patternSl[3],
                patternSl[8], patternSl[7], patternSl[6]
            ]),
            16 => new string([
                patternSl[3], patternSl[2], patternSl[1], patternSl[0],
                patternSl[7], patternSl[6], patternSl[5], patternSl[4],
                patternSl[11], patternSl[10], patternSl[9], patternSl[8],
                patternSl[15], patternSl[14], patternSl[13], patternSl[12]
            ]),
            _ => throw new ArgumentException($"Invalid pattern length: {patternSl}")
        };
    }

    /// <summary>
    /// Analyzes the provided <see cref="Bitmap"/> to determine jewel colors in each board cell and
    /// computes a recommended swap solution if any match patterns apply.
    /// </summary>
    /// <param name="bitmap">A screenshot of the game board. The method computes scaling based on the bitmap width/height and the current mode's reference offsets.</param>
    /// <returns>
    /// A <see cref="Solution"/> describing the source and destination screen points to swap, or null if no solution was found.
    /// </returns>
    public Solution? GetSolution(Bitmap bitmap)
    {
        // Save the original captured bitmap for debugging
        // BitmapSaver.SaveBitmap(bitmap, "original_capture");
        
        var factor = (float)(bitmap.Width - 8) / 1024f;
        Logger.LogBoardAnalysis(_mode.ToString(), bitmap.Width, bitmap.Height, factor);

        _jewelWidth = (int)Math.Round(factor * JEWEL_WIDTH);
        _jewelHeight = _jewelWidth;
        _boardLeftReal = (int)Math.Round(factor * (_boardLeftRef - 4)) + 4;
        _boardTopReal = bitmap.Height - ((int)Math.Round(factor * (802 - _boardTopRef - 4)) + 4);
        
        Logger.Debug($"Board dimensions - Left: {_boardLeftReal}, Top: {_boardTopReal}, Jewel size: {_jewelWidth}x{_jewelHeight}");

        var jewels = new JewelColor[ROW_COUNT, COL_COUNT];
        var aMin = (int)Math.Round(_jewelWidth * 0.366);
        var aMax = (int)Math.Round(_jewelWidth * 0.610);

        for (int i = 0; i < ROW_COUNT; i++)
        {
            int y = _boardTopReal + (_jewelHeight * i);
            for (int j = 0; j < COL_COUNT; j++)
            {
                int x = _boardLeftReal + (_jewelWidth * j);

                int colorR = 0, colorG = 0, colorB = 0, colorCount = 0;

                for (int a = aMin; a < aMax; a++)
                {
                    for (int b = aMin; b < aMax; b++)
                    {
                        var pixel = bitmap.GetPixel(x + a, y + b);
                        colorR += pixel.R;
                        colorG += pixel.G;
                        colorB += pixel.B;
                        colorCount++;
                    }
                }

                colorR /= colorCount;
                colorG /= colorCount;
                colorB /= colorCount;

                // Normalize near-white colors
                if (Math.Abs(colorG / (float)colorR - 1) < 0.15f &&
                    Math.Abs(colorB / (float)colorR - 1) < 0.15f)
                {
                    colorG = colorR;
                    colorB = colorR;
                }

                var colorAvg = Color.FromArgb(colorR, colorG, colorB);
                var hue = (int)colorAvg.GetHue();
                var sat = (int)(colorAvg.GetSaturation() * 1000);

                jewels[i, j] = GetColorFromHueSat(hue, sat);
                
                // Log color detection for first few cells or if debugging specific issues
                if ((i < 2 && j < 2) || Logger.IsEnabled)
                {
                    Logger.LogColorDetection(i, j, jewels[i, j].ToString(), hue, sat);
                }
            }
        }

        // Create a debug bitmap showing the detected board area
        CreateDebugBoardBitmap(bitmap, jewels);
        
        Logger.Debug("Color detection completed, processing jewel patterns");
        return ProcessAllJewels(jewels);
    }

    /// <summary>
    /// Maps a hue and saturation sample to the nearest <see cref="JewelColor"/> using predefined thresholds.
    /// Low saturation values map to <see cref="JewelColor.White"/>.
    /// </summary>
    /// <param name="hue">Hue in degrees (0-360).</param>
    /// <param name="sat">Saturation scaled by 1000 (as used in this code).</param>
    /// <returns>The best matching <see cref="JewelColor"/>.</returns>
    private static JewelColor GetColorFromHueSat(int hue, int sat)
    {
        if (sat < 180)
            return JewelColor.White;

        return hue switch
        {
            > (HUE_RED + HUE_PURPLE) / 2 => JewelColor.Red,
            > (HUE_PURPLE + HUE_BLUE) / 2 => JewelColor.Purple,
            > (HUE_BLUE + HUE_GREEN) / 2 => JewelColor.Blue,
            > (HUE_GREEN + HUE_YELLOW) / 2 => JewelColor.Green,
            > (HUE_YELLOW + HUE_ORANGE) / 2 => JewelColor.Yellow,
            > (HUE_ORANGE + HUE_WHITE) / 2 => JewelColor.Orange,
            _ => JewelColor.White
        };
    }

    /// <summary>
    /// Processes a full board of <see cref="JewelColor"/> values and searches for matching patterns for all colors.
    /// It ranks found methods and returns the best computed swap as a <see cref="Solution"/> or null if none found.
    /// </summary>
    /// <param name="jewels">2D array of detected jewel colors sized [ROW_COUNT, COL_COUNT].</param>
    /// <returns>A chosen <see cref="Solution"/> or null when no valid match is discovered.</returns>
    private Solution? ProcessAllJewels(JewelColor[,] jewels)
    {
        var methods = new List<MatchMethod>();
        Logger.Debug("Starting pattern matching for all colors");

        foreach (var color in AllColors)
        {
            var methodCountBefore = methods.Count;
            var colorJewels = GetSpecColorJewels(jewels, color);
            
            // Count how many jewels of this color exist
            int colorCount = 0;
            for (int i = 0; i < ROW_COUNT; i++)
            {
                for (int j = 0; j < COL_COUNT; j++)
                {
                    if (colorJewels[i, j] == 1) colorCount++;
                }
            }
            
            ProcessBoolJewels(colorJewels, methods);
            var newMethods = methods.Count - methodCountBefore;
            
            if (colorCount > 0)
            {
                Logger.Debug($"Color {color}: {colorCount} jewels on board, {newMethods} patterns found");
            }
        }

        if (methods.Count == 0)
        {
            Logger.Debug("No valid patterns found on board");
            return null;
        }

        var random = new Random();

        // Add randomization and mode-specific priority adjustments
        var methodsWithPriority = methods.Select(method =>
        {
            var priority = method.Pattern.Priority;// + (int)Math.Round(random.NextDouble());
            if (_mode == GameMode.Classic)
                priority += 2 * (ROW_COUNT - method.LeftTopRowIndex);

            return new { Method = method, Priority = priority };
        })
        .OrderByDescending(x => x.Priority)
        .ToList();

        var bestMethod = methodsWithPriority[0].Method;
        var bestPriority = methodsWithPriority[0].Priority;
        var sideLen = bestMethod.Pattern.SideLength;
        
        Logger.Info($"Best pattern selected: {sideLen}x{sideLen} pattern at ({bestMethod.LeftTopRowIndex},{bestMethod.LeftTopColIndex}) with priority {bestPriority}");

        var pointSrc = new Point(
            _boardLeftReal + (_jewelWidth / 2) +
            ((bestMethod.LeftTopColIndex + (bestMethod.Pattern.MoveSrc % sideLen)) * _jewelWidth),
            _boardTopReal + (_jewelWidth / 2) +
            ((bestMethod.LeftTopRowIndex + (bestMethod.Pattern.MoveSrc / sideLen)) * _jewelHeight)
        );

        var pointDst = new Point(
            _boardLeftReal + (_jewelWidth / 2) +
            ((bestMethod.LeftTopColIndex + (bestMethod.Pattern.MoveDst % sideLen)) * _jewelWidth),
            _boardTopReal + (_jewelWidth / 2) +
            ((bestMethod.LeftTopRowIndex + (bestMethod.Pattern.MoveDst / sideLen)) * _jewelHeight)
        );
        
        Logger.Debug($"Calculated relative coordinates - Source: ({pointSrc.X},{pointSrc.Y}), Destination: ({pointDst.X},{pointDst.Y})");
        return new Solution(pointSrc, pointDst, bestMethod);
    }

    /// <summary>
    /// Scans the provided boolean mask (1 = matching color, 0 = other) for all positions where 3x3 and 4x4 patterns can be applied,
    /// and adds discovered <see cref="MatchMethod"/> instances to the provided collection.
    /// </summary>
    /// <param name="jewels">An int[,] mask with values 1 for cells of interest and 0 otherwise.</param>
    /// <param name="methods">List to populate with discovered match methods.</param>
    private void ProcessBoolJewels(int[,] jewels, List<MatchMethod> methods)
    {
        var initialCount = methods.Count;
        
        for (int i = 0; i < ROW_COUNT; i++)
        {
            for (int j = 0; j < COL_COUNT; j++)
            {
                // Check 4x4 patterns
                if (i <= ROW_COUNT - 4 && j <= COL_COUNT - 4)
                {
                    var binary = CalculateBinary4x4(jewels, i, j);
                    foreach (var pattern in _matchPatterns4x4)
                    {
                        if ((binary & pattern.MatchBinary) == pattern.MatchBinary)
                        {
                            methods.Add(new MatchMethod(pattern, i, j));
                            Logger.Debug($"Match found at ({i},{j}): board binary={Convert.ToString(binary, 2).PadLeft(9, '0')}, pattern binary={Convert.ToString(pattern.MatchBinary, 2).PadLeft(9, '0')}");
                        }
                    }
                }

                // Check 3x3 patterns
                if (i <= ROW_COUNT - 3 && j <= COL_COUNT - 3)
                {
                    var binary = CalculateBinary3x3(jewels, i, j);
                    foreach (var pattern in _matchPatterns3x3)
                    {
                        if ((binary & pattern.MatchBinary) == pattern.MatchBinary)
                        {
                            methods.Add(new MatchMethod(pattern, i, j));
                            Logger.Debug($"Match found at ({i},{j}): board binary={Convert.ToString(binary, 2).PadLeft(9, '0')}, pattern binary={Convert.ToString(pattern.MatchBinary, 2).PadLeft(9, '0')}");
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Calculates a 16-bit binary representation of the 4x4 block starting at <paramref name="startRow"/>, <paramref name="startCol"/>.
    /// Bit 15 corresponds to the top-left cell, bit 0 to the bottom-right cell. Each cell's value (0/1) is placed into the bit.
    /// </summary>
    /// <param name="jewels">Mask array with values 0 or 1.</param>
    /// <param name="startRow">Top row index of the 4x4 block.</param>
    /// <param name="startCol">Left column index of the 4x4 block.</param>
    /// <returns>An integer encoding the 4x4 block as bits.</returns>
    private static int CalculateBinary4x4(int[,] jewels, int startRow, int startCol)
    {
        var binary = 0;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                binary |= jewels[startRow + i, startCol + j] << (15 - (i * 4 + j));
            }
        }
        return binary;
    }

    /// <summary>
    /// Calculates a 9-bit binary representation of the 3x3 block starting at <paramref name="startRow"/>, <paramref name="startCol"/>.
    /// Bit 8 corresponds to the top-left cell, bit 0 to the bottom-right cell. Each cell's value (0/1) is placed into the bit.
    /// </summary>
    /// <param name="jewels">Mask array with values 0 or 1.</param>
    /// <param name="startRow">Top row index of the 3x3 block.</param>
    /// <param name="startCol">Left column index of the 3x3 block.</param>
    /// <returns>An integer encoding the 3x3 block as bits.</returns>
    private static int CalculateBinary3x3(int[,] jewels, int startRow, int startCol)
    {
        var binary = 0;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                binary |= jewels[startRow + i, startCol + j] << (8 - (i * 3 + j));
            }
        }
        return binary;
    }

    /// <summary>
    /// Produces an int[,] binary mask where cells equal to the specified <paramref name="color"/> are 1 and others 0.
    /// </summary>
    /// <param name="jewels">2D array of <see cref="JewelColor"/> values sized [ROW_COUNT, COL_COUNT].</param>
    /// <param name="color">The color to produce the mask for.</param>
    /// <returns>An int[ROW_COUNT, COL_COUNT] mask.</returns>
    private static int[,] GetSpecColorJewels(JewelColor[,] jewels, JewelColor color)
    {
        var result = new int[ROW_COUNT, COL_COUNT];
        for (int i = 0; i < ROW_COUNT; i++)
        {
            for (int j = 0; j < COL_COUNT; j++)
            {
                result[i, j] = jewels[i, j] == color ? 1 : 0;
            }
        }
        return result;
    }

    /// <summary>
    /// Creates a debug bitmap showing the detected board area and jewel colors for debugging purposes.
    /// </summary>
    /// <param name="originalBitmap">The original captured bitmap.</param>
    /// <param name="jewels">The detected jewel color array.</param>
    private void CreateDebugBoardBitmap(Bitmap originalBitmap, JewelColor[,] jewels)
    {
        if (!BitmapSaver.IsEnabled) return;

        try
        {
            using var debugBitmap = new Bitmap(originalBitmap.Width, originalBitmap.Height);
            using var g = Graphics.FromImage(debugBitmap);
            
            // Draw the original bitmap
            g.DrawImage(originalBitmap, 0, 0);
            
            // Draw board grid overlay
            using var gridPen = new Pen(Color.Yellow, 2);
            using var textBrush = new SolidBrush(Color.White);
            using var font = new Font("Arial", 8, FontStyle.Bold);
            
            // Draw vertical grid lines
            for (int j = 0; j <= COL_COUNT; j++)
            {
                int x = _boardLeftReal + (j * _jewelWidth);
                g.DrawLine(gridPen, x, _boardTopReal, x, _boardTopReal + (ROW_COUNT * _jewelHeight));
            }
            
            // Draw horizontal grid lines
            for (int i = 0; i <= ROW_COUNT; i++)
            {
                int y = _boardTopReal + (i * _jewelHeight);
                g.DrawLine(gridPen, _boardLeftReal, y, _boardLeftReal + (COL_COUNT * _jewelWidth), y);
            }
            
            // Draw detected colors in each cell
            for (int i = 0; i < ROW_COUNT; i++)
            {
                for (int j = 0; j < COL_COUNT; j++)
                {
                    int x = _boardLeftReal + (j * _jewelWidth);
                    int y = _boardTopReal + (i * _jewelHeight);
                    
                    var color = jewels[i, j];
                    var colorText = color.ToString().Substring(0, 1); // First letter of color
                    var textBounds = g.MeasureString(colorText, font);
                    
                    var textX = x + (_jewelWidth - textBounds.Width) / 2;
                    var textY = y + (_jewelHeight - textBounds.Height) / 2;
                    
                    g.DrawString(colorText, font, textBrush, textX, textY);
                }
            }
            
            BitmapSaver.SaveBitmapWithName(debugBitmap, $"board_analysis_{_mode}_{DateTime.Now:yyyyMMddHHmmssffff}");
            Logger.Debug("Saved debug board analysis bitmap");
        }
        catch (Exception ex)
        {
            Logger.Error("Failed to create debug board bitmap", ex);
        }
    }
}