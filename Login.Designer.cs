using System.Windows.Forms;

namespace YourProjectNamespace
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // ... existing code ...
            
            // Add a PictureBox for the logo
            this.logoPictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).BeginInit();
            
            // Configure the PictureBox - using the correct namespace and approach
            // Instead of: this.logoPictureBox.Image = global::YourProjectNamespace.Properties.Resources.DAA_Logo;
            // Use one of these alternatives:
            
            // Option 1: If the icon is set as the form's icon:
            this.logoPictureBox.Image = this.Icon.ToBitmap();
            
            // OR Option 2: Load directly from file:
            // this.logoPictureBox.Image = new System.Drawing.Icon("Resources/DAA_Logo.ico").ToBitmap();
            
            this.logoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.logoPictureBox.Size = new System.Drawing.Size(150, 150);
            this.logoPictureBox.Location = new System.Drawing.Point(
                (this.ClientSize.Width - 150) / 2,
                30);
            this.logoPictureBox.Name = "logoPictureBox";
            this.logoPictureBox.TabIndex = 10;
            
            // Add the PictureBox to the form's controls
            this.Controls.Add(this.logoPictureBox);
            
            // Adjust the position of other controls if needed
            // For example, you might need to move the Welcome Back label down
            // this.welcomeBackLabel.Location = new System.Drawing.Point(...);
            
            // ... existing code ...
            
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).EndInit();
        }

        // Declare the PictureBox field
        private System.Windows.Forms.PictureBox logoPictureBox;
    }
} 