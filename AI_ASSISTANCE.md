# Student App - AI Development Assistance Document

## Application Overview
This is a Windows Forms application designed for student activities management, featuring a unified authentication system and modular architecture. The application uses a single token system for all API communications and maintains a consistent UI through LayoutForms.

### System Tray Integration
- **Purpose**: Application runs in system tray for easy access and monitoring
- **Implementation**:
  ```csharp
  public class SystemTrayApplication : Form
  {
      private NotifyIcon trayIcon;
      private ContextMenuStrip trayMenu;
      private bool isExiting = false;

      public SystemTrayApplication()
      {
          InitializeTrayIcon();
          InitializeTrayMenu();
      }

      private void InitializeTrayIcon()
      {
          trayIcon = new NotifyIcon
          {
              Icon = Properties.Resources.AppIcon, // Application icon
              Text = "Student App",
              Visible = true
          };

          // Handle double-click to show main form
          trayIcon.DoubleClick += (s, e) => ShowMainForm();
          
          // Handle form closing
          this.FormClosing += (s, e) =>
          {
              if (!isExiting)
              {
                  e.Cancel = true;
                  this.Hide();
                  return;
              }
          };
      }

      private void InitializeTrayMenu()
      {
          trayMenu = new ContextMenuStrip();
          trayMenu.Items.Add("Open Dashboard", null, (s, e) => ShowMainForm());
          trayMenu.Items.Add("Reports", null, (s, e) => ShowReports());
          trayMenu.Items.Add("Attendance", null, (s, e) => ShowAttendance());
          trayMenu.Items.AddSeparator();
          trayMenu.Items.Add("Exit", null, (s, e) => ExitApplication());

          trayIcon.ContextMenuStrip = trayMenu;
      }

      private void ShowMainForm()
      {
          this.Show();
          this.WindowState = FormWindowState.Normal;
          this.Activate();
      }

      private void ExitApplication()
      {
          isExiting = true;
          trayIcon.Visible = false;
          Application.Exit();
      }
  }
  ```

- **Features**:
  - Persistent system tray icon
  - Context menu for quick actions
  - Minimize to tray instead of closing
  - Quick access to main features
  - Status indicators in icon

- **Best Practices**:
  - Always provide a way to exit the application
  - Show status through icon changes
  - Handle system events (sleep, wake)
  - Maintain state between sessions
  - Provide quick access to common actions

## Core Architecture

### 1. Authentication System
- **Single Token System**: Uses one authentication token for all services
- **Token Types**:
  - Access Token: Short-lived token for immediate API access
  - Refresh Token: Long-lived token for obtaining new access tokens
- **Implementation**: `TokenService` handles all token-related operations
  - Automatic token refresh
  - Token storage and management
  - Secure token handling

### 2. API Communication Layer
- **Base Implementation**: `BaseApiService` provides core API functionality
- **Features**:
  - Automatic token inclusion in all requests
  - Error handling and logging
  - JSON serialization/deserialization
- **Usage Pattern**:
  ```csharp
  // Example of extending BaseApiService
  public class ReportsService : BaseApiService
  {
      public ReportsService(ITokenService tokenService) : base(tokenService) { }
      
      public async Task<Report> GetReportAsync(string reportId)
      {
          return await GetAsync<Report>($"reports/{reportId}");
      }
  }
  ```

### 3. UI Architecture
- **LayoutForm System**:
  - Base form class providing consistent UI elements
  - Handles common layout components
  - Standardizes form appearance and behavior

- **Base LayoutForm Implementation**:
  ```csharp
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
              disposedValue = true;
          }
          base.Dispose(disposing);
      }
  }
  ```

- **Key Features**:
  1. **Base Panel**: `mainContentPanel` provides a consistent container for all forms
  2. **Virtual Methods**: Allow customization while maintaining consistency
  3. **Proper Disposal**: Handles resource cleanup correctly
  4. **Standard Styling**: Common colors, sizes, and positioning

- **Inheritance Pattern**:
  ```csharp
  public partial class CustomForm : LayoutForm
  {
      // Initialize form-specific controls at declaration
      private TextBox customTextBox = new();
      private Button customButton = new();
      private Label customLabel = new();

      public CustomForm()
      {
          InitializeComponent();
          InitializeCustomControls();
      }

      protected override void InitializeComponent()
      {
          base.InitializeComponent();  // Keep base styling
          this.Text = "Custom Form";   // Override specific properties
          this.Size = new Size(400, 500);
      }

      private void InitializeCustomControls()
      {
          // Initialize control properties
          customLabel.Text = "Custom Form";
          customLabel.Font = new Font("Segoe UI", 20, FontStyle.Bold);
          
          // Add controls to the mainContentPanel
          mainContentPanel.Controls.AddRange(new Control[] {
              customLabel,
              customTextBox,
              customButton
          });
      }
  }
  ```

