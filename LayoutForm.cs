using System;
using System.Drawing;
using System.Windows.Forms;
using Student_App.UI;
using Student_App.Forms;

namespace Student_App
{
    public partial class LayoutForm : Form
    {
        private bool disposedValue;
        protected Panel mainContentPanel = new();
        protected Panel headerPanel = new();
        protected Panel sideMenuPanel = new();
        protected Panel footerPanel = new();
        protected Label titleLabel = new();
        protected Label userLabel = new();
        protected Panel contentWrapper = new();
        protected Dictionary<string, Button> menuButtons = new();

        public LayoutForm()
        {
            InitializeComponent();
            InitializeLayoutComponents();
        }

        protected virtual void InitializeComponent()
        {
            this.Text = "Student App";
            
            // Replace fixed width with a more flexible size
            // this.Size = new Size(AppLayout.FormWidth, 800);
            this.Size = new Size(AppLayout.DefaultInitialWidth, AppLayout.DefaultInitialHeight);
            
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = AppColors.Background;
            
            // Keep a reasonable minimum size
            this.MinimumSize = new Size(800, 600);
            
            // Adding WindowState maximized for better use of screen space
            this.WindowState = FormWindowState.Maximized;
        }

        protected virtual void InitializeLayoutComponents()
        {
            // Header Panel
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = AppLayout.HeaderHeight;
            headerPanel.BackColor = AppColors.HeaderFooter;
            headerPanel.Padding = new Padding(20);

            titleLabel.Text = "Student App";
            titleLabel.ForeColor = Color.White;
            titleLabel.Font = AppFonts.Title;
            titleLabel.AutoSize = true;
            titleLabel.Location = new Point(20, 15);
            headerPanel.Controls.Add(titleLabel);

            userLabel.Text = "Welcome";
            userLabel.ForeColor = Color.White;
            userLabel.Font = AppFonts.Body;
            userLabel.AutoSize = true;
            userLabel.Location = new Point(headerPanel.Width - 150, 20);
            headerPanel.Controls.Add(userLabel);

            // Side Menu Panel
            sideMenuPanel.Dock = DockStyle.Left;
            sideMenuPanel.Width = 200;
            sideMenuPanel.BackColor = Color.FromArgb(51, 51, 54);
            sideMenuPanel.Padding = new Padding(10);

            // Initialize menu items
            InitializeMenuItems();

            // Content Wrapper (holds main content)
            contentWrapper.Dock = DockStyle.Fill;
            contentWrapper.Padding = new Padding(20);
            contentWrapper.BackColor = AppColors.Background;

            // Main Content Panel
            mainContentPanel.Dock = DockStyle.Fill;
            mainContentPanel.BackColor = Color.White;
            mainContentPanel.Padding = new Padding(20);

            // Footer Panel
            footerPanel.Dock = DockStyle.Bottom;
            footerPanel.Height = AppLayout.FooterHeight;
            footerPanel.BackColor = AppColors.HeaderFooter;

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

        private void InitializeMenuItems()
        {
            var menuItems = new[]
            {
                "Dashboard",
                "Attendance",
                "Reports",
                "Settings"
            };

            int yPos = 20;
            foreach (var item in menuItems)
            {
                var button = new Button
                {
                    Text = item,
                    Width = sideMenuPanel.Width - 20,
                    Height = 40,
                    Location = new Point(10, yPos),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = AppColors.Secondary,
                    ForeColor = Color.White,
                    Font = AppFonts.Body,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Padding = new Padding(10, 0, 0, 0)
                };

                button.FlatAppearance.BorderSize = 0;
                button.Click += (s, e) => HandleMenuClick(item);

                menuButtons[item] = button;
                sideMenuPanel.Controls.Add(button);
                yPos += 50;
            }
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
        protected void SetActiveMenuItem(string menuItem)
        {
            foreach (var button in menuButtons.Values)
            {
                button.BackColor = AppColors.Secondary;
            }

            if (menuButtons.ContainsKey(menuItem))
            {
                menuButtons[menuItem].BackColor = AppColors.Primary;
            }
        }

        protected virtual void HandleMenuClick(string menuItem)
        {
            SetActiveMenuItem(menuItem);
            
            // Let derived classes handle their own menu clicks
            // Remove any code that opens new windows here
        }
    }
} 