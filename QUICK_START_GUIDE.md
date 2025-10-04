# Quick Start Guide - Google Authentication

This guide will help you quickly set up and test the Google authentication endpoints.

## Prerequisites

- .NET 9.0 SDK
- PostgreSQL database
- Google Cloud Project with OAuth 2.0 credentials

## Step 1: Install Required Packages

All necessary packages are already configured in the project files. Restore them by running:

```powershell
cd c:\Users\ASUS\source\EducationalPlatform
dotnet restore
```

## Step 2: Configure Google OAuth

### 2.1 Create Google OAuth Credentials

1. Go to [Google Cloud Console](https://console.cloud.google.com/)
2. Create a new project or select existing
3. Navigate to **APIs & Services** → **Credentials**
4. Click **Create Credentials** → **OAuth 2.0 Client ID**
5. Configure OAuth consent screen if prompted
6. Select **Web application** as application type
7. Add Authorized redirect URIs:
   - `https://localhost:5001/signin-google`
   - `http://localhost:5000/signin-google`
8. Click **Create** and copy the **Client ID**

### 2.2 Update Configuration

Edit `Edu_Base/appsettings.Development.json`:

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

**Replace:**
- `YOUR_ACTUAL_CLIENT_ID` with your Google Client ID
- Database connection string with your PostgreSQL credentials

## Step 3: Setup Database

### 3.1 Ensure PostgreSQL is Running

Make sure your PostgreSQL server is running on `localhost:5432`.

### 3.2 Create Database

```sql
CREATE DATABASE EducationDb;
```

### 3.3 Run Migrations (if available)

```powershell
cd Edu_Base
dotnet ef database update
```

If migrations don't exist, the DbContext will create tables on first run.

## Step 4: Build and Run

```powershell
cd Edu_Base
dotnet build
dotnet run
```

The application will start at:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`

Swagger UI will be available at the root: `http://localhost:5000/`

## Step 5: Test the Endpoints

### Option A: Using Swagger UI

1. Open browser to `http://localhost:5000/`
2. You'll see the Swagger documentation
3. Expand the `/api/StudentAuth/google-login` endpoint
4. Click **Try it out**
5. Enter test data (you'll need a valid Google ID token)
6. Click **Execute**

### Option B: Using Google OAuth Playground

1. Go to [Google OAuth 2.0 Playground](https://developers.google.com/oauthplayground/)
2. Click the gear icon (⚙️) in the top right
3. Check "Use your own OAuth credentials"
4. Enter your Client ID
5. In Step 1, select **Google OAuth2 API v2** → **userinfo.email** and **userinfo.profile**
6. Click **Authorize APIs**
7. In Step 2, click **Exchange authorization code for tokens**
8. Copy the **id_token** from the response

### Option C: Using cURL or Postman

**Student Login Request:**

```bash
curl -X POST "http://localhost:5000/api/StudentAuth/google-login" \
  -H "Content-Type: application/json" \
  -d '{
    "idToken": "YOUR_GOOGLE_ID_TOKEN",
    "deviceId": "test-device-123",
    "phoneNumber": "+1234567890",
    "dateOfBirth": "2005-01-15",
    "gender": "Male",
    "educationYear": "High School Senior",
    "locationMaps": null
  }'
```

**Instructor Login Request:**

```bash
curl -X POST "http://localhost:5000/api/InstructorAuth/google-login" \
  -H "Content-Type: application/json" \
  -d '{
    "idToken": "YOUR_GOOGLE_ID_TOKEN",
    "phoneNumber": "+1234567890",
    "dateOfBirth": "1990-05-20",
    "gender": "Female",
    "educationYear": "Ph.D. in Computer Science",
    "locationMaps": null
  }'
```

**Logout Request:**

```bash
curl -X POST "http://localhost:5000/api/StudentAuth/logout" \
  -H "Content-Type: application/json" \
  -d '"3fa85f64-5717-4562-b3fc-2c963f66afa6"'
```

## Step 6: Verify Database

After successful login, verify the data in PostgreSQL:

```sql
-- Check users
SELECT * FROM "Users" WHERE "GmailExternal" IS NOT NULL;

-- Check students
SELECT * FROM "Students";

-- Check instructors
SELECT * FROM "Instructors";
```

## Common Issues & Solutions

### Issue 1: "Google Client ID is not configured"
**Solution:** Make sure you've updated `appsettings.Development.json` with your actual Google Client ID.

### Issue 2: "Invalid Google token"
**Solution:** 
- Ensure the ID token is recent (they expire quickly)
- Verify the Client ID in the token matches your application's Client ID
- Get a fresh token from Google OAuth Playground

### Issue 3: Database Connection Error
**Solution:** 
- Verify PostgreSQL is running
- Check connection string in `appsettings.Development.json`
- Ensure the database `EducationDb` exists

### Issue 4: "Assembly Application could not be loaded"
**Solution:**
```powershell
cd Application
dotnet build
cd ../Infrastructure
dotnet build
cd ../Edu_Base
dotnet build
```

### Issue 5: FluentValidation Errors
**Solution:** Make sure all required fields are provided:
- Students need `DeviceId`
- Both need `PhoneNumber`, `DateOfBirth`, `Gender`, `EducationYear`
- Age validation: Students ≥ 10 years, Instructors ≥ 18 years

## Project Structure

```
EducationalPlatform/
├── Domain/                      # Entities (User, Student, Instructor)
├── Application/                 # Business Logic
│   ├── DTOs/Auth/              # Data Transfer Objects
│   ├── Features/Auth/          # MediatR Commands & Handlers
│   └── Interfaces/             # Service Interfaces
├── Infrastructure/              # External Services
│   ├── Services/               # GoogleAuthService
│   ├── Repositories/           # UserRepository
│   └── Data/                   # DbContext
└── Edu_Base/                   # API Layer
    ├── Controllers/            # StudentAuthController, InstructorAuthController
    ├── Program.cs              # Dependency Injection Setup
    └── appsettings.json        # Configuration
```

## API Endpoints Summary

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/StudentAuth/google-login` | Student Google login |
| POST | `/api/StudentAuth/logout` | Student logout |
| POST | `/api/InstructorAuth/google-login` | Instructor Google login |
| POST | `/api/InstructorAuth/logout` | Instructor logout |

## Next Steps

1. **Add JWT Token Generation:** Implement JWT tokens for stateless authentication
2. **Add Authorization:** Implement role-based authorization using `[Authorize]` attributes
3. **Add Refresh Tokens:** Implement refresh token mechanism
4. **Client Integration:** Integrate with your frontend application
5. **Add Logging:** Configure Serilog or similar for better logging
6. **Add Unit Tests:** Create unit tests for handlers and services

## Additional Resources

- [Full Documentation](./GOOGLE_AUTH_README.md)
- [Google OAuth Documentation](https://developers.google.com/identity/protocols/oauth2)
- [MediatR Documentation](https://github.com/jbogard/MediatR)
- [Clean Architecture Guide](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

## Getting Help

If you encounter issues:
1. Check the console output for detailed error messages
2. Review the logs in the console
3. Verify all configuration settings
4. Ensure all NuGet packages are restored
5. Check the database connection and migrations

Happy coding! 🚀
