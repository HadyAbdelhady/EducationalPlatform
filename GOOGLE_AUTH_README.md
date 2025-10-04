# Google Authentication Implementation

This document describes the Google OAuth authentication implementation for the Educational Platform, following Clean Architecture principles and the MediatR design pattern.

## Architecture Overview

The implementation follows Clean Architecture with clear separation of concerns:

```
┌─────────────────────────────────────────────────────────────┐
│                        Presentation Layer                    │
│                 (Edu_Base - API Controllers)                 │
│  • StudentAuthController                                     │
│  • InstructorAuthController                                  │
└─────────────────┬───────────────────────────────────────────┘
                  │
                  │ HTTP Requests
                  │
┌─────────────────▼───────────────────────────────────────────┐
│                      Application Layer                       │
│  • Commands & Handlers (MediatR)                            │
│  • DTOs & Validation (FluentValidation)                     │
│  • Interfaces                                                │
│    - IGoogleAuthService                                      │
│    - IUserRepository                                         │
└─────────────────┬───────────────────────────────────────────┘
                  │
                  │ Dependencies
                  │
┌─────────────────▼───────────────────────────────────────────┐
│                   Infrastructure Layer                       │
│  • GoogleAuthService (Google.Apis.Auth)                     │
│  • UserRepository (EF Core)                                  │
│  • EducationDbContext                                        │
└─────────────────┬───────────────────────────────────────────┘
                  │
                  │ Entity References
                  │
┌─────────────────▼───────────────────────────────────────────┐
│                       Domain Layer                           │
│  • User Entity                                               │
│  • Student Entity (with DeviceId, ScreenshotTrial)          │
│  • Instructor Entity                                         │
└─────────────────────────────────────────────────────────────┘
```

## Features

### Student Authentication
- **Google Login**: Authenticates students using Google OAuth
- **Additional Properties**: Captures student-specific data (DeviceId, ScreenshotTrial)
- **Account Creation**: Automatically creates student account on first login
- **Profile Update**: Updates student profile on subsequent logins

### Instructor Authentication
- **Google Login**: Authenticates instructors using Google OAuth
- **Account Creation**: Automatically creates instructor account on first login
- **Profile Update**: Updates instructor profile on subsequent logins

### Logout
- Shared logout functionality for both students and instructors
- Can be extended for session invalidation and token revocation

## API Endpoints

### Student Endpoints

#### POST `/api/StudentAuth/google-login`
Authenticates a student using Google OAuth.

**Request Body:**
```json
{
  "idToken": "google_id_token_here",
  "deviceId": "unique_device_identifier",
  "phoneNumber": "+1234567890",
  "dateOfBirth": "2005-01-15",
  "gender": "Male",
  "educationYear": "High School Senior",
  "locationMaps": "optional_location_url"
}
```

**Response (200 OK):**
```json
{
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "fullName": "John Doe",
  "email": "john.doe@gmail.com",
  "profilePictureUrl": "https://lh3.googleusercontent.com/...",
  "userRole": "Student",
  "isNewUser": true,
  "authenticatedAt": "2025-10-04T14:23:53Z"
}
```

#### POST `/api/StudentAuth/logout`
Logs out a student.

**Request Body:**
```json
"3fa85f64-5717-4562-b3fc-2c963f66afa6"
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Logged out successfully"
}
```

### Instructor Endpoints

#### POST `/api/InstructorAuth/google-login`
Authenticates an instructor using Google OAuth.

**Request Body:**
```json
{
  "idToken": "google_id_token_here",
  "phoneNumber": "+1234567890",
  "dateOfBirth": "1990-05-20",
  "gender": "Female",
  "educationYear": "Ph.D. in Computer Science",
  "locationMaps": "optional_location_url"
}
```

**Response (200 OK):**
```json
{
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "fullName": "Jane Smith",
  "email": "jane.smith@gmail.com",
  "profilePictureUrl": "https://lh3.googleusercontent.com/...",
  "userRole": "Instructor",
  "isNewUser": false,
  "authenticatedAt": "2025-10-04T14:23:53Z"
}
```

#### POST `/api/InstructorAuth/logout`
Logs out an instructor.

**Request Body:**
```json
"3fa85f64-5717-4562-b3fc-2c963f66afa6"
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Logged out successfully"
}
```

## Configuration

### Required Packages

The following NuGet packages are used:

- **Application Layer:**
  - `MediatR` (12.4.1)
  - `FluentValidation.DependencyInjectionExtensions` (11.10.0)

- **Infrastructure Layer:**
  - `Google.Apis.Auth` (1.68.0)
  - `Microsoft.EntityFrameworkCore` (9.0.9)
  - `Npgsql.EntityFrameworkCore.PostgreSQL` (9.0.2)

- **Presentation Layer:**
  - `Microsoft.AspNetCore.Authentication.Google` (9.0.0)

### Google OAuth Setup

