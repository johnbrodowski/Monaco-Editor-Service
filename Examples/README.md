# Monaco Editor Service - Usage Examples

This directory contains practical examples demonstrating how to use the Monaco Editor Service in your applications.

## BasicEditorForm.cs

A complete Windows Forms application showing all major features of the Monaco Editor Service.

### Features Demonstrated

- **Editor Initialization** - Setting up the editor with initial code
- **File Operations** - Loading and saving files
- **Line Highlighting** - Highlighting specific line ranges
- **Bookmarks** - Adding/removing bookmark decorations
- **Cursor Operations** - Getting and setting cursor position
- **Text Insertion** - Inserting text at cursor position
- **Streaming** - Real-time text streaming (perfect for AI code generation)

### How to Use This Example

#### Option 1: Copy to Your Project

1. Create a new Windows Forms App (.NET 10.0 or compatible)
2. Add reference to the MonacoEditorService project or NuGet package
3. Copy `BasicEditorForm.cs` to your project
4. Set it as the startup form or run it from your main form

#### Option 2: Create as Standalone App

1. Create a new Windows Forms project:
```bash
dotnet new winforms -n MonacoEditorExample
cd MonacoEditorExample
```

2. Add reference to MonacoEditorService:
```bash
dotnet add reference ../MonacoEditorService/MonacoEditorService.csproj
```

3. Replace `Form1.cs` with `BasicEditorForm.cs`

4. Update `Program.cs`:
```csharp
using System;
using System.Windows.Forms;

namespace MonacoEditorExample
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new BasicEditorForm());
        }
    }
}
```

5. Build and run:
```bash
dotnet run
```

### Prerequisites

Before running the example, you need:

1. **Monaco Editor Files**: Download from [Monaco Editor](https://microsoft.github.io/monaco-editor/)
   - Extract to your application directory
   - Ensure the path `monaco-editor/min/vs/` exists

2. **WebView2 Runtime**: Usually pre-installed on Windows 10/11
   - If needed, download from [Microsoft](https://developer.microsoft.com/en-us/microsoft-edge/webview2/)

### Directory Structure

Your application directory should look like this:

```
YourApp/
├── MonacoEditorExample.exe
├── monaco-editor/
│   └── min/
│       └── vs/
│           ├── loader.js
│           ├── editor/
│           │   ├── editor.main.js
│           │   └── editor.main.css
│           └── ...
```

## What You'll Learn

By studying this example, you'll understand how to:

### 1. Initialize the Editor

```csharp
editorService = new MonacoEditorService(webView);
await editorService.InitializeAsync(
    appDirectory,
    initialCode,
    language: "csharp",
    width: 1000,
    height: 700
);
await editorService.EditorReady;
```

### 2. Work with Files

```csharp
// Load
await editorService.LoadFromFileAsync(filePath);

// Save
await editorService.SaveToFileAsync(filePath);
```

### 3. Highlight and Navigate

```csharp
// Highlight lines
await editorService.HighlightLineRangeAsync(5, 10);

// Move cursor
await editorService.SetCursorPositionAsync(7, 1);

// Clear highlights
await editorService.ClearHighlightAsync();
```

### 4. Add Bookmarks

```csharp
// Toggle bookmark on line
await editorService.ToggleBookmarkAsync(lineNumber);
```

### 5. Insert Text

```csharp
// Get current position
var (line, column) = await editorService.GetPositionAsync();

// Insert at cursor
await editorService.InsertTextAsync(line, column, "// Your text here\n");
```

### 6. Stream Content (AI Code Generation)

```csharp
await editorService.BeginStreamAsync();

foreach (var chunk in codeChunks)
{
    await editorService.StreamChunkAsync(chunk);
    await Task.Delay(50); // Smooth visual effect
}

await editorService.EndStreamAsync();
```

## Customization Ideas

Extend this example by adding:

- **Language Selection** - Dropdown to change syntax highlighting
- **Theme Switching** - Toggle between light/dark themes
- **Find/Replace** - Implement search functionality
- **Multi-file Tabs** - TabControl with multiple editors
- **Auto-save** - Timer-based automatic file saving
- **Minimap** - Enable/disable the code minimap
- **Git Integration** - Show diff decorations
- **IntelliSense** - Add autocomplete support

## Troubleshooting

### Editor doesn't load
- Ensure monaco-editor files are in the correct directory
- Check that WebView2 Runtime is installed
- Verify the file paths in InitializeAsync

### Buttons are disabled
- Wait for the "Editor ready!" message in the status bar
- Check for initialization errors in the message box

### File operations fail
- Ensure you have read/write permissions
- Check file paths are valid
- Verify the file format is supported

## Need Help?

- Check the main [README.md](../README.md) for API documentation
- Review [CONTRIBUTING.md](../CONTRIBUTING.md) for development guidelines
- Open an issue on GitHub for bugs or questions

---

**Happy Coding with Monaco Editor Service!**
