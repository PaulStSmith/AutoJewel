# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

AutoJewel is a C# Windows Forms application designed to automatically play the Bejeweled 3 game. It uses image processing and pattern matching to analyze the game board and execute optimal moves. The project has been modernized from a legacy .NET Framework 2.0 application to .NET 9, with improved performance and maintainability.

### Project Structure

- **AutoJewelModern/**: Current modern .NET 9 implementation (actively maintained)
- **AutoJewel/**: Legacy .NET Framework 2.0 version (preserved for reference)
- **AutoJewel/Backup/**: Historical backup of original codebase

## Build Commands

### Modern .NET 9 Version (AutoJewelModern)
- **Build**: Use .NET CLI or Visual Studio to build the modern version
  ```
  cd AutoJewelModern
  dotnet build
  ```
- **Run**: Execute directly with dotnet or compiled executable
  ```
  dotnet run
  # or
  .\bin\Debug\net9.0-windows\AutoJewelModern.exe
  ```
- **Release Build**: Build optimized release version
  ```
  dotnet build --configuration Release
  ```

### Legacy Version (AutoJewel)
- **Build Solution**: Use MSBuild for legacy .NET Framework version
  ```
  msbuild AutoJewel.sln
  ```
- **Run**: Execute `AutoJewel.exe` from `bin\Debug\` or `bin\Release\` directory

## Architecture Overview

### Core Components (Modern Version)

1. **MainForm.cs** (`AutoJewelModern/MainForm.cs:6`): Modern main application window with improved UX
   - Manages game modes with optimized settings dictionary (`MainForm.cs:12`)
   - Enhanced async/await pattern for better responsiveness (`MainForm.cs:34`)
   - Global hotkey support (Ctrl+F8) for automation control (`MainForm.cs:44`)
   - Modern Windows Forms with nullable reference types

2. **JewelMatcher.cs** (`AutoJewelModern/JewelMatcher.cs`): Modernized pattern matching engine
   - **Async Pattern Loading**: Modern async file I/O operations
   - **Records & LINQ**: Clean pattern matching using modern C# features
   - **Performance Optimizations**: Reduced GC pressure and efficient algorithms
   - **Ice Storm Mode**: Specifically optimized for high-speed gameplay

3. **Win32Helper.cs** (`AutoJewelModern/Win32Helper.cs`): Modern Windows API integration
   - **LibraryImport**: Modern P/Invoke patterns replacing DllImport
   - **Better Error Handling**: Improved exception handling for API calls
   - **Memory Safety**: Enhanced pointer safety with modern C# features

4. **Program.cs** (`AutoJewelModern/Program.cs`): Modern application entry point
   - Modern top-level statements and nullable context
   - Improved application initialization patterns

### Key Design Patterns

- **Strategy Pattern**: Game mode handling with dictionary-based settings (`MainForm.cs:12`)
- **Template Method**: Pattern matching with configurable algorithms
- **Modern Async**: Async/await throughout for better performance
- **Records & Value Types**: Efficient data structures for pattern matching
- **Facade Pattern**: Win32Helper simplifies Windows API complexity

### External Dependencies

- **Modern Version**: 
  - `patterns.txt` file for match patterns (async loaded at `MainForm.cs:38`)
  - .NET 9 runtime and Windows Forms
  - Targets `Bejeweled3.exe` or `popcapgame1.exe` processes

- **Legacy Version**: 
  - .NET Framework 2.0 runtime
  - Windows GDI+ for graphics operations

## Development Notes

### Modern Version Features

- **.NET 9** with modern C# language features
- **Nullable reference types** for better null safety
- **LibraryImport** for efficient Windows API calls
- **Async/await** patterns for responsive UI
- **LINQ and Records** for cleaner code
- **Ice Storm optimization** - primary modernization driver
- **Improved error handling** and stability

### Legacy Considerations

- Original .NET Framework 2.0 codebase preserved for reference
- Uses legacy P/Invoke patterns (DllImport)
- Synchronous file I/O and older C# syntax
- Timer-based automation with fixed intervals