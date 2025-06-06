namespace Student_App.Forms
{
    partial class Login
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        // Add this field declaration to the Login class (outside of any methods)
        private System.Windows.Forms.PictureBox logoPictureBox;
        private System.Windows.Forms.Button btnExit;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Login));
            logoPictureBox = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)logoPictureBox).BeginInit();
            SuspendLayout();
            // 
            // logoPictureBox
            // 
            logoPictureBox.BackColor = Color.Transparent;
            logoPictureBox.Image = (Image)resources.GetObject("logoPictureBox.Image");
            logoPictureBox.Location = new Point(114, 78);
            logoPictureBox.Name = "logoPictureBox";
            logoPictureBox.Size = new Size(150, 150);
            logoPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            logoPictureBox.TabIndex = 10;
            logoPictureBox.TabStop = false;
            // 
            // Login
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(400, 300);
            Controls.Add(logoPictureBox);
            Name = "Login";
            Text = "Login";
            // Create a stylish X button
            btnExit = new System.Windows.Forms.Button();
            
            // Configure the Exit button as an X icon
            btnExit.FlatAppearance.BorderSize = 0;
            btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnExit.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnExit.ForeColor = System.Drawing.Color.FromArgb(0, 126, 249); // Blue color to match your theme
            btnExit.Text = "✕"; // X symbol
            btnExit.Location = new System.Drawing.Point(this.ClientSize.Width - 40, 5); // Top-right corner
            btnExit.Name = "btnExit";
            btnExit.Size = new System.Drawing.Size(35, 35);
            btnExit.TabIndex = 15; // Adjust as needed
            btnExit.UseVisualStyleBackColor = true;
            btnExit.Cursor = System.Windows.Forms.Cursors.Hand;
            
            // Add the button click event handler
            btnExit.Click += new System.EventHandler(this.btnExit_Click);
            
            // Add the button to the form's controls
            this.Controls.Add(btnExit);
            this.btnExit.BringToFront(); // Make sure it's on top
            ((System.ComponentModel.ISupportInitialize)logoPictureBox).EndInit();
            ResumeLayout(false);
        }

        #endregion
    }
} 