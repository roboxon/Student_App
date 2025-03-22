using System.Drawing;
using System.Windows.Forms;

namespace Student_App.Forms
{
    public partial class Dashboard : LayoutForm
    {
        private Panel mainPanel = new();
        private Label welcomeLabel = new();

        public Dashboard()
        {
            InitializeComponent();
            InitializeDashboardControls();
        }

        protected override void InitializeComponent()
        {
            base.InitializeComponent();
            this.Text = "Student Dashboard";
        }

        private void InitializeDashboardControls()
        {
            // Initialize mainPanel
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.Padding = new Padding(10);

            // Initialize welcomeLabel
            welcomeLabel.Text = "Welcome to Student Dashboard";
            welcomeLabel.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            welcomeLabel.AutoSize = true;
            welcomeLabel.Location = new Point(20, 20);

            // Add controls to mainPanel
            mainPanel.Controls.Add(welcomeLabel);
            this.Controls.Add(mainPanel);
        }
    }
} 