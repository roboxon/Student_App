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

namespace Student_App.Forms
{
    public partial class Login : Form
    {
        private readonly HttpClient _httpClient;
        private const string API_URL = "https://training.elexbo.de/studentLogin/loginByemailPassword";
        private const string DEFAULT_COMPANY_ID = "1"; // Default company ID

        private TextBox? EmailTextBox;
        private TextBox? PasswordTextBox;
        private Button? LoginButton;
        private Label? ErrorLabel;

        public Login()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
            InitializeLoginControls();
        }

        private void InitializeLoginControls()
        {
            // Create and configure email textbox
            EmailTextBox = new TextBox
            {
                Location = new Point(50, 100),
                Size = new Size(200, 20),
                Name = "EmailTextBox",
                PlaceholderText = "Email"
            };

            // Create and configure password textbox
            PasswordTextBox = new TextBox
            {
                Location = new Point(50, 130),
                Size = new Size(200, 20),
                Name = "PasswordTextBox",
                PasswordChar = '*',
                PlaceholderText = "Password"
            };

            // Create and configure login button
            LoginButton = new Button
            {
                Location = new Point(50, 160),
                Size = new Size(200, 30),
                Name = "LoginButton",
                Text = "Login"
            };
            LoginButton.Click += LoginButton_Click;

            // Create and configure error label
            ErrorLabel = new Label
            {
                Location = new Point(50, 200),
                Size = new Size(200, 20),
                Name = "ErrorLabel",
                ForeColor = Color.Red,
                Text = ""
            };

            // Add controls to form
            this.Controls.Add(EmailTextBox);
            this.Controls.Add(PasswordTextBox);
            this.Controls.Add(LoginButton);
            this.Controls.Add(ErrorLabel);
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
                        var dashboard = new Dashboard();
                        this.Hide();
                        dashboard.ShowDialog();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Invalid response format from server.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show($"Login failed: {responseContent}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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