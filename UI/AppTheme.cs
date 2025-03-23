using System.Drawing;

namespace Student_App.UI
{
    public static class AppColors
    {
        public static readonly Color Primary = Color.FromArgb(0, 123, 255);
        public static readonly Color Secondary = Color.FromArgb(64, 64, 64);
        public static readonly Color HeaderFooter = Color.FromArgb(50, 50, 50);
        public static readonly Color Success = Color.FromArgb(40, 167, 69);
        public static readonly Color Error = Color.FromArgb(220, 53, 69);
        public static readonly Color Text = Color.FromArgb(33, 37, 41);
        public static readonly Color Background = Color.White;
        public static readonly Color LightGray = Color.FromArgb(245, 245, 245);
        public static readonly Color Border = Color.FromArgb(222, 226, 230);
    }

    public static class AppFonts
    {
        public static readonly Font Title = new Font("Segoe UI", 24, FontStyle.Regular);
        public static readonly Font Subtitle = new Font("Segoe UI", 18, FontStyle.Regular);
        public static readonly Font Body = new Font("Segoe UI", 12, FontStyle.Regular);
        public static readonly Font Small = new Font("Segoe UI", 10, FontStyle.Regular);
        public static readonly Font Bold = new Font("Segoe UI", 12, FontStyle.Bold);
        public static readonly Font BoldSmall = new Font("Segoe UI", 10, FontStyle.Bold);
    }

    public static class AppLayout
    {
        public static readonly int FormWidth = 850;
        public static readonly int HeaderHeight = 60;
        public static readonly int FooterHeight = 30;
        public static readonly int Padding = 10;
        public static readonly int Margin = 8;
        public static readonly int BorderRadius = 5;
        public static readonly int CardElevation = 2;
        public static readonly int ContentWidth = FormWidth - (2 * Margin);
    }
} 