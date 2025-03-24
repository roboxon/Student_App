tudent_App_Development_Guide.md
Student App Development Guide
1. Application Overview
The Student App is a Windows Forms application designed for student activity management in educational programs. It follows a model-centric architecture where models handle their own API communications, combined with local caching for performance. The application provides authentication, student information display, curriculum navigation, and weekly reporting functionality.
2. Architecture
2.1 Core Architecture Pattern
Model-Centric Approach: Models handle their own API communication
Local Caching: Responses are cached locally to reduce API calls
Base Layout Pattern: All forms inherit from LayoutForm
Theming System: AppTheme provides centralized styling
2.2 Key Components
Authentication: Login form with credential validation
Dashboard: Main interface showing student info and curriculum
Weekly Reporting: System for creating, managing, and submitting reports
Curriculum View: Display and navigation of course structure
2.3 Folder Structure
/Forms: Main application forms (Login, Dashboard, WeeklyReportForm)
/Models: Data models (Student, Release, WeeklyReport)
/Services: Service components (Configuration, Reports)
/UI: UI components and theming (AppTheme, ReportContentPanel)
3. Authentication Flow
User enters credentials in Login form
Login form calls Student.LoginAsync() (model handles API call)
Upon success, Student object is created with data from response
Student object is passed to Dashboard
Email is saved if "Remember Email" is checked

// Key implementation in Login.cs
var student = await Student.LoginAsync(emailInput.Text, passwordInput.Text);
if (rememberEmailCheckbox.Checked)
{
    SaveEmailToConfig(emailInput.Text);
}
var dashboard = new Dashboard(student);
dashboard.Show();
4. Dashboard Implementation
The Dashboard displays student information and curriculum data:
Receives Student object from Login form
Displays student profile information (name, course, group, etc.)
Calls Release.GetReleaseAsync(student) to fetch curriculum
Renders curriculum subjects with expandable topics

// Curriculum loading in Dashboard.cs
private async void LoadCurriculum()
{
    try
    {
        var release = await Release.GetReleaseAsync(student);
        DisplayCurriculum(release);
    }
    catch (Exception ex)
    {
        // Error handling
    }
}
5. Curriculum Data Model
The Release model represents curriculum data with a hierarchical structure:
Release: Container with metadata
Program: Course program details
Subjects: List of subjects in the curriculum
Topics: List of topics for each subject
Lessons: Content within topics
Resources: Learning materials
Apply to APPI.md

// Release data fetch with caching in Release.cs
public static async Task<Release> GetReleaseAsync(Student? student, bool forceRefresh = false)
{
    // Check cache first
    if (!forceRefresh && IsLocalReleaseValid(releaseId))
    {
        var cachedRelease = LoadReleaseFromLocal(releaseId);
        if (cachedRelease != null)
            return cachedRelease;
    }
    
    // Fetch from API if needed
    var response = await _client.GetAsync(endpoint);
    // Process response...
    
    // Save to cache
    SaveReleaseToLocal(releaseResponse, releaseId);
    return releaseResponse.data;
}
6. Weekly Reporting System
6.1 Report Data Structure
WeeklyReport: Container for a week's reports
DailyReport: Reports for each day
HourlyReport: Individual hour entries with subject/topic information
6.2 Key Components
ReportCalendarView: Calendar for selecting reporting weeks
WeeklyReportForm: Main form for report management
ReportContentPanel: Panel for editing hourly reports
6.3 Subject/Topic Selection
The application allows students to select subjects and topics for hourly reports:
Subject dropdown populated from Release curriculum
Topic dropdown dynamically updated based on selected subject
Selected topic is saved to the hourly report

