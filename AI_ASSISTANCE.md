# Student App - AI Development Assistance Document

## Application Overview
This is a Windows Forms application designed for student activities management, featuring a unified authentication system and modular architecture. The application uses a single token system for all API communications and includes a development mode for debugging API responses.

### Development Mode
- **Status**: Implemented
- **Location**: Forms/ApiResponseViewer.cs
- **Features**:
  - JSON response formatting
  - Readable display of API responses
  - Monospace font for better readability
  - Close button for easy dismissal
  - Automatic JSON indentation
- **Configuration**:
  - Controlled by `Environment` setting in App.config
  - Default: Development mode enabled
  - Production mode disables response viewer
- **Usage**:
  - Automatically shows API responses in development mode
  - Helps with debugging and API integration
  - Disabled in production for security

### System Tray Integration
- **Status**: Implemented
- **Location**: Forms/SystemTrayApplication.cs
- **Features**:
  - Tray icon with application icon
  - Context menu with the following items:
    - Open Dashboard
    - Reports (placeholder)
    - Attendance (placeholder)
    - Exit
  - Form state management:
    - Minimizes to tray instead of closing
    - Double-click to restore
    - Proper cleanup on exit
  - Dashboard integration:
    - Maintains single instance
    - Handles window state
    - Proper disposal

### Dashboard Implementation
- **Current Status**: Modern UI Implementation
- **Location**: Forms/Dashboard.cs
- **Features**:
  - Modern header with user info
  - Sleek side menu with hover effects
  - Clean content area with rounded corners
  - Status footer with version info
  - Stats cards with key metrics
  - Recent activity list
  - Upcoming schedule panel
  - Responsive layout
  - Professional color scheme
  - Consistent typography
- **Layout Components**:
  1. Header Panel:
     - Application title
     - User information
     - Professional styling
  2. Side Menu:
     - Navigation links with hover effects
     - Active state indicators
     - Clean, modern design
  3. Main Content:
     - Stats cards with metrics
     - Activity feed with detailed view
     - Schedule panel
     - Rounded corners and shadows
  4. Footer:
     - Version information
     - Connection status
     - Clean, minimal design

### Authentication System Implementation
- **Status**: Implemented and Enhanced
- **Location**: Forms/Login.cs
- **Features**:
  - Modern split-panel design with gradient background
  - Form-data based authentication
  - Token management through TokenService
  - Remember Email functionality with checkbox
  - Email persistence across sessions
  - Hardcoded company ID (no UI input needed)
  - Error handling and user feedback
  - Proper form control management
  - Development mode response viewing
  - Draggable borderless window
  - Rounded corners with shadow effects
  - Professional branding panel
  - Modern input styling with floating labels
  - Interactive hover effects
  - Smooth animations and transitions
- **Visual Components**:
  1. Brand Panel (Left):
     - Gradient background with smooth transition
     - Circular logo with shadow
     - Welcome heading
     - Subtitle text
     - Professional typography
  2. Login Panel (Right):
     - Clean white background
     - Modern input fields with floating labels
     - Password field with bullet points
     - Remember Email checkbox with hover effects
     - Sign-in button with hover effect
     - Error message area
     - Close button with red hover effect
  3. Interactive Features:
     - Window dragging from both panels
     - Input field focus effects
     - Button hover animations
     - Checkbox hover effects with color transition
     - Close button color transition
- **API Endpoint**: https://training.elexbo.de/studentLogin/loginByemailPassword
- **Request Format**:
  ```
  email=student@example.com
  password=password123
  company_id=1  // Hardcoded, no UI input needed
  ```
- **Response Structure**:
  ```json
  {
      "response_code": 200,
      "message": "OK",
      "count": 1,
      "service_message": "login seccessfull",
      "data": {
          "student": {
              "id": 1,
              "company_id": 1,
              "course_id": 20,
              "grade_score": null,
              "group_name": "string",
              "email": "string",
              "first_name": "string",
              "last_name": "string",
              "register_at": "string",
              "register_by": 1,
              "mentor_id": null,
              "mentor_name": "string",
              "advisor_id": "string",
              "advisor_name": "string",
              "join_course_date": "string",
              "exit_course_date": "string",
              "course_plan_id": 1,
              "branch_id": 1,
              "release_id": 1,
              "program_id": 1,
              "start_date": "string",
              "end_date": "string",
              "plan_name": "string",
              "tag": "string",
              "is_active": 1
          },
          "days": [
              {
                  "id": 1,
                  "course_plan_id": 35,
                  "day_number": 1,
                  "start_time": "08:00:00",
                  "end_time": "16:00:00",
                  "day_name": "Monday"
              }
          ],
          "access_token": "JWT_ACCESS_TOKEN",
          "refresh_token": "JWT_REFRESH_TOKEN"
      }
  }
  ```

