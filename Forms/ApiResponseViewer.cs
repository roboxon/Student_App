using System.Text.Json;

namespace Student_App.Forms
{
    public partial class ApiResponseViewer : Form
    {
        public ApiResponseViewer(string title, string response)
        {
            InitializeComponent();
            this.Text = title;
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Create rich text box for response
            var responseBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 10),
                ReadOnly = true,
                Text = FormatJson(response)
            };

            // Create close button
            var closeButton = new Button
            {
                Text = "Close",
                Dock = DockStyle.Bottom,
                Height = 30
            };
            closeButton.Click += (s, e) => this.Close();

            // Add controls to form
            this.Controls.Add(responseBox);
            this.Controls.Add(closeButton);
        }

        private string FormatJson(string json)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var jsonElement = JsonSerializer.Deserialize<JsonElement>(json);
                return JsonSerializer.Serialize(jsonElement, options);
            }
            catch
            {
                return json; // Return original if not valid JSON
            }
        }
    }
} 