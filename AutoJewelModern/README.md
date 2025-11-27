# AutoJewel - Modernized

An automated Bejeweled 3 player using pattern recognition, modernized for .NET 9.

## Features

- **Automatic gameplay** for all Bejeweled 3 game modes
- **Pattern recognition** using advanced image processing
- **Optimized for Ice Storm mode** - the original reason for this modernization
- **Global hotkey support** (Ctrl+F8) for easy start/stop
- **Modern .NET 9** implementation with improved performance
- **Clean, maintainable code** with modern C# features

## Game Modes Supported

- Classic
- Zen  
- Lightning
- Ice Storm (optimized)
- Balance

## Requirements

- Windows 10/11
- .NET 9 Runtime
- Bejeweled 3 (Standard or Trial version)
- Game running in windowed mode at 1024x768 resolution for best results

## Usage

1. Start Bejeweled 3
2. Launch AutoJewel
3. Select your game mode and process type
4. Press **Ctrl+F8** or click **Start** to begin automation
5. Press **Ctrl+F8** or click **Stop** to end automation

## Building from Source

```bash
cd AutoJewelModern
dotnet build
```

## Original Credits

- **Original Author**: Wudi <wudicgi@gmail.com>
- **Original License**: GPL v2
- **Original Blog**: https://blog.wudilabs.com/tag/autojewel
- **Modernized**: 2025 by Claude Code

## License

This modernized version maintains the original GPL v2 license from the original work.

## Technical Notes

### Modernization Improvements

- **LibraryImport**: Modern P/Invoke patterns for Windows API calls
- **Records**: Pattern matching and solution data structures
- **Nullable reference types**: Better null safety
- **Modern async/await**: Improved file I/O operations
- **LINQ**: Cleaner collection operations
- **Switch expressions**: More concise pattern matching

### Architecture

- `JewelMatcher`: Core pattern recognition and game logic
- `Win32Helper`: Windows API integration for screen capture and input
- `MainForm`: Modern Windows Forms UI with improved UX
- `patterns.txt`: Configurable match patterns for different jewel arrangements

### Performance

The modernized version includes several performance improvements:
- More efficient memory usage with modern collection types
- Better exception handling to prevent interruptions
- Optimized image processing algorithms
- Reduced garbage collection pressure

## Ice Storm Mode

This modernization was specifically motivated by the difficulty of Ice Storm mode. The pattern recognition has been tuned to work well with the faster-paced gameplay and special mechanics of this mode.