// Topic loading when subject is selected (ReportContentPanel.cs)
private async void LoadTopicsForSelectedSubject()
{
    selectSpecialTopicDay.Items.Clear();
    
    // Get selected subject
    var selectedSubject = subjectDropdown.SelectedItem as SubjectItem;
    if (selectedSubject == null) return;
    
    // Get release data
    var release = await Release.GetReleaseAsync(student);
    
    // Find selected subject and load its topics
    foreach (var subjectWithTopics in release.content.subjects)
    {
        if (subjectWithTopics.subject?.id == selectedSubject.Id)
        {
            // Add topics to dropdown
            // ...
        }
    }
    
    // Handle case with no topics
    if (!topicsFound)
    {
        selectSpecialTopicDay.Items.Add(new TopicItem(0, "No defined topic"));
    }
}

6.4 Report Persistence
Reports can be saved locally as JSON
Reports can be submitted to server when complete
Status visualization shows completion state
7. UI Theming System
The application uses a centralized theming system defined in AppTheme.cs:
7.1 Key Theme Components
AppColors: Color definitions (Primary, Secondary, HeaderFooter, etc.)
AppFonts: Typography definitions (Title, Subtitle, Body, etc.)
AppLayout: Layout measurements (FormWidth, HeaderHeight, etc.)

// Sample theme usage
panel.BackColor = AppColors.Background;
label.Font = AppFonts.Title;
panel.Height = AppLayout.HeaderHeight;
7.2 Base Layout Form
The LayoutForm class provides consistent layout across the application:
Header panel with title and user info
Side menu panel with navigation
Main content area
Footer panel with status info
8. Key Files and Their Purposes
8.1 Models
Student.cs: Student data model with authentication logic
Release.cs: Curriculum data model with caching
WeeklyReport.cs: Report container with validation logic
DailyReport.cs: Daily report container
HourlyReport.cs: Individual hour report entries
8.2 Forms
Login.cs: Authentication interface
Dashboard.cs: Main application interface
WeeklyReportForm.cs: Weekly reporting interface
CurriculumView.cs: Detailed curriculum navigation
8.3 UI Components
ReportContentPanel.cs: Hourly report editing interface
ReportCalendarView.cs: Week selection calendar
AppTheme.cs: Centralized styling definitions
9. Development Process
9.1 Adding New Features
Plan the feature and identify required models/UI components
Update or extend existing models as needed
Implement UI components following the AppTheme guidelines
Integrate with existing forms and navigation
9.2 Common Patterns
Use async/await for API calls
Implement local caching where appropriate
Pass model instances between forms
Follow the established UI theming system
9.3 Error Handling
Models include proper API error handling
UI components display appropriate error messages
Local storage fallbacks when network unavailable
10. Recent Implementations
10.1 Subject/Topic Selection
Recently implemented in the hourly report interface:
Subject dropdown shows subjects from curriculum
Topic dropdown dynamically updates based on selected subject
Selected topic is saved to the hourly report
10.2 Curriculum Display
Hierarchical display of subjects and topics
Expandable sections for detailed view
Hours tracking for each subject
11. API Integration
11.1 Authentication Endpoint
https://training.elexbo.de/studentLogin/loginByemailPassword
POST request with email, password, company_id
Returns student data and authentication tokens
11.2 Release Endpoint
https://learnpath.elexbo.de/release/read/?id={releaseId}
GET request with Authorization header
Returns curriculum data for the specified release ID
11.3 Report Endpoints
Report submission API integration for weekly reports
Authorization using student tokens
JSON payloads following specified structure
12. Local Storage
12.1 Release Data Caching
Stored in LocalApplicationData folder
Filename pattern: release_{releaseId}.json
Validation before using cached data
12.2 Report Drafts
Weekly reports can be saved locally
Follows specified JSON structure
Includes metadata for identification
13. Next Development Areas
The application has several potential enhancement areas:
Usability improvements for subject/topic selection
Report status visualization enhancements
Validation and error handling improvements
Performance optimization
Data synchronization refinements
UI polishing and accessibility features
Reporting analytics implementation
This document provides a comprehensive overview of the Student App's architecture, implementation, and development practices. When adding new features or debugging issues, refer to this guide to understand the application's structure and follow the established patterns.
