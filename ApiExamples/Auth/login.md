# Student Login API

## Endpoint
```
POST https://training.elexbo.de/studentLogin/loginByemailPassword
```

## Request
The request should be sent as form data (application/x-www-form-urlencoded) with the following fields:

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| email | string | Yes | Student's email address |
| password | string | Yes | Student's password |

### Example Request
```
email=max@muster.com&password=your_password
```

> **Note**: All APIs in this system accept form data (application/x-www-form-urlencoded) for POST requests. This is the standard format for all RESTful endpoints.

## Response

### Success Response (200 OK)
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
            "group_name": "A",
            "email": "max@muster.com",
            "password": "",
            "first_name": "max",
            "last_name": "Muster",
            "register_at": "2025-03-01 16:48:20",
            "register_by": 55,
            "supervisor_id": null,
            "mentor_id": 55,
            "mentor_name": "Khorsandfard, Ali",
            "advisor_id": "55",
            "advisor_name": "Khorsandfard, Ali",
            "join_course_date": "0000-00-00",
            "exit_course_date": "0000-00-00",
            "graduate": null,
            "course_plan_id": 35,
            "branch_id": 36,
            "release_id": 10,
            "program_id": 40,
            "start_date": "2025-10-16",
            "end_date": "2027-10-16",
            "plan_name": "FISI Winter 2025",
            "tag": "IT.FISI.W.2025",
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
            },
            {
                "id": 2,
                "course_plan_id": 35,
                "day_number": 2,
                "start_time": "08:00:00",
                "end_time": "16:00:00",
                "day_name": "Tuesday"
            },
            {
                "id": 3,
                "course_plan_id": 35,
                "day_number": 3,
                "start_time": "08:00:00",
                "end_time": "16:00:00",
                "day_name": "Wednesday"
            },
            {
                "id": 4,
                "course_plan_id": 35,
                "day_number": 4,
                "start_time": "08:00:00",
                "end_time": "16:00:00",
                "day_name": "Thursday"
            },
            {
                "id": 5,
                "course_plan_id": 35,
                "day_number": 5,
                "start_time": "08:00:00",
                "end_time": "15:00:00",
                "day_name": "Friday"
            }
        ],
        "access_token": "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9...",
        "refresh_token": "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9..."
    }
}
```

## Data Models

### Student Object
| Field | Type | Description |
|-------|------|-------------|
| id | integer | Unique identifier for the student |
| company_id | integer | ID of the company the student belongs to |
| course_id | integer | ID of the enrolled course |
| grade_score | float | Student's grade score (null if not set) |
| group_name | string | Student's group name |
| email | string | Student's email address |
| first_name | string | Student's first name |
| last_name | string | Student's last name |
| register_at | datetime | Registration timestamp |
| register_by | integer | ID of the user who registered the student |
| mentor_id | integer | ID of the student's mentor |
| mentor_name | string | Full name of the mentor |
| advisor_id | string | ID of the student's advisor |
| advisor_name | string | Full name of the advisor |
| join_course_date | date | Date when student joined the course |
| exit_course_date | date | Date when student exited the course |
| course_plan_id | integer | ID of the course plan |
| branch_id | integer | ID of the branch |
| release_id | integer | ID of the release |
| program_id | integer | ID of the program |
| start_date | date | Course start date |
| end_date | date | Course end date |
| plan_name | string | Name of the course plan |
| tag | string | Course tag/identifier |
| is_active | boolean | Whether the student is active |

### Working Days Object
The `days` array contains information about the working days in a week, including their schedule.

| Field | Type | Description |
|-------|------|-------------|
| id | integer | Unique identifier for the day |
| course_plan_id | integer | ID of the associated course plan |
| day_number | integer | Day number in the week (1-5) |
| start_time | time | Start time of the working day (HH:mm:ss) |
| end_time | time | End time of the working day (HH:mm:ss) |
| day_name | string | Name of the day |

### Authentication Tokens
The response includes two JWT tokens:
1. `access_token`: Used for authenticating API requests
2. `refresh_token`: Used to obtain a new access token when it expires

## Notes
- The working days are Monday through Friday
- Friday has a different end time (15:00) compared to other days (16:00)
- The access token and refresh token are JWT tokens that should be stored securely
- The student's password is not returned in the response for security reasons 