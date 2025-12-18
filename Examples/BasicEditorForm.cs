using AnthropicApp.DynamicIDE;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MonacoEditorExample
{
    /// <summary>
    /// Example Windows Forms application demonstrating Monaco Editor Service usage.
    /// This example shows basic editor operations, file I/O, highlighting, and bookmarks.
    /// </summary>
    public partial class BasicEditorForm : Form
    {
        private WebView2 webView;
        private MonacoEditorService editorService;
        private Panel buttonPanel;
        private Button btnLoadFile;
        private Button btnSaveFile;
        private Button btnHighlightLines;
        private Button btnClearHighlight;
        private Button btnToggleBookmark;
        private Button btnGetCursor;
        private Button btnInsertText;
        private Button btnStreamDemo;
        private TextBox txtLineNumber;
        private Label lblStatus;

        public BasicEditorForm()
        {
            InitializeComponents();
            InitializeEditorAsync();
        }

        private void InitializeComponents()
        {
            // Form setup
            this.Text = "Monaco Editor Service - Example";
            this.Size = new System.Drawing.Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Button panel on the left
            buttonPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 200,
                Padding = new Padding(10)
            };

            // Status label at bottom
            lblStatus = new Label
            {
                Dock = DockStyle.Bottom,
                Height = 30,
                Text = "Initializing editor...",
                Padding = new Padding(10, 5, 10, 5),
                BackColor = System.Drawing.Color.LightGray
            };

            // WebView2 control
            webView = new WebView2
            {
                Dock = DockStyle.Fill
            };

            // Create buttons
            int yPos = 10;
            btnLoadFile = CreateButton("Load File", yPos);
            btnLoadFile.Click += BtnLoadFile_Click;

            yPos += 40;
            btnSaveFile = CreateButton("Save File", yPos);
            btnSaveFile.Click += BtnSaveFile_Click;

            yPos += 50;
            txtLineNumber = new TextBox
            {
                Left = 10,
                Top = yPos,
                Width = 180,
                PlaceholderText = "Line number..."
            };
            buttonPanel.Controls.Add(txtLineNumber);

            yPos += 35;
            btnHighlightLines = CreateButton("Highlight Lines 5-10", yPos);
            btnHighlightLines.Click += BtnHighlightLines_Click;

            yPos += 40;
            btnClearHighlight = CreateButton("Clear Highlight", yPos);
            btnClearHighlight.Click += BtnClearHighlight_Click;

            yPos += 40;
            btnToggleBookmark = CreateButton("Toggle Bookmark", yPos);
            btnToggleBookmark.Click += BtnToggleBookmark_Click;

            yPos += 50;
            btnGetCursor = CreateButton("Get Cursor Position", yPos);
            btnGetCursor.Click += BtnGetCursor_Click;

            yPos += 40;
            btnInsertText = CreateButton("Insert Comment", yPos);
            btnInsertText.Click += BtnInsertText_Click;

            yPos += 50;
            btnStreamDemo = CreateButton("Stream Demo", yPos);
            btnStreamDemo.Click += BtnStreamDemo_Click;

            // Add controls to form
            this.Controls.Add(webView);
            this.Controls.Add(buttonPanel);
            this.Controls.Add(lblStatus);

            // Disable buttons until editor is ready
            DisableButtons();
        }

        private Button CreateButton(string text, int yPosition)
        {
            var button = new Button
            {
                Text = text,
                Left = 10,
                Top = yPosition,
                Width = 180,
                Height = 30
            };
            buttonPanel.Controls.Add(button);
            return button;
        }

        private void DisableButtons()
        {
            foreach (Control control in buttonPanel.Controls)
            {
                if (control is Button btn)
                    btn.Enabled = false;
            }
        }

        private void EnableButtons()
        {
            foreach (Control control in buttonPanel.Controls)
            {
                if (control is Button btn)
                    btn.Enabled = true;
            }
        }

        private async void InitializeEditorAsync()
        {
            try
            {
                // Create editor service
                editorService = new MonacoEditorService(webView);

                // Initial code sample
                string initialCode = @"// Welcome to Monaco Editor Service!
// This is a fully functional code editor embedded in a Windows Forms app.

using System;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(""Hello, Monaco Editor!"");

            // Try the buttons on the left:
            // - Load/Save files
            // - Highlight lines 5-10
            // - Toggle bookmarks
            // - Stream text in real-time

            int sum = CalculateSum(5, 10);
            Console.WriteLine($""Sum: {sum}"");
        }

        static int CalculateSum(int a, int b)
        {
            return a + b;
        }
    }
}";

                // Initialize with C# syntax highlighting
                string appDirectory = Application.StartupPath;
                await editorService.InitializeAsync(
                    appDirectory,
                    initialCode,
                    language: "csharp",
                    width: 1000,
                    height: 700
                );

                // Wait for editor to be ready
                await editorService.EditorReady;

                // Update UI
                lblStatus.Text = "Editor ready! Try the buttons on the left.";
                EnableButtons();
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
                MessageBox.Show($"Failed to initialize editor: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ===== EVENT HANDLERS =====

        private async void BtnLoadFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "C# files (*.cs)|*.cs|All files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        await editorService.LoadFromFileAsync(openFileDialog.FileName);
                        lblStatus.Text = $"Loaded: {Path.GetFileName(openFileDialog.FileName)}";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private async void BtnSaveFile_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "C# files (*.cs)|*.cs|All files (*.*)|*.*";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        await editorService.SaveToFileAsync(saveFileDialog.FileName);
                        lblStatus.Text = $"Saved: {Path.GetFileName(saveFileDialog.FileName)}";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error saving file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private async void BtnHighlightLines_Click(object sender, EventArgs e)
        {
            try
            {
                await editorService.HighlightLineRangeAsync(5, 10);
                await editorService.SetCursorPositionAsync(7, 1); // Jump to middle of highlight
                lblStatus.Text = "Highlighted lines 5-10";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnClearHighlight_Click(object sender, EventArgs e)
        {
            try
            {
                await editorService.ClearHighlightAsync();
                lblStatus.Text = "Cleared highlights";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnToggleBookmark_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtLineNumber.Text, out int lineNumber))
            {
                try
                {
                    await editorService.ToggleBookmarkAsync(lineNumber);
                    lblStatus.Text = $"Toggled bookmark on line {lineNumber}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid line number in the text box.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void BtnGetCursor_Click(object sender, EventArgs e)
        {
            try
            {
                (int line, int column) = await editorService.GetPositionAsync();
                lblStatus.Text = $"Cursor at Line {line}, Column {column}";
                txtLineNumber.Text = line.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnInsertText_Click(object sender, EventArgs e)
        {
            try
            {
                (int line, int column) = await editorService.GetPositionAsync();
                await editorService.InsertTextAsync(line, column, "// TODO: Add implementation here\n");
                lblStatus.Text = "Inserted comment at cursor position";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnStreamDemo_Click(object sender, EventArgs e)
        {
            try
            {
                lblStatus.Text = "Streaming text...";
                btnStreamDemo.Enabled = false;

                await editorService.BeginStreamAsync();

                string[] chunks = new[]
                {
                    "\n\n// === Streamed Content ===\n",
                    "// This demonstrates real-time text streaming\n",
                    "// Perfect for AI code generation!\n\n",
                    "public class StreamedExample\n",
                    "{\n",
                    "    public void StreamedMethod()\n",
                    "    {\n",
                    "        // This text was streamed chunk by chunk\n",
                    "        Console.WriteLine(\"Streaming is working!\");\n",
                    "    }\n",
                    "}\n"
                };

                foreach (var chunk in chunks)
                {
                    await editorService.StreamChunkAsync(chunk);
                    await Task.Delay(100); // Delay for visual effect
                }

                await editorService.EndStreamAsync();

                lblStatus.Text = "Streaming complete!";
                btnStreamDemo.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnStreamDemo.Enabled = true;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            editorService?.Dispose();
            base.OnFormClosing(e);
        }

        // Program entry point
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new BasicEditorForm());
        }
    }
}
