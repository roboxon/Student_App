using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using Student_App.Forms;

namespace Student_App
{
    public class TrayApplicationContext : ApplicationContext
    {
        private NotifyIcon trayIcon;
        private Form currentForm;
        private static TrayApplicationContext instance;

        // Singleton pattern
        public static TrayApplicationContext Instance
        {
            get
            {
                if (instance == null)
                    instance = new TrayApplicationContext();
                return instance;
            }
        }

        private TrayApplicationContext()
        {
            InitializeTrayIcon();
        }

        private void InitializeTrayIcon()
        {
            try
            {
                string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resource", "DAA_Logo.ico");
                
                trayIcon = new NotifyIcon
                {
                    Icon = new Icon(iconPath),
                    Text = "Student App",
                    Visible = true
                };

                var contextMenu = new ContextMenuStrip();
                contextMenu.Items.Add("Show Application", null, ShowForm);
                contextMenu.Items.Add("-");
                contextMenu.Items.Add("Exit", null, Exit);
                
                trayIcon.ContextMenuStrip = contextMenu;
                trayIcon.DoubleClick += ShowForm;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing tray icon: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void SetForm(Form form)
        {
            // Remove event handlers from previous form
            if (currentForm != null)
            {
                currentForm.FormClosing -= Form_FormClosing;
                currentForm.Resize -= Form_Resize;
            }

            currentForm = form;
            
            // Add event handlers to new form
            if (currentForm != null)
            {
                currentForm.FormClosing += Form_FormClosing;
                currentForm.Resize += Form_Resize;
            }
        }

        private void Form_Resize(object sender, EventArgs e)
        {
            if (currentForm.WindowState == FormWindowState.Minimized)
            {
                currentForm.Hide();
                trayIcon.Visible = true;
            }
        }

        private void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                currentForm.Hide();
                trayIcon.Visible = true;
            }
        }

        private void ShowForm(object sender, EventArgs e)
        {
            if (currentForm != null)
            {
                currentForm.Show();
                currentForm.WindowState = FormWindowState.Normal;
                currentForm.Activate();
            }
        }

        private void Exit(object sender, EventArgs e)
        {
            trayIcon.Visible = false;
            Application.Exit();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                trayIcon.Visible = false;
                trayIcon.Dispose();
            }
            base.Dispose(disposing);
        }

        public void ExitApplication()
        {
            trayIcon.Visible = false;
            trayIcon.Dispose();
            ExitThread();
        }
    }
} 