### Form Development Best Practices

#### 1. Control Declaration
- **DO**: Initialize controls at declaration
  ```csharp
  private TextBox usernameTextBox = new();
  private Button submitButton = new();
  ```
- **DON'T**: Leave controls uninitialized
  ```csharp
  private TextBox usernameTextBox; // Avoid this
  ```

#### 2. Form Initialization
- **DO**: Follow the initialization order
  ```csharp
  public CustomForm()
  {
      InitializeComponent();      // First: Base setup
      InitializeCustomControls(); // Second: Custom setup
  }
  ```
- **DON'T**: Skip base initialization
  ```csharp
  protected override void InitializeComponent()
  {
      // Don't forget base.InitializeComponent();
      this.Text = "Custom Form"; // Wrong: Base not called
  }
  ```

#### 3. Control Layout
- **DO**: Use the mainContentPanel
  ```csharp
  mainContentPanel.Controls.AddRange(new Control[] {
      headerLabel,
      contentPanel,
      footerPanel
  });
  ```
- **DON'T**: Add controls directly to the form
  ```csharp
  this.Controls.Add(someControl); // Avoid this
  ```

#### 4. Event Handling
- **DO**: Use null-safe event handlers
  ```csharp
  private async void Button_Click(object? sender, EventArgs e)
  ```
- **DON'T**: Ignore nullable warnings
  ```csharp
  private void Button_Click(object sender, EventArgs e) // Avoid this
  ```

#### 5. Resource Management
- **DO**: Properly dispose of resources
  ```csharp
  protected override void Dispose(bool disposing)
  {
      if (!disposedValue)
      {
          if (disposing)
          {
              customResource?.Dispose();
          }
          disposedValue = true;
      }
      base.Dispose(disposing);
  }
  ```
- **DON'T**: Create disposable resources without proper cleanup

#### 6. Form Styling
- **DO**: Use consistent colors and fonts
  ```csharp
  button.BackColor = Color.FromArgb(0, 122, 204); // Standard blue
  label.Font = new Font("Segoe UI", 10);          // Standard font
  ```
- **DON'T**: Use hardcoded or inconsistent styles

#### 7. Error Handling
- **DO**: Provide user feedback
  ```csharp
  private void ShowError(string message)
  {
      errorLabel.Text = message;
      errorLabel.Visible = true;
  }
  ```
- **DON'T**: Silently fail or show technical errors

#### 8. Form Navigation
- **DO**: Handle form lifecycle properly
  ```csharp
  private void ShowNextForm()
  {
      var nextForm = new NextForm();
      this.Hide();
      nextForm.ShowDialog();
      this.Close();
  }
  ```
- **DON'T**: Leave forms in memory

### Form Testing Checklist
1. **Initialization**
   - [ ] Base properties inherited correctly
   - [ ] Custom properties set properly
   - [ ] All controls initialized

2. **Layout**
   - [ ] Controls properly positioned
   - [ ] Responsive to window resizing
   - [ ] Consistent padding and margins

3. **Functionality**
   - [ ] All event handlers working
   - [ ] Error handling in place
   - [ ] Navigation working correctly

4. **Resource Management**
   - [ ] No memory leaks
   - [ ] Resources properly disposed
   - [ ] Event handlers properly cleaned up

## Design Patterns

### 1. Service Factory Pattern
- **Purpose**: Manages service instances and dependencies
- **Implementation**: `ServiceFactory` class
- **Usage**:
  ```csharp
  var tokenService = ServiceFactory.GetTokenService();
  var apiService = ServiceFactory.GetApiService();
  ```

### 2. Interface-based Design
- **Purpose**: Enables loose coupling and testability
- **Key Interfaces**:
  - `ITokenService`
  - `IApiService`
- **Benefits**:
  - Easy to mock for testing
  - Simple to swap implementations
  - Clear contract definition

### 3. Configuration Management
- **Purpose**: Centralized configuration handling
- **Implementation**: `AppConfig` class
- **Features**:
  - Environment-specific settings
  - Default values
  - Type-safe configuration access

## API Integration Guidelines

