using System.Drawing;

namespace Student_App.UI
{
    public static class AppColors
    {
        public static readonly Color Primary = Color.FromArgb(0, 123, 255);
        public static readonly Color Secondary = Color.FromArgb(108, 117, 125);
        public static readonly Color Success = Color.FromArgb(40, 167, 69);
        public static readonly Color Error = Color.FromArgb(220, 53, 69);
        public static readonly Color Text = Color.FromArgb(33, 37, 41);
        public static readonly Color Background = Color.White;
    }

    public static class AppFonts
    {
        public static readonly Font Title = new Font("Segoe UI", 24, FontStyle.Regular);
        public static readonly Font Subtitle = new Font("Segoe UI", 18, FontStyle.Regular);
        public static readonly Font Body = new Font("Segoe UI", 12, FontStyle.Regular);
        public static readonly Font Small = new Font("Segoe UI", 10, FontStyle.Regular);
    }
} 