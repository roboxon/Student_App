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
            
            // Main container with three sections
            var mainContainer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                BackColor = Color.White
            };
            
            mainContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            mainContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            mainContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 70F));
            mainContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));
            
            // 1. LEFT PANEL: Calendar and status
            calendarPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = AppColors.LightGray,
                Padding = new Padding(10)
            };
            
            var instructionsLabel = new Label
            {
                Text = "Select a week to create/edit reports:",
                AutoSize = true,
                Font = AppFonts.BoldSmall,
                Location = new Point(10, 10)
            };
            calendarPanel.Controls.Add(instructionsLabel);
            
            // Create single calendar
            calendar = new MonthCalendar
            {
                MaxSelectionCount = 7,
                ShowWeekNumbers = true,
                FirstDayOfWeek = Day.Monday,
                CalendarDimensions = new Size(1, 1),
                Location = new Point(10, 40)
            };
            
            calendar.DateSelected += Calendar_DateSelected;
            calendarPanel.Controls.Add(calendar);
            
            // Status indicators for calendar
            var statusPanel = new Panel
            {
                Width = 200,
                Height = 120,
                Location = new Point(10, 230)
            };
            
            AddStatusIndicator(statusPanel, AppColors.Success, "Complete", 0);
            AddStatusIndicator(statusPanel, Color.Orange, "Partial", 30);
            AddStatusIndicator(statusPanel, AppColors.Secondary, "Not Started", 60);
            calendarPanel.Controls.Add(statusPanel);
            
            // 2. TOP RIGHT: Daily tabs
            var tabsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };
            
            weekRangeLabel = new Label
            {
                Text = $"Week of: {selectedWeekStart:MMM dd, yyyy} to {selectedWeekStart.AddDays(6):MMM dd, yyyy}",
                Font = AppFonts.Bold,
                AutoSize = true,
                Location = new Point(0, 10)
            };
            tabsPanel.Controls.Add(weekRangeLabel);
            
            dailyTabs = new TabControl
            {
                Dock = DockStyle.Fill,
                Location = new Point(0, 40),
                Size = new Size(tabsPanel.Width, tabsPanel.Height - 50)
            };
            tabsPanel.Controls.Add(dailyTabs);
            
            // 3. BOTTOM RIGHT: Weekly summary and buttons
            var summaryPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };
            
            var summaryLabel = new Label
            {
                Text = "Weekly Summary:",
                Font = AppFonts.Bold,
                AutoSize = true,
                Location = new Point(0, 10)
            };
            summaryPanel.Controls.Add(summaryLabel);
            
            weeklySummaryBox = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Location = new Point(0, 40),
                Size = new Size(summaryPanel.Width - 20, 60)
            };
            summaryPanel.Controls.Add(weeklySummaryBox);
            
            // Action buttons
            var buttonsPanel = new Panel
            {
                Width = summaryPanel.Width - 20,
                Height = 40,
                Location = new Point(0, 110)
            };
            
            saveLocalButton = new Button
            {
                Text = "Save Report Locally",
                Width = 150,
                Height = 35,
                BackColor = AppColors.Primary,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(0, 0)
            };
            saveLocalButton.Click += SaveLocalButton_Click;
            
            submitButton = new Button
            {
                Text = "Submit Report",
                Width = 150,
                Height = 35,
                BackColor = AppColors.Success,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(160, 0)
            };
            submitButton.Click += SubmitButton_Click;
            
            statusLabel = new Label
            {
                Text = "Select a week to get started",
                AutoSize = true,
                Location = new Point(320, 10),
                ForeColor = AppColors.Secondary
            };
            
            buttonsPanel.Controls.Add(saveLocalButton);
            buttonsPanel.Controls.Add(submitButton);
            buttonsPanel.Controls.Add(statusLabel);
            summaryPanel.Controls.Add(buttonsPanel);
            
            // Add panels to table layout
            mainContainer.Controls.Add(calendarPanel, 0, 0);
            mainContainer.Controls.Add(tabsPanel, 1, 0);
            mainContainer.Controls.Add(summaryPanel, 1, 1);
            mainContainer.SetRowSpan(calendarPanel, 2);
            
            // Add to main content
            mainContentPanel.Controls.Add(mainContainer);
            
            // Load initial report
            LoadWeeklyReport();
        }
        
        // Helper method to add status indicators
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

                // Clear existing tabs
                dailyTabs.TabPages.Clear();
                
                // Add tabs for each daily report
                foreach (var dailyReport in currentReport.DailyReports)
                {
                    // Format the day name
                    string dayName = dailyReport.Date.ToString("dddd, MMM dd");
                    var tabPage = new TabPage(dayName);
                    
                    // Container with padding
                    var container = new Panel
                    {
                        Dock = DockStyle.Fill,
                        Padding = new Padding(15)
                    };
                    
                    // Daily summary
                    var dailySummaryLabel = new Label
                    {
                        Text = "Daily Summary:",
                        Font = AppFonts.Bold,
                        Location = new Point(0, 10),
                        AutoSize = true
                    };
                    
                    var dailySummaryBox = new TextBox
                    {
                        Width = tabPage.Width - 40,
                        Height = 60,
                        Multiline = true,
                        Location = new Point(0, 40),
                        Text = dailyReport.DailySummary ?? "",
                        Tag = dailyReport  // Store reference to the daily report
                    };
                    
                    dailySummaryBox.TextChanged += DailySummaryBox_TextChanged;
                    
                    // Hourly reports header
                    var hourlyLabel = new Label
                    {
                        Text = "Hourly Reports:",
                        Font = AppFonts.Bold,
                        Location = new Point(0, 110),
                        AutoSize = true
                    };
                    
                    // Hourly reports container with scroll
                    var hourlyContainer = new Panel
                    {
                        Width = tabPage.Width - 40,
                        Height = 400,
                        Location = new Point(0, 140),
                        AutoScroll = true,
                        BorderStyle = BorderStyle.FixedSingle
                    };
                    
                    int yPos = 10;
                    
                    // Add hourly report rows
                    if (dailyReport.HourlyReports != null)
                    {
                        foreach (var hourlyReport in dailyReport.HourlyReports)
                        {
                            if (hourlyReport != null)
                            {
                                // Hour row panel
                                var hourPanel = new Panel
                                {
                                    Width = hourlyContainer.Width - 25,
                                    Height = 85,
                                    Location = new Point(5, yPos),
                                    BackColor = Color.FromArgb(250, 250, 250),
                                    BorderStyle = BorderStyle.FixedSingle
                                };
                                
                                // Time label
                                var timeLabel = new Label
                                {
                                    Text = hourlyReport.TimeRange ?? "Unknown time",
                                    Width = 100,
                                    Location = new Point(10, 10),
                                    Font = AppFonts.Bold
                                };
                                
                                // Subject/Topic dropdown
                                var subjectDropdown = new ComboBox
                                {
                                    Width = 220,
                                    Location = new Point(120, 8),
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
                                    subjectDropdown.SelectedIndex = hourlyReport.SubjectId % 3 + 1;
                                }
                                
                                subjectDropdown.SelectedIndexChanged += SubjectDropdown_SelectedIndexChanged;
                                
                                // Learning description
                                var descriptionLabel = new Label
                                {
                                    Text = "Description:",
                                    Location = new Point(10, 45),
                                    AutoSize = true
                                };
                                
                                // Description textbox
                                var descriptionBox = new TextBox
                                {
                                    Width = hourPanel.Width - 150,
                                    Height = 25,
                                    Location = new Point(120, 42),
                                    Text = hourlyReport.LearningDescription ?? "",
                                    Tag = hourlyReport  // Store reference to the hourly report
                                };
                                
                                descriptionBox.TextChanged += DescriptionBox_TextChanged;
                                
                                // Add controls to hour panel
                                hourPanel.Controls.Add(timeLabel);
                                hourPanel.Controls.Add(subjectDropdown);
                                hourPanel.Controls.Add(descriptionLabel);
                                hourPanel.Controls.Add(descriptionBox);
                                
                                // Add hour panel to container
                                hourlyContainer.Controls.Add(hourPanel);
                            }
                            
                            yPos += 95;
                        }
                    }
                    
                    container.Controls.Add(dailySummaryLabel);
                    container.Controls.Add(dailySummaryBox);
                    container.Controls.Add(hourlyLabel);
                    container.Controls.Add(hourlyContainer);
                    
                    // Add container to tab
                    tabPage.Controls.Add(container);
                    dailyTabs.TabPages.Add(tabPage);
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
    }
} 