### 1. Token Management
- All API requests must use the token service
- Token refresh is handled automatically
- No direct token manipulation in service classes

### 2. API Service Implementation
- Extend `BaseApiService` for new services
- Use provided GET/POST methods
- Handle errors appropriately
- Example:
  ```csharp
  public class AttendanceService : BaseApiService
  {
      public async Task<AttendanceRecord> MarkAttendanceAsync(AttendanceData data)
      {
          return await PostAsync<AttendanceRecord>("attendance/mark", data);
      }
  }
  ```

### 3. Error Handling
- Use try-catch blocks
- Log errors appropriately
- Provide meaningful error messages
- Handle token-related errors

### 4. API Endpoints Structure
- **Authentication Endpoints**:
  - `/auth/login`: Obtain access and refresh tokens
  - `/auth/refresh`: Refresh access token
  - `/auth/logout`: Invalidate current tokens

- **Reports Endpoints**:
  - `/reports/hourly`: Submit hourly reports
  - `/reports/daily`: Submit daily reports
  - `/reports/weekly`: Submit weekly reports
  - `/reports/history`: Get report history

- **Attendance Endpoints**:
  - `/attendance/mark`: Mark attendance
  - `/attendance/status`: Get attendance status
  - `/attendance/history`: Get attendance history

- **Schedule Endpoints**:
  - `/schedule/current`: Get current schedule
  - `/schedule/upcoming`: Get upcoming activities
  - `/schedule/calendar`: Get calendar events

### 5. API Response Structure
```json
{
    "success": true,
    "data": {
        // Response data specific to endpoint
    },
    "error": null,
    "message": "Operation successful"
}
```

### 6. API Error Codes
- 400: Bad Request
- 401: Unauthorized
- 403: Forbidden
- 404: Not Found
- 500: Internal Server Error
- 503: Service Unavailable

## UI Development Guidelines

### 1. LayoutForm Usage
- Inherit from `LayoutForm` for all forms
- Maintain consistent styling
- Use provided layout components
- Example:
  ```csharp
  public class ReportForm : LayoutForm
  {
      private Panel contentPanel;
      
      public ReportForm()
      {
          InitializeComponent();
          InitializeLayoutComponents();
      }
      
      private void InitializeLayoutComponents()
      {
          // Add custom layout components
      }
  }
  ```

### 2. UI Components
- Use standard Windows Forms controls
- Maintain consistent styling
- Follow accessibility guidelines
- Implement responsive design

### 3. LayoutForm Common Components
- **Header Panel**:
  - Application logo
  - User information
  - Navigation menu
  - Logout button

- **Side Menu**:
  - Navigation links
  - Collapsible sections
  - Active state indicators
  - Icons for visual recognition

- **Main Content Area**:
  - Scrollable panel
  - Responsive layout
  - Loading indicators
  - Error message display

- **Footer Panel**:
  - Copyright information
  - Version number
  - Status indicators
  - Quick action buttons

### 4. Common Styling
```csharp
// Color Scheme
public static class AppColors
{
    public static Color Primary = Color.FromArgb(45, 45, 48);
    public static Color Secondary = Color.FromArgb(0, 122, 204);
    public static Color Background = Color.FromArgb(240, 240, 240);
    public static Color Text = Color.FromArgb(45, 45, 48);
    public static Color Error = Color.FromArgb(232, 17, 35);
    public static Color Success = Color.FromArgb(16, 124, 16);
}

// Font Settings
public static class AppFonts
{
    public static Font Title = new Font("Segoe UI", 16, FontStyle.Bold);
    public static Font Heading = new Font("Segoe UI", 14, FontStyle.Bold);
    public static Font Body = new Font("Segoe UI", 9, FontStyle.Regular);
    public static Font Small = new Font("Segoe UI", 8, FontStyle.Regular);
}

// Common Margins and Padding
public static class AppSpacing
{
    public static int Small = 4;
    public static int Medium = 8;
    public static int Large = 16;
    public static int ExtraLarge = 24;
}
```

## Future Development Guidelines

### 1. Adding New Features
1. Create new service class extending `BaseApiService`
2. Define necessary interfaces
3. Implement UI forms inheriting from `LayoutForm`
4. Add configuration settings if needed
5. Update service factory if required

### 2. API Integration
1. Define API endpoints in `AppConfig`
2. Create service class for the API
3. Implement necessary models
4. Add error handling
5. Update documentation

