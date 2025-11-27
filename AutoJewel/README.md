# AutoJewel - Original .NET Framework 2.0 Version

**⚠️ This is the legacy version - See [AutoJewelModern](../AutoJewelModern/) for the current .NET 9 implementation**

## Original Project (2011)

This folder contains the original AutoJewel implementation by Wudi:

- **Author**: Wudi <wudicgi@gmail.com>
- **Created**: 2011
- **Platform**: .NET Framework 2.0
- **License**: GPL v2
- **Blog**: https://blog.wudilabs.com/tag/autojewel

## What This Version Does

Automated Bejeweled 3 player that uses pattern recognition to:
- Capture game window screenshots
- Analyze jewel colors and positions
- Match against predefined patterns
- Execute optimal moves via mouse simulation

## Legacy Codebase Structure

```
AutoJewel/
├── FormMain.cs           # Main application window and UI
├── JewelMatcher.cs       # Core pattern matching engine
├── Win32Helper.cs        # Windows API wrapper functions
├── Program.cs            # Application entry point
├── patterns.txt          # Match pattern definitions
├── FormAbout.cs          # About dialog
├── Properties/           # Assembly info and resources
├── Backup/               # Historical backup files
└── bin/Debug/            # Compiled output directory
```

## Key Features of Original Version

- **Classic .NET Framework 2.0** syntax and patterns
- **Synchronous operations** for file I/O and pattern loading
- **Legacy P/Invoke** with DllImport attributes
- **Timer-based automation** with fixed intervals
- **Basic error handling** with MessageBox alerts

## Differences from Modern Version

| Feature | Legacy (2011) | Modern (2025) |
|---------|---------------|---------------|
| **Runtime** | .NET Framework 2.0 | .NET 9 |
| **Language** | C# 2.0 syntax | C# 12 with modern features |
| **P/Invoke** | DllImport | LibraryImport |
| **File Operations** | Synchronous | Async/await |
| **Error Handling** | Basic try-catch | Comprehensive logging |
| **Debugging** | None | Advanced logging + bitmap saving |
| **Data Structures** | Classes | Records and value types |
| **Collections** | Arrays/Lists | LINQ and modern collections |
| **Null Safety** | None | Nullable reference types |

## Historical Significance

This represents the original automated game-playing approach using:
- **Image processing** with GDI+ for screen capture
- **Color analysis** using RGB-to-HSV conversion
- **Pattern matching** with binary mask algorithms
- **Win32 API integration** for mouse simulation

## Why It Was Modernized

The legacy version served as the foundation for the [AutoJewelModern](../AutoJewelModern/) implementation, which was created to:

1. **Target Ice Storm Mode**: Optimize for faster, high-precision gameplay
2. **Modern .NET**: Leverage .NET 9 performance and language features
3. **Better Debugging**: Add comprehensive logging and visual debugging
4. **Code Maintainability**: Use modern C# patterns and error handling
5. **Enhanced Reliability**: Improve stability and error recovery

## Running the Legacy Version

**Requirements:**
- .NET Framework 2.0 or later
- Windows XP/Vista/7/8/10/11
- Bejeweled 3 in windowed mode

**To compile:**
```bash
# Visual Studio 2005 or later
msbuild AutoJewel.sln

# Or open AutoJewel.sln in Visual Studio
```

**Known Limitations:**
- Limited error reporting
- No debug visualization
- Fixed timing intervals
- Basic pattern matching logging
- Manual pattern file management

## Migration to Modern Version

If you're using this legacy version, consider migrating to [AutoJewelModern](../AutoJewelModern/) for:

- ✅ **Better Performance**: .NET 9 optimizations
- ✅ **Advanced Debugging**: Live logging and bitmap saving
- ✅ **Improved Reliability**: Better error handling and recovery
- ✅ **Modern UI**: Enhanced user interface with debug controls
- ✅ **Ice Storm Optimization**: Specifically tuned for competitive play

## Preservation Notice

This folder is preserved for:
- **Historical reference** of the original implementation
- **Academic study** of early automated game-playing techniques
- **Backup compatibility** with older Windows systems
- **Algorithm comparison** between 2011 and 2025 approaches

---

**For active development and usage, please use [AutoJewelModern](../AutoJewelModern/)**