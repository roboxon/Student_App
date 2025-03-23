using System;
using System.Net.Http;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Web;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.Json;
using System.Drawing.Drawing2D;
using JsonException = Newtonsoft.Json.JsonException;
using System.Configuration;
using Student_App.Models;
using Student_App.UI;
using System.Drawing;
using System.IO;
using Student_App.Forms;

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
        private CheckBox? RememberEmailCheckBox;
        private bool isDragging = false;
        private Point dragStart;

        public Login()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
            InitializeLoginControls();
            ApplyCommonStyling();
            LoadSavedEmail();
            try
            {
                string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resource", "DAA_Logo.ico");
                if (File.Exists(iconPath))
                {
                    this.Icon = new Icon(iconPath);
                }
            }
            catch (Exception ex)
            {
                // Silently continue if icon loading fails
                // The application will use the default icon
            }
        }

        private void LoadSavedEmail()
        {
            if (EmailTextBox != null)
            {
                var savedEmail = ConfigurationManager.AppSettings["RememberedEmail"];
                if (!string.IsNullOrEmpty(savedEmail))
                {
                    EmailTextBox.Text = savedEmail;
                    if (RememberEmailCheckBox != null)
                    {
                        RememberEmailCheckBox.Checked = true;
                    }
                }
            }
        }

        private void SaveEmail(string email)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings.Remove("RememberedEmail");
            config.AppSettings.Settings.Add("RememberedEmail", email);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        private void ClearSavedEmail()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings.Remove("RememberedEmail");
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
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
                loginPanel?.Controls.Add(emailContainer);
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
                loginPanel?.Controls.Add(passwordContainer);
            }

            // Style remember email checkbox
            if (RememberEmailCheckBox != null && loginPanel != null)
            {
                var checkboxContainer = new Panel
                {
                    Size = new Size(350, 35),
                    Location = new Point(40, 270),
                    BackColor = Color.White
                };

                RememberEmailCheckBox.Font = new Font(AppFonts.Body.FontFamily, 11);
                RememberEmailCheckBox.ForeColor = Color.FromArgb(70, 70, 70);
                RememberEmailCheckBox.Cursor = Cursors.Hand;
                RememberEmailCheckBox.Location = new Point(0, 5);

                // Add hover effect
                RememberEmailCheckBox.MouseEnter += (s, e) => 
                {
                    RememberEmailCheckBox.ForeColor = AppColors.Secondary;
                };
                RememberEmailCheckBox.MouseLeave += (s, e) => 
                {
                    RememberEmailCheckBox.ForeColor = Color.FromArgb(70, 70, 70);
                };

                checkboxContainer.Controls.Add(RememberEmailCheckBox);
                loginPanel.Controls.Add(checkboxContainer);

                // Update other control positions
                if (LoginButton != null)
                {
                    LoginButton.Location = new Point(40, 315);
                }
                if (ErrorLabel != null)
                {
                    ErrorLabel.Location = new Point(40, 370);
                }
            }

            // Style login button
            if (LoginButton != null)
            {
                LoginButton.FlatStyle = FlatStyle.Flat;
                LoginButton.BackColor = AppColors.Secondary;
                LoginButton.ForeColor = Color.White;
                LoginButton.Size = new Size(350, 45);
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

                loginPanel?.Controls.Add(LoginButton);
            }

            // Style error label
            if (ErrorLabel != null)
            {
                ErrorLabel.Font = AppFonts.Small;
                ErrorLabel.ForeColor = AppColors.Error;
                ErrorLabel.AutoSize = true;
                ErrorLabel.MaximumSize = new Size(350, 0);
                loginPanel?.Controls.Add(ErrorLabel);
            }

            // Add controls to login panel
            loginPanel.Controls.Add(closeButton);
            loginPanel.Controls.Add(titleLabel);

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

            // Create and configure remember email checkbox
            RememberEmailCheckBox = new CheckBox
            {
                Name = "RememberEmailCheckBox",
                Text = "Remember Email",
                AutoSize = true,
                Font = new Font(AppFonts.Body.FontFamily, 11, FontStyle.Regular),
                ForeColor = Color.FromArgb(70, 70, 70),
                Location = new Point(40, 270),
                Padding = new Padding(0, 5, 0, 5)
            };

            // Create and configure login button
            LoginButton = new Button
            {
                Name = "LoginButton",
                Text = "Sign In",
                Location = new Point(40, 310)
            };
            LoginButton.Click += LoginButton_Click;

            // Create and configure error label
            ErrorLabel = new Label
            {
                Name = "ErrorLabel",
                Text = "",
                AutoSize = true,
                Location = new Point(40, 365)
            };
        }

        private async void LoginButton_Click(object? sender, EventArgs e)
        {
            try
            {
                // Validate input fields
                if (string.IsNullOrWhiteSpace(EmailTextBox?.Text) || string.IsNullOrWhiteSpace(PasswordTextBox?.Text))
                {
                    MessageBox.Show("Please enter both email and password.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Disable the login button to prevent multiple clicks
                if (LoginButton != null)
                {
                    LoginButton.Enabled = false;
                    LoginButton.Text = "Logging in...";
                }

                // Handle remember email
                if (RememberEmailCheckBox?.Checked == true && EmailTextBox != null)
                {
                    SaveEmail(EmailTextBox.Text);
                }
                else
                {
                    ClearSavedEmail();
                }

                // Login using the Student model
                var student = await Student.LoginAsync(EmailTextBox!.Text, PasswordTextBox!.Text);

                // Create and show dashboard
                OpenDashboard(student);
            }
            catch (ApiException ex)
            {
                MessageBox.Show(ex.Message, "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception)
            {
                MessageBox.Show($"An unexpected error occurred.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Re-enable the login button
                if (LoginButton != null)
                {
                    LoginButton.Enabled = true;
                    LoginButton.Text = "Login";
                }
            }
        }

        private void OpenDashboard(Student student)
        {
            var dashboard = new Dashboard(student);
            dashboard.Show();
            
            // Update the application context with the new form
            TrayApplicationContext.Instance.SetForm(dashboard);
            
            this.Hide();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            // Close the application
            Application.Exit();
        }
    }
} 