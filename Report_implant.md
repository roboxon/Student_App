Purpose: Calculate report status efficiently with client-side processing
Methods:
- CalculateWeekStatus(WeeklyReport, DateTime) : ReportStatus
- GetRequiredHoursForWeek(DateTime) : decimal
- GetReportedHoursFromData(WeeklyReport) : decimal
- InvalidateCache(DateTime) : void

Purpose: Manage local storage of report files
Methods:
- SaveWeekReportAsync(DateTime, WeeklyReport) : Task<bool>
- GetWeekReportAsync(DateTime) : Task<WeeklyReport>
- RemoveAfterSync(DateTime) : void
- GetStorageFilePath(DateTime) : string

Purpose: Display calendar with status visualization
Features:
- Month navigation
- Week selection
- Color-coded status indicators
- Date range restriction

Purpose: Main interface for creating/editing reports
Sections:
- Calendar view panel
- Daily report tabs
- Hourly report grid
- Summary section
- Action buttons (Save/Submit)

Phase 1: Core Models & Data Structures
Create data models aligned with JSON structures in requirements
Implement serialization/deserialization logic
Define status enums and constants
Phase 2: Client-Side Processing Components
Implement ReportStatusCalculator with caching
Create ReportDateValidator with enrollment date checking
Develop LocalReportStorageManager for file operations
Build WeekReportManager to coordinate workflow
Phase 3: Basic UI Framework
Create ReportCalendarView control
Develop WeeklyReportForm inheriting from LayoutForm
Implement basic navigation and integration with dashboard
Phase 4: Detailed UI Components
Build HourlyReportControl with subject/topic selection
Develop DailySummaryControl for daily overview
Create status visualization components
Implement drag-drop functionality if needed
Phase 5: API Integration & Synchronization
Add API endpoints to AppConfig
Implement report submission in Weekly Report model
Create synchronization logic with error handling
Test offline/online transitions
Phase 6: Testing & Refinement
Unit test client-side calculations
Verify date restrictions
Test local storage operations
Validate API integration
Performance test with large data sets
5. Component Details
5.1 ReportStatusCalculator
5.2 LocalReportStorageManager
5.3 ReportCalendarView
5.4 WeeklyReportForm
6. UI Integration
The Weekly Reporting System will be integrated into the existing application using:
New menu item in Dashboard side menu
WeeklyReportForm inheriting from LayoutForm
Consistent styling using AppTheme
Fixed width of 850px following app standards
7. Data Flow
Student selects week in calendar
System loads existing reports if available
Student enters hourly reports
System calculates status client-side
Student saves locally or submits to server
System updates calendar visualization
8. API Endpoints
| Endpoint | Method | Purpose |
|----------|--------|---------|
| /weeklyReport/upload | POST | Submit completed report |
| /weeklyReport/{year}/{weekNumber} | GET | Retrieve existing report |
9. Performance Considerations
Implement calculation caching to reduce CPU usage
Use background tasks for non-UI calculations
Progressive loading of calendar data
Efficient local storage with cleanup after sync
10. Implementation Timeline
| Phase | Estimated Time | Dependencies |
|-------|----------------|--------------|
| Phase 1: Core Models | 2 days | None |
| Phase 2: Client-Side Processing | 3 days | Phase 1 |
| Phase 3: Basic UI Framework | 2 days | Phase 1 |
| Phase 4: Detailed UI Components | 4 days | Phase 3 |
| Phase 5: API Integration | 3 days | Phase 1, 2 |
| Phase 6: Testing & Refinement | 3 days | All previous phases |
11. Integration Checkpoints
Models integrated with existing Student model
UI components follow AppTheme guidelines
Form navigation works with existing Dashboard
Local storage uses consistent patterns
API calls follow existing authentication pattern
12. Next Steps
Implement models and basic data structures
Create UI mockups for approval
Develop client-side calculation components
Build UI framework and integrate with existing app
Complete remaining components as per phases
