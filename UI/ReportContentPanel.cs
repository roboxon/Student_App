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
        private Student currentStudent;
        private WeeklyReport currentReport;
        private DateTime selectedWeekStart;
        
        // UI Controls
        private MonthCalendar calendar;
        private TabControl dailyTabs;
        private TextBox weeklySummaryBox;
        private Button saveLocalButton;
        private Button submitButton;
        private Label statusLabel;
        private Label weekRangeLabel;
        
        // Add a field to store the curriculum data
        private Release curriculumRelease;
        private List<SubjectWithTopics> curriculumSubjects;
        
        public ReportContentPanel(Student student)
        {
            currentStudent = student;
            selectedWeekStart = GetCurrentWeekStart();
            curriculumSubjects = new List<SubjectWithTopics>();
            
            InitializeComponent();
            LoadCurriculumData(); // This will load the subjects and topics
        }
        
        private DateTime GetCurrentWeekStart()
        {
            DateTime now = DateTime.Now.Date;
            int diff = (7 + (now.DayOfWeek - DayOfWeek.Monday)) % 7;
            return now.AddDays(-1 * diff);
        }
        
        private void InitializeComponent()
        {
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
            
            // Add the split container to this UserControl
            this.Controls.Add(mainContainer);
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

        private async void LoadWeeklyReport()
        {
            try
            {
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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading report: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void LoadCurriculumData()
        {
            try
            {
                // Show loading indicator
                statusLabel.Text = "Loading curriculum data...";
                
                // Load release data from the student's release_id
                curriculumRelease = await Release.GetReleaseAsync(currentStudent);
                
                if (curriculumRelease?.content?.subjects != null)
                {
                    curriculumSubjects = curriculumRelease.content.subjects;
                    statusLabel.Text = $"Loaded {curriculumSubjects.Count} subjects";
                }
                else
                {
                    statusLabel.Text = "No curriculum data available";
                }
                
                // Now load the weekly report
                LoadWeeklyReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading curriculum: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                // Still try to load report data
                LoadWeeklyReport();
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
                foreach (TabPage tab in dailyTabs.TabPages)
                {
                    tab.Controls.Clear();
                }

                // Update tabs for each daily report
                for (int i = 0; i < Math.Min(dailyTabs.TabPages.Count, currentReport.DailyReports.Count); i++)
                {
                    var tabPage = dailyTabs.TabPages[i];
                    var dailyReport = currentReport.DailyReports[i];
                    
                    // Main container for tab content
                    var container = new Panel
                    {
                        Dock = DockStyle.Fill,
                        AutoScroll = true,
                        Padding = new Padding(10)
                    };
                    
                    // Daily summary at the top
                    var summaryLabel = new Label
                    {
                        Text = "Daily Summary:",
                        Font = AppFonts.Bold,
                        AutoSize = true,
                        Location = new Point(10, 10)
                    };
                    container.Controls.Add(summaryLabel);
                    
                    var summaryBox = new TextBox
                    {
                        Multiline = true,
                        ScrollBars = ScrollBars.Vertical,
                        Location = new Point(10, 35),
                        Size = new Size(tabPage.Width - 40, 70),
                        Text = dailyReport?.DailySummary ?? "",
                        Tag = dailyReport
                    };
                    summaryBox.TextChanged += DailySummaryBox_TextChanged;
                    container.Controls.Add(summaryBox);
                    
                    // Hourly reports section
                    var hourlyLabel = new Label
                    {
                        Text = "Hourly Reports:",
                        Font = AppFonts.Bold,
                        AutoSize = true,
                        Location = new Point(10, 115)
                    };
                    container.Controls.Add(hourlyLabel);
                    
                    // Panel for hourly reports
                    var hourlyPanel = new Panel
                    {
                        AutoScroll = true,
                        Location = new Point(10, 140),
                        Size = new Size(tabPage.Width - 40, 350),
                        BorderStyle = BorderStyle.FixedSingle
                    };
                    container.Controls.Add(hourlyPanel);
                    
                    int yPosition = 10;
                    
                    // Add hourly report entries
                    if (dailyReport?.HourlyReports != null)
                    {
                        foreach (var hourlyReport in dailyReport.HourlyReports)
                        {
                            if (hourlyReport != null)
                            {
                                // Create a panel for each hour
                                var hourPanel = new Panel
                                {
                                    Width = hourlyPanel.Width - 25,
                                    Height = 130,
                                    Location = new Point(5, yPosition),
                                    BackColor = Color.FromArgb(248, 248, 248),
                                    BorderStyle = BorderStyle.FixedSingle
                                };
                                
                                // Time range
                                var timeLabel = new Label
                                {
                                    Text = hourlyReport.TimeRange ?? $"{hourlyReport.StartTime}-{hourlyReport.EndTime}",
                                    Font = AppFonts.Bold,
                                    AutoSize = true,
                                    Location = new Point(10, 10)
                                };
                                hourPanel.Controls.Add(timeLabel);
                                
                                // Subject selection
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
                                    Width = 250,
                                    Location = new Point(80, 37),
                                    Tag = hourlyReport
                                };
                                
                                // Add subjects from curriculum
                                subjectDropdown.Items.Add("Select a subject");
                                
                                foreach (var subjectWithTopics in curriculumSubjects)
                                {
                                    if (subjectWithTopics?.subject != null)
                                    {
                                        string subjectName = subjectWithTopics.subject.subject_name ?? "Unknown Subject";
                                        subjectDropdown.Items.Add(new SubjectItem(
                                            subjectWithTopics.subject.id, 
                                            subjectName,
                                            subjectWithTopics
                                        ));
                                    }
                                }
                                
                                // Set selected subject if exists
                                subjectDropdown.SelectedIndex = 0;
                                if (hourlyReport.SubjectId > 0)
                                {
                                    for (int j = 1; j < subjectDropdown.Items.Count; j++)
                                    {
                                        if (subjectDropdown.Items[j] is SubjectItem subItem && 
                                            subItem.Id == hourlyReport.SubjectId)
                                        {
                                            subjectDropdown.SelectedIndex = j;
                                            break;
                                        }
                                    }
                                }
                                
                                subjectDropdown.SelectedIndexChanged += SubjectDropdown_SelectedIndexChanged;
                                hourPanel.Controls.Add(subjectDropdown);
                                
                                // Topic selection
                                var topicLabel = new Label
                                {
                                    Text = "Topic:",
                                    AutoSize = true,
                                    Location = new Point(10, 70)
                                };
                                hourPanel.Controls.Add(topicLabel);
                                
                                var topicDropdown = new ComboBox
                                {
                                    DropDownStyle = ComboBoxStyle.DropDownList,
                                    Width = 250,
                                    Location = new Point(80, 67),
                                    Tag = hourlyReport,
                                    Enabled = subjectDropdown.SelectedIndex > 0
                                };
                                
                                // Add initial placeholder
                                topicDropdown.Items.Add("Select a topic");
                                topicDropdown.SelectedIndex = 0;
                                
                                // If a subject is selected, populate topics
                                if (subjectDropdown.SelectedItem is SubjectItem selectedSubject && 
                                    selectedSubject.SubjectWithTopics?.topics != null)
                                {
                                    foreach (var topicWithLessons in selectedSubject.SubjectWithTopics.topics)
                                    {
                                        if (topicWithLessons?.topic != null)
                                        {
                                            string topicName = topicWithLessons.topic.topic_name ?? "Unknown Topic";
                                            topicDropdown.Items.Add(new TopicItem(
                                                topicWithLessons.topic.id,
                                                topicName,
                                                topicWithLessons
                                            ));
                                            
                                            // Set selected if matches
                                            if (hourlyReport.TopicId == topicWithLessons.topic.id)
                                            {
                                                topicDropdown.SelectedIndex = topicDropdown.Items.Count - 1;
                                            }
                                        }
                                    }
                                }
                                
                                topicDropdown.SelectedIndexChanged += TopicDropdown_SelectedIndexChanged;
                                hourPanel.Controls.Add(topicDropdown);
                                
                                // Link subject and topic dropdowns
                                subjectDropdown.Tag = new DropdownPair(hourlyReport, topicDropdown);
                                
                                // Description
                                var descLabel = new Label
                                {
                                    Text = "Description:",
                                    AutoSize = true,
                                    Location = new Point(10, 100)
                                };
                                hourPanel.Controls.Add(descLabel);
                                
                                var descBox = new TextBox
                                {
                                    Width = hourPanel.Width - 100,
                                    Location = new Point(80, 97),
                                    Text = hourlyReport.LearningDescription ?? "",
                                    Tag = hourlyReport
                                };
                                descBox.TextChanged += DescriptionBox_TextChanged;
                                hourPanel.Controls.Add(descBox);
                                
                                hourlyPanel.Controls.Add(hourPanel);
                                yPosition += 140; // Space between hour panels
                            }
                        }
                    }
                    
                    tabPage.Controls.Add(container);
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

        private void SaveLocalButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Update weekly summary
                currentReport.WeeklySummary = weeklySummaryBox.Text;
                
                // Update totals
                currentReport.UpdateTotals();
                
                // Save locally
                LocalReportStorageManager.SaveWeekReportAsync(selectedWeekStart, currentReport).ContinueWith(task =>
                {
                    if (task.Result)
                    {
                        statusLabel.Text = "Report saved locally";
                    }
                    else
                    {
                        statusLabel.Text = "Error saving report";
                        MessageBox.Show("Failed to save the report locally.", "Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                });
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
                // Silent error handling for UI events
            }
        }

        private void SubjectDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var dropdown = (ComboBox)sender;
                
                if (dropdown.Tag is DropdownPair pair)
                {
                    var report = pair.Report;
                    var topicDropdown = pair.TopicDropdown;
                    
                    // Clear existing topics
                    topicDropdown.Items.Clear();
                    topicDropdown.Items.Add("Select a topic");
                    topicDropdown.SelectedIndex = 0;
                    
                    if (dropdown.SelectedIndex > 0 && dropdown.SelectedItem is SubjectItem selectedSubject)
                    {
                        // Update the report with selected subject
                        report.SubjectId = selectedSubject.Id;
                        report.SubjectName = selectedSubject.Name;
                        report.LastUpdated = DateTime.Now;
                        
                        // Enable topic dropdown
                        topicDropdown.Enabled = true;
                        
                        // Add topics for this subject
                        if (selectedSubject.SubjectWithTopics?.topics != null)
                        {
                            foreach (var topicWithLessons in selectedSubject.SubjectWithTopics.topics)
                            {
                                if (topicWithLessons?.topic != null)
                                {
                                    string topicName = topicWithLessons.topic.topic_name ?? "Unknown Topic";
                                    topicDropdown.Items.Add(new TopicItem(
                                        topicWithLessons.topic.id,
                                        topicName,
                                        topicWithLessons
                                    ));
                                }
                            }
                        }
                    }
                    else
                    {
                        // Clear subject selection
                        report.SubjectId = 0;
                        report.SubjectName = null;
                        
                        // Disable topic dropdown
                        topicDropdown.Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                // Silent handling for UI events
                System.Diagnostics.Debug.WriteLine($"Error in subject dropdown: {ex.Message}");
            }
        }

        private void TopicDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var dropdown = (ComboBox)sender;
                var report = (HourlyReport)dropdown.Tag;
                
                if (dropdown.SelectedIndex > 0 && dropdown.SelectedItem is TopicItem selectedTopic)
                {
                    // Update the report with selected topic
                    report.TopicId = selectedTopic.Id;
                    report.TopicName = selectedTopic.Name;
                    report.LastUpdated = DateTime.Now;
                }
                else
                {
                    // Clear topic selection
                    report.TopicId = 0;
                    report.TopicName = null;
                }
            }
            catch (Exception ex)
            {
                // Silent handling for UI events
                System.Diagnostics.Debug.WriteLine($"Error in topic dropdown: {ex.Message}");
            }
        }

        private void DescriptionBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                var textBox = (TextBox)sender;
                var hourlyReport = (HourlyReport)textBox.Tag;
                
                hourlyReport.LearningDescription = textBox.Text;
            }
            catch (Exception ex)
            {
                // Silent error handling for UI events
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

        // Move nested helper classes here
        private class SubjectItem
        {
            public int Id { get; }
            public string Name { get; }
            public SubjectWithTopics SubjectWithTopics { get; }
            
            public SubjectItem(int id, string name, SubjectWithTopics subjectWithTopics)
            {
                Id = id;
                Name = name;
                SubjectWithTopics = subjectWithTopics;
            }
            
            public override string ToString() => Name;
        }
        
        private class TopicItem
        {
            public int Id { get; }
            public string Name { get; }
            public TopicWithLessons TopicWithLessons { get; }
            
            public TopicItem(int id, string name, TopicWithLessons topicWithLessons)
            {
                Id = id;
                Name = name;
                TopicWithLessons = topicWithLessons;
            }
            
            public override string ToString() => Name;
        }
        
        private class PrimarySubjectData
        {
            public DailyReport DailyReport { get; }
            public List<ComboBox> TopicDropdowns { get; }
            
            public PrimarySubjectData(DailyReport dailyReport, List<ComboBox> topicDropdowns)
            {
                DailyReport = dailyReport;
                TopicDropdowns = topicDropdowns;
            }
        }

        // Helper class to link dropdowns
        private class DropdownPair
        {
            public HourlyReport Report { get; }
            public ComboBox TopicDropdown { get; }
            
            public DropdownPair(HourlyReport report, ComboBox topicDropdown)
            {
                Report = report;
                TopicDropdown = topicDropdown;
            }
        }
    }
} 