## Core Architecture

### 1. Authentication System
- **Single Token System**: Uses one authentication token for all services
- **Token Types**:
  - Access Token: Short-lived token for immediate API access
  - Refresh Token: Long-lived token for obtaining new access tokens
- **Implementation**: `TokenService` handles all token-related operations
  - Token storage in memory
  - Token validation
  - Token refresh capability

### 2. API Communication Layer
- **Base Implementation**: `BaseApiService` provides core API functionality
- **Features**:
  - Automatic token inclusion in requests
  - Error handling
  - JSON serialization/deserialization
  - Development mode response viewing
- **Usage Pattern**:
  ```csharp
  public class CustomApiService : BaseApiService
  {
      public CustomApiService(ITokenService tokenService) : base(tokenService) { }
      
      public async Task<T?> GetDataAsync<T>(string endpoint) where T : class
      {
          return await GetAsync<T>(endpoint);
      }
  }
  ```

### 3. Configuration Management
- **Purpose**: Centralized configuration handling
- **Implementation**: `AppConfig` class
- **Features**:
  - Base URLs and authentication settings
  - Environment-specific settings
  - Default values with fallbacks
  - Development mode control
- **Structure**:
  ```csharp
  public static class AppConfig
  {
      // Environment settings
      public static class Environment
      {
          public static string Current => ConfigurationManager.AppSettings["Environment"] ?? "Development";
          public static bool IsDevelopment => Current.Equals("Development", StringComparison.OrdinalIgnoreCase);
          public static bool IsProduction => Current.Equals("Production", StringComparison.OrdinalIgnoreCase);
      }

      // Base URLs and authentication
      public static string TokenEndpoint => ConfigurationManager.AppSettings["TokenEndpoint"] ?? "https://training.elexbo.de/studentLogin/loginByemailPassword";
      public static string ApiBaseUrl => ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "https://training.elexbo.de";
      public static int TokenRefreshThreshold => int.Parse(ConfigurationManager.AppSettings["TokenRefreshThreshold"] ?? "300");
  }
  ```

### 4. Application State Management
- **Status**: Planning Phase
- **Purpose**: Centralized state management for application-wide data
- **Implementation**: `AppState` class
- **Features**:
  - Singleton pattern for global access
  - Thread-safe state management
  - Automatic state persistence
  - State change notifications
  - Cache management for API responses
- **Structure**:
  ```csharp
  public class AppState
  {
      private static AppState? _instance;
      private static readonly object _lock = new object();
      private bool _isInitialized = false;

      // Core application state
      public Student? CurrentStudent { get; private set; }
      public List<WorkingDay>? WorkingDays { get; private set; }
      public DateTime? LastApiCall { get; private set; }
      public int ApiCallCount { get; private set; }

      // Cache settings
      private const int CACHE_DURATION_MINUTES = 30;
      private const int MAX_API_CALLS_PER_HOUR = 100;

      // Events for state changes
      public event EventHandler<Student>? StudentChanged;
      public event EventHandler<List<WorkingDay>>? WorkingDaysChanged;
      public event EventHandler<Exception>? StateError;

      private AppState() { }

      public static AppState Instance
      {
          get
          {
              if (_instance == null)
              {
                  lock (_lock)
                  {
                      _instance ??= new AppState();
                  }
              }
              return _instance;
          }
      }

      public void Initialize(Student student, List<WorkingDay> workingDays)
      {
          if (_isInitialized) return;

          CurrentStudent = student;
          WorkingDays = workingDays;
          LastApiCall = DateTime.Now;
          _isInitialized = true;

          StudentChanged?.Invoke(this, student);
          WorkingDaysChanged?.Invoke(this, workingDays);
      }

      public bool ShouldRefreshData()
      {
          if (!LastApiCall.HasValue) return true;
          if (ApiCallCount >= MAX_API_CALLS_PER_HOUR) return false;

          return (DateTime.Now - LastApiCall.Value).TotalMinutes >= CACHE_DURATION_MINUTES;
      }

      public void UpdateStudent(Student student)
      {
          CurrentStudent = student;
          LastApiCall = DateTime.Now;
          ApiCallCount++;
          StudentChanged?.Invoke(this, student);
      }

      public void UpdateWorkingDays(List<WorkingDay> workingDays)
      {
          WorkingDays = workingDays;
          LastApiCall = DateTime.Now;
          ApiCallCount++;
          WorkingDaysChanged?.Invoke(this, workingDays);
      }

      public void Reset()
      {
          CurrentStudent = null;
          WorkingDays = null;
          LastApiCall = null;
          ApiCallCount = 0;
          _isInitialized = false;
      }
  }
  ```
