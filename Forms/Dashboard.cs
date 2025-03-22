using System.Drawing;
using System.Windows.Forms;
using Student_App.Services.Configuration;

namespace Student_App.Forms
{
    public partial class Dashboard : LayoutForm
    {
        private Panel statsPanel;
        private Panel activityPanel;
        private Panel schedulePanel;

        public Dashboard()
        {
            InitializeComponent();
            InitializeDashboardControls();
            SetActiveMenuItem("Dashboard");
        }

        protected override void InitializeComponent()
        {
            base.InitializeComponent();
            this.Text = "Student Dashboard";
            UpdateUserInfo("John Doe"); // This should be updated with actual user info
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
                ("Total Hours", "120"),
                ("Attendance", "95%"),
                ("Tasks", "15/20"),
                ("Grade", "A")
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
                Text = "Recent Activity",
                Font = AppFonts.Heading,
                AutoSize = true,
                Location = new Point(0, 0)
            };

            var activityList = new ListView
            {
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Location = new Point(0, 40),
                Size = new Size(mainContentPanel.Width - 40, 240),
                Font = AppFonts.Body
            };

            activityList.Columns.Add("Date", 150);
            activityList.Columns.Add("Activity", 300);
            activityList.Columns.Add("Status", 150);

            // Add sample data
            var items = new[]
            {
                new ListViewItem(new[] { "2024-03-20", "Submitted Assignment #3", "Completed" }),
                new ListViewItem(new[] { "2024-03-19", "Attended Workshop", "Present" }),
                new ListViewItem(new[] { "2024-03-18", "Project Meeting", "Completed" })
            };
            activityList.Items.AddRange(items);

            activityPanel.Controls.Add(activityTitle);
            activityPanel.Controls.Add(activityList);

            // Schedule Panel
            schedulePanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            var scheduleTitle = new Label
            {
                Text = "Upcoming Schedule",
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

            scheduleList.Columns.Add("Time", 150);
            scheduleList.Columns.Add("Subject", 300);
            scheduleList.Columns.Add("Room", 150);

            // Add sample data
            var scheduleItems = new[]
            {
                new ListViewItem(new[] { "09:00 - 10:30", "Programming Fundamentals", "Room 101" }),
                new ListViewItem(new[] { "11:00 - 12:30", "Database Design", "Room 203" }),
                new ListViewItem(new[] { "14:00 - 15:30", "Web Development", "Room 105" })
            };
            scheduleList.Items.AddRange(scheduleItems);

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