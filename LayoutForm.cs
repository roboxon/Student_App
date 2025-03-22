using System;
using System.Drawing;
using System.Windows.Forms;

namespace Student_App
{
    public partial class LayoutForm : Form
    {
        private bool disposedValue;
        protected Panel mainContentPanel = new();

        public LayoutForm()
        {
            InitializeComponent();
            InitializeLayoutComponents();
        }

        protected virtual void InitializeComponent()
        {
            this.Text = "Student App";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 240, 240);
        }

        protected virtual void InitializeLayoutComponents()
        {
            // Initialize main content panel
            mainContentPanel.Dock = DockStyle.Fill;
            mainContentPanel.Padding = new Padding(20);
            mainContentPanel.BackColor = Color.White;

            this.Controls.Add(mainContentPanel);
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    mainContentPanel?.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
            base.Dispose(disposing);
        }
    }
} 