# Student App - AI Development Assistance Document

## Application Overview
This is a Windows Forms application designed for student activities management, featuring a unified authentication system and modular architecture. The application uses a single token system for all API communications and includes a development mode for debugging API responses.

### Login Form Implementation
- **Status**: Implemented
- **Location**: Forms/Login.cs
- **Features**:
  - Modern UI with gradient background
  - Rounded corners and shadows
  - Draggable panels for easy movement
  - Remember email functionality
  - Comprehensive error handling
  - Development mode API response viewer
  - Secure token management
  - Form validation
  - User-friendly error messages
  - Smooth transitions and animations

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
  - Clean shutdown handling

### Dashboard Implementation
- **Status**: Implemented
- **Location**: Forms/Dashboard.cs
- **Features**:
  - Modern header with user info
  - Sleek side menu with hover effects
  - Clean content area with rounded corners
  - Status footer with version info
  - Stats cards showing:
    - Course information
    - Group details
    - Grade score
    - Active status
  - Student information panel with:
    - Email
    - Mentor details
    - Advisor information
    - Course dates
    - Program details
  - Working schedule panel with:
    - Day information
    - Start/End times
    - Schedule layout
  - Responsive layout
  - Professional color scheme
  - Consistent typography

### LayoutForm Base Class
- **Status**: Implemented
- **Location**: LayoutForm.cs
- **Features**:
  - Header panel with:
    - Application title
    - User information
    - Professional styling
  - Side menu with:
    - Navigation items
    - Active state indicators
    - Hover effects
  - Content wrapper with:
    - Rounded corners
    - Subtle shadows
    - Proper padding
  - Footer panel with:
    - Version information
    - Status indicators
  - Event handling for:
    - Window resizing
    - Menu item selection
    - Form state changes
  - Proper resource disposal
  - Clean inheritance structure

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

## Design Patterns

### 1. Service Factory Pattern
- **Implementation**: 
  1. TokenService singleton pattern for token management
  2. ServiceFactory for service instance management
- **Usage**:
  ```csharp
  // Get token service instance (singleton)
  var tokenService = TokenService.Instance;
  
  // Get service instances through factory
  var serviceFactory = new ServiceFactory();
  var apiService = serviceFactory.GetApiService();
  ```
- **Benefits**:
  - Single instance for token management
  - Centralized service creation
  - Proper dependency management
  - Easy access from any form or service

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
- **Authentication Endpoint**:
  - `https://training.elexbo.de/studentLogin/loginByemailPassword`
  - Method: POST
  - Parameters:
    - email: Student email
    - password: Student password
    - company_id: Hardcoded to "1"
  - Response: LoginResponse object containing:
    - Student information
    - Working days schedule
    - Access and refresh tokens

### 5. API Response Structure
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
    public int id { get; set; }
    public int company_id { get; set; }
    public int course_id { get; set; }
    public float? grade_score { get; set; }
    public string? group_name { get; set; }
    public string? email { get; set; }
    public string? first_name { get; set; }
    public string? last_name { get; set; }
    public string? register_at { get; set; }
    public int register_by { get; set; }
    public int? mentor_id { get; set; }
    public string? mentor_name { get; set; }
    public string? advisor_id { get; set; }
    public string? advisor_name { get; set; }
    public string? join_course_date { get; set; }
    public string? exit_course_date { get; set; }
    public int course_plan_id { get; set; }
    public int branch_id { get; set; }
    public int release_id { get; set; }
    public int program_id { get; set; }
    public string? start_date { get; set; }
    public string? end_date { get; set; }
    public string? plan_name { get; set; }
    public string? tag { get; set; }
    public int is_active { get; set; }
}

public class WorkingDay
{
    public int id { get; set; }
    public int course_plan_id { get; set; }
    public int day_number { get; set; }
    public string? start_time { get; set; }
    public string? end_time { get; set; }
    public string? day_name { get; set; }
}

public class LoginResponse
{
    public int response_code { get; set; }
    public string? message { get; set; }
    public int count { get; set; }
    public string? service_message { get; set; }
    public LoginData? data { get; set; }
}

public class LoginData
{
    public Student? student { get; set; }
    public List<WorkingDay>? days { get; set; }
    public string? access_token { get; set; }
    public string? refresh_token { get; set; }
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
// Note: These DTOs are planned for future implementation
public class ReportSubmissionDto
{
    public int StudentId { get; set; }
    public string Content { get; set; }
    public ReportType Type { get; set; }
}

public class AttendanceSubmissionDto
{
    public int StudentId { get; set; }
    public DateTime Date { get; set; }
    public AttendanceStatus Status { get; set; }
    public string Notes { get; set; }
}
```

### 4. Model Relationships
- LoginResponse contains:
  - One LoginData object
  - Response metadata (code, message, count)
- LoginData contains:
  - One Student object
  - List of WorkingDay objects
  - Access and refresh tokens
- Student has:
  - Basic information (id, name, email, etc.)
  - Course information (course_id, course_plan_id, etc.)
  - Program details (program_id, branch_id, etc.)
- WorkingDay has:
  - Schedule information (day_number, start_time, end_time)
  - Course plan reference (course_plan_id)

### 5. Data Validation Rules
- Student:
  - id: Required, unique integer
  - email: Required, valid email format
  - first_name: Required, 2-50 characters
  - last_name: Required, 2-50 characters
  - is_active: Required, 0 or 1

- WorkingDay:
  - id: Required, unique integer
  - course_plan_id: Required, valid course plan reference
  - day_number: Required, positive integer
  - start_time: Required, valid time format (HH:mm:ss)
  - end_time: Required, valid time format (HH:mm:ss)
  - day_name: Required, valid day name

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

- **Concrete Implementation**:
  ```csharp
  public class ApiService : BaseApiService
  {
      public ApiService(ITokenService tokenService, string? baseUrl = null)
          : base(tokenService, baseUrl)
      {
      }
  }
  ```

- **Service Factory**:
  ```csharp
  public class ServiceFactory
  {
      public ITokenService GetTokenService()
      {
          return TokenService.Instance;
      }

      public IApiService GetApiService()
      {
          return new ApiService(GetTokenService());
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
- **Implementation**: TokenService singleton pattern
- **Usage**:
  ```csharp
  // Get token service instance
  var tokenService = TokenService.Instance;
  
  // Store tokens
  tokenService.StoreTokens(accessToken, refreshToken);
  ```
- **Benefits**:
  - Single instance throughout application
  - Thread-safe token management
  - Centralized token storage
  - Easy access from any form or service

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