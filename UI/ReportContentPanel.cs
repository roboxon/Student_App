using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Student_App.Models;
using Student_App.Services.Reports;

namespace Student_App.UI
{
    public class ReportContentPanel : UserControl
    {
        // Fields for student and report data
        private Student currentStudent;
        private WeeklyReport currentReport;
        private DateTime selectedWeekStart;
        private List<SubjectWithTopics> curriculumSubjects = new List<SubjectWithTopics>();
        
        // UI Controls
        private MonthCalendar calendar;
        private TextBox weeklySummaryBox;
        private Button saveLocalButton;
        private Button submitButton;
        private Label statusLabel;
        private Label weekRangeLabel;
        private Panel rightContentPanel;
        
        // Constructor
        public ReportContentPanel(Student student)
        {
            this.currentStudent = student;
            this.selectedWeekStart = GetCurrentWeekStart();
            
            InitializeComponent();
            
            // Load data in sequence
            LoadData();
        }
        
        private void LoadData()
        {
            try
            {
                // Set status
                if (statusLabel != null)
                    statusLabel.Text = "Loading report data...";
                
                // Load curriculum and report data asynchronously
                LoadCurriculumData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                if (statusLabel != null)
                    statusLabel.Text = "Error loading data";
            }
        }
        
        private DateTime GetCurrentWeekStart()
        {
            DateTime now = DateTime.Now.Date;
            int diff = (7 + (now.DayOfWeek - DayOfWeek.Monday)) % 7;
            return now.AddDays(-1 * diff);
        }
        
        private void InitializeComponent()
        {
            // Use Dock property for all main containers to ensure they expand with the available space
            this.Dock = DockStyle.Fill;
            
            // Main container with two panels - left for calendar, right for reports
            TableLayoutPanel mainContainer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.White
            };
            
