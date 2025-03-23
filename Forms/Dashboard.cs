using System.Drawing;
using System.Windows.Forms;
using Student_App.Services.Configuration;
using Student_App.Models;
using Student_App.Forms;
using System.IO;

namespace Student_App.Forms
{
    public partial class Dashboard : LayoutForm
    {
        private Panel statsPanel = new();
        private Panel studentInfoPanel = new();
        private Panel curriculumPanel = new();
        private Student? currentStudent;
        private Release? currentRelease;
        private System.ComponentModel.IContainer components = null;

        public Dashboard(Student student)
        {
            if (student == null)
            {
                MessageBox.Show("No student information available. Redirecting to login.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                var loginForm = new Login();
                loginForm.Show();
                this.Close();
                return;
            }
            
            currentStudent = student;
            
            // Initialize components first
            InitializeComponent();
            
            // Set icon and initialize dashboard controls
            this.Icon = new Icon(Path.Combine(Application.StartupPath, "Resource", "DAA_Logo.ico"));
            InitializeDashboardControls();
            SetActiveMenuItem("Dashboard");
            
            // Load Release data asynchronously
            LoadReleaseDataAsync();
        }

        private async void LoadReleaseDataAsync()
        {
            try
            {
                // Show loading indicator
                var loadingPanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = Color.FromArgb(100, 255, 255, 255)
                };
                
                var loadingLabel = new Label
                {
                    Text = "Loading curriculum data...",
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill,
                    Font = new Font(AppFonts.Body.FontFamily, 12, FontStyle.Bold)
                };
                
                loadingPanel.Controls.Add(loadingLabel);
                loadingPanel.Tag = "LoadingPanel";
                
                mainContentPanel.Controls.Add(loadingPanel);
                loadingPanel.BringToFront();
                
                // Ensure currentStudent is not null before calling API
                if (currentStudent != null)
                {
                    // Load release data
                    currentRelease = await Release.GetReleaseAsync(currentStudent);
                    
                    // Update UI with the data
                    UpdateCurriculumPanel();
                }
                
                // Remove loading panel
                mainContentPanel.Controls.Remove(loadingPanel);
                loadingPanel.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load curriculum data: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                // Clean up loading panel if it exists
                foreach (Control c in mainContentPanel.Controls)
                {
                    if (c is Panel p && p.Tag?.ToString() == "LoadingPanel")
                    {
                        mainContentPanel.Controls.Remove(p);
                        p.Dispose();
                        break;
                    }
                }
            }
        }

