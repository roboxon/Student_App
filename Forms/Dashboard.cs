using System;
using System.Drawing;
using System.Windows.Forms;
using Student_App.UI;
using Student_App.Models;
using Student_App.Forms;
using System.IO;
using System.Linq;

namespace Student_App.Forms
{
    public partial class Dashboard : LayoutForm
    {
        private Panel statsPanel = new();
        private Panel studentInfoPanel = new();
        private Panel curriculumPanel = new();
        private CurriculumView curriculumView = new();
        private Student? currentStudent;
        private System.ComponentModel.IContainer components = null;

        public Dashboard(Student student)
        {
            if (student == null)
            {
                MessageBox.Show("No student information available. Redirecting to login.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                var loginForm = new Login();
                loginForm.Show();
                this.Close();
                return;
            }
            
            currentStudent = student;
            
            // Initialize components first
            InitializeComponent();
            
            // Set icon and initialize dashboard controls
            this.Icon = new Icon(Path.Combine(Application.StartupPath, "Resource", "DAA_Logo.ico"));
            InitializeDashboardControls();
            SetActiveMenuItem("Dashboard");
            
            // Load Release data asynchronously
            LoadReleaseDataAsync();
            
            // Set up the report button
            SetupReportButton();
        }

        private void LoadReleaseDataAsync()
        {
            try
            {
                // Ensure currentStudent is not null before calling API
                if (currentStudent != null)
                {
                    // Use the CurriculumView to load release data
                    curriculumView.LoadRelease(currentStudent);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load curriculum data: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void InitializeComponent()
        {
            base.InitializeComponent();
            this.Text = "Student Dashboard";
            
            // Update header with student info
            if (currentStudent != null)
            {
                UpdateUserInfo($"{currentStudent.first_name} {currentStudent.last_name}");
                
                // Add stats to header
                var statsItems = new[]
                {
                    new { Label = "Course", Value = currentStudent.plan_name ?? "N/A" },
                    new { Label = "Group", Value = currentStudent.group_name ?? "N/A" },
                    new { Label = "Grade", Value = currentStudent.grade_score?.ToString("F1") ?? "N/A" },
                    new { Label = "Status", Value = currentStudent.is_active == 1 ? "Active" : "Inactive" }
                };
                
                // Use the existing header space for stats
                int headerWidth = headerPanel.Width;
                int startX = 400; // After welcome message
                int itemWidth = 140;
                
                foreach (var item in statsItems)
                {
                    var statPanel = new Panel
                    {
                        Width = itemWidth,
                        Height = 40,
                        Location = new Point(startX, 10),
                        BackColor = Color.Transparent
                    };
                    
                    var labelControl = new Label
                    {
                        Text = item.Label,
                        Font = new Font(AppFonts.Body.FontFamily, 9, FontStyle.Regular),
                        ForeColor = Color.LightGray,
                        AutoSize = true,
                        Location = new Point(0, 0)
                    };
                    
                    var valueControl = new Label
                    {
                        Text = item.Value,
                        Font = new Font(AppFonts.Body.FontFamily, 11, FontStyle.Bold),
                        ForeColor = Color.White,
                        AutoSize = true,
                        Location = new Point(0, 18)
                    };
                    
                    statPanel.Controls.Add(labelControl);
                    statPanel.Controls.Add(valueControl);
                    headerPanel.Controls.Add(statPanel);
                    
                    startX += itemWidth;
                }
            }
        }

        private void InitializeDashboardControls()
        {
            // Student Information panel with proper formatting
            studentInfoPanel = new Panel
            {
                Height = 70, // Further reduced height
                Dock = DockStyle.Top,
                Padding = new Padding(10, 5, 10, 5),
                BackColor = Color.White,
                BorderStyle = BorderStyle.None
            };
            
            var infoTitle = new Label
            {
                Text = "Student Information",
                Font = new Font(AppFonts.Title.FontFamily, 12, FontStyle.Bold),
                ForeColor = AppColors.Text,
                AutoSize = true,
                Location = new Point(5, 5)
            };
            studentInfoPanel.Controls.Add(infoTitle);
            
            // Create a horizontal panel for student info items
            var infoItemsPanel = new Panel
            {
                Width = mainContentPanel.Width - 20,
                Height = 40,
                Location = new Point(5, 25),
                BorderStyle = BorderStyle.None
            };
            
            // Only include essential information - all on one line
            if (currentStudent != null)
            {
                var infoItems = new[]
                {
                    new { Label = "Email", Value = currentStudent.email ?? "N/A" },
                    new { Label = "Mentor", Value = currentStudent.mentor_name ?? "N/A" },
                    new { Label = "Advisor", Value = currentStudent.advisor_name ?? "N/A" },
                    new { Label = "Start Date", Value = currentStudent.start_date ?? "N/A" },
                    new { Label = "End Date", Value = currentStudent.end_date ?? "N/A" }
                };
                
                int infoItemWidth = 180;
                int xPos = 0;
                
                foreach (var item in infoItems)
                {
                    var infoItem = new Panel
                    {
                        Width = infoItemWidth,
                        Height = 40,
                        Location = new Point(xPos, 0),
                        BorderStyle = BorderStyle.None
                    };
                    
                    var labelControl = new Label
                    {
                        Text = item.Label,
                        Font = new Font(AppFonts.Body.FontFamily, 9, FontStyle.Bold),
                        ForeColor = AppColors.Secondary,
                        AutoSize = true,
                        Location = new Point(0, 0)
                    };
                    
                    var valueControl = new Label
                    {
                        Text = item.Value,
                        Font = new Font(AppFonts.Body.FontFamily, 10, FontStyle.Regular),
                        ForeColor = AppColors.Text,
                        AutoSize = true,
                        Location = new Point(0, 20)
                    };
                    
                    infoItem.Controls.Add(labelControl);
                    infoItem.Controls.Add(valueControl);
                    infoItemsPanel.Controls.Add(infoItem);
                    
                    xPos += infoItemWidth;
                }
            }
            
            studentInfoPanel.Controls.Add(infoItemsPanel);
            
            // Add a subtle bottom border to the student info panel
            studentInfoPanel.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(230, 230, 230), 1))
                {
                    e.Graphics.DrawLine(pen, 0, studentInfoPanel.Height - 1, studentInfoPanel.Width, studentInfoPanel.Height - 1);
                }
            };
            
            // Curriculum Panel - main content area
            curriculumPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10, 10, 10, 10),
                BorderStyle = BorderStyle.None
            };
            