1. **Create Google OAuth Credentials:**
   - Go to [Google Cloud Console](https://console.cloud.google.com/)
   - Create a new project or select an existing one
   - Enable Google+ API
   - Go to "Credentials" → "Create Credentials" → "OAuth 2.0 Client ID"
   - Select "Web application"
   - Add authorized redirect URIs (e.g., `https://localhost:5001/signin-google`)
   - Copy the Client ID

2. **Update Configuration:**
   
   Edit `appsettings.json`:
   ```json
   {
     "Authentication": {
       "Google": {
         "ClientId": "YOUR_ACTUAL_CLIENT_ID.apps.googleusercontent.com"
       }
     },
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=EducationDb;Username=postgres;Password=your_password"
     }
   }
   ```

   For production, use environment variables or Azure Key Vault:
   ```bash
   dotnet user-secrets set "Authentication:Google:ClientId" "YOUR_CLIENT_ID"
   ```

## Key Components

### Application Layer

#### Commands
- `StudentGoogleLoginCommand`: Command for student Google authentication
- `InstructorGoogleLoginCommand`: Command for instructor Google authentication
- `GoogleLogoutCommand`: Command for logout operations

#### Handlers
- `StudentGoogleLoginCommandHandler`: Processes student Google login
- `InstructorGoogleLoginCommandHandler`: Processes instructor Google login
- `GoogleLogoutCommandHandler`: Processes logout requests

#### Validators
- `StudentGoogleLoginCommandValidator`: Validates student login requests
- `InstructorGoogleLoginCommandValidator`: Validates instructor login requests

#### DTOs
- `GoogleUserInfo`: Contains Google user information
- `StudentGoogleLoginRequest`: Student-specific login request
- `InstructorGoogleLoginRequest`: Instructor-specific login request
- `AuthenticationResponse`: Unified authentication response

### Infrastructure Layer

#### Services
- `GoogleAuthService`: Validates Google ID tokens using Google.Apis.Auth

#### Repositories
- `UserRepository`: Manages user, student, and instructor data persistence

## Validation Rules

### Student Validation
- **ID Token**: Required
- **Device ID**: Required (student-specific)
- **Phone Number**: Required, valid international format
- **Date of Birth**: Required, minimum age 10 years
- **Gender**: Required, must be "Male", "Female", or "Other"
- **Education Year**: Required

### Instructor Validation
- **ID Token**: Required
- **Phone Number**: Required, valid international format
- **Date of Birth**: Required, minimum age 18 years
- **Gender**: Required, must be "Male", "Female", or "Other"
- **Education Year**: Required

## Database Schema

### User Table
- `Id` (Guid, PK)
- `FullName`
- `Ssn`
- `PhoneNumber`
- `GmailExternal` - Stores Google email
- `PersonalPictureUrl` - Stores Google profile picture
- `DateOfBirth`
- `Gender`
- `EducationYear`
- `LocationMaps`
- `CreatedAt`
- `UpdatedAt`
- `IsDeleted`

### Student Table
- `UserId` (Guid, FK to User)
- `DeviceId` - Unique device identifier
- `ScreenshotTrial` - Counter for screenshot attempts
- `CreatedAt`
- `UpdatedAt`
- `IsDeleted`

### Instructor Table
- `UserId` (Guid, FK to User)
- `CreatedAt`
- `UpdatedAt`
- `IsDeleted`

## Security Considerations

1. **Token Validation**: Google ID tokens are validated using Google's official library
2. **Email Verification**: Only verified Google emails are accepted
3. **HTTPS**: Always use HTTPS in production
4. **Secrets Management**: Never commit Client IDs/Secrets to source control
5. **CORS**: Configure CORS policy appropriately for production

## Client Integration

### Frontend Implementation (Example)

```javascript
// Initialize Google Sign-In
google.accounts.id.initialize({
  client_id: 'YOUR_CLIENT_ID.apps.googleusercontent.com',
  callback: handleGoogleResponse
});

// Handle Google Sign-In Response
async function handleGoogleResponse(response) {
  const idToken = response.credential;
  
  // For Student Login
  const studentData = {
    idToken: idToken,
    deviceId: getDeviceId(),
    phoneNumber: "+1234567890",
    dateOfBirth: "2005-01-15",
    gender: "Male",
    educationYear: "High School Senior"
  };
  
  const result = await fetch('/api/StudentAuth/google-login', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(studentData)
  });
  
  const authResponse = await result.json();
  console.log('Authenticated:', authResponse);
}
```

## Error Handling

### Common Error Responses

**401 Unauthorized:**
```json
{
  "message": "Invalid Google token or email not verified."
}
```

**400 Bad Request:**
```json
{
  "errors": {
    "PhoneNumber": ["Phone number must be in a valid format."],
    "DateOfBirth": ["Student must be at least 10 years old."]
  }
}
```

**500 Internal Server Error:**
```json
{
  "message": "An error occurred during authentication"
}
```

## Testing

### Manual Testing with Swagger/OpenAPI

1. Run the application
2. Navigate to the OpenAPI endpoint (configured in development mode)
3. Use the Swagger UI to test endpoints
4. Obtain a Google ID token from Google OAuth Playground

### Unit Testing Example

```csharp
[Fact]
public async Task StudentGoogleLogin_ValidToken_ReturnsAuthResponse()
{
    // Arrange
    var command = new StudentGoogleLoginCommand
    {
        IdToken = "valid_token",
        DeviceId = "device123",
        // ... other properties
    };
    
    // Act
    var result = await _handler.Handle(command, CancellationToken.None);
    
    // Assert
    Assert.NotNull(result);
    Assert.Equal("Student", result.UserRole);
}
```

## Future Enhancements

- Add refresh token support
- Implement JWT token generation for API authorization
- Add session management
- Implement token revocation
- Add audit logging for authentication events
- Support for additional OAuth providers (Apple, Facebook, etc.)

## Support

For issues or questions, please refer to:
- Google OAuth Documentation: https://developers.google.com/identity
- MediatR Documentation: https://github.com/jbogard/MediatR
- FluentValidation Documentation: https://docs.fluentvalidation.net/