        private void UpdateCurriculumPanel()
        {
            if (currentRelease == null || currentRelease.content == null)
                return;
            
            // Clear existing controls except the loading panel
            foreach (Control control in curriculumPanel.Controls)
            {
                if (control is Panel panel && panel.Tag?.ToString() != "LoadingPanel")
                {
                    curriculumPanel.Controls.Remove(control);
                    control.Dispose();
                }
            }
            
            // Add title for the curriculum panel
            var titleLabel = new Label
            {
                Text = $"Curriculum: {currentRelease.title}",
                Font = new Font(AppFonts.Heading.FontFamily, 14, FontStyle.Bold),
                ForeColor = AppColors.Text,
                AutoSize = true,
                Location = new Point(10, 10)
            };
            curriculumPanel.Controls.Add(titleLabel);
            
            // Add program description
            var programDescLabel = new Label
            {
                Text = currentRelease.content.program?.program_description ?? "No description available",
                Font = new Font(AppFonts.Body.FontFamily, 10, FontStyle.Regular),
                ForeColor = AppColors.Secondary,
                AutoSize = false,
                Size = new Size(curriculumPanel.Width - 40, 40),
                Location = new Point(10, 40)
            };
            curriculumPanel.Controls.Add(programDescLabel);
            
            // Create the curriculum tree view
            var treeView = new TreeView
            {
                Location = new Point(10, 90),
                Size = new Size(curriculumPanel.Width - 40, curriculumPanel.Height - 100),
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
            
            curriculumPanel.Controls.Add(treeView);
        }

        protected override void InitializeComponent()
        {
            base.InitializeComponent();
            this.Text = "Student Dashboard";
            
            // Update header with student info
            if (currentStudent != null)
            {
                UpdateUserInfo($"{currentStudent.first_name} {currentStudent.last_name}");
                
                // Add stats to header
                var statsItems = new[]
                {
                    new { Label = "Course", Value = currentStudent.plan_name ?? "N/A" },
                    new { Label = "Group", Value = currentStudent.group_name ?? "N/A" },
                    new { Label = "Grade", Value = currentStudent.grade_score?.ToString("F1") ?? "N/A" },
                    new { Label = "Status", Value = currentStudent.is_active == 1 ? "Active" : "Inactive" }
                };
                
                // Use the existing header space for stats
                int headerWidth = headerPanel.Width;
                int startX = 400; // After welcome message
                int itemWidth = 140;
                
                foreach (var item in statsItems)
                {
                    var statPanel = new Panel
                    {
                        Width = itemWidth,
                        Height = 40,
                        Location = new Point(startX, 10),
                        BackColor = Color.Transparent
                    };
                    
                    var labelControl = new Label
                    {
                        Text = item.Label,
                        Font = new Font(AppFonts.Body.FontFamily, 9, FontStyle.Regular),
                        ForeColor = Color.LightGray,
                        AutoSize = true,
                        Location = new Point(0, 0)
                    };
                    
                    var valueControl = new Label
                    {
                        Text = item.Value,
                        Font = new Font(AppFonts.Body.FontFamily, 11, FontStyle.Bold),
                        ForeColor = Color.White,
                        AutoSize = true,
                        Location = new Point(0, 18)
                    };
                    
                    statPanel.Controls.Add(labelControl);
                    statPanel.Controls.Add(valueControl);
                    headerPanel.Controls.Add(statPanel);
                    
                    startX += itemWidth;
                }
            }
        }

        private void InitializeDashboardControls()
        {
            // Student Information panel with proper formatting
            studentInfoPanel = new Panel
            {
                Height = 70, // Further reduced height
                Dock = DockStyle.Top,
                Padding = new Padding(10, 5, 10, 5),
                BackColor = Color.White,
                BorderStyle = BorderStyle.None
            };
            
            var infoTitle = new Label
            {
                Text = "Student Information",
                Font = new Font(AppFonts.Heading.FontFamily, 12, FontStyle.Bold),
                ForeColor = AppColors.Text,
                AutoSize = true,
                Location = new Point(5, 5)
            };
            studentInfoPanel.Controls.Add(infoTitle);
            
            // Create a horizontal panel for student info items
            var infoItemsPanel = new Panel
            {
                Width = mainContentPanel.Width - 20,
                Height = 40,
                Location = new Point(5, 25),
                BorderStyle = BorderStyle.None
            };
            
            // Only include essential information - all on one line
            if (currentStudent != null)
            {
                var infoItems = new[]
                {
                    new { Label = "Email", Value = currentStudent.email ?? "N/A" },
                    new { Label = "Mentor", Value = currentStudent.mentor_name ?? "N/A" },
                    new { Label = "Advisor", Value = currentStudent.advisor_name ?? "N/A" },
                    new { Label = "Start Date", Value = currentStudent.start_date ?? "N/A" },
                    new { Label = "End Date", Value = currentStudent.end_date ?? "N/A" }
                };
                
                int infoItemWidth = 180;
                int xPos = 0;
                
                foreach (var item in infoItems)
                {
                    var infoItem = new Panel
                    {
                        Width = infoItemWidth,
                        Height = 40,
                        Location = new Point(xPos, 0),
                        BorderStyle = BorderStyle.None
                    };
                    
                    var labelControl = new Label
                    {
                        Text = item.Label,
                        Font = new Font(AppFonts.Body.FontFamily, 9, FontStyle.Bold),
                        ForeColor = AppColors.Secondary,
                        AutoSize = true,
                        Location = new Point(0, 0)
                    };
                    
                    var valueControl = new Label
                    {
                        Text = item.Value,
                        Font = new Font(AppFonts.Body.FontFamily, 10, FontStyle.Regular),
                        ForeColor = AppColors.Text,
                        AutoSize = true,
                        Location = new Point(0, 20)
                    };
                    
                    infoItem.Controls.Add(labelControl);
                    infoItem.Controls.Add(valueControl);
                    infoItemsPanel.Controls.Add(infoItem);
                    
                    xPos += infoItemWidth;
                }
            }
            
            studentInfoPanel.Controls.Add(infoItemsPanel);
            
            // Add a subtle bottom border to the student info panel
            studentInfoPanel.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(230, 230, 230), 1))
                {
                    e.Graphics.DrawLine(pen, 0, studentInfoPanel.Height - 1, studentInfoPanel.Width, studentInfoPanel.Height - 1);
                }
            };
            
            // Curriculum Panel - main content area
            curriculumPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10, 10, 10, 10),
                BorderStyle = BorderStyle.None
            };
            
            // Add panels to main content in new order
            mainContentPanel.Controls.Add(curriculumPanel);
            mainContentPanel.Controls.Add(studentInfoPanel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
} 