### 3. UI Development
1. Create new form inheriting from `LayoutForm`
2. Implement required UI components
3. Add event handlers
4. Implement data binding
5. Test responsiveness and accessibility

## Best Practices

### 1. Code Organization
- Follow namespace conventions
- Keep classes focused and single-responsibility
- Use appropriate access modifiers
- Document public APIs

### 2. Error Handling
- Use try-catch blocks
- Log errors appropriately
- Provide user-friendly error messages
- Handle edge cases

### 3. Performance
- Implement proper disposal of resources
- Use async/await appropriately
- Cache data when appropriate
- Optimize API calls

### 4. Security
- Never store sensitive data in code
- Use secure communication
- Implement proper authentication
- Follow security best practices

## Testing Guidelines

### 1. Unit Testing
- Test service classes
- Mock dependencies
- Test error scenarios
- Verify token handling

### 2. Integration Testing
- Test API communication
- Verify token refresh
- Test error handling
- Validate data flow

### 3. UI Testing
- Test form layouts
- Verify responsiveness
- Test user interactions
- Validate accessibility

## Maintenance

### 1. Regular Tasks
- Update dependencies
- Review error logs
- Monitor API performance
- Update documentation

### 2. Security Updates
- Update token handling
- Review security practices
- Update authentication
- Monitor for vulnerabilities

### 3. Performance Optimization
- Review API calls
- Optimize data caching
- Monitor memory usage
- Update UI responsiveness

## Data Models

### 1. Core Models
```csharp
public class Student
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Course { get; set; }
    public DateTime EnrollmentDate { get; set; }
}

public class Report
{
    public string Id { get; set; }
    public string StudentId { get; set; }
    public DateTime Timestamp { get; set; }
    public string Content { get; set; }
    public ReportType Type { get; set; }
    public ReportStatus Status { get; set; }
}

public class Attendance
{
    public string Id { get; set; }
    public string StudentId { get; set; }
    public DateTime Date { get; set; }
    public AttendanceStatus Status { get; set; }
    public string Notes { get; set; }
}
```

### 2. Enums
```csharp
public enum ReportType
{
    Hourly,
    Daily,
    Weekly
}

public enum ReportStatus
{
    Draft,
    Submitted,
    Approved,
    Rejected
}

public enum AttendanceStatus
{
    Present,
    Absent,
    Late,
    Excused
}
```

### 3. DTOs (Data Transfer Objects)
```csharp
public class ReportSubmissionDto
{
    public string StudentId { get; set; }
    public string Content { get; set; }
    public ReportType Type { get; set; }
}

public class AttendanceSubmissionDto
{
    public string StudentId { get; set; }
    public DateTime Date { get; set; }
    public AttendanceStatus Status { get; set; }
    public string Notes { get; set; }
}
```

### 4. Model Relationships
- Student has many Reports (1:N)
- Student has many Attendance records (1:N)
- Report belongs to one Student (N:1)
- Attendance belongs to one Student (N:1)

### 5. Data Validation Rules
- Student:
  - Id: Required, unique
  - Name: Required, 2-100 characters
  - Email: Required, valid email format
  - Course: Required, valid course code

- Report:
  - Id: Required, unique
  - StudentId: Required, valid student reference
  - Content: Required, max 5000 characters
  - Type: Required, valid enum value
  - Status: Required, valid enum value

- Attendance:
  - Id: Required, unique
  - StudentId: Required, valid student reference
  - Date: Required, valid date
  - Status: Required, valid enum value
  - Notes: Optional, max 500 characters

## Build Process Learnings

### 1. Framework and Dependencies
- **Target Framework**: Use .NET 8.0 for latest features and support
  ```xml
  <TargetFramework>net8.0-windows</TargetFramework>
  ```
- **Package Management**:
  - Keep package versions aligned with framework version
  - Use explicit versions for stability
  ```xml
  <PackageReference Include="System.Configuration.ConfigurationManager" Version="8.0.0" />
  ```

### 2. Form Inheritance Hierarchy
- **Base Form Structure**:
  ```csharp
  public partial class LayoutForm : Form
  {
      protected virtual void InitializeComponent()
      {
          // Base initialization
      }

      protected virtual void InitializeLayoutComponents()
      {
          // Custom layout components
      }
  }
  ```

- **Derived Forms**:
  ```csharp
  public partial class CustomForm : LayoutForm
  {
      protected override void InitializeComponent()
      {
          base.InitializeComponent();
          // Custom initialization
      }
  }
  ```

