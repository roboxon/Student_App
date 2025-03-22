using System.Drawing;

namespace Student_App.Services.Configuration
{
    public static class AppColors
    {
        public static Color Primary = Color.FromArgb(45, 45, 48);
        public static Color Secondary = Color.FromArgb(0, 122, 204);
        public static Color Background = Color.FromArgb(240, 240, 240);
        public static Color Text = Color.FromArgb(45, 45, 48);
        public static Color Error = Color.FromArgb(232, 17, 35);
        public static Color Success = Color.FromArgb(16, 124, 16);
    }

    public static class AppFonts
    {
        public static Font Title = new Font("Segoe UI", 16, FontStyle.Bold);
        public static Font Heading = new Font("Segoe UI", 14, FontStyle.Bold);
        public static Font Body = new Font("Segoe UI", 9, FontStyle.Regular);
        public static Font Small = new Font("Segoe UI", 8, FontStyle.Regular);
    }

    public static class AppSpacing
    {
        public static int Small = 4;
        public static int Medium = 8;
        public static int Large = 16;
        public static int ExtraLarge = 24;
    }
} 