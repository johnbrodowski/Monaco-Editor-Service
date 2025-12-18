# Contributing to Monaco Editor Service

Thank you for considering contributing to Monaco Editor Service! This document provides guidelines and instructions for contributing.

## Code of Conduct

Be respectful and constructive in all interactions. We're all here to make this project better.

## How Can I Contribute?

### Reporting Bugs

Before creating a bug report:
- Check existing issues to avoid duplicates
- Update to the latest version and verify the bug still exists
- Collect relevant information (OS version, .NET version, WebView2 version)

When submitting a bug report, include:
- Clear, descriptive title
- Steps to reproduce the issue
- Expected vs actual behavior
- Code samples or screenshots if applicable
- Environment details (.NET version, OS, WebView2 version)

### Suggesting Enhancements

Enhancement suggestions are tracked as GitHub issues. When creating an enhancement suggestion:
- Use a clear, descriptive title
- Provide detailed description of the proposed functionality
- Explain why this enhancement would be useful
- Include code examples showing how it might work

### Pull Requests

1. **Fork the repository** and create your branch from `main`
2. **Make your changes** following the coding standards below
3. **Test your changes** thoroughly
4. **Update documentation** (README, XML comments) if needed
5. **Commit with clear messages** describing what and why
6. **Submit a pull request** with a clear description

## Development Setup

### Prerequisites

- Visual Studio 2022 or later (or VS Code with C# extension)
- .NET 10.0 SDK (or modify target framework)
- Monaco Editor files for testing

### Getting Started

```bash
# Clone your fork
git clone https://github.com/YOUR_USERNAME/Monaco-Editor-Service.git
cd Monaco-Editor-Service

# Create a branch for your feature
git checkout -b feature/your-feature-name

# Build the project
dotnet build

# Make your changes...
```

## Coding Standards

### C# Style Guidelines

- **Naming Conventions**:
  - PascalCase for public methods and properties
  - camelCase for private fields (with underscore prefix `_fieldName`)
  - UPPERCASE for constants

- **Async Methods**:
  - All async methods must end with `Async` suffix
  - Always use `await` properly, never block on async code
  - Use `ConfigureAwait(false)` where appropriate

- **Documentation**:
  - Add XML documentation comments for all public APIs
  - Include `<summary>`, `<param>`, `<returns>`, and `<exception>` tags
  - Provide code examples in `<example>` tags for complex methods

- **Error Handling**:
  - Use try-catch in JavaScript injection code
  - Validate parameters where appropriate
  - Throw meaningful exceptions with clear messages

### Example XML Documentation

```csharp
/// <summary>
/// Sets the cursor position in the editor and focuses it.
/// </summary>
/// <param name="lineNumber">The line number (1-based)</param>
/// <param name="column">The column position (1-based)</param>
/// <returns>A task representing the async operation</returns>
/// <example>
/// <code>
/// await editorService.SetCursorPositionAsync(10, 5);
/// </code>
/// </example>
public async Task SetCursorPositionAsync(int lineNumber, int column)
{
    // Implementation...
}
```

## Testing

### Manual Testing

Create a simple Windows Forms app to test your changes:

```csharp
// Test form for verifying functionality
public class TestForm : Form
{
    private MonacoEditorService _editor;

    // Add buttons and test scenarios
}
```

### Testing Checklist

Before submitting a PR, verify:
- [ ] Editor initializes correctly
- [ ] File load/save operations work
- [ ] Text manipulation methods function properly
- [ ] Decorations (highlights, bookmarks) display correctly
- [ ] No memory leaks (dispose properly)
- [ ] Works with different languages (C#, JavaScript, Python, etc.)
- [ ] Streaming functionality works smoothly

## Commit Messages

Write clear, concise commit messages:

### Format
```
type: Brief description (50 chars or less)

More detailed explanation if needed (wrap at 72 chars).
Explain what and why, not how.

- Bullet points are fine
- Reference issues: Fixes #123
```

### Types
- `feat:` New feature
- `fix:` Bug fix
- `docs:` Documentation changes
- `style:` Code style changes (formatting, etc.)
- `refactor:` Code refactoring
- `test:` Adding or updating tests
- `chore:` Maintenance tasks

### Examples
```
feat: Add support for custom themes

- Added SetThemeAsync method
- Updated HTML template to support theme changes
- Added theme parameter to InitializeAsync

Fixes #45
```

## Pull Request Process

1. **Update Documentation**: Ensure README and XML comments reflect your changes
2. **Self-Review**: Review your own code before submitting
3. **Keep PRs Focused**: One feature/fix per PR
4. **Respond to Feedback**: Address review comments promptly
5. **Squash Commits**: Keep git history clean (if needed)

### PR Title Format
```
[Type] Brief description

Examples:
[Feature] Add custom theme support
[Fix] Resolve memory leak in disposal
[Docs] Update API documentation
```

## Areas for Contribution

Here are some areas where contributions would be especially valuable:

### High Priority
- Unit tests and integration tests
- Performance optimizations
- Memory leak detection and fixes
- Better error handling and validation

### Features
- Custom theme support
- IntelliSense/autocomplete integration
- Multi-cursor support
- Diff editor support
- Minimap customization
- Find/replace functionality

### Documentation
- Video tutorials
- More code examples
- Architecture documentation
- API reference improvements

### Quality
- Code cleanup and refactoring
- Better async patterns
- Improved error messages
- Accessibility improvements

## Questions?

If you have questions about contributing:
- Open a discussion issue
- Check existing issues and PRs
- Review the README and code examples

## License

By contributing, you agree that your contributions will be licensed under the Apache License 2.0, the same license as the project.

---

**Thank you for contributing to Monaco Editor Service!**
