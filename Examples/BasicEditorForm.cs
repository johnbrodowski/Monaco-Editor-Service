using MonacoEditor;
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
        private Panel sidePanel;
        private GroupBox grpFileOps;
        private GroupBox grpDecorations;
        private GroupBox grpTextOps;
        private GroupBox grpRangeOps;
        private Button btnLoadFile;
        private Button btnSaveFile;
        private Button btnHighlightLines;
        private Button btnClearHighlight;
        private Button btnToggleBookmark;
        private Button btnGetCursor;
        private Button btnInsertText;
        private Button btnStreamDemo;
        private Button btnGetRange;
        private Button btnReplaceRange;
        private Button btnDeleteRange;
        private Button btnSelectRange;
        private TextBox txtLineNumber;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel lblStatus;
        private ToolStripStatusLabel lblEditorInfo;

        public BasicEditorForm()
        {
            InitializeComponents();
            InitializeEditorAsync();
        }

        private void InitializeComponents()
        {
            // Form setup
            this.Text = "Monaco Editor Service - Feature Showcase";
            this.Size = new System.Drawing.Size(1400, 850);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);

            // Side panel with modern styling
            sidePanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 280,
                BackColor = System.Drawing.Color.FromArgb(45, 45, 48),
                Padding = new Padding(10),
                AutoScroll = true
            };

            // Modern status strip
            statusStrip = new StatusStrip
            {
                BackColor = System.Drawing.Color.FromArgb(0, 122, 204),
                ForeColor = System.Drawing.Color.White
            };

            lblStatus = new ToolStripStatusLabel
            {
                Text = "Initializing Monaco Editor...",
                Spring = true,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                ForeColor = System.Drawing.Color.White
            };

            lblEditorInfo = new ToolStripStatusLabel
            {
                Text = "Line: - | Col: -",
                ForeColor = System.Drawing.Color.White,
                BorderSides = ToolStripStatusLabelBorderSides.Left
            };

            statusStrip.Items.Add(lblStatus);
            statusStrip.Items.Add(lblEditorInfo);

            // WebView2 control
            webView = new WebView2
            {
                Dock = DockStyle.Fill
            };

            // Create grouped controls
            CreateFileOperationsGroup();
            CreateDecorationsGroup();
            CreateTextOperationsGroup();
            CreateRangeOperationsGroup();

            // Add controls to form
            this.Controls.Add(webView);
            this.Controls.Add(sidePanel);
            this.Controls.Add(statusStrip);

            // Disable buttons until editor is ready
            DisableButtons();
        }

        private void CreateFileOperationsGroup()
        {
            grpFileOps = new GroupBox
            {
                Text = "📁 File Operations",
                ForeColor = System.Drawing.Color.White,
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(260, 100),
                FlatStyle = FlatStyle.Flat
            };

            btnLoadFile = CreateStyledButton("📂 Load File", 25);
            btnLoadFile.Click += BtnLoadFile_Click;

            btnSaveFile = CreateStyledButton("💾 Save File", 60);
            btnSaveFile.Click += BtnSaveFile_Click;

            grpFileOps.Controls.Add(btnLoadFile);
            grpFileOps.Controls.Add(btnSaveFile);
            sidePanel.Controls.Add(grpFileOps);
        }

        private void CreateDecorationsGroup()
        {
            grpDecorations = new GroupBox
            {
                Text = "🎨 Visual Decorations",
                ForeColor = System.Drawing.Color.White,
                Location = new System.Drawing.Point(10, 120),
                Size = new System.Drawing.Size(260, 190),
                FlatStyle = FlatStyle.Flat
            };

            txtLineNumber = new TextBox
            {
                Left = 10,
                Top = 25,
                Width = 240,
                Height = 25,
                PlaceholderText = "Line number for bookmark...",
                BackColor = System.Drawing.Color.FromArgb(60, 60, 65),
                ForeColor = System.Drawing.Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            btnHighlightLines = CreateStyledButton("✨ Highlight Lines 5-10", 60);
            btnHighlightLines.Click += BtnHighlightLines_Click;

            btnClearHighlight = CreateStyledButton("🧹 Clear Highlight", 95);
            btnClearHighlight.Click += BtnClearHighlight_Click;

            btnToggleBookmark = CreateStyledButton("🔖 Toggle Bookmark", 130);
            btnToggleBookmark.Click += BtnToggleBookmark_Click;

            grpDecorations.Controls.Add(txtLineNumber);
            grpDecorations.Controls.Add(btnHighlightLines);
            grpDecorations.Controls.Add(btnClearHighlight);
            grpDecorations.Controls.Add(btnToggleBookmark);
            sidePanel.Controls.Add(grpDecorations);
        }

        private void CreateTextOperationsGroup()
        {
            grpTextOps = new GroupBox
            {
                Text = "✏️ Text Operations",
                ForeColor = System.Drawing.Color.White,
                Location = new System.Drawing.Point(10, 320),
                Size = new System.Drawing.Size(260, 155),
                FlatStyle = FlatStyle.Flat
            };

            btnGetCursor = CreateStyledButton("📍 Get Cursor Position", 25);
            btnGetCursor.Click += BtnGetCursor_Click;

            btnInsertText = CreateStyledButton("💬 Insert Comment", 60);
            btnInsertText.Click += BtnInsertText_Click;

            btnStreamDemo = CreateStyledButton("⚡ Stream Demo", 95);
            btnStreamDemo.Click += BtnStreamDemo_Click;

            grpTextOps.Controls.Add(btnGetCursor);
            grpTextOps.Controls.Add(btnInsertText);
            grpTextOps.Controls.Add(btnStreamDemo);
            sidePanel.Controls.Add(grpTextOps);
        }

        private void CreateRangeOperationsGroup()
        {
            grpRangeOps = new GroupBox
            {
                Text = "🎯 Range Operations",
                ForeColor = System.Drawing.Color.White,
                Location = new System.Drawing.Point(10, 485),
                Size = new System.Drawing.Size(260, 190),
                FlatStyle = FlatStyle.Flat
            };

            btnGetRange = CreateStyledButton("📋 Get Range Text", 25);
            btnGetRange.Click += BtnGetRange_Click;

            btnReplaceRange = CreateStyledButton("🔄 Replace Range", 60);
            btnReplaceRange.Click += BtnReplaceRange_Click;

            btnDeleteRange = CreateStyledButton("🗑️ Delete Range", 95);
            btnDeleteRange.Click += BtnDeleteRange_Click;

            btnSelectRange = CreateStyledButton("🔍 Select Range", 130);
            btnSelectRange.Click += BtnSelectRange_Click;

            grpRangeOps.Controls.Add(btnGetRange);
            grpRangeOps.Controls.Add(btnReplaceRange);
            grpRangeOps.Controls.Add(btnDeleteRange);
            grpRangeOps.Controls.Add(btnSelectRange);
            sidePanel.Controls.Add(grpRangeOps);
        }

        private Button CreateStyledButton(string text, int yPosition)
        {
            var button = new Button
            {
                Text = text,
                Left = 10,
                Top = yPosition,
                Width = 240,
                Height = 30,
                FlatStyle = FlatStyle.Flat,
                BackColor = System.Drawing.Color.FromArgb(0, 122, 204),
                ForeColor = System.Drawing.Color.White,
                Cursor = Cursors.Hand,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Padding = new Padding(5, 0, 0, 0)
            };

            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(28, 151, 234);
            button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(0, 102, 184);

            return button;
        }

        private void DisableButtons()
        {
            foreach (Control group in sidePanel.Controls)
            {
                if (group is GroupBox grp)
                {
                    foreach (Control control in grp.Controls)
                    {
                        if (control is Button btn)
                            btn.Enabled = false;
                    }
                }
            }
        }

        private void EnableButtons()
        {
            foreach (Control group in sidePanel.Controls)
            {
                if (group is GroupBox grp)
                {
                    foreach (Control control in grp.Controls)
                    {
                        if (control is Button btn)
                            btn.Enabled = true;
                    }
                }
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

                // Update UI on the UI thread
                this.Invoke((MethodInvoker)delegate
                {
                    lblStatus.Text = "Editor ready! Try the buttons on the left.";
                    EnableButtons();
                });
            }
            catch (Exception ex)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    lblStatus.Text = $"Error: {ex.Message}";
                    MessageBox.Show($"Failed to initialize editor: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                });
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
                lblStatus.Text = $"Cursor position retrieved";
                lblEditorInfo.Text = $"Line: {line} | Col: {column}";
                txtLineNumber.Text = line.ToString();
                MessageBox.Show($"Cursor Position:\n\nLine: {line}\nColumn: {column}",
                    "Cursor Position", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private async void BtnGetRange_Click(object sender, EventArgs e)
        {
            try
            {
                // Get text from range (lines 5-7, columns 1-20)
                string rangeText = await editorService.GetTextInRangeAsync(5, 1, 7, 20);
                MessageBox.Show($"Text in range (5:1 to 7:20):\n\n{rangeText}",
                    "Range Text", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Highlight the range for visual feedback
                await editorService.SelectRangeAsync(5, 1, 7, 20);
                lblStatus.Text = "Retrieved text from range (5:1 to 7:20)";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnReplaceRange_Click(object sender, EventArgs e)
        {
            try
            {
                // Replace text in range (lines 2-3, columns 1-50) with new text
                string newText = "// This text was replaced using ReplaceRangeAsync!\n// Range operations are powerful!";
                await editorService.ReplaceRangeAsync(2, 1, 3, 50, newText);

                // Highlight the new text
                await editorService.HighlightLineRangeAsync(2, 3);
                lblStatus.Text = "Replaced range (2:1 to 3:50) with new text";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnDeleteRange_Click(object sender, EventArgs e)
        {
            try
            {
                // Delete text in range (lines 8-10, columns 1-100)
                var result = MessageBox.Show(
                    "This will delete text in range (8:1 to 10:100). Continue?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.Yes)
                {
                    await editorService.DeleteRangeAsync(8, 1, 10, 100);
                    lblStatus.Text = "Deleted range (8:1 to 10:100)";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnSelectRange_Click(object sender, EventArgs e)
        {
            try
            {
                // Select and highlight a range (lines 12-15, columns 5-30)
                await editorService.SelectRangeAsync(12, 5, 15, 30);
                await editorService.HighlightLineRangeAsync(12, 15);
                lblStatus.Text = "Selected and highlighted range (12:5 to 15:30)";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
