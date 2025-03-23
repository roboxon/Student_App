using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using Student_App.Models;
using Student_App.UI;

namespace Student_App.Forms
{
    public class CurriculumView : UserControl
    {
        private Release? currentRelease;
        private System.ComponentModel.IContainer components = null;
        
        // UI elements
        private Panel headerPanel;
        private Panel contentPanel;
        private Label titleLabel;
        private Label descriptionLabel;
        
        public CurriculumView()
        {
            InitializeComponent();
        }
        
        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            this.AutoScroll = false;
            this.Dock = DockStyle.Fill;
            this.Padding = new Padding(0);
            this.BackColor = Color.FromArgb(245, 245, 247);
            
            // Header panel
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.White,
                Padding = new Padding(20, 15, 20, 15)
            };
            
            // Title
            titleLabel = new Label
            {
                Text = "Curriculum",
                Font = new Font(AppFonts.Title.FontFamily, 16, FontStyle.Bold),
                ForeColor = AppColors.Text,
                AutoSize = true,
                Location = new Point(20, 15)
            };
            headerPanel.Controls.Add(titleLabel);
            
            // Description
            descriptionLabel = new Label
            {
                Text = "Your learning journey and course materials",
                Font = new Font(AppFonts.Body.FontFamily, 11, FontStyle.Regular),
                ForeColor = AppColors.Secondary,
                AutoSize = true,
                Location = new Point(20, 45)
            };
            headerPanel.Controls.Add(descriptionLabel);
            
            // Content panel with scrolling
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(20, 20, 20, 20)
            };
            
            this.Controls.Add(contentPanel);
            this.Controls.Add(headerPanel);
            
            this.ResumeLayout(false);
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        
        public async void LoadRelease(Student student)
        {
            if (student == null)
                return;
                
            try
            {
                // Show loading indicator
                ShowLoadingIndicator("Loading curriculum data...");
                
                // Load release data
                currentRelease = await Release.GetReleaseAsync(student);
                
                // Hide loading indicator
                HideLoadingIndicator();
                
                // Update the UI with the loaded data
                UpdateUI();
            }
            catch (Exception ex)
            {
                HideLoadingIndicator();
                MessageBox.Show($"Failed to load curriculum data: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void ShowLoadingIndicator(string message)
        {
            // Clear current content
            contentPanel.Controls.Clear();
            
            // Create simple loading indicator
            var loadingPanel = CurriculumViewRenderer.CreateLoadingPanel(message, contentPanel);
            contentPanel.Controls.Add(loadingPanel);
        }
        
        private void HideLoadingIndicator()
        {
            foreach (Control control in contentPanel.Controls)
            {
                if (control.Tag?.ToString() == "LoadingPanel")
                {
                    contentPanel.Controls.Remove(control);
                    control.Dispose();
                    break;
                }
            }
        }
        
        private void UpdateUI()
        {
            if (currentRelease == null || currentRelease.content == null)
                return;
            
            // Clear existing content
            contentPanel.Controls.Clear();
            
            // Update header
            titleLabel.Text = $"Curriculum: {currentRelease.title}";
            descriptionLabel.Text = currentRelease.content.program?.program_description ?? 
                "Your learning journey and course materials";
            
            // Add subjects
            if (currentRelease.content.subjects != null)
            {
                // Sort subjects
                var sortedSubjects = currentRelease.content.subjects
                    .Where(s => s.subject != null)
                    .OrderBy(s => s.subject.subject_tag ?? s.subject.subject_name)
                    .ToList();
                
                int yPos = 0;
                foreach (var subjectItem in sortedSubjects)
                {
                    if (subjectItem.subject == null)
                        continue;
                    
                    // Create a simple subject card with topics
                    var subjectCard = CurriculumViewRenderer.CreateSubjectCard(
                        subjectItem, 
                        contentPanel.Width - 40
                    );
                    
                    subjectCard.Location = new Point(0, yPos);
                    contentPanel.Controls.Add(subjectCard);
                    
                    yPos += subjectCard.Height + 20;
                }
                
                // No subjects message
                if (sortedSubjects.Count == 0)
                {
                    var emptyLabel = new Label
                    {
                        Text = "No subjects found in this curriculum",
                        Font = new Font(AppFonts.Body.FontFamily, 14, FontStyle.Regular),
                        ForeColor = Color.Gray,
                        AutoSize = true,
                        Location = new Point(20, 20)
                    };
                    contentPanel.Controls.Add(emptyLabel);
                }
            }
        }
        
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            
            // Update subject cards width
            if (contentPanel != null)
            {
                foreach (Control control in contentPanel.Controls)
                {
                    if (control is Panel panel)
                    {
                        panel.Width = contentPanel.Width - 40;
                    }
                }
            }
        }
    }
} 