- **Integration Points**:
  1. **Login Form**:
     - Initialize AppState after successful login
     - Store student and working days data
     - Handle state errors

  2. **Dashboard**:
     - Subscribe to state change events
     - Use cached data when available
     - Trigger data refresh when needed

  3. **API Services**:
     - Check AppState before making API calls
     - Update AppState after successful API calls
     - Handle cache invalidation

- **Benefits**:
  1. **Performance Optimization**:
     - Reduce unnecessary API calls
     - Cache frequently accessed data
     - Implement rate limiting

  2. **State Consistency**:
     - Centralized data management
     - Consistent state across forms
     - Automatic state updates

  3. **Error Handling**:
     - Graceful state recovery
     - Error notifications
     - State validation

- **Implementation Steps**:
  1. Create AppState class with singleton pattern
  2. Implement state management methods
  3. Add event system for state changes
  4. Integrate with existing forms
  5. Add cache management
  6. Implement error handling
  7. Add state persistence
  8. Test state management
  9. Document usage patterns

- **Usage Example**:
  ```csharp
  // In Login form after successful login
  AppState.Instance.Initialize(student, workingDays);

  // In Dashboard form
  AppState.Instance.StudentChanged += (s, student) => {
      UpdateUI(student);
  };

  // In API service
  if (AppState.Instance.ShouldRefreshData())
  {
      var data = await FetchDataFromApi();
      AppState.Instance.UpdateStudent(data.Student);
      AppState.Instance.UpdateWorkingDays(data.WorkingDays);
  }
  else
  {
      // Use cached data
      var student = AppState.Instance.CurrentStudent;
      var workingDays = AppState.Instance.WorkingDays;
  }
  ```

### Implementation Status

#### Completed Components
1. **Authentication**
   - ✓ Modern split-panel login form design
   - ✓ Professional branding with gradient background
   - ✓ Interactive UI elements with hover effects
   - ✓ Remember Email functionality
   - ✓ Email persistence across sessions
   - ✓ Enhanced checkbox styling and visibility
   - ✓ Draggable borderless window with rounded corners
   - ✓ Token service integration
   - ✓ Error handling and validation
   - ✓ API response processing
   - ✓ Navigation to dashboard
   - ✓ Form control management
   - ✓ Development mode response viewing

2. **System Tray**
   - ✓ Tray icon implementation
   - ✓ Context menu with placeholders
   - ✓ Form state management
   - ✓ Dashboard integration
   - ✓ Clean exit handling
   - ✓ Single instance management

3. **Core Services**
   - ✓ TokenService implementation
   - ✓ BaseApiService implementation
   - ✓ Configuration management
   - ✓ Interface definitions
   - ✓ Environment handling
   - ✓ Development mode support

4. **Dashboard**
   - ✓ Modern header with user info
   - ✓ Sleek side menu with hover effects
   - ✓ Clean content area with rounded corners
   - ✓ Status footer with version info
   - ✓ Stats cards with key metrics:
     - Course information
     - Group details
     - Grade score
     - Active status
   - ✓ Student Information panel with detailed view:
     - Email
     - Mentor details
     - Advisor information
     - Course dates
     - Program details
   - ✓ Working Schedule panel
   - ✓ Responsive layout
   - ✓ Professional color scheme
   - ✓ Consistent typography

5. **API Integration**
   - ✓ Authentication endpoints
   - ✓ Token management
   - ✓ Error handling
   - ✓ Development mode support
   - ✓ Response validation
   - ✓ JSON parsing and deserialization

### Pending Features
1. **Reports Module**
   - Not started
   - Core functionality needed
   - Required endpoints defined
   - Need to implement report types
   - Need to add report validation

2. **Attendance Module**
   - Not started
   - Core functionality needed
   - Required endpoints defined
   - Need to implement attendance types
   - Need to add attendance validation

3. **Additional Enhancements**
   - User profile management
   - Comprehensive logging system
   - Enhanced error handling
   - Offline mode support
   - Data caching
   - Real-time notifications

