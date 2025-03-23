using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Student_App.Models;
using Student_App.Services.Reports;
using Student_App.UI;

namespace Student_App.Forms
{
    public class WeeklyReportForm : LayoutForm
    {
        private Student currentStudent;
        private WeeklyReport currentReport;
        private DateTime selectedWeekStart;
        
        // UI Controls
        private ReportCalendarView calendarView;
        private TabControl dailyTabs;
        private Panel summaryPanel;
        private RichTextBox weeklySummaryBox;
        private Button saveLocalButton;
        private Button submitButton;
        private Label statusLabel;
        
        public WeeklyReportForm(Student student)
        {
            currentStudent = student;
            selectedWeekStart = GetCurrentWeekStart();
            InitializeComponent();
            SetActiveMenuItem("Reports");
            UpdateUserInfo($"{student.first_name} {student.last_name}");
            titleLabel.Text = "Weekly Reports";
        }
        
        private DateTime GetCurrentWeekStart()
        {
            DateTime now = DateTime.Now.Date;
            int diff = (7 + (now.DayOfWeek - DayOfWeek.Monday)) % 7;
            return now.AddDays(-1 * diff);
        }
        
        protected override void InitializeComponent()
        {
            base.InitializeComponent();
            
            // Calendar Panel (left side)
            var calendarPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 300,
                Padding = new Padding(10)
            };
            
            calendarView = new ReportCalendarView(currentStudent);
            calendarView.Dock = DockStyle.Fill;
            calendarView.WeekSelected += CalendarView_WeekSelected;
            calendarPanel.Controls.Add(calendarView);
            
            // Daily Tabs (center/right)
            dailyTabs = new TabControl
            {
                Dock = DockStyle.Fill,
                
            };
            
            // Summary Panel (bottom)
            summaryPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 150,
                Padding = new Padding(10)
            };
            
            var summaryLabel = new Label
            {
                Text = "Weekly Summary:",
                AutoSize = true,
                Font = AppFonts.Bold
            };
            
            weeklySummaryBox = new RichTextBox
            {
                Dock = DockStyle.Bottom,
                Height = 100
            };
            
            // Action Buttons
            saveLocalButton = new Button
            {
                Text = "Save Report Locally",
                Width = 150,
                Height = 30,
                BackColor = AppColors.Primary,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            saveLocalButton.Click += SaveLocalButton_Click;
            
            submitButton = new Button
            {
                Text = "Submit Report",
                Width = 150,
                Height = 30,
                BackColor = AppColors.Success,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Left = 160
            };
            submitButton.Click += SubmitButton_Click;
            
            statusLabel = new Label
            {
                Text = "Select a week to begin",
                AutoSize = true,
                Top = 35
            };
            
            // Add controls
            summaryPanel.Controls.Add(summaryLabel);
            summaryPanel.Controls.Add(weeklySummaryBox);
            summaryPanel.Controls.Add(saveLocalButton);
            summaryPanel.Controls.Add(submitButton);
            summaryPanel.Controls.Add(statusLabel);
            
            // Add to main content
            mainContentPanel.Controls.Add(dailyTabs);
            mainContentPanel.Controls.Add(summaryPanel);
            mainContentPanel.Controls.Add(calendarPanel);
            
            // Load initial report
            LoadWeeklyReport();
        }
        
        private void CalendarView_WeekSelected(object sender, DateTime weekStart)
        {
            if (weekStart != selectedWeekStart)
            {
                selectedWeekStart = weekStart;
                LoadWeeklyReport();
            }
        }
        
