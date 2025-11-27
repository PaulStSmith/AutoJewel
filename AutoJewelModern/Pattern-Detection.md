# AutoJewel Pattern Detection System

This document explains how AutoJewel uses the patterns stored in the TXT file to detect combinations on screen and requirements for game window mode.

## Overview

AutoJewel Modern analyzes screenshots of the Bejeweled 3 game board to:
1. **Color Detection**: Identify jewel colors at each board position using HSV color analysis
2. **Pattern Matching**: Compare the detected board state against predefined match patterns
3. **Move Calculation**: Generate optimal swap moves based on priority algorithms

## Window Mode Requirements

### **Windowed Mode Required**
The application **requires windowed mode** for proper operation. Based on the about dialog (`MainForm.cs:358`):

> "The program works best in windowed mode at 1024x768 resolution."

### Why Windowed Mode?
- **Window Capture**: Uses `Win32Helper.CaptureWindow()` to capture specific game window (`MainForm.cs:132`)
- **Coordinate Mapping**: Calculates mouse coordinates relative to window position (`MainForm.cs:148-156`)
- **Scaling Detection**: Determines board scaling based on window dimensions (`JewelMatcher.cs:427`)

### Scaling Support
The system supports different window sizes through dynamic scaling:
- **Reference Resolution**: Based on 1024px width (`JewelMatcher.cs:427`)
- **Scaling Factor**: `(bitmap.Width - 8) / 1024f`
- **Adaptive Coordinates**: All board positions scale proportionally

## Pattern File Format

The `patterns.txt` file defines match patterns using a text-based format:

### Pattern Symbols
- **`X`**: Required jewel of matching color
- **`-`**: Any jewel or empty space (ignored)
- **`A`**: Source position for swap move
- **`B`**: Destination position for swap move

### Pattern Sizes
- **3x3 patterns**: 9-character strings (3 rows × 3 columns)
- **4x4 patterns**: 16-character strings (4 rows × 4 columns)

### Example Patterns
```
X--    # 3x3: Horizontal line with gap
X--    #      A moves to B to complete line
BA-

XXBA   # 4x4: Longer horizontal pattern  
--X-   #      More complex matching
--X-
--X-
```

## Color Detection Process

### HSV Color Analysis (`JewelMatcher.cs:472-476`)

1. **Pixel Sampling**: Samples center area of each jewel cell (36.6% to 61% of jewel size)
2. **Color Averaging**: Averages RGB values across sample area
3. **HSV Conversion**: Converts to Hue/Saturation/Value for robust color matching
4. **Color Classification**: Maps to 7 jewel colors based on hue thresholds

### Color Mapping (`JewelMatcher.cs:489-503`)
| Color | Hue Range | Saturation Threshold |
|-------|-----------|---------------------|
| Red | >325° | >18% |
| Purple | 255°-325° | >18% |
| Blue | 170°-255° | >18% |
| Green | 92°-170° | >18% |
| Yellow | 43°-92° | >18% |
| Orange | 20°-43° | >18% |
| White | Any | <18% |

## Pattern Matching Algorithm

### 1. Pattern Loading (`JewelMatcher.cs:283-340`)
- Reads patterns from file separated by blank lines
- **Expands patterns** through rotations and flips:
  - 4 rotations (0°, 90°, 180°, 270°)
  - 3 flip variations (original, horizontal, vertical)
  - Total: 12 variations per base pattern
- Converts to binary masks for efficient matching

### 2. Board Analysis (`JewelMatcher.cs:512-556`)
- **Per-Color Matching**: Tests each jewel color separately
- **Binary Conversion**: Converts board regions to binary masks
- **Pattern Testing**: Uses bitwise operations for fast matching:
  ```csharp
  if ((binary & pattern.MatchBinary) == pattern.MatchBinary)
  ```

### 3. Move Prioritization (`JewelMatcher.cs:527-536`)
- **Base Priority**: Number of jewels in pattern
- **Random Factor**: Adds randomization to prevent repetitive moves
- **Mode-Specific**: Classic mode prefers lower board positions
- **Best Selection**: Chooses highest priority valid match

## Game Mode Adaptations

Different game modes have specific board offsets (`JewelMatcher.cs:220-228`):

| Mode | Left Offset | Top Offset |
|------|-------------|------------|
| Classic | 338px | 77px |
| Zen | 338px | 77px |
| Lightning | 338px | 115px |
| Ice Storm | 338px | 105px |
| Balance | 293px | 103px |

### Ice Storm Optimization
- **Faster Intervals**: 300ms vs 1500ms for Classic mode (`MainForm.cs:34-38`)
- **Quick Move Time**: 50ms mouse drag vs 100ms for Classic
- **High-Speed Processing**: Optimized for rapid gameplay requirements

## Technical Implementation

### Binary Pattern Encoding
Patterns are encoded as integers for efficient bitwise operations:
- **3x3 patterns**: 9-bit integers (positions 0-8)
- **4x4 patterns**: 16-bit integers (positions 0-15)
- **Bit ordering**: Left-to-right, top-to-bottom

### Move Coordinate Calculation
Final screen coordinates computed from:
1. **Board Position**: Scaled board offset + jewel grid position
2. **Window Offset**: Added to convert to screen coordinates  
3. **Centering**: Adjusted to jewel center for accurate clicking

### Performance Optimizations
- **Async Pattern Loading**: Non-blocking file I/O (`JewelMatcher.cs:283`)
- **Binary Operations**: Fast bitwise pattern matching
- **Targeted Sampling**: Efficient pixel sampling for color detection
- **Priority Caching**: Pre-calculated pattern priorities

## Error Handling

The system includes robust error handling:
- **Window Validation**: Ensures target game window exists
- **Coordinate Bounds**: Validates moves are within window
- **Pattern Validation**: Verifies loaded patterns are valid sizes
- **Graceful Degradation**: Continues operation if individual moves fail