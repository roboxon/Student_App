using System.Drawing;
using System.Windows.Forms;
using Student_App.Services.Configuration;
using Student_App.Models;
using Student_App.Forms;
using System.IO;

namespace Student_App.Forms
{
    public partial class Dashboard : LayoutForm
    {
        private Panel statsPanel = new();
        private Panel activityPanel = new();
        private Panel schedulePanel = new();
        private Student? currentStudent;
        private List<WorkingDay>? workingDays;
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
            workingDays = null;
            InitializeComponent();
            this.Icon = new Icon(Path.Combine(Application.StartupPath, "Resource", "DAA_Logo.ico"));
            InitializeDashboardControls();
            SetActiveMenuItem("Dashboard");
        }

        protected override void InitializeComponent()
        {
            base.InitializeComponent();
            this.Text = "Student Dashboard";
            
            // Modify the header panel
            headerPanel.Height = 60; // Reduced height
            headerPanel.Padding = new Padding(10, 0, 10, 0); // Reduce vertical padding
            
            if (currentStudent != null)
            {
                // Create a single row panel for all header info
                var headerInfoPanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = Color.Transparent
                };
                
                // Welcome message - now as first item
                var welcomeLabel = new Label
                {
                    Text = $"Welcome, {currentStudent.first_name} {currentStudent.last_name}",
                    Font = new Font(AppFonts.Body.FontFamily, 12, FontStyle.Regular),
                    ForeColor = Color.White,
                    AutoSize = true,
                    Location = new Point(5, 20) // Centered vertically
                };
                headerInfoPanel.Controls.Add(welcomeLabel);
                
                // Stats items with smaller font
                var statsItems = new[]
                {
                    new { Label = "Course", Value = currentStudent.plan_name ?? "N/A" },
                    new { Label = "Group", Value = currentStudent.group_name ?? "N/A" },
                    new { Label = "Grade", Value = currentStudent.grade_score?.ToString("F1") ?? "N/A" },
                    new { Label = "Status", Value = currentStudent.is_active == 1 ? "Active" : "Inactive" }
                };
                
                // Calculate positions - starting after welcome message
                int startX = welcomeLabel.Width + 80; // Start after welcome with some padding
                int itemWidth = 140; // Reduced width for each item
                
                foreach (var item in statsItems)
                {
                    var statPanel = new Panel
                    {
                        Width = itemWidth,
                        Height = 40,
                        Location = new Point(startX, 10), // Centered in header
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
                    headerInfoPanel.Controls.Add(statPanel);
                    
                    startX += itemWidth;
                }
                
                // Clear existing controls and add the new panel
                headerPanel.Controls.Clear();
                headerPanel.Controls.Add(headerInfoPanel);
            }
        }

        private void InitializeDashboardControls()
        {
            // Student Information panel with proper formatting
            var studentInfoPanel = new Panel
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
                Font = new Font(AppFonts.Heading.FontFamily, 12, FontStyle.Bold),
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
            
            // Working Schedule Panel - now has maximum space
            schedulePanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                BorderStyle = BorderStyle.None
            };
            
            // Modify the existing schedule code to add proper title and styling
            var scheduleTitle = new Label
            {
                Text = "Working Schedule",
                Font = new Font(AppFonts.Heading.FontFamily, 12, FontStyle.Bold),
                ForeColor = AppColors.Text,
                AutoSize = true,
                Location = new Point(5, 5)
            };
            
            // Schedule list view with better styling
            var scheduleList = new ListView
            {
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Location = new Point(5, 30),
                Size = new Size(mainContentPanel.Width - 30, mainContentPanel.Height - 120), // Adjust height to fill available space
                Font = new Font(AppFonts.Body.FontFamily, 10, FontStyle.Regular),
                BorderStyle = BorderStyle.FixedSingle
            };
            
            scheduleList.Columns.Add("Day", 150);
            scheduleList.Columns.Add("Start Time", 150);
            scheduleList.Columns.Add("End Time", 150);
            
            if (currentStudent?.working_days != null)
            {
                var scheduleItems = currentStudent.working_days.Select(day => 
                    new ListViewItem(new[] { 
                        day.day_name ?? "N/A",
                        day.start_time ?? "N/A",
                        day.end_time ?? "N/A"
                    })
                ).ToArray();
                scheduleList.Items.AddRange(scheduleItems);
            }
            
            schedulePanel.Controls.Add(scheduleTitle);
            schedulePanel.Controls.Add(scheduleList);
            
            // Add panels to main content in new order
            mainContentPanel.Controls.Add(schedulePanel);
            mainContentPanel.Controls.Add(studentInfoPanel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Remove system tray disposal
                components?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
} 