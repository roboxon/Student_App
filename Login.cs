using System;
using System.Drawing;
using System.Windows.Forms;

namespace Student_App
{
    public partial class Login : LayoutForm
    {
        private TextBox usernameTextBox = new();
        private TextBox passwordTextBox = new();
        private Button loginButton = new();
        private Label titleLabel = new();
        private Label errorLabel = new();

        public Login()
        {
            InitializeComponent();
            InitializeLoginControls();
        }

        protected override void InitializeComponent()
        {
            base.InitializeComponent();
            this.Text = "Student App - Login";
            this.Size = new Size(400, 500);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
        }

        private void InitializeLoginControls()
        {
            // Title label
            titleLabel.Text = "Student App Login";
            titleLabel.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            titleLabel.ForeColor = Color.FromArgb(45, 45, 48);
            titleLabel.AutoSize = true;
            titleLabel.Location = new Point(20, 40);
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;

            // Username field
            var usernameLabel = new Label
            {
                Text = "Username",
                Font = new Font("Segoe UI", 10),
                Location = new Point(20, 120),
                AutoSize = true
            };

            usernameTextBox.Location = new Point(20, 150);
            usernameTextBox.Size = new Size(340, 30);
            usernameTextBox.Font = new Font("Segoe UI", 10);
            usernameTextBox.BorderStyle = BorderStyle.FixedSingle;

            // Password field
            var passwordLabel = new Label
            {
                Text = "Password",
                Font = new Font("Segoe UI", 10),
                Location = new Point(20, 200),
                AutoSize = true
            };

            passwordTextBox.Location = new Point(20, 230);
            passwordTextBox.Size = new Size(340, 30);
            passwordTextBox.Font = new Font("Segoe UI", 10);
            passwordTextBox.BorderStyle = BorderStyle.FixedSingle;
            passwordTextBox.UseSystemPasswordChar = true;

            // Login button
            loginButton.Text = "Login";
            loginButton.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            loginButton.Location = new Point(20, 300);
            loginButton.Size = new Size(340, 40);
            loginButton.BackColor = Color.FromArgb(0, 122, 204);
            loginButton.ForeColor = Color.White;
            loginButton.FlatStyle = FlatStyle.Flat;
            loginButton.Cursor = Cursors.Hand;

            // Error label
            errorLabel.Text = "";
            errorLabel.Font = new Font("Segoe UI", 9);
            errorLabel.ForeColor = Color.FromArgb(232, 17, 35);
            errorLabel.Location = new Point(20, 350);
            errorLabel.Size = new Size(340, 20);
            errorLabel.TextAlign = ContentAlignment.MiddleCenter;
            errorLabel.Visible = false;

            // Add event handlers
            loginButton.Click += LoginButton_Click;

            // Add controls to main panel
            mainContentPanel.Controls.AddRange(new Control[] {
                titleLabel,
                usernameLabel,
                usernameTextBox,
                passwordLabel,
                passwordTextBox,
                loginButton,
                errorLabel
            });
        }

        private async void LoginButton_Click(object? sender, EventArgs e)
        {
            try
            {
                // Clear previous error
                errorLabel.Visible = false;
                errorLabel.Text = "";

                // Validate inputs
                if (string.IsNullOrWhiteSpace(usernameTextBox.Text))
                {
                    ShowError("Please enter your username");
                    return;
                }

                if (string.IsNullOrWhiteSpace(passwordTextBox.Text))
                {
                    ShowError("Please enter your password");
                    return;
                }

                // Disable login button
                loginButton.Enabled = false;
                loginButton.Text = "Logging in...";

                // TODO: Implement actual login logic here
                await Task.Delay(1000); // Simulate API call

                // Show dashboard on success
                var dashboard = new Forms.Dashboard();
                this.Hide();
                dashboard.ShowDialog();
                this.Close();
            }
            catch (Exception ex)
            {
                ShowError($"Login failed: {ex.Message}");
            }
            finally
            {
                // Re-enable login button
                loginButton.Enabled = true;
                loginButton.Text = "Login";
            }
        }

        private void ShowError(string message)
        {
            errorLabel.Text = message;
            errorLabel.Visible = true;
        }
    }
}
