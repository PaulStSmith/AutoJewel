# AutoJewel - Automated Bejeweled 3 Player

An automated Bejeweled 3 player using advanced pattern recognition and image processing, originally created by Wudi and modernized for .NET 9.

## Overview

AutoJewel automatically plays Bejeweled 3 by:
1. **Capturing** screenshots of the game window
2. **Analyzing** jewel colors using HSV color space detection  
3. **Pattern matching** against predefined move templates
4. **Executing** optimal moves via simulated mouse input

## Project History

### Original Version (2011)
- **Author**: Wudi <wudicgi@gmail.com>
- **Blog**: https://blog.wudilabs.com/tag/autojewel
- **License**: GPL v2
- **Platform**: .NET Framework 2.0

### Modernized Version (2025)
- **Modernized by**: Claude Code (Anthropic)
- **Platform**: .NET 9 with Windows Forms
- **License**: GPL v2 (maintained)
- **Key Improvements**: See [Modernization Features](#modernization-features)

## How It Works

### 1. Game Window Capture
- Uses Win32 API to capture the Bejeweled 3 window
- **Requirements**: Game must run in windowed mode (best at 1024×768)
- Supports dynamic scaling for different window sizes

### 2. Color Detection Algorithm
```
For each board position (8×8 grid):
├── Sample center area of jewel (36.6% to 61% of jewel size)
├── Average RGB values across sample area  
├── Convert to HSV (Hue, Saturation, Value)
├── Map to jewel colors based on hue thresholds:
│   ├── Red: >325° hue
│   ├── Purple: 255°-325° hue  
│   ├── Blue: 170°-255° hue
│   ├── Green: 92°-170° hue
│   ├── Yellow: 43°-92° hue
│   ├── Orange: 20°-43° hue
│   └── White: <18% saturation (any hue)
└── Build 8×8 color matrix
```

### 3. Pattern Matching Engine
- **Pattern File**: `patterns.txt` contains 21+ base match templates
- **Pattern Expansion**: Each base pattern generates 12 variations:
  - 4 rotations (0°, 90°, 180°, 270°)  
  - 3 orientations (original, horizontal flip, vertical flip)
- **Binary Matching**: Converts board sections to binary masks for efficient pattern comparison
- **Pattern Format**:
  ```
  ---A    # 4×4 pattern example
  XXXB    # X = required jewel, - = any/empty
  ---X    # A = source position, B = destination  
  ---X    # Complete 3-in-a-row when A moves to B
  ```

### 4. Move Execution
- **Prioritization**: Patterns ranked by jewel count and board position
- **Coordinate Calculation**: Converts board positions to screen coordinates
- **Mouse Simulation**: Drags from source to destination position
- **Timing**: Configurable intervals per game mode (300-1500ms)

## Game Mode Support

| Mode | Timer Interval | Move Duration | Optimizations |
|------|---------------|---------------|---------------|
| **Classic** | 1500ms | 100ms | Prioritizes lower board positions |
| **Zen** | 500ms | 50ms | Standard pattern matching |
| **Lightning** | 300ms | 50ms | High-speed processing |
| **Ice Storm** | 300ms | 50ms | **Primary optimization target** |
| **Balance** | 300ms | 50ms | Different board offset (293,103) |

## Project Structure

```
AutoJewel/
├── AutoJewelModern/          # Modern .NET 9 version (ACTIVE)
│   ├── MainForm.cs           # UI and automation control
│   ├── JewelMatcher.cs       # Pattern recognition engine  
│   ├── Win32Helper.cs        # Windows API integration
│   ├── Logger.cs             # Debug logging system
│   ├── LogWindow.cs          # Debug output window
│   ├── BitmapSaver.cs        # Screenshot debugging utility
│   └── patterns.txt          # Match pattern definitions
├── AutoJewel/                # Legacy .NET Framework 2.0 (REFERENCE)
└── Pattern-Detection.md      # Technical documentation
```

## Modernization Features

### Technical Improvements
- **.NET 9**: Modern runtime with performance optimizations
- **Nullable Reference Types**: Enhanced null safety
- **LibraryImport**: Modern P/Invoke patterns replacing DllImport
- **Records**: Clean data structures for patterns and solutions
- **Async/Await**: Non-blocking file operations and responsive UI
- **LINQ**: Simplified collection operations and pattern processing

### New Debug Features
- **Live Logging**: Real-time debug window with timestamped entries
- **Screenshot Saving**: Automatic bitmap capture to `bitmaps/` folder
- **Board Visualization**: Debug overlays showing detected colors and board grid
- **Pattern Match Logging**: Detailed binary pattern matching information
- **Toggle Control**: Enable/disable debugging via UI checkbox

### Enhanced Error Handling
- **Graceful Failures**: Continues operation on temporary errors
- **Bounds Validation**: Prevents clicks outside game window
- **Process Validation**: Robust game window detection
- **Exception Logging**: Detailed error reporting for troubleshooting

## Usage

### Prerequisites
- Windows 10/11
- .NET 9 Runtime
- Bejeweled 3 (Standard or Trial version)

### Setup
1. Launch Bejeweled 3 in **windowed mode**
2. Set resolution to 1024×768 for optimal results
3. Run `AutoJewelModern.exe`
4. Select game mode and process type
5. *(Optional)* Check "Log & Bitmaps" for debugging

### Controls
- **Ctrl+F8**: Global hotkey to start/stop automation
- **Start/Stop**: UI buttons for automation control
- **Debug Checkbox**: Enable logging and bitmap saving

## Technical Deep Dive

### Pattern Matching Algorithm
```csharp
// For each color (Red, Purple, Blue, Green, Yellow, Orange, White):
foreach (var color in AllColors)
{
    // Create binary mask where 1 = this color, 0 = other
    var colorMask = CreateColorMask(board, color);
    
    // Test all possible 3×3 and 4×4 positions
    for (int row = 0; row <= 8-patternSize; row++)
    {
        for (int col = 0; col <= 8-patternSize; col++)
        {
            var boardBinary = CalculateBinary(colorMask, row, col);
            
            // Test against all loaded patterns
            foreach (var pattern in patterns)
            {
                if ((boardBinary & pattern.Binary) == pattern.Binary)
                {
                    // Pattern match found!
                    solutions.Add(new Solution(row, col, pattern));
                }
            }
        }
    }
}
```

### Performance Optimizations
- **Binary Operations**: Bitwise pattern matching for O(1) comparisons
- **Targeted Sampling**: Only samples jewel center areas for color detection  
- **Efficient Scaling**: Single scaling calculation per capture
- **Memory Management**: Proper disposal of graphics objects
- **Conditional Logging**: Debug operations only when enabled

## Debugging Features

When "Log & Bitmaps" is enabled:
- **`autojewel.log`**: Complete operation log with timestamps
- **`original_capture_*.png`**: Raw game screenshots  
- **`board_analysis_*.png`**: Debug overlays showing:
  - Yellow grid lines marking detected board area
  - Color letters (R/P/B/G/Y/O/W) in each cell
  - Board scaling and positioning information

## License

GPL v2 - Maintains original licensing from Wudi's 2011 implementation.

## Credits

- **Original Development**: Wudi (wudicgi@gmail.com) - 2011
- **Modernization**: Claude Code (Anthropic) - 2025
- **Motivation**: Ice Storm mode optimization and modern .NET compatibility