        private async void LoadWeeklyReport()
        {
            statusLabel.Text = "Loading...";
            
            // First try to load from local storage
            currentReport = await LocalReportStorageManager.GetWeekReportAsync(selectedWeekStart);
            
            // If not found locally, create a new one or try to fetch from server
            if (currentReport == null)
            {
                // Try to fetch from server
                currentReport = await WeeklyReport.FetchFromServerAsync(
                    currentStudent.id, selectedWeekStart, currentStudent.access_token);
                    
                // If still null, create a new one
                if (currentReport == null)
                {
                    currentReport = new WeeklyReport
                    {
                        StudentId = currentStudent.id,
                        StartDate = selectedWeekStart,
                        EndDate = selectedWeekStart.AddDays(6),
                        WeekNumber = GetIso8601WeekOfYear(selectedWeekStart),
                        Year = selectedWeekStart.Year
                    };
                    
                    // Initialize with empty daily reports based on schedule
                    currentReport.InitializeWeek(currentStudent);
                }
            }
            
            // Update UI
            UpdateUI();
            statusLabel.Text = $"Week of {selectedWeekStart:MMM dd, yyyy} loaded";
        }
        
        private void UpdateUI()
        {
            // Clear existing tabs
            dailyTabs.TabPages.Clear();
            
            // Add tabs for each daily report
            foreach (var dailyReport in currentReport.DailyReports)
            {
                var tabPage = new TabPage(dailyReport.Date.ToString("dddd, MMM dd"));
                
                // Create hourly report panel
                var hourlyPanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    AutoScroll = true
                };
                
                int yPos = 10;
                
                // Add hourly report rows
                foreach (var hourlyReport in dailyReport.HourlyReports)
                {
                    // Time label
                    var timeLabel = new Label
                    {
                        Text = hourlyReport.TimeRange,
                        Width = 80,
                        Location = new Point(10, yPos + 5),
                        Font = AppFonts.Bold
                    };
                    
                    // Subject/Topic dropdown
                    var subjectDropdown = new ComboBox
                    {
                        Width = 200,
                        Location = new Point(100, yPos),
                        DropDownStyle = ComboBoxStyle.DropDownList,
                        Tag = hourlyReport  // Store reference to the hourly report
                    };
                    
                    // Add sample subjects - in a real app, this would come from curriculum data
                    subjectDropdown.Items.Add("Select Subject/Topic");
                    subjectDropdown.Items.Add("Fachquali FISI-02 - Datenbanken");
                    subjectDropdown.Items.Add("Programmierung - C#");
                    subjectDropdown.Items.Add("Netzwerktechnik - TCP/IP");
                    
                    subjectDropdown.SelectedIndex = 0;
                    if (hourlyReport.SubjectId > 0)
                    {
                        // Try to select the right item based on subjectId
                        // In real implementation, we'd match with actual data
                        subjectDropdown.SelectedIndex = hourlyReport.SubjectId % 3 + 1;
                    }
                    
                    subjectDropdown.SelectedIndexChanged += SubjectDropdown_SelectedIndexChanged;
                    
                    // Description textbox
                    var descriptionBox = new TextBox
                    {
                        Width = 400,
                        Height = 50,
                        Multiline = true,
                        Location = new Point(310, yPos),
                        Text = hourlyReport.LearningDescription,
                        Tag = hourlyReport  // Store reference to the hourly report
                    };
                    
                    descriptionBox.TextChanged += DescriptionBox_TextChanged;
                    
                    // Status indicator
                    var statusIcon = new Panel
                    {
                        Width = 16,
                        Height = 16,
                        Location = new Point(720, yPos + 5),
                        BackColor = hourlyReport.IsSubmitted ? AppColors.Success : AppColors.Secondary
                    };
                    
                    // Add controls
                    hourlyPanel.Controls.Add(timeLabel);
                    hourlyPanel.Controls.Add(subjectDropdown);
                    hourlyPanel.Controls.Add(descriptionBox);
                    hourlyPanel.Controls.Add(statusIcon);
                    
                    yPos += 60;
                }
                
                // Daily summary
                var dailySummaryLabel = new Label
                {
                    Text = "Daily Summary:",
                    Location = new Point(10, yPos + 10),
                    Font = AppFonts.Bold,
                    AutoSize = true
                };
                
                var dailySummaryBox = new TextBox
                {
                    Width = 600,
                    Height = 60,
                    Multiline = true,
                    Location = new Point(10, yPos + 30),
                    Text = dailyReport.DailySummary,
                    Tag = dailyReport  // Store reference to the daily report
                };
                
                dailySummaryBox.TextChanged += DailySummaryBox_TextChanged;
                
                hourlyPanel.Controls.Add(dailySummaryLabel);
                hourlyPanel.Controls.Add(dailySummaryBox);
                
                // Add to tab
                tabPage.Controls.Add(hourlyPanel);
                dailyTabs.TabPages.Add(tabPage);
            }
            
