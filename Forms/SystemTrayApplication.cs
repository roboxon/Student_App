using System;
using System.Drawing;
using System.Windows.Forms;

namespace Student_App.Forms
{
    public class SystemTrayApplication : Form
    {
        private NotifyIcon? trayIcon;
        private ContextMenuStrip? trayMenu;
        private bool isExiting = false;
        private Dashboard? mainDashboard;

        public SystemTrayApplication()
        {
            InitializeTrayIcon();
            InitializeTrayMenu();
            InitializeMainDashboard();
        }

        private void InitializeTrayIcon()
        {
            trayIcon = new NotifyIcon
            {
                Icon = SystemIcons.Application, // Using system icon temporarily
                Text = "Student App",
                Visible = true
            };

            // Handle double-click to show main form
            trayIcon.DoubleClick += (s, e) => ShowMainDashboard();
            
            // Handle form closing
            this.FormClosing += (s, e) =>
            {
                if (!isExiting)
                {
                    e.Cancel = true;
                    this.Hide();
                    return;
                }
            };
        }

        private void InitializeTrayMenu()
        {
            trayMenu = new ContextMenuStrip();
            var openDashboardItem = new ToolStripMenuItem("Open Dashboard");
            openDashboardItem.Click += (s, e) => ShowMainDashboard();
            
            var reportsItem = new ToolStripMenuItem("Reports");
            reportsItem.Click += (s, e) => ShowReports();
            
            var attendanceItem = new ToolStripMenuItem("Attendance");
            attendanceItem.Click += (s, e) => ShowAttendance();
            
            var exitItem = new ToolStripMenuItem("Exit");
            exitItem.Click += (s, e) => ExitApplication();

            trayMenu.Items.AddRange(new ToolStripItem[] 
            { 
                openDashboardItem,
                reportsItem,
                attendanceItem,
                new ToolStripSeparator(),
                exitItem
            });

            if (trayIcon != null)
            {
                trayIcon.ContextMenuStrip = trayMenu;
            }
        }

        private void InitializeMainDashboard()
        {
            mainDashboard = new Dashboard();
            mainDashboard.FormClosing += (s, e) =>
            {
                if (!isExiting)
                {
                    e.Cancel = true;
                    mainDashboard.Hide();
                    return;
                }
            };
        }

        private void ShowMainDashboard()
        {
            if (mainDashboard != null)
            {
                mainDashboard.Show();
                mainDashboard.WindowState = FormWindowState.Normal;
                mainDashboard.Activate();
            }
        }

        private void ShowReports()
        {
            // TODO: Implement reports form
            MessageBox.Show("Reports functionality coming soon!");
        }

        private void ShowAttendance()
        {
            // TODO: Implement attendance form
            MessageBox.Show("Attendance functionality coming soon!");
        }

        private void ExitApplication()
        {
            isExiting = true;
            if (trayIcon != null)
            {
                trayIcon.Visible = false;
            }
            Application.Exit();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.Hide(); // Hide the main form on startup
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                trayIcon?.Dispose();
                trayMenu?.Dispose();
                mainDashboard?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
} 