            // Set column widths - first column fixed width for calendar
            mainContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 350F));
            mainContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            
            // Row height
            mainContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            
            // 1. LEFT PANEL: Calendar Panel - IMPORTANT: FIX CALENDAR SIZE
            Panel calendarPanel = new Panel
            {
                BackColor = Color.FromArgb(240, 240, 240),
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };
            
            // Calendar heading
            Label calendarHeading = new Label
            {
                Text = "Select a week to create/edit reports",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true, 
                Location = new Point(15, 15)
            };
            calendarPanel.Controls.Add(calendarHeading);
            
            // Create ONLY ONE calendar with SPECIFIC SIZE SETTINGS
            calendar = new MonthCalendar
            {
                ShowWeekNumbers = true,
                FirstDayOfWeek = Day.Monday,
                // IMPORTANT: Set CalendarDimensions to show only one month
                CalendarDimensions = new Size(1, 1),
                Location = new Point(15, 45),
                Font = new Font("Segoe UI", 9)
            };
            
            calendar.DateSelected += Calendar_DateSelected;
            calendarPanel.Controls.Add(calendar);
            
            // Add a second panel to hold the legend (don't put it directly in the calendar panel)
            Panel legendPanel = new Panel
            {
                Width = 300,
                Height = 120,
                Location = new Point(15, 230), // Moved up since we're only showing one month
                BackColor = calendarPanel.BackColor
            };
            
            // Add color indicators
            AddStatusIndicator(legendPanel, Color.FromArgb(40, 167, 69), "Complete", 0);
            AddStatusIndicator(legendPanel, Color.FromArgb(255, 193, 7), "Partial", 40);
            AddStatusIndicator(legendPanel, Color.FromArgb(108, 117, 125), "Not Started", 80);
            
            // Add the legend panel to a separate container
            Panel legendContainer = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 150,
                BackColor = calendarPanel.BackColor
            };
            legendContainer.Controls.Add(legendPanel);
            calendarPanel.Controls.Add(legendContainer);
            
            // 2. RIGHT PANEL: Report content
            Panel reportPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(15),
                BackColor = Color.White
            };
            
            // Week heading at top
            weekRangeLabel = new Label
            {
                Text = $"Week of: {selectedWeekStart:MMM dd, yyyy} to {selectedWeekStart.AddDays(6):MMM dd, yyyy}",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 123, 255),
                AutoSize = true,
                Location = new Point(10, 10)
            };
            reportPanel.Controls.Add(weekRangeLabel);
            
            // Content area (will be filled dynamically)
            rightContentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Location = new Point(10, 40),
                Padding = new Padding(0, 45, 0, 150) // Make room for header and footer
            };
            reportPanel.Controls.Add(rightContentPanel);
            
            // Weekly summary at bottom
            Panel weeklyPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 140,
                BackColor = Color.FromArgb(248, 249, 250),
                Padding = new Padding(10),
                BorderStyle = BorderStyle.FixedSingle
            };
            
            Label summaryLabel = new Label
            {
                Text = "Weekly Summary:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(10, 10)
            };
            weeklyPanel.Controls.Add(summaryLabel);
            
            weeklySummaryBox = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Location = new Point(10, 35),
                Size = new Size(weeklyPanel.Width - 30, 50),
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            weeklyPanel.Controls.Add(weeklySummaryBox);
            
            // Action buttons
            FlowLayoutPanel buttonPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                Location = new Point(10, 95),
                Size = new Size(weeklyPanel.Width - 30, 40),
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            
            saveLocalButton = new Button
            {
                Text = "Save Report Locally",
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(150, 35),
                Margin = new Padding(0, 0, 10, 0)
            };
            saveLocalButton.FlatAppearance.BorderSize = 0;
            saveLocalButton.Click += SaveLocalButton_Click;
            
            submitButton = new Button
            {
                Text = "Submit Report",
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(150, 35),
                Margin = new Padding(0, 0, 10, 0)
            };
            submitButton.FlatAppearance.BorderSize = 0;
            submitButton.Click += SubmitButton_Click;
            
            statusLabel = new Label
            {
                Text = "Loading...",
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 10, 0, 0)
            };
            
            buttonPanel.Controls.Add(saveLocalButton);
            buttonPanel.Controls.Add(submitButton);
            buttonPanel.Controls.Add(statusLabel);
            
            weeklyPanel.Controls.Add(buttonPanel);
            reportPanel.Controls.Add(weeklyPanel);
            
            // Add panels to main container
            mainContainer.Controls.Add(calendarPanel, 0, 0);
            mainContainer.Controls.Add(reportPanel, 1, 0);
            
            // Set single month calendar selection
            calendar.SelectionStart = selectedWeekStart;
            calendar.SelectionEnd = selectedWeekStart.AddDays(6);
            
            // Add the main container to this control
            this.Controls.Add(mainContainer);
            
            // Handle resize events
            this.Resize += (s, e) => {
                // Adjust UI elements on resize if needed
                weeklySummaryBox.Width = weeklyPanel.Width - 30;
                buttonPanel.Width = weeklyPanel.Width - 30;
            };
        }
        
        private void AddStatusIndicator(Panel container, Color color, string text, int yOffset)
        {
            Panel indicator = new Panel
            {
                BackColor = color,
                Size = new Size(16, 16),
                Location = new Point(0, yOffset + 2)
            };
            
            Label label = new Label
            {
                Text = text,
                AutoSize = true,
                Location = new Point(25, yOffset)
            };
            
            container.Controls.Add(indicator);
            container.Controls.Add(label);
        }
        
        private async void LoadCurriculumData()
        {
            try
            {
                if (currentStudent != null)
                {
                    // First get the curriculum data
                    var release = await Release.GetReleaseAsync(currentStudent);
                    if (release?.content?.subjects != null)
                    {
                        curriculumSubjects = release.content.subjects;
                    }
                }
                
                // After curriculum is loaded, load the weekly report
                LoadWeeklyReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading curriculum: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                // Still try to load report data even if curriculum failed
                LoadWeeklyReport();
            }
        }
        
        private async void LoadWeeklyReport()
        {
            try
            {
                if (statusLabel != null)
                    statusLabel.Text = "Loading weekly report...";
                    
                // Create a basic weekly report structure
                currentReport = new WeeklyReport
                {
                    StudentId = currentStudent?.id ?? 0,
                    StartDate = selectedWeekStart,
                    EndDate = selectedWeekStart.AddDays(6),
                    WeekNumber = GetIso8601WeekOfYear(selectedWeekStart),
                    Year = selectedWeekStart.Year,
                    DailyReports = new List<DailyReport>()
                };
                
                // Add empty daily reports
                for (int i = 0; i < 7; i++)
                {
                    var date = selectedWeekStart.AddDays(i);
                    var dailyReport = new DailyReport
                    {
                        Date = date,
                        HourlyReports = new List<HourlyReport>()
                    };
                    
                    // Add placeholder hourly reports
                    for (int hour = 9; hour < 17; hour++)
                    {
                        dailyReport.HourlyReports.Add(new HourlyReport
                        {
                            StartTime = $"{hour:00}:00",
                            EndTime = $"{hour+1:00}:00",
                            LastUpdated = DateTime.Now
                        });
                    }
                    
                    currentReport.DailyReports.Add(dailyReport);
                }
                
                // Try to get existing report from local storage
                var storedReport = await LocalReportStorageManager.GetWeekReportAsync(selectedWeekStart);
                if (storedReport != null)
                {
                    currentReport = storedReport;
                }
                
                // Update UI with loaded data
                UpdateUI();
                
                if (statusLabel != null)
                    statusLabel.Text = $"Week of {selectedWeekStart:MMM dd, yyyy} loaded";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading report: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                if (statusLabel != null)
                    statusLabel.Text = "Error loading report";
            }
        }
        
        private void Calendar_DateSelected(object sender, DateRangeEventArgs e)
        {
            try
            {
                // Find the start of the selected week
                DateTime firstDay = e.Start;
                int diff = (7 + (firstDay.DayOfWeek - DayOfWeek.Monday)) % 7;
                DateTime weekStart = firstDay.AddDays(-1 * diff);
                
                // Ensure we select the entire week
                calendar.SelectionStart = weekStart;
                calendar.SelectionEnd = weekStart.AddDays(6);
                
                if (weekStart != selectedWeekStart)
                {
                    selectedWeekStart = weekStart;
                    weekRangeLabel.Text = $"Week of: {selectedWeekStart:MMM dd, yyyy} to {selectedWeekStart.AddDays(6):MMM dd, yyyy}";
                    
                    // Set the month view to show the month containing the selected week
                    calendar.SetDate(weekStart);
                    
                    LoadWeeklyReport();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting date: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void UpdateUI()
        {
            try
            {
                // Clear existing content
                rightContentPanel.Controls.Clear();
                
                // Simple layout with TabControl for days
                TabControl dayTabs = new TabControl
                {
                    Dock = DockStyle.Fill
                };
                
                // Add a tab for each day of the week
                foreach (var dailyReport in currentReport.DailyReports)
                {
                    string dayName = dailyReport.Date.ToString("dddd, MMM dd");
                    TabPage dayTab = new TabPage(dayName);
                    
                    // Create the day content
                    Panel dayPanel = new Panel
                    {
                        Dock = DockStyle.Fill,
                        Padding = new Padding(10),
                        AutoScroll = true
                    };
                    
                    // Daily summary
                    Label summaryLabel = new Label
                    {
                        Text = "Daily Summary:",
                        Font = new Font("Segoe UI", 10, FontStyle.Bold),
                        AutoSize = true,
                        Location = new Point(10, 10)
                    };
                    dayPanel.Controls.Add(summaryLabel);
                    
                    TextBox summaryBox = new TextBox
                    {
                        Multiline = true,
                        ScrollBars = ScrollBars.Vertical,
                        Size = new Size(dayPanel.Width - 40, 60),
                        Location = new Point(10, 35),
                        Text = dailyReport.DailySummary ?? "",
                        Tag = dailyReport,
                        Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
                    };
                    summaryBox.TextChanged += (s, e) => {
                        var report = (s as TextBox)?.Tag as DailyReport;
                        if (report != null) 
                            report.DailySummary = (s as TextBox).Text;
                    };
                    dayPanel.Controls.Add(summaryBox);
                    
                    // Hourly reports heading
                    Label hoursLabel = new Label
                    {
                        Text = "Hourly Reports:",
                        Font = new Font("Segoe UI", 10, FontStyle.Bold),
                        AutoSize = true,
                        Location = new Point(10, 105)
                    };
                    dayPanel.Controls.Add(hoursLabel);
                    
                    // Primary subject selection
                    Label subjectLabel = new Label
                    {
                        Text = "Primary Subject for Today:",
                        Font = new Font("Segoe UI", 9),
                        AutoSize = true,
                        Location = new Point(150, 106)
                    };
                    dayPanel.Controls.Add(subjectLabel);
                    
                    ComboBox subjectDropdown = new ComboBox
                    {
                        DropDownStyle = ComboBoxStyle.DropDownList,
                        Size = new Size(250, 25),
                        Location = new Point(300, 103),
                        Tag = dailyReport
                    };
                    
                    // Add subjects option
                    subjectDropdown.Items.Add("Select a subject");
                    subjectDropdown.SelectedIndex = 0;
                    
                    // Add subjects from curriculum if available
                    if (curriculumSubjects != null && curriculumSubjects.Count > 0)
                    {
                        foreach (var subject in curriculumSubjects)
                        {
                            if (subject?.subject != null)
                            {
                                subjectDropdown.Items.Add(subject.subject.subject_name);
                            }
                        }
                    }
                    dayPanel.Controls.Add(subjectDropdown);
                    
                    // Add container for hourly reports
                    Panel hoursContainer = new Panel
                    {
                        AutoScroll = true,
                        Size = new Size(dayPanel.Width - 40, 300),
                        Location = new Point(10, 135),
                        Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
                    };
                    
                    // Add hourly reports
                    int yPos = 10;
                    if (dailyReport.HourlyReports != null)
                    {
                        foreach (var hourReport in dailyReport.HourlyReports)
                        {
                            // Panel for this hour
                            Panel hourPanel = new Panel
                            {
                                Size = new Size(hoursContainer.Width - 25, 75),
                                Location = new Point(5, yPos),
                                BorderStyle = BorderStyle.FixedSingle,
                                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
                            };
                            
                            // Time label
                            Label timeLabel = new Label
                            {
                                Text = $"{hourReport.StartTime} - {hourReport.EndTime}",
                                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                                Location = new Point(10, 10),
                                AutoSize = true
                            };
                            hourPanel.Controls.Add(timeLabel);
                            
                            // Topic selection
                            Label topicLabel = new Label
                            {
                                Text = "Topic:",
                                Location = new Point(120, 10),
                                AutoSize = true
                            };
                            hourPanel.Controls.Add(topicLabel);
                            
                            ComboBox topicDropdown = new ComboBox
                            {
                                DropDownStyle = ComboBoxStyle.DropDownList,
                                Size = new Size(200, 25),
                                Location = new Point(165, 7),
                                Tag = hourReport
                            };
                            topicDropdown.Items.Add("Select a topic");
                            topicDropdown.SelectedIndex = 0;
                            hourPanel.Controls.Add(topicDropdown);
                            
                            // Description
                            Label descLabel = new Label
                            {
                                Text = "Description:",
                                Location = new Point(10, 40),
                                AutoSize = true
                            };
                            hourPanel.Controls.Add(descLabel);
                            
                            TextBox descBox = new TextBox
                            {
                                Location = new Point(85, 37),
                                Size = new Size(hourPanel.Width - 100, 25),
                                Text = hourReport.LearningDescription ?? "",
                                Tag = hourReport,
                                Anchor = AnchorStyles.Left | AnchorStyles.Right
                            };
                            descBox.TextChanged += (s, e) => {
                                var report = (s as TextBox)?.Tag as HourlyReport;
                                if (report != null)
                                    report.LearningDescription = (s as TextBox).Text;
                            };
                            hourPanel.Controls.Add(descBox);
                            
                            hoursContainer.Controls.Add(hourPanel);
                            yPos += 85;
                        }
                    }
                    
                    dayPanel.Controls.Add(hoursContainer);
                    dayTab.Controls.Add(dayPanel);
                    dayTabs.TabPages.Add(dayTab);
                }
                
                rightContentPanel.Controls.Add(dayTabs);
                
                // Update weekly summary
                if (weeklySummaryBox != null && currentReport != null)
                    weeklySummaryBox.Text = currentReport.WeeklySummary ?? "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating UI: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private async void SaveLocalButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentReport == null)
                    return;
                    
                // Update weekly summary from text box
                currentReport.WeeklySummary = weeklySummaryBox.Text;
                
                // Save locally
                bool success = await LocalReportStorageManager.SaveWeekReportAsync(selectedWeekStart, currentReport);
                
                if (success)
                {
                    statusLabel.Text = "Report saved locally";
                }
                else
                {
                    statusLabel.Text = "Error saving report";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving report: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private async void SubmitButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentReport == null)
                    return;
                    
                // Update weekly summary from text box
                currentReport.WeeklySummary = weeklySummaryBox.Text;
                
                // Save locally
                await LocalReportStorageManager.SaveWeekReportAsync(selectedWeekStart, currentReport);
                
                // Submit to server
                statusLabel.Text = "Submitting report...";
                bool success = await currentReport.SubmitToServerAsync(currentStudent.access_token);
                
                if (success)
                {
                    // Remove local copy after successful submission
                    LocalReportStorageManager.RemoveAfterSync(selectedWeekStart);
                    statusLabel.Text = "Report submitted successfully";
                }
                else
                {
                    statusLabel.Text = "Error submitting report";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error submitting report: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
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