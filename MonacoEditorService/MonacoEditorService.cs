using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

 
using System;
 
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace AnthropicApp.DynamicIDE
{


    public sealed class MonacoEditorService : IDisposable
    {
        private readonly WebView2 _webView;
        private readonly TaskCompletionSource<bool> _editorReadyCompletionSource = new TaskCompletionSource<bool>();
        public Task EditorReady => _editorReadyCompletionSource.Task;



        // --- STATE MANAGEMENT FIELDS ---
        private string _currentLineHighlightId = null;
        private List<string> _currentBookmarkIds = new List<string>();

        #region HTML Template

 //Good
private string EditorHtmlTemplate2 = @"<!DOCTYPE html>
<html>
<head>
    <meta http-equiv=""Content-Type"" content=""text/html;charset=utf-8"" />
    <link rel=""stylesheet"" data-name=""vs/editor/editor.main"" href=""monaco-editor/min/vs/editor/editor.main.css""/>
    <style>
        .line-highlight { background-color: #404080; display: block; } /* For HighlightLineRange */
        .search-highlight { background-color: #885500; } /* For Search results */
        .bookmark-gutter-icon { background: #2680F3; width: 5px !important; margin-left: 5px; }
    </style>
</head>
<body>
    <h2>Monaco Editor Sync Loading Sample</h2>
    <div id=""container"" style=""width: <WIDTH>px; height: <HEIGHT>px; border: 1px solid grey""></div>

    <script>var require = { paths: { vs: 'monaco-editor/min/vs' } };</script>
    <script src=""monaco-editor/min/vs/loader.js""></script>
    <script src=""monaco-editor/min/vs/editor/editor.main.js""></script>
    <script>
        var bookmarkIdsByLine = {};
        var currentHighlightIds = [];
        var editor;
        require(['vs/editor/editor.main'], function() {
            editor = monaco.editor.create(document.getElementById('container'), {
                value: `<CODE>`,
                language: '<LANGUAGE>',
                theme: 'vs-dark',
                fontSize: 10,
                automaticLayout: true,
                glyphMargin: true
            });

            window.streamToEditor = function(text) {
                const selection = editor.getSelection();
                const edit = { range: selection, text: text, forceMoveMarkers: true };
                editor.executeEdits('stream-source', [edit]);
            };

            editor.onDidChangeModelContent(function() {
                console.log('Model content changed');
            });

            if (window.chrome && window.chrome.webview) {
                window.chrome.webview.postMessage({ type: 'editorReady' });
            }
        });
    </script>
</body>
</html>";



        //Good
        private string EditorHtmlTemplate = @"<!DOCTYPE html>
<html>
<head>
    <meta http-equiv=""Content-Type"" content=""text/html;charset=utf-8"" />
    <link rel=""stylesheet"" data-name=""vs/editor/editor.main"" href=""monaco-editor/min/vs/editor/editor.main.css""/>
    <style>
        .line-highlight { background-color: #404080; display: block; } /* For HighlightLineRange */
        .search-highlight { background-color: #885500; } /* For Search results */
        .bookmark-gutter-icon { background: #2680F3; width: 5px !important; margin-left: 5px; }
    </style>
</head>
<body>
    <h2>Monaco Editor Sync Loading Sample</h2>
    <div id=""container"" style=""width: <WIDTH>px; height: <HEIGHT>px; border: 1px solid grey""></div>

    <script>var require = { paths: { vs: 'monaco-editor/min/vs' } };</script>
    <script src=""monaco-editor/min/vs/loader.js""></script>
    <script src=""monaco-editor/min/vs/editor/editor.main.js""></script>
    <script>
        var bookmarkIdsByLine = {}; // Tracks bookmark decoration IDs by line number
        var currentHighlightIds = []; // Tracks highlight decoration IDs
        var editor;
        require(['vs/editor/editor.main'], function() {
            editor = monaco.editor.create(document.getElementById('container'), {
                value: `<CODE>`,
                language: '<LANGUAGE>',
                theme: 'vs-dark',
                fontSize: 10,
                automaticLayout: true,
                glyphMargin: true // Required for bookmark icons
            });

            window.streamToEditor = function(text) {
                const selection = editor.getSelection();
                const edit = { range: selection, text: text, forceMoveMarkers: true };
                editor.executeEdits('stream-source', [edit]);
            }

            editor.onDidChangeModelContent(() => {

            });

            if (window.chrome && window.chrome.webview) {
                window.chrome.webview.postMessage({ type: 'editorReady' });
            }
        });
    </script>
</body>
</html>";



        #endregion

        public MonacoEditorService(WebView2 webView) { _webView = webView; }

        public async Task InitializeAsync(string appDirectory, string initialCode, string language, int width = 800, int height = 600)
        {
            await _webView.EnsureCoreWebView2Async(null);
            _webView.CoreWebView2.WebMessageReceived += OnWebMessageReceived;
            var finalHtml = EditorHtmlTemplate
                .Replace("<CODE>", initialCode.Replace("`", "\\`"))
                .Replace("<LANGUAGE>", language)
                .Replace("<WIDTH>", width.ToString())
                .Replace("<HEIGHT>", height.ToString());
            var htmlPath = Path.Combine(appDirectory, "editor.html");
            File.WriteAllText(htmlPath, finalHtml);
            _webView.Source = new Uri(htmlPath);
        }

        private void OnWebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs args)
        {
            var message = JsonDocument.Parse(args.WebMessageAsJson).RootElement;
            if (message.TryGetProperty("type", out var type) && type.GetString() == "editorReady")
            {
                _editorReadyCompletionSource.TrySetResult(true);
            }
        }

        // --- ALL METHODS REQUESTED ARE NOW INCLUDED AND CORRECT ---

        #region File I/O and Content

        public async Task SetValueAsync(string value)
        {
            await _editorReadyCompletionSource.Task;
            var escapedValue = JsonSerializer.Serialize(value);
            await _webView.CoreWebView2.ExecuteScriptAsync($@"
                try {{
                    editor.setValue({escapedValue});
                }} catch (e) {{
                    console.error('SetValue error: ' + e.message);
                }}
            ");
        }








        public async Task<(int LineNumber, int Column)> GetPositionAsync()
        {
            await _editorReadyCompletionSource.Task;
            string script = @"
        try {
            const position = editor.getPosition();
            #if DEBUG
            console.log('GetPosition: line ' + position.lineNumber + ', column ' + position.column);
            #endif
            JSON.stringify(position);
        } catch (e) {
            JSON.stringify({ error: 'GetPosition failed: ' + e.message });
        }";
            string json = await _webView.CoreWebView2.ExecuteScriptAsync(script);
            var result = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json)
                ?? throw new JsonException("Failed to deserialize position JSON");
            if (result.TryGetValue("error", out var error))
            {
                throw new Exception(error.GetString());
            }
            return (result["lineNumber"].GetInt32(), result["column"].GetInt32());
        }








        public async Task<string?> GetSelectedTextAsync()
        {
            await _editorReadyCompletionSource.Task;
            var json = await _webView.CoreWebView2.ExecuteScriptAsync($@"
                try {{
                    const selection = editor.getSelection();
                    const model = editor.getModel();
                    const text = model.getValueInRange(selection);
                    text;
                }} catch (e) {{
                    console.error('GetSelectedText error: ' + e.message);
                    'Error: GetSelectedText failed: ' + e.message;
                }}
            ");
            return JsonSerializer.Deserialize<string>(json);
        }

         
        public async Task<string?> GetAllTextAsync()
        {
            await _editorReadyCompletionSource.Task;
            var json = await _webView.CoreWebView2.ExecuteScriptAsync("editor.getValue();");
            return JsonSerializer.Deserialize<string?>(json);
        }

 

        public async Task<string?> GetLineTextAsync(int lineNumber)
        {
            await _editorReadyCompletionSource.Task;
            var json = await _webView.CoreWebView2.ExecuteScriptAsync($@"
                try {{
                    const model = editor.getModel();
                    const validLine = Math.max(1, Math.min({lineNumber}, model.getLineCount()));
                    model.getLineContent(validLine);
                }} catch (e) {{
                console.error('GetLineRange error: ' + e.message);
                'Error: GetLineText ' + e.message;
                }}
            ");
            return JsonSerializer.Deserialize<string>(json);
        }
 
        public async Task<string> GetLineRangeAsync(int startLine, int endLine)
        {
            await _editorReadyCompletionSource.Task;

            var script = $@"
                try {{
                    const model = editor.getModel();
                    const validStart = Math.max(1, Math.min({startLine}, model.getLineCount()));
                    const validEnd = Math.max(validStart, Math.min({endLine}, model.getLineCount()));
                    const text = model.getValueInRange(new monaco.Range(validStart, 1, validEnd, model.getLineMaxColumn(validEnd)));
                    console.log('GetLineRange for lines ' + validStart + '-' + validEnd + ': ' + text.substring(0, 100) + '...');
                    text;
                }} catch (e) {{
                    console.error('GetLineRange error: ' + e.message);
                    'Error: GetLineRange' + e.message;
                }}
            ";
            string result = await _webView.CoreWebView2.ExecuteScriptAsync(script);
            // Remove surrounding quotes from JSON string result
            result = result.Trim('"').Replace("\\n", "\n").Replace("\\r", "\r").Replace("\\t", "\t").Replace("\\\"", "\"");
            System.Diagnostics.Debug.WriteLine($"GetLineRange lines {startLine}-{endLine}: {result.Substring(0, Math.Min(100, result.Length))}...");
            return result;
        }

  
        public async Task SaveToFileAsync(string filePath)
        {
            string content = await GetAllTextAsync() ?? string.Empty;
            await File.WriteAllTextAsync(filePath, content);
        }

        public async Task LoadFromFileAsync(string filePath)
        {
            string content = await File.ReadAllTextAsync(filePath) ?? string.Empty;
            await _webView.CoreWebView2.ExecuteScriptAsync($"editor.setValue({JsonSerializer.Serialize(content)});");
        }

        public async Task<int> CountLinesAsync()
        {
            await _editorReadyCompletionSource.Task;
            var json = await _webView.CoreWebView2.ExecuteScriptAsync("editor.getModel().getLineCount();");
            return int.Parse(json);
        }
        #endregion

        #region Editing and Manipulation
        public async Task InsertTextAsync(int lineNumber, int column, string text)
        {
            await _editorReadyCompletionSource.Task;
            var script = $"editor.executeEdits('api', [{{ range: new monaco.Range({lineNumber}, {column}, {lineNumber}, {column}), text: {JsonSerializer.Serialize(text)} }}]);";
            await _webView.CoreWebView2.ExecuteScriptAsync(script);
        }

        public async Task ReplaceLineAsync(int lineNumber, string newText)
        {
            await _editorReadyCompletionSource.Task;
            var script = $"const range = new monaco.Range({lineNumber}, 1, {lineNumber}, editor.getModel().getLineLength({lineNumber}) + 1); editor.executeEdits('api', [{{ range: range, text: {JsonSerializer.Serialize(newText)} }}]);";
            await _webView.CoreWebView2.ExecuteScriptAsync(script);
        }
 
        public async Task ReplaceLineRangeAsync(int startLine, int endLine, string newText)
        {
            await _editorReadyCompletionSource.Task;

            var script = $@"
                editor.executeEdits('replace-range', [
                    {{
                        range: new monaco.Range({startLine}, 1, {endLine}, 1),
                        text: {JsonSerializer.Serialize(newText)}
                    }}
                ]);
            ";
            await _webView.CoreWebView2.ExecuteScriptAsync(script);

            System.Diagnostics.Debug.WriteLine($"Replaced lines {startLine}-{endLine} with: {newText}");
        }


        public async Task DeleteLineRangeAsync(int startLine, int endLine)
        {
            await _editorReadyCompletionSource.Task;
            var script = $"editor.executeEdits('api', [{{ range: new monaco.Range({startLine}, 1, {endLine + 1}, 1), text: '' }}]);";
            await _webView.CoreWebView2.ExecuteScriptAsync(script);
        }

        public async Task SetCursorPositionAsync(int lineNumber, int column)
        {
            await _editorReadyCompletionSource.Task;
            await _webView.CoreWebView2.ExecuteScriptAsync($"editor.setPosition({{ lineNumber: {lineNumber}, column: {column} }}); editor.focus();");
        }

        public async Task BeginStreamAsync()
        {
            await _editorReadyCompletionSource.Task;
            await _webView.CoreWebView2.ExecuteScriptAsync("const model = editor.getModel(); editor.setPosition({ lineNumber: model.getLineCount(), column: model.getLineLength(model.getLineCount()) + 1 }); editor.focus(); editor.pushUndoStop();");
        }

        public async Task StreamChunkAsync(string text)
        {
            if (string.IsNullOrEmpty(text) || !_editorReadyCompletionSource.Task.IsCompleted) return;
            await _webView.CoreWebView2.ExecuteScriptAsync($"window.streamToEditor({JsonSerializer.Serialize(text)})");
        }

        public async Task EndStreamAsync()
        {
            await _editorReadyCompletionSource.Task;
            await _webView.CoreWebView2.ExecuteScriptAsync("editor.pushUndoStop();");
        }
        #endregion

        #region Highlight and Bookmarks (Corrected)

     
        public async Task HighlightLineRangeAsync(int startLine, int endLine)
        {
            await _editorReadyCompletionSource.Task;

            var script = $@"
            try {{
                console.log('Current highlight IDs before: ' + JSON.stringify(currentHighlightIds));
                // Clear all existing highlights
                if (currentHighlightIds.length > 0) {{
                    editor.deltaDecorations(currentHighlightIds, []);
                    currentHighlightIds = [];
                }}
            // Apply new highlight
            const newDecorations = [{{
                range: new monaco.Range({startLine}, 1, {endLine}, 1),
                options: {{ 
                    isWholeLine: true, 
                    className: 'line-highlight',
                    stickiness: 1
                }}
            }}];
            currentHighlightIds = editor.deltaDecorations([], newDecorations);
            // Force full layout and render
            editor.revealRangeInCenter(new monaco.Range({startLine}, 1, {endLine}, 1), 1);
            editor.layout();
            editor.updateOptions({{ theme: editor._themeService.getCurrentTheme().themeName }});
            console.log('Highlight applied for lines {startLine}-{endLine}, IDs: ' + JSON.stringify(currentHighlightIds));
            JSON.stringify(currentHighlightIds);
                }} catch (e) {{
                    console.error('HighlightLineRange error: ' + e.message);
                    'Error: ' + e.message;
                }}
            ";
            string result = await _webView.CoreWebView2.ExecuteScriptAsync(script);
            System.Diagnostics.Debug.WriteLine($"Highlighted lines {startLine}-{endLine}: {result}");
        }

 
        public async Task HighlightLineRangeAsyncX(int startLine, int endLine)
        {
            await _editorReadyCompletionSource.Task;

            var script = $@"
                try {{
                    console.log('Current highlight IDs before: ' + JSON.stringify(currentHighlightIds));
                    // Clear all existing highlights
                    if (currentHighlightIds.length > 0) {{
                        editor.deltaDecorations(currentHighlightIds, []);
                        currentHighlightIds = [];
                    }}
                    // Apply new highlight
                    const newDecorations = [{{
                        range: new monaco.Range({startLine}, 1, {endLine}, 1),
                        options: {{ 
                            isWholeLine: true, 
                            className: 'line-highlight',
                            stickiness: 1
                        }}
                    }}];
                    currentHighlightIds = editor.deltaDecorations([], newDecorations);
                    // Force full layout and render
                    editor.revealRangeInCenter(new monaco.Range({startLine}, 1, {endLine}, 1), 1);
                    editor.layout();
                    editor.updateOptions({{ theme: editor._themeService.getCurrentTheme().themeName }});
                    console.log('Highlight applied for lines {startLine}-{endLine}, IDs: ' + JSON.stringify(currentHighlightIds));
                    JSON.stringify(currentHighlightIds);
                }} catch (e) {{
                    console.error('HighlightLineRange error: ' + e.message);
                    'Error: ' + e.message;
                }}
            ";
            string result = await _webView.CoreWebView2.ExecuteScriptAsync(script);
            System.Diagnostics.Debug.WriteLine($"Highlighted lines {startLine}-{endLine}: {result}");
        }



        public async Task ToggleBookmarkAsync(int lineNumber)
        {
            await _editorReadyCompletionSource.Task;

            var script = $@"
                (function() {{
                    const line = {lineNumber};
                    if (bookmarkIdsByLine[line]) {{
                        // Remove bookmark
                        editor.deltaDecorations([bookmarkIdsByLine[line]], []);
                        delete bookmarkIdsByLine[line];
                        return 'Bookmark removed';
                    }} else {{
                        // Add bookmark
                        const newDecorations = [{{
                            range: new monaco.Range(line, 1, line, 1),
                            options: {{ glyphMarginClassName: 'bookmark-gutter-icon' }}
                        }}];
                        const newIds = editor.deltaDecorations([], newDecorations);
                        bookmarkIdsByLine[line] = newIds[0];
                        return 'Bookmark added';
                    }}
                }})()
            ";

            string result = await _webView.CoreWebView2.ExecuteScriptAsync(script);
            System.Diagnostics.Debug.WriteLine($"Bookmark operation on line {lineNumber}: {result}");
        }

 

        public async Task ClearHighlightAsync()
        {
            await _editorReadyCompletionSource.Task;

            var script = @"
                try {
                    console.log('Clearing highlights, current IDs: ' + JSON.stringify(currentHighlightIds));
                    if (currentHighlightIds.length > 0) {
                        editor.deltaDecorations(currentHighlightIds, []);
                        currentHighlightIds = [];
                        editor.layout();
                        editor.updateOptions({ theme: editor._themeService.getCurrentTheme().themeName });
                        console.log('Highlights cleared');
                    }
                    'Highlights cleared';
                } catch (e) {
                    console.error('ClearHighlight error: ' + e.message);
                    'Error: ' + e.message;
                }
            ";
            string result = await _webView.CoreWebView2.ExecuteScriptAsync(script);
            System.Diagnostics.Debug.WriteLine($"ClearHighlightAsync: {result}");
        }


        public async Task ClearAllDecorationsAsync()
        {
            await _editorReadyCompletionSource.Task;

            var script = @"
                try {
                    console.log('Clearing all decorations, highlights: ' + JSON.stringify(currentHighlightIds) + ', bookmarks: ' + JSON.stringify(bookmarkIdsByLine));
                    if (currentHighlightIds.length > 0) {
                        editor.deltaDecorations(currentHighlightIds, []);
                        currentHighlightIds = [];
                    }
                    for (const line in bookmarkIdsByLine) {
                        editor.deltaDecorations([bookmarkIdsByLine[line]], []);
                    }
                    bookmarkIdsByLine = {};
                    editor.layout();
                    editor.updateOptions({ theme: editor._themeService.getCurrentTheme().themeName });
                    console.log('All decorations cleared');
                    'All decorations cleared';
                } catch (e) {
                    console.error('ClearAllDecorations error: ' + e.message);
                    'Error: ' + e.message;
                }
            ";
            string result = await _webView.CoreWebView2.ExecuteScriptAsync(script);
            System.Diagnostics.Debug.WriteLine($"ClearAllDecorationsAsync: {result}");
        }

 


        public async Task<string> GetDecorationInfoAsync(int lineNumber)
        {
            await _editorReadyCompletionSource.Task;
            var json = await _webView.CoreWebView2.ExecuteScriptAsync($@"
                try {{
                    const decorations = editor.getLineDecorations({lineNumber});
                    const bookmarkId = bookmarkIdsByLine[{lineNumber}];
                    const result = {{ lineDecorations: decorations, bookmarkId: bookmarkId }};
                    JSON.stringify(result);
                }} catch (e) {{
                    JSON.stringify({{ error: 'GetDecorationInfo failed for line {lineNumber}: ' + e.message }});
                }}
            ");
            return JsonSerializer.Deserialize<string>(json);
        }

        public async Task<string> GetDecorationInfoAsyncX(int lineNumber)
        {
            await _editorReadyCompletionSource.Task;

            var script = $@"
                try {{
                    const line = {lineNumber};
                    const decorations = editor.getModel().getLineDecorations(line) || [];
                    const bookmarkId = bookmarkIdsByLine[line] || null;
                    const result = {{
                        lineDecorations: decorations.map(d => ({{
                            id: d.id,
                            className: d.options.className,
                            glyphMarginClassName: d.options.glyphMarginClassName,
                            isWholeLine: d.options.isWholeLine
                        }})),
                        bookmarkId: bookmarkId
                    }};
                    console.log('Decoration info for line ' + line + ': ' + JSON.stringify(result));
                    JSON.stringify(result);
                }} catch (e) {{
                    console.error('GetDecorationInfo error: ' + e.message);
                    'Error: ' + e.message;
                }}
            ";
            string result = await _webView.CoreWebView2.ExecuteScriptAsync(script);
            System.Diagnostics.Debug.WriteLine($"Decoration info for line {lineNumber}: {result}");
            return result;
        }
 
        #endregion

        public void Dispose()
        {
            if (_webView?.CoreWebView2 != null)
            {
                _webView.CoreWebView2.WebMessageReceived -= OnWebMessageReceived;
            }
        }
    }
}