### 3. Windows Forms Designer Integration
- **Designer File Structure**:
  ```csharp
  partial class FormName
  {
      private System.ComponentModel.IContainer components = null;

      protected override void Dispose(bool disposing)
      {
          if (disposing && (components != null))
          {
              components.Dispose();
          }
          base.Dispose(disposing);
      }
  }
  ```

- **Best Practices**:
  - Keep designer-generated code separate
  - Use partial classes for clean separation
  - Override InitializeComponent carefully
  - Handle component disposal properly

### 4. API Service Architecture
- **Interface Definition**:
  ```csharp
  public interface IApiService
  {
      Task<T?> GetAsync<T>(string endpoint) where T : class;
      Task<T?> PostAsync<T>(string endpoint, object data) where T : class;
  }
  ```

- **Base Implementation**:
  ```csharp
  public abstract class BaseApiService : IApiService
  {
      protected readonly ITokenService _tokenService;
      protected readonly string _baseUrl;

      protected BaseApiService(ITokenService tokenService, string? baseUrl = null)
      {
          _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
          _baseUrl = baseUrl ?? AppConfig.ApiBaseUrl;
      }
  }
  ```

### 5. Nullable Reference Types
- **Service Methods**:
  - Use nullable return types for API responses
  - Add class constraints for generic type parameters
  - Handle null cases explicitly
  ```csharp
  public virtual async Task<T?> GetAsync<T>(string endpoint) where T : class
  ```

- **Property Declarations**:
  - Initialize required fields in constructor
  - Use null-forgiving operator when appropriate
  - Document nullable behavior

### 6. Service Factory Pattern
- **Implementation**:
  ```csharp
  public static class ServiceFactory
  {
      private static ITokenService? _tokenService;
      private static IApiService? _apiService;

      public static IApiService GetApiService()
      {
          if (_apiService == null)
          {
              _apiService = new ApiService(GetTokenService(), AppConfig.ApiBaseUrl);
          }
          return _apiService;
      }
  }
  ```

- **Best Practices**:
  - Lazy initialization of services
  - Proper parameter order in constructors
  - Clear dependency management
  - Service lifetime control

## Common Issues and Solutions

### 1. Designer File Conflicts
**Problem**: Multiple InitializeComponent methods in derived forms
**Solution**: 
- Make base InitializeComponent virtual
- Override in derived classes
- Remove duplicate designer-generated methods

### 2. Nullable Reference Warnings
**Problem**: Compiler warnings about possible null references
**Solution**:
- Add proper constraints to generic type parameters
- Initialize fields in constructor
- Use null-coalescing operators
- Document nullable behavior

### 3. Service Dependencies
**Problem**: Incorrect parameter order in service construction
**Solution**:
- Follow consistent parameter order
- Use named parameters when appropriate
- Document parameter requirements
- Add null checks in constructors

### 4. Form Inheritance
**Problem**: Conflicts between base and derived form initialization
**Solution**:
- Use partial classes
- Make base methods virtual
- Override carefully in derived classes
- Maintain proper initialization order

## Development Guidelines

### 1. Form Development
1. Create partial class
2. Inherit from LayoutForm
3. Override InitializeComponent
4. Add custom initialization
5. Handle component disposal

### 2. Service Development
1. Define interface
2. Create base implementation
3. Add nullable constraints
4. Implement error handling
5. Use dependency injection

### 3. Build Process
1. Update target framework
2. Align package versions
3. Fix compiler warnings
4. Handle nullable references
5. Test thoroughly

## Testing Strategy

### 1. Build Verification
- Run full build before commits
- Address all warnings
- Test form initialization
- Verify service creation

### 2. Runtime Testing
- Test form lifecycle
- Verify service dependencies
- Check error handling
- Validate UI behavior

### 3. Integration Testing
- Test form navigation
- Verify API communication
- Check token handling
- Validate data flow

## Future Development

### 1. Adding Features
1. Plan inheritance hierarchy
2. Design service interfaces
3. Implement base classes
4. Add specific functionality
5. Update documentation

### 2. Maintenance
1. Keep framework updated
2. Monitor dependencies
3. Address technical debt
4. Improve error handling
5. Update best practices

### 3. Performance
1. Optimize service calls
2. Improve form loading
3. Enhance error handling
4. Reduce memory usage
5. Monitor API performance

## Conclusion
This document serves as a living guide for development, incorporating lessons learned from the build process and establishing best practices for future development. Regular updates should be made as new patterns and solutions are discovered. 