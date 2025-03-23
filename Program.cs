using System;
using System.Windows.Forms;
using Student_App.Forms;

namespace Student_App
{
    internal static class Program
    {
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

            // Handle application exit
            Application.ApplicationExit += (s, e) =>
            {
                // Ensure cleanup of any remaining system tray icons
                foreach (var process in System.Diagnostics.Process.GetProcessesByName("Student_App"))
                {
                    try
                    {
                        if (process.Id != System.Diagnostics.Process.GetCurrentProcess().Id)
                        {
                            process.Kill();
                        }
                    }
                    catch { }
                }
            };

            // Start with the login form
            Application.Run(new Login());
        }
    }
}