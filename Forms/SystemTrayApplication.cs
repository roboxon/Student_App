using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace Student_App.Forms
{
    public class SystemTrayApplication : IDisposable
    {
        // Singleton instance
        private static SystemTrayApplication instance;
        private static readonly object lockObject = new object();
        
        private NotifyIcon trayIcon;
        private ContextMenuStrip trayMenu;
        private Form mainForm;
        private bool disposed;

        // Private constructor for singleton
        private SystemTrayApplication() 
        {
            InitializeTrayIcon();
        }
        
        // Get singleton instance
        public static SystemTrayApplication Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObject)
                    {
                        if (instance == null)
                        {
                            instance = new SystemTrayApplication();
                        }
                    }
                }
                return instance;
            }
        }
        
        // Set the main form to control
        public void SetMainForm(Form form)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));
                
            // Detach from previous form if exists
            if (mainForm != null)
            {
                mainForm.Resize -= OnMainFormResize;
                mainForm.FormClosing -= OnMainFormClosing;
            }
            
            mainForm = form;
            
            // Attach to new form
            mainForm.Resize += OnMainFormResize;
            mainForm.FormClosing += OnMainFormClosing;
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

                // Create context menu
                trayMenu = new ContextMenuStrip();
                trayMenu.Items.Add("Open Dashboard", null, OnOpenDashboard);
                trayMenu.Items.Add("-"); // Separator
                trayMenu.Items.Add("Exit", null, OnExit);

                // Assign menu to tray icon
                trayIcon.ContextMenuStrip = trayMenu;
                trayIcon.DoubleClick += OnOpenDashboard;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing system tray: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnMainFormResize(object sender, EventArgs e)
        {
            if (mainForm.WindowState == FormWindowState.Minimized)
            {
                mainForm.Hide();
                trayIcon.Visible = true; // Ensure tray icon is visible
            }
        }

        private void OnMainFormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                mainForm.Hide();
                trayIcon.Visible = true; // Ensure tray icon is visible
            }
        }

        private void OnOpenDashboard(object sender, EventArgs e)
        {
            if (mainForm != null && !mainForm.IsDisposed)
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
            if (!disposed)
            {
                // Unsubscribe from events
                if (mainForm != null)
                {
                    mainForm.Resize -= OnMainFormResize;
                    mainForm.FormClosing -= OnMainFormClosing;
                }
                
                // Only clean up at application exit
                if (Application.OpenForms.Count == 0)
                {
                    trayIcon.Visible = false;
                    trayIcon.Dispose();
                    trayMenu.Dispose();
                }
                
                disposed = true;
            }
        }
    }
} 