### Next Steps
1. Implement Reports functionality
2. Add Attendance tracking
3. Enhance error handling with comprehensive logging
4. Add user profile management
5. Implement data caching
6. Add offline mode support
7. Implement real-time notifications
8. Add comprehensive API response validation
9. Enhance development mode features

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
- Add hover effects for interactivity
- Use shadows and rounded corners
- Implement proper spacing

### 3. LayoutForm Common Components
- **Header Panel**:
  - Application title with proper font
  - User information display
  - Professional color scheme
  - Responsive positioning

- **Side Menu**:
  - Navigation links with hover effects
  - Active state indicators
  - Clean, modern design
  - Proper spacing and padding

- **Main Content Area**:
  - Rounded corners
  - Subtle shadows
  - Proper padding
  - Responsive layout
  - Loading indicators
  - Error message display

- **Footer Panel**:
  - Version information
  - Status indicators
  - Clean, minimal design
  - Professional styling

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

### 5. UI Best Practices
- Use consistent spacing throughout
- Implement proper padding and margins
- Add visual feedback for interactions
- Maintain clean typography hierarchy
- Use shadows and rounded corners appropriately
- Ensure proper contrast for readability
- Implement responsive layouts
- Handle window resizing properly
- Add loading states for async operations
- Provide clear error feedback

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

## Implementation Status

### Completed Components
1. **Authentication**
   - ✓ Modern split-panel login form design
   - ✓ Professional branding with gradient background
   - ✓ Interactive UI elements with hover effects
   - ✓ Remember Email functionality
   - ✓ Email persistence across sessions
   - ✓ Enhanced checkbox styling and visibility
   - ✓ Draggable borderless window with rounded corners
   - ✓ Token service integration
   - ✓ Error handling and validation
   - ✓ API response processing
   - ✓ Navigation to dashboard
   - ✓ Form control management
   - ✓ Development mode response viewing

2. **System Tray**
   - ✓ Tray icon implementation
   - ✓ Context menu with placeholders
   - ✓ Form state management
   - ✓ Dashboard integration
   - ✓ Clean exit handling
   - ✓ Single instance management

3. **Core Services**
   - ✓ TokenService implementation
   - ✓ BaseApiService implementation
   - ✓ Configuration management
   - ✓ Interface definitions
   - ✓ Environment handling
   - ✓ Development mode support

4. **Dashboard**
   - ✓ Modern header with user info
   - ✓ Sleek side menu with hover effects
   - ✓ Clean content area with rounded corners
   - ✓ Status footer with version info
   - ✓ Stats cards with key metrics:
     - Course information
     - Group details
     - Grade score
     - Active status
   - ✓ Student Information panel with detailed view:
     - Email
     - Mentor details
     - Advisor information
     - Course dates
     - Program details
   - ✓ Working Schedule panel
   - ✓ Responsive layout
   - ✓ Professional color scheme
   - ✓ Consistent typography

5. **API Integration**
   - ✓ Authentication endpoints
   - ✓ Token management
   - ✓ Error handling
   - ✓ Development mode support
   - ✓ Response validation
   - ✓ JSON parsing and deserialization

### Pending Features
1. **Reports Module**
   - Not started
   - Core functionality needed
   - Required endpoints defined
   - Need to implement report types
   - Need to add report validation

2. **Attendance Module**
   - Not started
   - Core functionality needed
   - Required endpoints defined
   - Need to implement attendance types
   - Need to add attendance validation

3. **Additional Enhancements**
   - User profile management
   - Comprehensive logging system
   - Enhanced error handling
   - Offline mode support
   - Data caching
   - Real-time notifications

### Next Steps
1. Implement Reports functionality
2. Add Attendance tracking
3. Enhance error handling with comprehensive logging
4. Add user profile management
5. Implement data caching
6. Add offline mode support
7. Implement real-time notifications
8. Add comprehensive API response validation
9. Enhance development mode features

## Development Mode
- **Status**: Implemented
- **Location**: Forms/ApiResponseViewer.cs
- **Features**:
  - JSON response formatting
  - Readable display of API responses
  - Monospace font for better readability
  - Close button for easy dismissal
  - Automatic JSON indentation
- **Configuration**:
  - Controlled by `Environment` setting in App.config
  - Default: Development mode enabled
  - Production mode disables response viewer
- **Usage**:
  - Automatically shows API responses in development mode
  - Helps with debugging and API integration
  - Disabled in production for security

## Conclusion
This document serves as a living guide for development, incorporating lessons learned from the build process and establishing best practices for future development. Regular updates should be made as new patterns and solutions are discovered. 