            // Update weekly summary
            weeklySummaryBox.Text = currentReport.WeeklySummary;
            
            // Update the calendar display (in case status changed)
            calendarView.RefreshCalendar();
        }
        
        private void SubjectDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            var dropdown = (ComboBox)sender;
            var hourlyReport = (HourlyReport)dropdown.Tag;
            
            if (dropdown.SelectedIndex > 0)
            {
                // In a real app, we'd get actual subject/topic IDs
                int subjectId = dropdown.SelectedIndex;
                hourlyReport.SubjectId = subjectId;
                hourlyReport.SubjectName = dropdown.SelectedItem.ToString();
                hourlyReport.LastUpdated = DateTime.Now;
            }
        }
        
        private void DescriptionBox_TextChanged(object sender, EventArgs e)
        {
            var textBox = (TextBox)sender;
            var hourlyReport = (HourlyReport)textBox.Tag;
            
            hourlyReport.LearningDescription = textBox.Text;
            hourlyReport.LastUpdated = DateTime.Now;
        }
        
        private void DailySummaryBox_TextChanged(object sender, EventArgs e)
        {
            var textBox = (TextBox)sender;
            var dailyReport = (DailyReport)textBox.Tag;
            
            dailyReport.DailySummary = textBox.Text;
        }
        
        private async void SaveLocalButton_Click(object sender, EventArgs e)
        {
            // Update weekly summary
            currentReport.WeeklySummary = weeklySummaryBox.Text;
            
            // Update totals
            currentReport.UpdateTotals();
            
            // Save locally
            bool success = await LocalReportStorageManager.SaveWeekReportAsync(selectedWeekStart, currentReport);
            
            if (success)
            {
                statusLabel.Text = "Report saved locally";
                
                // Refresh calendar to show updated status
                calendarView.RefreshCalendar();
            }
            else
            {
                statusLabel.Text = "Error saving report";
            }
        }
        
        private async void SubmitButton_Click(object sender, EventArgs e)
        {
            // Update weekly summary
            currentReport.WeeklySummary = weeklySummaryBox.Text;
            
            // Update totals
            currentReport.UpdateTotals();
            
            // First save locally
            await LocalReportStorageManager.SaveWeekReportAsync(selectedWeekStart, currentReport);
            
            // Then submit to server
            statusLabel.Text = "Submitting to server...";
            bool success = await currentReport.SubmitToServerAsync(currentStudent.access_token);
            
            if (success)
            {
                statusLabel.Text = "Report submitted successfully";
                
                // Remove local file after successful submission
                LocalReportStorageManager.RemoveAfterSync(selectedWeekStart);
                
                // Update UI to show submitted status
                UpdateUI();
            }
            else
            {
                statusLabel.Text = "Error submitting report";
            }
        }
        
        // Helper to get ISO8601 week number
        private int GetIso8601WeekOfYear(DateTime date)
        {
            System.Globalization.CultureInfo cultureInfo = 
                System.Globalization.CultureInfo.CurrentCulture;
            return cultureInfo.Calendar.GetWeekOfYear(
                date, 
                System.Globalization.CalendarWeekRule.FirstFourDayWeek, 
                DayOfWeek.Monday);
        }
    }
} 