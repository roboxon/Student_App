using System;
using System.Drawing;
using System.Windows.Forms;
using Student_App.Services.Configuration;

namespace Student_App
{
    public partial class LayoutForm : Form
    {
        private bool disposedValue;
        protected Panel mainContentPanel;
        protected Panel headerPanel;
        protected Panel sideMenuPanel;
        protected Panel footerPanel;
        protected Label titleLabel;
        protected Label userLabel;
        protected Panel contentWrapper;

        public LayoutForm()
        {
            InitializeComponent();
            InitializeLayoutComponents();
        }

        protected virtual void InitializeComponent()
        {
            this.Text = "Student App";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = AppColors.Background;
            this.MinimumSize = new Size(800, 600);
        }

        protected virtual void InitializeLayoutComponents()
        {
            // Header Panel
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = AppColors.Primary
            };

            titleLabel = new Label
            {
                Text = "Student App",
                ForeColor = Color.White,
                Font = AppFonts.Title,
                AutoSize = true,
                Location = new Point(20, 15)
            };

            userLabel = new Label
            {
                Text = "Welcome",
                ForeColor = Color.White,
                Font = AppFonts.Body,
                AutoSize = true,
                Location = new Point(headerPanel.Width - 150, 20)
            };

            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(userLabel);

            // Side Menu Panel
            sideMenuPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 200,
                BackColor = Color.FromArgb(51, 51, 54)
            };

            // Add menu items
            var menuItems = new string[] { "Dashboard", "Reports", "Attendance", "Schedule", "Profile" };
            var yPos = 20;
            foreach (var item in menuItems)
            {
                var menuButton = new Button
                {
                    Text = item,
                    FlatStyle = FlatStyle.Flat,
                    ForeColor = Color.White,
                    Font = AppFonts.Body,
                    Size = new Size(180, 40),
                    Location = new Point(10, yPos),
                    TextAlign = ContentAlignment.MiddleLeft,
                    Padding = new Padding(20, 0, 0, 0),
                    BackColor = Color.Transparent
                };
                menuButton.FlatAppearance.BorderSize = 0;
                menuButton.MouseEnter += (s, e) => menuButton.BackColor = AppColors.Secondary;
                menuButton.MouseLeave += (s, e) => menuButton.BackColor = Color.Transparent;

                sideMenuPanel.Controls.Add(menuButton);
                yPos += 50;
            }

            // Content Wrapper (holds main content)
            contentWrapper = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                BackColor = AppColors.Background
            };

            // Main Content Panel
            mainContentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            // Footer Panel
            footerPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 30,
                BackColor = AppColors.Primary
            };

            var versionLabel = new Label
            {
                Text = "Version 1.0",
                ForeColor = Color.White,
                Font = AppFonts.Small,
                AutoSize = true,
                Location = new Point(10, 8)
            };

            var statusLabel = new Label
            {
                Text = "Connected",
                ForeColor = AppColors.Success,
                Font = AppFonts.Small,
                AutoSize = true,
                Location = new Point(footerPanel.Width - 100, 8)
            };

            footerPanel.Controls.Add(versionLabel);
            footerPanel.Controls.Add(statusLabel);

            // Add shadow effect to main content
            mainContentPanel.Paint += (s, e) =>
            {
                var rect = new Rectangle(0, 0, mainContentPanel.Width - 1, mainContentPanel.Height - 1);
                using (var pen = new Pen(Color.FromArgb(20, 0, 0, 0), 1))
                {
                    e.Graphics.DrawRectangle(pen, rect);
                }
            };

            // Add rounded corners to main content
            mainContentPanel.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, mainContentPanel.Width, mainContentPanel.Height, 10, 10));
            mainContentPanel.Resize += (s, e) =>
            {
                mainContentPanel.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, mainContentPanel.Width, mainContentPanel.Height, 10, 10));
            };

            // Add controls in the correct order
            contentWrapper.Controls.Add(mainContentPanel);
            this.Controls.Add(footerPanel);
            this.Controls.Add(contentWrapper);
            this.Controls.Add(sideMenuPanel);
            this.Controls.Add(headerPanel);

            // Handle form resize
            this.Resize += (s, e) =>
            {
                userLabel.Location = new Point(headerPanel.Width - 150, 20);
                statusLabel.Location = new Point(footerPanel.Width - 100, 8);
            };
        }

        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    mainContentPanel?.Dispose();
                    headerPanel?.Dispose();
                    sideMenuPanel?.Dispose();
                    footerPanel?.Dispose();
                    contentWrapper?.Dispose();
                }
                disposedValue = true;
            }
            base.Dispose(disposing);
        }

        // Method to update user info in header
        protected void UpdateUserInfo(string userName)
        {
            if (userLabel != null)
            {
                userLabel.Text = $"Welcome, {userName}";
                userLabel.Location = new Point(headerPanel.Width - userLabel.Width - 20, 20);
            }
        }

        // Method to set active menu item
        protected void SetActiveMenuItem(string menuName)
        {
            foreach (Control control in sideMenuPanel.Controls)
            {
                if (control is Button button)
                {
                    button.BackColor = button.Text == menuName ? AppColors.Secondary : Color.Transparent;
                }
            }
        }
    }
} 