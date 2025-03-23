using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Student_App.UI
{
    public static class ControlExtensions
    {
        // Style for non-flat progress bars
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);
        
        private const int WM_USER = 0x400;
        private const int PBM_SETSTATE = WM_USER + 16;
        private const int PBST_NORMAL = 0x0001;
        private const int PBST_ERROR = 0x0002;
        private const int PBST_PAUSED = 0x0003;
        
        public static void SetStyle(this ProgressBar pBar, ProgressBarStyle style)
        {
            // Apply Windows 10 style to progress bar
            SendMessage(pBar.Handle, PBM_SETSTATE, PBST_NORMAL, 0);
        }
        
        public static void SetErrorState(this ProgressBar pBar)
        {
            SendMessage(pBar.Handle, PBM_SETSTATE, PBST_ERROR, 0);
        }
        
        public static void SetPausedState(this ProgressBar pBar)
        {
            SendMessage(pBar.Handle, PBM_SETSTATE, PBST_PAUSED, 0);
        }
    }
} 