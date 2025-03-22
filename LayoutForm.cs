using System;
using System.Drawing;
using System.Windows.Forms;

public partial class LayoutForm : Form
{
    public LayoutForm()
    {
        InitializeComponent();
        InitializeLayoutComponents();
    }

    private void InitializeLayoutComponents()
    {
        // Basic form settings
        this.StartPosition = FormStartPosition.CenterScreen;
        this.MinimumSize = new Size(800, 600);  // Standard minimum size
        this.BackColor = Color.FromArgb(240, 240, 240);  // Light gray background
        
        // Set default font for the entire form
        this.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
    }
} 