using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Collections.Generic;
using Student_App.Models;

namespace Student_App.UI
{
    public static class CurriculumViewRenderer
    {
        // Enhanced colors for better visual appeal
        private static readonly Color CardBackground = Color.White;
        private static readonly Color CardBorder = Color.FromArgb(230, 230, 240);
        private static readonly Color SubjectHeaderBackground = Color.FromArgb(248, 249, 250);
        private static readonly Color TopicBorder = Color.FromArgb(240, 240, 250);
        private static readonly Color SubjectAccent = Color.FromArgb(13, 110, 253);  // Blue accent
        private static readonly Color TopicAccent = Color.FromArgb(102, 16, 242);    // Purple accent
        
        // Helper method to create rounded rectangle path
        private static GraphicsPath CreateRoundedRectangle(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            GraphicsPath path = new GraphicsPath();

            // Top left arc
            path.AddArc(arc, 180, 90);

            // Top right arc
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // Bottom right arc
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // Bottom left arc
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }
        
        // Renders a heading with title and description
        public static void RenderHeading(Control parent, string title, string description)
        {
            var titleLabel = new Label
            {
                Text = title,
                Font = new Font(AppFonts.Title.FontFamily, 16, FontStyle.Bold),
                ForeColor = AppColors.Text,
                AutoSize = true,
                Location = new Point(20, 15)
            };
            
            var descriptionLabel = new Label
            {
                Text = description,
                Font = new Font(AppFonts.Body.FontFamily, 11, FontStyle.Regular),
                ForeColor = AppColors.Secondary,
                AutoSize = false,
                Size = new Size(parent.Width - 40, 40),
                Location = new Point(20, 45)
            };
            
            parent.Controls.Add(titleLabel);
            parent.Controls.Add(descriptionLabel);
        }
        
        // Creates a subject card with topics - now with improved styling
        public static Panel CreateSubjectCard(dynamic subjectItem, int width)
        {
            if (subjectItem == null || subjectItem.subject == null)
                return new Panel();
                
            dynamic subject = subjectItem.subject;
            
            // Main card
            var card = new Panel
            {
                Width = width,
                BackColor = CardBackground,
                Margin = new Padding(10, 5, 10, 15),
                Padding = new Padding(0),
                BorderStyle = BorderStyle.None // Remove border, we'll draw it with rounded corners
            };
            
            // Draw rounded rectangle with shadow
            card.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                
                // Draw shadow
                using (var shadowBrush = new SolidBrush(Color.FromArgb(20, 0, 0, 0)))
                {
                    var shadowRect = new Rectangle(2, 2, card.Width - 4, card.Height - 4);
                    var shadowPath = CreateRoundedRectangle(shadowRect, 8);
                    e.Graphics.FillPath(shadowBrush, shadowPath);
                }
                
                // Draw card background
                using (var bgBrush = new SolidBrush(CardBackground))
                {
                    var cardRect = new Rectangle(0, 0, card.Width - 2, card.Height - 2);
                    var cardPath = CreateRoundedRectangle(cardRect, 8);
                    e.Graphics.FillPath(bgBrush, cardPath);
                }
                
                // Draw card border
                using (var borderPen = new Pen(CardBorder))
                {
                    var borderRect = new Rectangle(0, 0, card.Width - 2, card.Height - 2);
                    var borderPath = CreateRoundedRectangle(borderRect, 8);
                    e.Graphics.DrawPath(borderPen, borderPath);
                }
            };
            
            // Header
            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.Transparent // Make transparent to show the rounded corners
            };
            
            // Header background with rounded top corners
            header.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                
                // Draw header background
                using (var bgBrush = new SolidBrush(SubjectHeaderBackground))
                {
                    var headerRect = new Rectangle(1, 1, header.Width - 3, header.Height - 1);
                    var headerPath = new GraphicsPath();
                    
                    int radius = 8;
                    int diameter = radius * 2;
                    var arc = new Rectangle(headerRect.Location, new Size(diameter, diameter));
                    
                    // Top left arc
                    headerPath.AddArc(arc, 180, 90);
                    
                    // Top right arc
                    arc.X = headerRect.Right - diameter;
                    headerPath.AddArc(arc, 270, 90);
                    
                    // Bottom right straight line
                    headerPath.AddLine(headerRect.Right, headerRect.Bottom, headerRect.Left, headerRect.Bottom);
                    
                    headerPath.CloseFigure();
                    e.Graphics.FillPath(bgBrush, headerPath);
                }
                
