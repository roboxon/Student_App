using System;
using System.Net.Http;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Web;
using Newtonsoft.Json;
using System.Collections.Generic;
using Student_App.Services;
using Student_App.Services.Configuration;
using System.Text.Json;
using System.Drawing.Drawing2D;
using JsonException = System.Text.Json.JsonException;

namespace Student_App.Forms
{
    public partial class Login : Form
    {
        private readonly HttpClient _httpClient;
        private const string API_URL = "https://training.elexbo.de/studentLogin/loginByemailPassword";
        private const string DEFAULT_COMPANY_ID = "1";

        private Panel? loginPanel;
        private PictureBox? logoBox;
        private TextBox? EmailTextBox;
        private TextBox? PasswordTextBox;
        private Button? LoginButton;
        private Label? ErrorLabel;
        private bool isDragging = false;
        private Point dragStart;

        public Login()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
            InitializeLoginControls();
            ApplyCommonStyling();
        }

        private void ApplyCommonStyling()
        {
            // Form styling
            this.Text = "Student Login";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new Size(900, 600);
            this.BackColor = Color.FromArgb(240, 242, 245);

            // Add rounded corners to form
            int radius = 10;
            GraphicsPath path = new GraphicsPath();
            path.AddArc(0, 0, radius * 2, radius * 2, 180, 90);
            path.AddArc(this.Width - radius * 2, 0, radius * 2, radius * 2, 270, 90);
            path.AddArc(this.Width - radius * 2, this.Height - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(0, this.Height - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseFigure();
            this.Region = new Region(path);

            // Add form shadow
            this.Paint += (s, e) =>
            {
                var rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
                using (var pen = new Pen(Color.FromArgb(50, 0, 0, 0), 1))
                {
                    e.Graphics.DrawPath(pen, path);
                }
            };

            // Create main container panel with two sections
            var mainContainer = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0),
                BackColor = Color.White
            };

            // Left section for branding
            var brandPanel = new Panel
            {
                Width = 400,
                Dock = DockStyle.Left,
                BackColor = AppColors.Primary
            };

            // Make brand panel draggable
            brandPanel.MouseDown += (s, e) =>
            {
                isDragging = true;
                dragStart = e.Location;
            };

            brandPanel.MouseMove += (s, e) =>
            {
                if (isDragging)
                {
                    Point p = PointToScreen(e.Location);
                    Location = new Point(p.X - dragStart.X, p.Y - dragStart.Y);
                }
            };

            brandPanel.MouseUp += (s, e) => isDragging = false;

            // Create gradient background for brand panel
            brandPanel.Paint += (s, e) =>
            {
                var rect = new Rectangle(0, 0, brandPanel.Width, brandPanel.Height);
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    rect,
                    AppColors.Primary,
                    Color.FromArgb(
                        Math.Max(0, AppColors.Primary.R - 40),
                        Math.Max(0, AppColors.Primary.G - 40),
                        Math.Max(0, AppColors.Primary.B - 40)
                    ),
                    45F))
                {
                    e.Graphics.FillRectangle(brush, rect);
                }
            };

            // Add logo and welcome text to brand panel
            logoBox = new PictureBox
            {
                Size = new Size(120, 120),
                Location = new Point((brandPanel.Width - 120) / 2, 100),
                BackColor = Color.Transparent
            };

            // Create a simple logo using graphics
            var logoBitmap = new Bitmap(120, 120);
            using (Graphics g = Graphics.FromImage(logoBitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (var brush = new LinearGradientBrush(
                    new Rectangle(0, 0, 120, 120),
                    Color.White,
                    Color.FromArgb(230, 230, 230),
                    45F))
                {
                    g.FillEllipse(brush, 10, 10, 100, 100);
                }
                using (var pen = new Pen(Color.White, 3))
                {
                    g.DrawEllipse(pen, 10, 10, 100, 100);
                }
            }
            logoBox.Image = logoBitmap;

            var welcomeLabel = new Label
            {
                Text = "Welcome Back!",
                ForeColor = Color.White,
                Font = new Font(AppFonts.Title.FontFamily, 24, FontStyle.Bold),
                AutoSize = false,
                Location = new Point(0, 250),
                Width = brandPanel.Width,
                Height = 40,
                TextAlign = ContentAlignment.MiddleCenter
            };

            var subtitleLabel = new Label
            {
                Text = "Sign in to continue to your dashboard",
                ForeColor = Color.FromArgb(220, 220, 220),
                Font = AppFonts.Body,
                AutoSize = false,
                Location = new Point(0, 290),
                Width = brandPanel.Width,
                Height = 30,
                TextAlign = ContentAlignment.MiddleCenter
            };

            brandPanel.Controls.Add(logoBox);
            brandPanel.Controls.Add(welcomeLabel);
            brandPanel.Controls.Add(subtitleLabel);

            // Right section for login form
            loginPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(40),
                BackColor = Color.White
            };

            // Make login panel draggable
            loginPanel.MouseDown += (s, e) =>
            {
                isDragging = true;
                dragStart = e.Location;
            };

            loginPanel.MouseMove += (s, e) =>
            {
                if (isDragging)
                {
                    Point p = PointToScreen(e.Location);
                    Location = new Point(p.X - dragStart.X, p.Y - dragStart.Y);
                }
            };

            loginPanel.MouseUp += (s, e) => isDragging = false;

            // Close button
            var closeButton = new Button
            {
                Text = "×",
                Size = new Size(30, 30),
                Location = new Point(mainContainer.Width - 430, 10),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.Gray,
                Font = new Font(AppFonts.Body.FontFamily, 20),
                Cursor = Cursors.Hand
            };
            closeButton.FlatAppearance.BorderSize = 0;
            closeButton.MouseEnter += (s, e) => closeButton.ForeColor = Color.FromArgb(220, 53, 69); // Red on hover
            closeButton.MouseLeave += (s, e) => closeButton.ForeColor = Color.Gray;
            closeButton.Click += (s, e) => Application.Exit();

            // Title
            var titleLabel = new Label
            {
                Text = "Sign In",
                Font = new Font(AppFonts.Title.FontFamily, 28, FontStyle.Bold),
                ForeColor = AppColors.Text,
                Location = new Point(40, 60),
                AutoSize = true
            };

            // Style email textbox
            if (EmailTextBox != null)
            {
                EmailTextBox.BorderStyle = BorderStyle.None;
                EmailTextBox.Font = new Font(AppFonts.Body.FontFamily, 12);
                EmailTextBox.Size = new Size(350, 40);
                EmailTextBox.Location = new Point(10, 8);
                
                var emailContainer = CreateTextBoxContainer(EmailTextBox, "Email Address");
                emailContainer.Location = new Point(40, 140);
                loginPanel.Controls.Add(emailContainer);
            }

            // Style password textbox
            if (PasswordTextBox != null)
            {
                PasswordTextBox.BorderStyle = BorderStyle.None;
                PasswordTextBox.Font = new Font(AppFonts.Body.FontFamily, 12);
                PasswordTextBox.Size = new Size(350, 40);
                PasswordTextBox.Location = new Point(10, 8);

                var passwordContainer = CreateTextBoxContainer(PasswordTextBox, "Password");
                passwordContainer.Location = new Point(40, 220);
                loginPanel.Controls.Add(passwordContainer);
            }

            // Style login button
            if (LoginButton != null)
            {
                LoginButton.FlatStyle = FlatStyle.Flat;
                LoginButton.BackColor = AppColors.Secondary;
                LoginButton.ForeColor = Color.White;
                LoginButton.Size = new Size(350, 45);
                LoginButton.Location = new Point(40, 300);
                LoginButton.Font = new Font(AppFonts.Body.FontFamily, 12, FontStyle.Bold);
                LoginButton.Cursor = Cursors.Hand;
                LoginButton.FlatAppearance.BorderSize = 0;

                // Add hover effect
                LoginButton.MouseEnter += (s, e) => LoginButton.BackColor = Color.FromArgb(
                    Math.Max(0, AppColors.Secondary.R - 20),
                    Math.Max(0, AppColors.Secondary.G - 20),
                    Math.Max(0, AppColors.Secondary.B - 20)
                );
                LoginButton.MouseLeave += (s, e) => LoginButton.BackColor = AppColors.Secondary;
            }

            // Style error label
            if (ErrorLabel != null)
            {
                ErrorLabel.Font = AppFonts.Small;
                ErrorLabel.ForeColor = AppColors.Error;
                ErrorLabel.AutoSize = true;
                ErrorLabel.Location = new Point(40, 355);
                ErrorLabel.MaximumSize = new Size(350, 0);
            }

            // Add controls to login panel
            loginPanel.Controls.Add(closeButton);
            loginPanel.Controls.Add(titleLabel);
            if (LoginButton != null) loginPanel.Controls.Add(LoginButton);
            if (ErrorLabel != null) loginPanel.Controls.Add(ErrorLabel);

            // Add panels to form
            mainContainer.Controls.Add(loginPanel);
            mainContainer.Controls.Add(brandPanel);
            this.Controls.Add(mainContainer);
        }

        private Panel CreateTextBoxContainer(TextBox textBox, string placeholder)
        {
            var container = new Panel
            {
                Size = new Size(350, 65),
                BackColor = Color.White
            };

            var label = new Label
            {
                Text = placeholder,
                Font = new Font(AppFonts.Small.FontFamily, 10),
                ForeColor = Color.Gray,
                Location = new Point(0, 0),
                AutoSize = true
            };

            var inputContainer = new Panel
            {
                Size = new Size(350, 40),
                Location = new Point(0, 20),
                BackColor = Color.FromArgb(245, 247, 250),
                Padding = new Padding(10, 8, 10, 8)
            };

            textBox.Location = new Point(10, 8);
            textBox.Width = 330;
            textBox.BackColor = Color.FromArgb(245, 247, 250);

            inputContainer.Controls.Add(textBox);
            container.Controls.Add(label);
            container.Controls.Add(inputContainer);

            // Add focus effect
            textBox.Enter += (s, e) =>
            {
                inputContainer.BackColor = Color.FromArgb(240, 242, 245);
                textBox.BackColor = Color.FromArgb(240, 242, 245);
            };

            textBox.Leave += (s, e) =>
            {
                inputContainer.BackColor = Color.FromArgb(245, 247, 250);
                textBox.BackColor = Color.FromArgb(245, 247, 250);
            };

            return container;
        }

        private void InitializeLoginControls()
        {
            // Create and configure email textbox
            EmailTextBox = new TextBox
            {
                Name = "EmailTextBox",
                PlaceholderText = "Enter your email"
            };

            // Create and configure password textbox
            PasswordTextBox = new TextBox
            {
                Name = "PasswordTextBox",
                PasswordChar = '•',
                PlaceholderText = "Enter your password"
            };

            // Create and configure login button
            LoginButton = new Button
            {
                Name = "LoginButton",
                Text = "Sign In"
            };
            LoginButton.Click += LoginButton_Click;

            // Create and configure error label
            ErrorLabel = new Label
            {
                Name = "ErrorLabel",
                Text = "",
                AutoSize = true
            };
        }

        private async void LoginButton_Click(object? sender, EventArgs e)
        {
            if (EmailTextBox == null || PasswordTextBox == null || LoginButton == null)
            {
                MessageBox.Show("Error: Form controls not initialized", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(EmailTextBox.Text) || string.IsNullOrWhiteSpace(PasswordTextBox.Text))
            {
                MessageBox.Show("Please enter both email and password.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                LoginButton.Enabled = false;
                LoginButton.Text = "Logging in...";
                if (ErrorLabel != null)
                {
                    ErrorLabel.Text = "";
                }

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("email", EmailTextBox.Text),
                    new KeyValuePair<string, string>("password", PasswordTextBox.Text),
                    new KeyValuePair<string, string>("company_id", DEFAULT_COMPANY_ID)
                });

                using var client = new HttpClient();
                var response = await client.PostAsync(API_URL, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                // Show response in development mode
                if (AppConfig.Environment.IsDevelopment)
                {
                    var viewer = new ApiResponseViewer("API Response", responseContent);
                    viewer.Show();
                }

                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = System.Text.Json.JsonSerializer.Deserialize<LoginResponse>(responseContent);
                    if (loginResponse?.data?.access_token != null && loginResponse.data.refresh_token != null)
                    {
                        TokenService.Instance.StoreTokens(loginResponse.data.access_token, loginResponse.data.refresh_token);
                        var dashboard = new Dashboard(loginResponse.data.student, loginResponse.data.days);
                        this.Hide();
                        dashboard.ShowDialog();
                        this.Close();
                    }
                    else if (ErrorLabel != null)
                    {
                        ErrorLabel.Text = "Invalid response format from server.";
                        MessageBox.Show("Invalid response format from server.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    var errorMessage = $"Login failed: {response.StatusCode}";
                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        try
                        {
                            var errorResponse = System.Text.Json.JsonSerializer.Deserialize<LoginResponse>(responseContent);
                            errorMessage += $"\n{errorResponse?.service_message ?? responseContent}";
                        }
                        catch
                        {
                            errorMessage += $"\n{responseContent}";
                        }
                    }
                    if (ErrorLabel != null)
                    {
                        ErrorLabel.Text = errorMessage;
                    }
                    MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (HttpRequestException ex)
            {
                var errorMessage = $"Network error: {ex.Message}";
                if (ErrorLabel != null)
                {
                    ErrorLabel.Text = errorMessage;
                }
                MessageBox.Show(errorMessage, "Network Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (JsonException ex)
            {
                var errorMessage = $"Invalid response format: {ex.Message}";
                if (ErrorLabel != null)
                {
                    ErrorLabel.Text = errorMessage;
                }
                MessageBox.Show(errorMessage, "Response Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                var errorMessage = $"An unexpected error occurred: {ex.Message}";
                if (ErrorLabel != null)
                {
                    ErrorLabel.Text = errorMessage;
                }
                MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (LoginButton != null)
                {
                    LoginButton.Enabled = true;
                    LoginButton.Text = "Login";
                }
            }
        }
    }

    // Response models
    public class LoginResponse
    {
        public int response_code { get; set; }
        public string? message { get; set; }
        public int count { get; set; }
        public string? service_message { get; set; }
        public LoginData? data { get; set; }
    }

    public class LoginData
    {
        public Student? student { get; set; }
        public List<WorkingDay>? days { get; set; }
        public string? access_token { get; set; }
        public string? refresh_token { get; set; }
    }

    public class Student
    {
        public int id { get; set; }
        public int company_id { get; set; }
        public int course_id { get; set; }
        public float? grade_score { get; set; }
        public string? group_name { get; set; }
        public string? email { get; set; }
        public string? first_name { get; set; }
        public string? last_name { get; set; }
        public string? register_at { get; set; }
        public int register_by { get; set; }
        public int? mentor_id { get; set; }
        public string? mentor_name { get; set; }
        public string? advisor_id { get; set; }
        public string? advisor_name { get; set; }
        public string? join_course_date { get; set; }
        public string? exit_course_date { get; set; }
        public int course_plan_id { get; set; }
        public int branch_id { get; set; }
        public int release_id { get; set; }
        public int program_id { get; set; }
        public string? start_date { get; set; }
        public string? end_date { get; set; }
        public string? plan_name { get; set; }
        public string? tag { get; set; }
        public int is_active { get; set; }
    }

    public class WorkingDay
    {
        public int id { get; set; }
        public int course_plan_id { get; set; }
        public int day_number { get; set; }
        public string? start_time { get; set; }
        public string? end_time { get; set; }
        public string? day_name { get; set; }
    }
} 