using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
        private Panel calendarPanel;
        private MonthCalendar calendar;
        private TabControl dailyTabs;
        private TextBox weeklySummaryBox;
        private Button saveLocalButton;
        private Button submitButton;
        private Label statusLabel;
        private Label weekRangeLabel;
        
        public WeeklyReportForm(Student student)
        {
            currentStudent = student;
            selectedWeekStart = GetCurrentWeekStart();
            InitializeWeeklyReportComponents();
            SetActiveMenuItem("Reports");
            UpdateUserInfo($"{student?.first_name} {student?.last_name}");
            titleLabel.Text = "Weekly Reports";
        }
        
        private DateTime GetCurrentWeekStart()
        {
            DateTime now = DateTime.Now.Date;
            int diff = (7 + (now.DayOfWeek - DayOfWeek.Monday)) % 7;
            return now.AddDays(-1 * diff);
        }
        
        protected void InitializeWeeklyReportComponents()
        {
            // Back button to return to dashboard
            var backButton = new Button
            {
                Text = "Back to Dashboard",
                Width = 150,
                Height = 30,
                BackColor = AppColors.Secondary,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(10, 15)
            };
            backButton.FlatAppearance.BorderSize = 0;
            backButton.Click += BackButton_Click;
            headerPanel.Controls.Add(backButton);
            
            // Main container with two panels - left for calendar, right for reports
            var mainContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                SplitterDistance = 250,
                FixedPanel = FixedPanel.Panel1,
                Panel1MinSize = 250,
                BorderStyle = BorderStyle.None
            };
            
            // Left Panel - Calendar and Legend
            mainContainer.Panel1.BackColor = AppColors.LightGray;
            mainContainer.Panel1.Padding = new Padding(10);
            
            var calendarLabel = new Label
            {
                Text = "Select a week to create/edit reports:",
                AutoSize = true,
                Font = AppFonts.BoldSmall,
                Location = new Point(10, 10)
            };
            mainContainer.Panel1.Controls.Add(calendarLabel);
            
            // Single calendar control
            calendar = new MonthCalendar
            {
                MaxSelectionCount = 7,
                ShowWeekNumbers = true,
                FirstDayOfWeek = Day.Monday,
                CalendarDimensions = new Size(1, 1),
                Location = new Point(10, 40)
            };
            
            calendar.DateSelected += Calendar_DateSelected;
            mainContainer.Panel1.Controls.Add(calendar);
            
            // Legend for status colors
            var legendPanel = new Panel
            {
                Width = 200,
                Height = 100,
                Location = new Point(10, 250)
            };
            
            AddStatusIndicator(legendPanel, AppColors.Success, "Complete", 0);
            AddStatusIndicator(legendPanel, Color.Orange, "Partial", 30);
            AddStatusIndicator(legendPanel, AppColors.Secondary, "Not Started", 60);
            mainContainer.Panel1.Controls.Add(legendPanel);
            
            // Right Panel - Reports content with TabControl
            mainContainer.Panel2.Padding = new Padding(10);
            
            // Week header
            weekRangeLabel = new Label
            {
                Text = $"Week of: {selectedWeekStart:MMM dd, yyyy} to {selectedWeekStart.AddDays(6):MMM dd, yyyy}",
                Font = AppFonts.Bold,
                AutoSize = true,
                Location = new Point(10, 10)
            };
            mainContainer.Panel2.Controls.Add(weekRangeLabel);
            
            // Tab control for days
            dailyTabs = new TabControl
            {
                Location = new Point(10, 40),
                Size = new Size(mainContainer.Panel2.Width - 20, mainContainer.Panel2.Height - 200),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            
            // Create a tab for each day of the week
            for (int i = 0; i < 7; i++)
            {
                var date = selectedWeekStart.AddDays(i);
                var tabPage = new TabPage(date.ToString("dddd, MMM dd"));
                dailyTabs.TabPages.Add(tabPage);
            }
            
            mainContainer.Panel2.Controls.Add(dailyTabs);
            
            // Weekly summary section
            var summaryLabel = new Label
            {
                Text = "Weekly Summary:",
                Font = AppFonts.Bold,
                AutoSize = true,
                Location = new Point(10, dailyTabs.Bottom + 10),
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom
            };
            mainContainer.Panel2.Controls.Add(summaryLabel);
            
            weeklySummaryBox = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Location = new Point(10, summaryLabel.Bottom + 5),
                Size = new Size(mainContainer.Panel2.Width - 20, 60),
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            mainContainer.Panel2.Controls.Add(weeklySummaryBox);
            
            // Action buttons
            saveLocalButton = new Button
            {
                Text = "Save Report Locally",
                Width = 150,
                Height = 35,
                BackColor = AppColors.Primary,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(10, weeklySummaryBox.Bottom + 10),
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom
            };
            saveLocalButton.FlatAppearance.BorderSize = 0;
            saveLocalButton.Click += SaveLocalButton_Click;
            mainContainer.Panel2.Controls.Add(saveLocalButton);
            
            submitButton = new Button
            {
                Text = "Submit Report",
                Width = 150,
                Height = 35,
                BackColor = AppColors.Success,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(170, weeklySummaryBox.Bottom + 10),
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom
            };
            submitButton.FlatAppearance.BorderSize = 0;
            submitButton.Click += SubmitButton_Click;
            mainContainer.Panel2.Controls.Add(submitButton);
            
            statusLabel = new Label
            {
                Text = "Select a week to get started",
                AutoSize = true,
                Location = new Point(330, weeklySummaryBox.Bottom + 20),
                ForeColor = AppColors.Secondary,
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom
            };
            mainContainer.Panel2.Controls.Add(statusLabel);
            
            // Add to main content
            mainContentPanel.Controls.Add(mainContainer);
            
            // Load initial report
            LoadWeeklyReport();
        }
        
        // Helper method for status indicators
        private void AddStatusIndicator(Panel container, Color color, string text, int yOffset)
        {
            var indicator = new Panel
            {
                BackColor = color,
                Size = new Size(16, 16),
                Location = new Point(0, yOffset + 2)
            };
            
            var label = new Label
            {
                Text = text,
                AutoSize = true,
                Location = new Point(25, yOffset)
            };
            
            container.Controls.Add(indicator);
            container.Controls.Add(label);
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
                    LoadWeeklyReport();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting date: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void BackButton_Click(object sender, EventArgs e)
        {
            // Show Dashboard again with the current student
            var dashboard = new Dashboard(currentStudent);
            dashboard.Show();
            this.Close();
        }
        
        private async void LoadWeeklyReport()
        {
            try
            {
                statusLabel.Text = "Loading...";
                
                // First try to load from local storage
                currentReport = await LocalReportStorageManager.GetWeekReportAsync(selectedWeekStart);
                
                // If not found locally, create a new one or try to fetch from server
                if (currentReport == null)
                {
                    // Try to fetch from server (but handle the case where we're not connected)
                    try
                    {
                        if (currentStudent != null && !string.IsNullOrEmpty(currentStudent.access_token))
                        {
                            currentReport = await WeeklyReport.FetchFromServerAsync(
                                currentStudent.id, selectedWeekStart, currentStudent.access_token);
                        }
                    }
                    catch (Exception)
                    {
                        // Silently fail and create a new report
                    }
                        
                    // If still null, create a new one
                    if (currentReport == null)
                    {
                        currentReport = new WeeklyReport
                        {
                            StudentId = currentStudent?.id ?? 0,
                            StartDate = selectedWeekStart,
                            EndDate = selectedWeekStart.AddDays(6),
                            WeekNumber = GetIso8601WeekOfYear(selectedWeekStart),
                            Year = selectedWeekStart.Year
                        };
                        
                        // Initialize with empty daily reports based on schedule
                        if (currentStudent != null)
                        {
                            currentReport.InitializeWeek(currentStudent);
                        }
                        else
                        {
                            // Create an empty report structure if student is null
                            currentReport.DailyReports = new List<DailyReport>();
                            for (int i = 0; i < 7; i++)
                            {
                                var date = selectedWeekStart.AddDays(i);
                                currentReport.DailyReports.Add(new DailyReport
                                {
                                    Date = date,
                                    HourlyReports = new List<HourlyReport>()
                                });
                            }
                        }
                    }
                }
                
                // Update UI
                UpdateUI();
                statusLabel.Text = $"Week of {selectedWeekStart:MMM dd, yyyy} loaded";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading report: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void UpdateUI()
        {
            try
            {
                // Safety check
                if (currentReport == null)
                {
                    statusLabel.Text = "Error: No report data available";
                    return;
                }

                // Update tabs for each daily report
                for (int i = 0; i < Math.Min(dailyTabs.TabPages.Count, currentReport.DailyReports.Count); i++)
                {
                    var tabPage = dailyTabs.TabPages[i];
                    var dailyReport = currentReport.DailyReports[i];
                    
                    // Clear existing controls
                    tabPage.Controls.Clear();
                    
                    // Daily summary at the top
                    var summaryLabel = new Label
                    {
                        Text = "Daily Summary:",
                        Font = AppFonts.Bold,
                        AutoSize = true,
                        Location = new Point(10, 10)
                    };
                    tabPage.Controls.Add(summaryLabel);
                    
                    var summaryBox = new TextBox
                    {
                        Multiline = true,
                        ScrollBars = ScrollBars.Vertical,
                        Location = new Point(10, 35),
                        Size = new Size(tabPage.Width - 30, 60),
                        Text = dailyReport?.DailySummary ?? "",
                        Tag = dailyReport
                    };
                    summaryBox.TextChanged += DailySummaryBox_TextChanged;
                    tabPage.Controls.Add(summaryBox);
                    
                    // Hourly reports section
                    var hourlyLabel = new Label
                    {
                        Text = "Hourly Reports:",
                        Font = AppFonts.Bold,
                        AutoSize = true,
                        Location = new Point(10, 105)
                    };
                    tabPage.Controls.Add(hourlyLabel);
                    
                    // Panel to hold hourly reports with scrolling
                    var hourlyPanel = new Panel
                    {
                        AutoScroll = true,
                        Location = new Point(10, 130),
                        Size = new Size(tabPage.Width - 30, tabPage.Height - 140),
                        BorderStyle = BorderStyle.FixedSingle
                    };
                    tabPage.Controls.Add(hourlyPanel);
                    
                    // Add hourly report entries
                    if (dailyReport?.HourlyReports != null)
                    {
                        int yPos = 5;
                        foreach (var hourlyReport in dailyReport.HourlyReports)
                        {
                            if (hourlyReport != null)
                            {
                                // Create panel for each hourly report
                                var hourPanel = new Panel
                                {
                                    Width = hourlyPanel.Width - 25,
                                    Height = 70,
                                    Location = new Point(5, yPos),
                                    BackColor = Color.FromArgb(248, 248, 248),
                                    BorderStyle = BorderStyle.FixedSingle
                                };
                                
                                // Time label
                                var timeLabel = new Label
                                {
                                    Text = hourlyReport.TimeRange ?? $"{hourlyReport.StartTime}-{hourlyReport.EndTime}",
                                    Font = AppFonts.Bold,
                                    AutoSize = true,
                                    Location = new Point(10, 10)
                                };
                                hourPanel.Controls.Add(timeLabel);
                                
                                // Subject dropdown
                                var subjectLabel = new Label
                                {
                                    Text = "Subject:",
                                    AutoSize = true,
                                    Location = new Point(10, 40)
                                };
                                hourPanel.Controls.Add(subjectLabel);
                                
                                var subjectDropdown = new ComboBox
                                {
                                    DropDownStyle = ComboBoxStyle.DropDownList,
                                    Width = 200,
                                    Location = new Point(80, 37),
                                    Tag = hourlyReport
                                };
                                
                                // Add sample subjects
                                subjectDropdown.Items.Add("Select Subject/Topic");
                                subjectDropdown.Items.Add("Fachquali FISI-02 - Datenbanken");
                                subjectDropdown.Items.Add("Programmierung - C#");
                                subjectDropdown.Items.Add("Netzwerktechnik - TCP/IP");
                                
                                subjectDropdown.SelectedIndex = 0;
                                if (hourlyReport.SubjectId > 0)
                                {
                                    // Select the appropriate subject based on ID
                                    subjectDropdown.SelectedIndex = Math.Min(hourlyReport.SubjectId, 3);
                                }
                                
                                subjectDropdown.SelectedIndexChanged += SubjectDropdown_SelectedIndexChanged;
                                hourPanel.Controls.Add(subjectDropdown);
                                
                                // Description label
                                var descLabel = new Label
                                {
                                    Text = "Description:",
                                    AutoSize = true,
                                    Location = new Point(290, 10)
                                };
                                hourPanel.Controls.Add(descLabel);
                                
                                // Description textbox
                                var descBox = new TextBox
                                {
                                    Width = hourPanel.Width - 400,
                                    Location = new Point(290, 37),
                                    Text = hourlyReport.LearningDescription ?? "",
                                    Tag = hourlyReport
                                };
                                descBox.TextChanged += DescriptionBox_TextChanged;
                                hourPanel.Controls.Add(descBox);
                                
                                hourlyPanel.Controls.Add(hourPanel);
                                yPos += 80; // Space between entries
                            }
                        }
                    }
                }
                
                // Update weekly summary
                weeklySummaryBox.Text = currentReport.WeeklySummary ?? "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating UI: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void SubjectDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating subject: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void DescriptionBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                var textBox = (TextBox)sender;
                var hourlyReport = (HourlyReport)textBox.Tag;
                
                hourlyReport.LearningDescription = textBox.Text;
                hourlyReport.LastUpdated = DateTime.Now;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating description: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void DailySummaryBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                var textBox = (TextBox)sender;
                var dailyReport = (DailyReport)textBox.Tag;
                
                dailyReport.DailySummary = textBox.Text;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating daily summary: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private async void SaveLocalButton_Click(object sender, EventArgs e)
        {
            try
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
                }
                else
                {
                    statusLabel.Text = "Error saving report";
                    MessageBox.Show("Failed to save the report locally.", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    MessageBox.Show("Failed to submit the report to the server.", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error submitting report: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        
        // Add a method to format the calendar with status colors
        private void StatusFormatCalendar()
        {
            // This is a placeholder for a real implementation
            // In a complete implementation, you would:
            // 1. Create bold dates for days with reports
            // 2. Create a custom calendar control to show different colors
            
            if (currentReport?.DailyReports != null)
            {
                DateTime[] boldDates = currentReport.DailyReports
                    .Where(dr => dr != null)
                    .Select(dr => dr.Date)
                    .ToArray();
                    
                if (boldDates.Length > 0)
                {
                    calendar.BoldedDates = boldDates;
                }
            }
        }
    }
} 