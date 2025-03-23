using System;
using System.Drawing;
using System.Windows.Forms;
using Student_App.Models;
using Student_App.UI;

namespace Student_App.Forms
{
    public class CurriculumView : UserControl
    {
        private Release? currentRelease;
        private System.ComponentModel.IContainer components = null;
        
        public CurriculumView()
        {
            this.AutoScroll = true;
            this.Dock = DockStyle.Fill;
            this.Padding = new Padding(10);
            this.BackColor = Color.White;
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
            var loadingPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(100, 255, 255, 255)
            };
            
            var loadingLabel = new Label
            {
                Text = message,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                Font = new Font(AppFonts.Body.FontFamily, 12, FontStyle.Bold)
            };
            
            loadingPanel.Controls.Add(loadingLabel);
            loadingPanel.Tag = "LoadingPanel";
            
            this.Controls.Add(loadingPanel);
            loadingPanel.BringToFront();
        }
        
        private void HideLoadingIndicator()
        {
            foreach (Control control in this.Controls)
            {
                if (control is Panel panel && panel.Tag?.ToString() == "LoadingPanel")
                {
                    this.Controls.Remove(panel);
                    panel.Dispose();
                    break;
                }
            }
        }
        
        private void UpdateUI()
        {
            if (currentRelease == null || currentRelease.content == null)
                return;
                
            // Clear existing controls except the loading panel
            foreach (Control control in this.Controls)
            {
                if (control is Panel panel && panel.Tag?.ToString() != "LoadingPanel")
                {
                    this.Controls.Remove(control);
                    control.Dispose();
                }
            }
            
            // Add title for the curriculum panel
            var titleLabel = new Label
            {
                Text = $"Curriculum: {currentRelease.title}",
                Font = new Font(AppFonts.Title.FontFamily, 14, FontStyle.Bold),
                ForeColor = AppColors.Text,
                AutoSize = true,
                Location = new Point(10, 10)
            };
            this.Controls.Add(titleLabel);
            
            // Add program description
            var programDescLabel = new Label
            {
                Text = currentRelease.content.program?.program_description ?? "No description available",
                Font = new Font(AppFonts.Body.FontFamily, 10, FontStyle.Regular),
                ForeColor = AppColors.Secondary,
                AutoSize = false,
                Size = new Size(this.Width - 40, 40),
                Location = new Point(10, 40)
            };
            this.Controls.Add(programDescLabel);
            
            // Create the curriculum tree view
            var treeView = new TreeView
            {
                Location = new Point(10, 90),
                Size = new Size(this.Width - 40, this.Height - 100),
                Font = new Font(AppFonts.Body.FontFamily, 10, FontStyle.Regular),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                ShowNodeToolTips = true
            };
            
            // Populate the tree with subjects and topics
            if (currentRelease.content.subjects != null)
            {
                foreach (var subjectWithTopics in currentRelease.content.subjects)
                {
                    if (subjectWithTopics.subject == null)
                        continue;
                    
                    var subject = subjectWithTopics.subject;
                    var subjectNode = new TreeNode(subject.subject_name ?? "Unnamed Subject");
                    subjectNode.ToolTipText = subject.subject_description;
                    subjectNode.Tag = subject;
                    
                    // Add details to node text
                    subjectNode.Text = $"{subject.subject_name} ({subject.subject_hours} hours)";
                    
                    // Add topics as child nodes
                    if (subjectWithTopics.topics != null)
                    {
                        foreach (var topicWithLessons in subjectWithTopics.topics)
                        {
                            if (topicWithLessons.topic == null)
                                continue;
                            
                            var topic = topicWithLessons.topic;
                            var topicNode = new TreeNode(topic.topic_name ?? "Unnamed Topic");
                            topicNode.ToolTipText = topic.topic_description;
                            topicNode.Tag = topic;
                            
                            // Add details to node text
                            topicNode.Text = $"{topic.topic_name} ({topic.topic_hours} hours)";
                            
                            // Add lessons as child nodes
                            if (topicWithLessons.lessons != null)
                            {
                                foreach (var lesson in topicWithLessons.lessons)
                                {
                                    var lessonNode = new TreeNode(lesson.lesson_name ?? "Unnamed Lesson");
                                    lessonNode.ToolTipText = lesson.lesson_description;
                                    lessonNode.Tag = lesson;
                                    
                                    // Add details to node text
                                    lessonNode.Text = $"{lesson.lesson_name} ({lesson.lesson_hours} hours)";
                                    
                                    topicNode.Nodes.Add(lessonNode);
                                }
                            }
                            
                            // Add resources as a special node if they exist
                            if (topicWithLessons.resources != null && topicWithLessons.resources.Count > 0)
                            {
                                var resourcesNode = new TreeNode("Resources");
                                resourcesNode.ForeColor = AppColors.Primary;
                                
                                foreach (var resource in topicWithLessons.resources)
                                {
                                    var resourceNode = new TreeNode(resource.description ?? "Unnamed Resource");
                                    resourceNode.ToolTipText = resource.url_link;
                                    resourceNode.Tag = resource;
                                    resourcesNode.Nodes.Add(resourceNode);
                                }
                                
                                topicNode.Nodes.Add(resourcesNode);
                            }
                            
                            subjectNode.Nodes.Add(topicNode);
                        }
                    }
                    
                    treeView.Nodes.Add(subjectNode);
                }
            }
            
            // Set up event handler for node double-click (for resources)
            treeView.NodeMouseDoubleClick += (s, e) => {
                if (e.Node.Tag is Resource resource && !string.IsNullOrEmpty(resource.url_link))
                {
                    try
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = resource.url_link,
                            UseShellExecute = true
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Could not open URL: {ex.Message}", "Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            };
            
            this.Controls.Add(treeView);
        }
    }
} 