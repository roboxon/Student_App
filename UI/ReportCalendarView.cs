using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Student_App.Models;
using Student_App.Services.Reports;

namespace Student_App.UI
{
    public class ReportCalendarView : UserControl
    {
        private MonthCalendar calendar;
        private Student currentStudent;
        private Dictionary<DateTime, ReportStatus> statusCache = new Dictionary<DateTime, ReportStatus>();
        
        // Event for when a week is selected
        public event EventHandler<DateTime> WeekSelected;
        
        public ReportCalendarView(Student student)
        {
            currentStudent = student;
            InitializeComponent();
            ConfigureCalendar();
            LoadReportData();
        }
        
        private void InitializeComponent()
        {
            this.Size = new Size(300, 300);
            
            calendar = new MonthCalendar
            {
                Dock = DockStyle.Fill,
                MaxSelectionCount = 7,
                ShowWeekNumbers = true,
                FirstDayOfWeek = Day.Monday,
                CalendarDimensions = new Size(1, 2)
            };
            
            calendar.DateSelected += Calendar_DateSelected;
            calendar.DateChanged += Calendar_DateChanged;
            
            this.Controls.Add(calendar);
        }
        
        private void ConfigureCalendar()
        {
            // Set min/max dates based on student enrollment
            DateTime firstValidWeek = ReportDateValidator.GetFirstValidWeek(currentStudent);
            DateTime lastValidWeek = ReportDateValidator.GetLastValidWeek(currentStudent);
            
            calendar.MinDate = firstValidWeek;
            calendar.MaxDate = lastValidWeek.AddDays(6); // End of last week
            
            // Start with current week selected
            DateTime currentWeekStart = GetCurrentWeekStart();
            calendar.SelectionStart = currentWeekStart;
            calendar.SelectionEnd = currentWeekStart.AddDays(6);
        }
        
        private DateTime GetCurrentWeekStart()
        {
            DateTime now = DateTime.Now.Date;
            int diff = (7 + (now.DayOfWeek - DayOfWeek.Monday)) % 7;
            return now.AddDays(-1 * diff);
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
                
                // Raise the event
                WeekSelected?.Invoke(this, weekStart);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting date: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void Calendar_DateChanged(object sender, DateRangeEventArgs e)
        {
            // This is needed for when the month is changed, to update the visual indicators
            UpdateCalendarDisplay();
        }
        
        private async void LoadReportData()
        {
            // Update the status cache with data for visible months
            var startDate = calendar.SelectionStart.AddMonths(-1); // Previous month
            var endDate = calendar.SelectionStart.AddMonths(2);    // Next month
            
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(7))
            {
                // Only process dates within the valid range
                if (date < calendar.MinDate || date > calendar.MaxDate)
                    continue;
                
                // Check if report exists locally
                WeeklyReport report = await LocalReportStorageManager.GetWeekReportAsync(date);
                
                if (report != null)
                {
                    // Calculate status based on report data
                    ReportStatus status = ReportStatusCalculator.CalculateWeekStatus(report, currentStudent);
                    statusCache[date] = status;
                }
                else
                {
                    // No report exists
                    statusCache[date] = ReportStatus.None;
                }
            }
            
            // Update display with new data
            UpdateCalendarDisplay();
        }
        
        public void UpdateCalendarDisplay()
        {
            // This is a placeholder - in a full implementation, we'd need to:
            // 1. Create a custom MonthCalendar control with custom drawing
            // 2. Override the paint method to draw color indicators
            // 3. Add visual cues for different report statuses
            
            // For now, we'll use the BoldedDates property to mark dates with reports
            List<DateTime> boldedDates = new List<DateTime>();
            
            foreach (var entry in statusCache)
            {
                if (entry.Value != ReportStatus.None)
                {
                    // Bold the entire week
                    for (int i = 0; i < 7; i++)
                    {
                        boldedDates.Add(entry.Key.AddDays(i));
                    }
                }
            }
            
            calendar.BoldedDates = boldedDates.ToArray();
        }
        
        // Call this when a report is saved or status changes
        public void RefreshCalendar()
        {
            LoadReportData();
        }
    }
} 