            // Add the curriculum view to the curriculum panel
            curriculumView.Dock = DockStyle.Fill;
            curriculumPanel.Controls.Add(curriculumView);
            
            // Add panels to main content in new order
            mainContentPanel.Controls.Add(curriculumPanel);
            mainContentPanel.Controls.Add(studentInfoPanel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        private void SetupReportButton()
        {
            // Find the Reports button if it exists
            if (menuButtons.ContainsKey("Reports"))
            {
                var reportButton = menuButtons["Reports"];
                
                // We can't use reportButton.Click = null
                // Instead, we'll just add our handler directly
                // The existing handler in LayoutForm should still work
                
                // Add our new handler
                reportButton.Click += (s, e) => OpenReportsForm();
            }
        }

        private void OpenReportsForm()
        {
            try
            {
                if (currentStudent == null)
                {
                    MessageBox.Show("Student information is not available. Please log in again.", 
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Create only ONE instance of the form
                var reportForm = new WeeklyReportForm(currentStudent);
                
                // Show it and hide this form
                reportForm.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening Reports form: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Get the current student
        private Student GetCurrentStudent()
        {
            // Use the existing currentStudent field
            return currentStudent;
        }

        protected override void InitializeLayoutComponents()
        {
            base.InitializeLayoutComponents();
            
            // After base initialization, modify the Reports button click handler
            SetupReportButton();
        }
    }
} 