using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace Student_App.Forms
{
    public class SystemTrayApplication : IDisposable
    {
        private NotifyIcon trayIcon;
        private ContextMenuStrip trayMenu;
        private Form mainForm;  // To keep track of the main form

        public SystemTrayApplication(Form form)
        {
            mainForm = form;
            
            // Initialize tray icon
            trayIcon = new NotifyIcon();
            trayIcon.Icon = new Icon(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resource", "DAA_Logo.ico"));
            trayIcon.Visible = true;

            // Create context menu
            trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("Open Dashboard", null, OnOpenDashboard);
            trayMenu.Items.Add("Exit", null, OnExit);

            // Assign menu to tray icon
            trayIcon.ContextMenuStrip = trayMenu;
            trayIcon.DoubleClick += OnOpenDashboard;
        }

        private void OnOpenDashboard(object sender, EventArgs e)
        {
            if (mainForm != null)
            {
                mainForm.Show();
                mainForm.WindowState = FormWindowState.Normal;
                mainForm.BringToFront();
            }
        }

        private void OnExit(object sender, EventArgs e)
        {
            trayIcon.Visible = false;
            Application.Exit();
        }

        public void Dispose()
        {
            trayIcon?.Dispose();
            trayMenu?.Dispose();
        }
    }
} 