                // Draw left accent bar
                using (var accentBrush = new SolidBrush(SubjectAccent))
                {
                    e.Graphics.FillRectangle(accentBrush, 1, 1, 4, header.Height - 1);
                }
            };
            
            // Subject title - with null checks
            string subjectName = "Unnamed Subject";
            if (subject.subject_name != null)
                subjectName = subject.subject_name;
            
            var titleLabel = new Label
            {
                Text = subjectName,
                Font = new Font(AppFonts.Body.FontFamily, 14, FontStyle.Bold),
                ForeColor = AppColors.Text,
                AutoSize = true,
                Location = new Point(15, 15),
                BackColor = Color.Transparent
            };
            
            // Subject hours - with null checks
            string hoursText = "N/A hours";
            try {
                hoursText = $"{subject.subject_hours} hours";
            } catch {}
            
            var hoursLabel = new Label
            {
                Text = hoursText,
                Font = new Font(AppFonts.Small.FontFamily, 10, FontStyle.Regular),
                ForeColor = AppColors.Secondary,
                AutoSize = true,
                Location = new Point(15, 35),
                BackColor = Color.Transparent
            };
            
            header.Controls.Add(titleLabel);
            header.Controls.Add(hoursLabel);
            
            // Add topics list panel
            var topicsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10, 10, 10, 10),
                AutoScroll = true,
                BackColor = Color.Transparent
            };
            
            // Add topics if available
            try {
                if (subjectItem.topics != null && subjectItem.topics.Count > 0)
                {
                    int yPos = 10;
                    
                    foreach (var topicItem in subjectItem.topics)
                    {
                        if (topicItem == null || topicItem.topic == null)
                            continue;
                        
                        dynamic topic = topicItem.topic;
                        
                        var topicPanel = new Panel
                        {
                            Width = width - 40,
                            Height = 60,
                            Location = new Point(10, yPos),
                            BackColor = Color.White,
                            BorderStyle = BorderStyle.None
                        };
                        
                        // Round corners and add shadow to topic panel
                        topicPanel.Paint += (s, e) =>
                        {
                            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                            
                            // Draw shadow
                            using (var shadowBrush = new SolidBrush(Color.FromArgb(10, 0, 0, 0)))
                            {
                                var shadowRect = new Rectangle(2, 2, topicPanel.Width - 4, topicPanel.Height - 4);
                                var shadowPath = CreateRoundedRectangle(shadowRect, 5);
                                e.Graphics.FillPath(shadowBrush, shadowPath);
                            }
                            
                            // Draw topic background
                            using (var bgBrush = new SolidBrush(topicPanel.BackColor))
                            {
                                var topicRect = new Rectangle(0, 0, topicPanel.Width - 2, topicPanel.Height - 2);
                                var topicPath = CreateRoundedRectangle(topicRect, 5);
                                e.Graphics.FillPath(bgBrush, topicPath);
                            }
                            
                            // Draw topic border
                            using (var borderPen = new Pen(TopicBorder))
                            {
                                var borderRect = new Rectangle(0, 0, topicPanel.Width - 2, topicPanel.Height - 2);
                                var borderPath = CreateRoundedRectangle(borderRect, 5);
                                e.Graphics.DrawPath(borderPen, borderPath);
                            }
                            
                            // Draw left accent
                            using (var accentBrush = new SolidBrush(TopicAccent))
                            {
                                e.Graphics.FillRectangle(accentBrush, 0, 0, 3, topicPanel.Height);
                            }
                        };
                        
                        // Topic name - with null checks
                        string topicName = "Unnamed Topic";
                        if (topic.topic_name != null)
                            topicName = topic.topic_name;
                        
                        var topicNameLabel = new Label
                        {
                            Text = topicName,
                            Font = new Font(AppFonts.Body.FontFamily, 12, FontStyle.Regular),
                            ForeColor = AppColors.Text,
                            AutoSize = true,
                            Location = new Point(10, 10),
                            BackColor = Color.Transparent
                        };
                        
                        // Topic hours - with null checks
                        string topicHours = "N/A hours";
                        try {
                            topicHours = $"{topic.topic_hours} hours";
                        } catch {}
                        
                        var topicHoursLabel = new Label
                        {
                            Text = topicHours,
                            Font = new Font(AppFonts.Small.FontFamily, 9, FontStyle.Regular),
                            ForeColor = AppColors.Secondary,
                            AutoSize = true,
                            Location = new Point(10, 35),
                            BackColor = Color.Transparent
                        };
                        
                        // Resources count - with null checks
                        try {
                            if (topicItem.resources != null && topicItem.resources.Count > 0)
                            {
                                var resourcesLabel = new Label
                                {
                                    Text = $"{topicItem.resources.Count} Resources",
                                    Font = new Font(AppFonts.Small.FontFamily, 9, FontStyle.Regular),
                                    ForeColor = AppColors.Primary,
                                    AutoSize = true,
                                    Location = new Point(topicPanel.Width - 100, 35),
                                    BackColor = Color.Transparent
                                };
                                topicPanel.Controls.Add(resourcesLabel);
                            }
                        } catch {}
                        
                        topicPanel.Controls.Add(topicNameLabel);
                        topicPanel.Controls.Add(topicHoursLabel);
                        
                        // Add tooltip with description - with null checks
                        try {
                            if (topic.topic_description != null)
                            {
                                var toolTip = new ToolTip();
                                toolTip.SetToolTip(topicPanel, topic.topic_description);
                            }
                        } catch {}
                        
                        topicsPanel.Controls.Add(topicPanel);
                        
                        yPos += 70; // Space for next topic
                    }
                    
                    // Set card height based on topics
                    card.Height = Math.Min(500, header.Height + yPos + 20);
                }
                else
                {
                    // No topics message
                    var emptyLabel = new Label
                    {
                        Text = "No topics available for this subject",
                        Font = new Font(AppFonts.Body.FontFamily, 11, FontStyle.Italic),
                        ForeColor = Color.Gray,
                        AutoSize = true,
                        Location = new Point(20, 20),
                        BackColor = Color.Transparent
                    };
                    topicsPanel.Controls.Add(emptyLabel);
                    
                    // Set a fixed height for the card
                    card.Height = 150;
                }
            } catch {
                // Error handling for topics
                var errorLabel = new Label
                {
                    Text = "Error loading topics",
                    Font = new Font(AppFonts.Body.FontFamily, 11, FontStyle.Italic),
                    ForeColor = Color.Gray,
                    AutoSize = true,
                    Location = new Point(20, 20),
                    BackColor = Color.Transparent
                };
                topicsPanel.Controls.Add(errorLabel);
                card.Height = 150;
            }
            
            card.Controls.Add(topicsPanel);
            card.Controls.Add(header);
            
            return card;
        }
        
        // Create a nicer loading panel with a smooth spinner
        public static Panel CreateLoadingPanel(string message, Control parent)
        {
            var loadingPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(250, 250, 252)
            };
            
            // Create animated spinner
            var spinner = new PictureBox
            {
                Size = new Size(40, 40),
                BackColor = Color.Transparent
            };
            
            // Center the spinner
            spinner.Location = new Point(
                (parent.Width - spinner.Width) / 2,
                (parent.Height - spinner.Height - 30) / 2
            );
            
            // Set up spinner animation
            // Explicitly use Windows.Forms.Timer to avoid ambiguity
            var spinnerTimer = new System.Windows.Forms.Timer { Interval = 50 };
            int angle = 0;
            
            spinnerTimer.Tick += (s, e) =>
            {
                angle = (angle + 10) % 360;
                spinner.Invalidate();
            };
            
            spinner.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                
                // Create a nice spinner graphic
                using (var pen = new Pen(AppColors.Primary, 3))
                {
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.Round;
                    
                    e.Graphics.TranslateTransform(spinner.Width / 2, spinner.Height / 2);
                    e.Graphics.RotateTransform(angle);
                    
                    // Draw spinner segments with varying opacity
                    for (int i = 0; i < 12; i++)
                    {
                        pen.Color = Color.FromArgb(255 - (i * 20), AppColors.Primary);
                        e.Graphics.DrawLine(pen, 0, 8, 0, 15);
                        e.Graphics.RotateTransform(30);
                    }
                }
            };
            
            spinnerTimer.Start();
            loadingPanel.Disposed += (s, e) => spinnerTimer.Stop();
            
            // Loading text with shadow
            var loadingLabel = new Label
            {
                Text = message,
                Font = new Font(AppFonts.Body.FontFamily, 14, FontStyle.Regular),
                ForeColor = AppColors.Text,
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            
            // Center the text
            var textSize = TextRenderer.MeasureText(message, loadingLabel.Font);
            loadingLabel.Location = new Point(
                (parent.Width - textSize.Width) / 2,
                spinner.Bottom + 20
            );
            
            loadingPanel.Controls.Add(spinner);
            loadingPanel.Controls.Add(loadingLabel);
            loadingPanel.Tag = "LoadingPanel";
            
            // Reposition spinner and text on resize
            parent.Resize += (s, e) =>
            {
                spinner.Location = new Point(
                    (parent.Width - spinner.Width) / 2,
                    (parent.Height - spinner.Height - 30) / 2
                );
                
                loadingLabel.Location = new Point(
                    (parent.Width - textSize.Width) / 2,
                    spinner.Bottom + 20
                );
            };
            
            return loadingPanel;
        }
    }
} 