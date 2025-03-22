using System.Drawing;
using System.Windows.Forms;
using Student_App.Services.Configuration;
using Student_App.Forms;

namespace Student_App.Forms
{
    public partial class Dashboard : LayoutForm
    {
        private Panel statsPanel = new();
        private Panel activityPanel = new();
        private Panel schedulePanel = new();
        private Student? currentStudent;
        private List<WorkingDay>? workingDays;

        public Dashboard(Student? student = null, List<WorkingDay>? days = null)
        {
            currentStudent = student;
            workingDays = days;
            InitializeComponent();
            InitializeDashboardControls();
            SetActiveMenuItem("Dashboard");
        }

        protected override void InitializeComponent()
        {
            base.InitializeComponent();
            this.Text = "Student Dashboard";
            if (currentStudent != null)
            {
                UpdateUserInfo($"{currentStudent.first_name} {currentStudent.last_name}");
            }
        }

        private void InitializeDashboardControls()
        {
            // Stats Panel
            statsPanel = new Panel
            {
                Height = 100,
                Dock = DockStyle.Top,
                Padding = new Padding(10)
            };

            var statsCards = new[] {
                ("Course", currentStudent?.plan_name ?? "N/A"),
                ("Group", currentStudent?.group_name ?? "N/A"),
                ("Grade", currentStudent?.grade_score?.ToString("F1") ?? "N/A"),
                ("Status", currentStudent?.is_active == 1 ? "Active" : "Inactive")
            };

            int cardWidth = (mainContentPanel.Width - 60) / 4;
            int xPos = 0;

            foreach (var (title, value) in statsCards)
            {
                var card = CreateStatsCard(title, value, cardWidth);
                card.Location = new Point(xPos, 0);
                statsPanel.Controls.Add(card);
                xPos += cardWidth + 20;
            }

            // Activity Panel
            activityPanel = new Panel
            {
                Height = 300,
                Dock = DockStyle.Top,
                Padding = new Padding(10),
                Margin = new Padding(0, 20, 0, 0)
            };

            var activityTitle = new Label
            {
                Text = "Student Information",
                Font = AppFonts.Heading,
                AutoSize = true,
                Location = new Point(0, 0)
            };

            var infoList = new ListView
            {
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Location = new Point(0, 40),
                Size = new Size(mainContentPanel.Width - 40, 240),
                Font = AppFonts.Body
            };

            infoList.Columns.Add("Field", 150);
            infoList.Columns.Add("Value", 300);

            if (currentStudent != null)
            {
                var items = new[]
                {
                    new ListViewItem(new[] { "Email", currentStudent.email ?? "N/A" }),
                    new ListViewItem(new[] { "Mentor", currentStudent.mentor_name ?? "N/A" }),
                    new ListViewItem(new[] { "Advisor", currentStudent.advisor_name ?? "N/A" }),
                    new ListViewItem(new[] { "Course Start", currentStudent.start_date ?? "N/A" }),
                    new ListViewItem(new[] { "Course End", currentStudent.end_date ?? "N/A" }),
                    new ListViewItem(new[] { "Program ID", currentStudent.program_id.ToString() }),
                    new ListViewItem(new[] { "Branch ID", currentStudent.branch_id.ToString() }),
                    new ListViewItem(new[] { "Release ID", currentStudent.release_id.ToString() })
                };
                infoList.Items.AddRange(items);
            }

            activityPanel.Controls.Add(activityTitle);
            activityPanel.Controls.Add(infoList);

            // Schedule Panel
            schedulePanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            var scheduleTitle = new Label
            {
                Text = "Working Schedule",
                Font = AppFonts.Heading,
                AutoSize = true,
                Location = new Point(0, 0)
            };

            var scheduleList = new ListView
            {
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Location = new Point(0, 40),
                Size = new Size(mainContentPanel.Width - 40, mainContentPanel.Height - 500),
                Font = AppFonts.Body
            };

            scheduleList.Columns.Add("Day", 150);
            scheduleList.Columns.Add("Start Time", 150);
            scheduleList.Columns.Add("End Time", 150);

            if (workingDays != null)
            {
                var scheduleItems = workingDays.Select(day => 
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

            // Add all panels to main content
            mainContentPanel.Controls.Add(schedulePanel);
            mainContentPanel.Controls.Add(activityPanel);
            mainContentPanel.Controls.Add(statsPanel);
        }

        private Panel CreateStatsCard(string title, string value, int width)
        {
            var card = new Panel
            {
                Width = width,
                Height = 80,
                BackColor = Color.White
            };

            // Add shadow effect
            card.Paint += (s, e) =>
            {
                var rect = new Rectangle(0, 0, card.Width - 1, card.Height - 1);
                using (var pen = new Pen(Color.FromArgb(20, 0, 0, 0), 1))
                {
                    e.Graphics.DrawRectangle(pen, rect);
                }
            };

            var titleLabel = new Label
            {
                Text = title,
                Font = AppFonts.Body,
                ForeColor = AppColors.Text,
                Location = new Point(15, 15),
                AutoSize = true
            };

            var valueLabel = new Label
            {
                Text = value,
                Font = new Font(AppFonts.Heading.FontFamily, 16, FontStyle.Bold),
                ForeColor = AppColors.Secondary,
                Location = new Point(15, 40),
                AutoSize = true
            };

            card.Controls.Add(titleLabel);
            card.Controls.Add(valueLabel);

            return card;
        }
    }
} 