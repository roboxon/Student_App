using System;
using System.Windows.Forms;
using Student_App.Forms;

namespace Student_App
{
    internal static class Program
    {
        // Keep a static reference to prevent garbage collection
        private static SystemTrayApplication? _systemTray;
        
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Set up exception handling
            Application.ThreadException += (s, e) => 
                MessageBox.Show($"Unexpected error: {e.Exception.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            
            AppDomain.CurrentDomain.UnhandledException += (s, e) => 
                MessageBox.Show($"Fatal error: {e.ExceptionObject}", "Fatal Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

            // Ensure tray icon cleanup on exit
            Application.ApplicationExit += (s, e) => 
            {
                _systemTray?.Dispose();
            };

            // Start with the login form
            Application.Run(new Login());
        }

        // Add this method to keep the tray reference alive
        public static void SetSystemTray(SystemTrayApplication tray)
        {
            _systemTray = tray;
        }
    }
}