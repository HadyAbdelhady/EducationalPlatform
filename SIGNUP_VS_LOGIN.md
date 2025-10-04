# SignUp vs Login - Understanding the Difference

## Overview

The application now has **separate endpoints** for SignUp and Login operations, both using Google OAuth authentication.

## Key Differences

### SignUp Endpoints
- **Purpose**: Create a new user account
- **Behavior**: 
  - ✅ Creates a new User + Student/Instructor record
  - ❌ Fails if user already exists (409 Conflict)
  - Returns `IsNewUser: true`
- **Use Case**: First-time users registering with Google

### Login Endpoints
- **Purpose**: Authenticate existing users OR auto-create if new
- **Behavior**: 
  - ✅ Returns existing user if found
  - ✅ Creates new user if not found
  - Updates profile picture on each login
  - Returns `IsNewUser: true/false` based on status
- **Use Case**: Returning users or flexible authentication

## API Endpoints

| Endpoint | Method | Purpose | New Users | Existing Users |
|----------|--------|---------|-----------|----------------|
| `/api/StudentAuth/google-signup` | POST | Student SignUp | ✅ Creates | ❌ 409 Conflict |
| `/api/StudentAuth/google-login` | POST | Student Login | ✅ Creates | ✅ Authenticates |
| `/api/InstructorAuth/google-signup` | POST | Instructor SignUp | ✅ Creates | ❌ 409 Conflict |
| `/api/InstructorAuth/google-login` | POST | Instructor Login | ✅ Creates | ✅ Authenticates |

## When to Use Each

### Use SignUp When:
1. You have a dedicated registration flow in your UI
2. You want strict separation between new and existing users
3. You need to ensure users go through a specific onboarding process
4. You want to prevent accidental account creation

**Example Flow:**
```
New User → Click "Sign Up with Google" → /google-signup → Success
Existing User → Click "Sign Up with Google" → /google-signup → 409 Error → Redirect to Login
```

### Use Login When:
1. You want a unified "Sign in with Google" button
2. You don't mind auto-creating accounts
3. You prefer a simpler user experience
4. Your app doesn't require explicit signup

**Example Flow:**
```
Any User → Click "Sign in with Google" → /google-login → Success (auto-creates if needed)
```

## Request/Response Examples

### Student SignUp Request

**POST** `/api/StudentAuth/google-signup`

```json
{
  "idToken": "eyJhbGciOiJSUzI1NiIs...",
  "deviceId": "device-123",
  "phoneNumber": "+201234567890",
  "dateOfBirth": "2005-03-15",
  "gender": "Male",
  "educationYear": "Grade 10",
  "locationMaps": null
}
```

**Success Response (200 OK):**
```json
{
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "fullName": "Ahmed Mohamed",
  "email": "ahmed.mohamed@gmail.com",
  "profilePictureUrl": "https://lh3.googleusercontent.com/...",
  "userRole": "Student",
  "isNewUser": true,
  "authenticatedAt": "2025-10-04T16:25:57Z"
}
```

**Error Response - User Exists (409 Conflict):**
```json
{
  "message": "User already exists. Please use login instead."
}
```

### Student Login Request

**POST** `/api/StudentAuth/google-login`

```json
{
  "idToken": "eyJhbGciOiJSUzI1NiIs...",
  "deviceId": "device-123",
  "phoneNumber": "+201234567890",
  "dateOfBirth": "2005-03-15",
  "gender": "Male",
  "educationYear": "Grade 10",
  "locationMaps": null
}
```

**Success Response - New User (200 OK):**
```json
{
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "fullName": "Ahmed Mohamed",
  "email": "ahmed.mohamed@gmail.com",
  "profilePictureUrl": "https://lh3.googleusercontent.com/...",
  "userRole": "Student",
  "isNewUser": true,
  "authenticatedAt": "2025-10-04T16:25:57Z"
}
```

**Success Response - Existing User (200 OK):**
```json
{
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "fullName": "Ahmed Mohamed",
  "email": "ahmed.mohamed@gmail.com",
  "profilePictureUrl": "https://lh3.googleusercontent.com/...",
  "userRole": "Student",
  "isNewUser": false,
  "authenticatedAt": "2025-10-04T16:25:57Z"
}
```

## Frontend Implementation Examples

### Option 1: Separate SignUp/Login Buttons

```typescript
// SignUp Flow
async function handleGoogleSignUp(credentialResponse: any) {
  try {
    const response = await axios.post('/api/StudentAuth/google-signup', {
      idToken: credentialResponse.credential,
      deviceId: getDeviceId(),
      // ... other fields
    });
    
    // Always new user
    showWelcomeMessage();
    redirectToOnboarding();
    
  } catch (error) {
    if (error.response?.status === 409) {
      // User exists - redirect to login
      showMessage('Account already exists. Please login.');
      redirectToLogin();
    } else {
      showError('Signup failed');
    }
  }
}

// Login Flow
async function handleGoogleLogin(credentialResponse: any) {
  try {
    const response = await axios.post('/api/StudentAuth/google-login', {
      idToken: credentialResponse.credential,
      deviceId: getDeviceId(),
      // ... other fields
    });
    
    if (response.data.isNewUser) {
      showWelcomeMessage();
      redirectToOnboarding();
    } else {
      showMessage('Welcome back!');
      redirectToDashboard();
    }
    
  } catch (error) {
    showError('Login failed');
  }
}
```

### Option 2: Unified Button (Auto-fallback)

```typescript
async function handleGoogleAuth(credentialResponse: any) {
  // Try signup first
  try {
    const response = await axios.post('/api/StudentAuth/google-signup', {
      idToken: credentialResponse.credential,
      deviceId: getDeviceId(),
      // ... other fields
    });
    
    showWelcomeMessage();
    redirectToOnboarding();
    
  } catch (error) {
    if (error.response?.status === 409) {
      // User exists, try login
      const loginResponse = await axios.post('/api/StudentAuth/google-login', {
        idToken: credentialResponse.credential,
        deviceId: getDeviceId(),
        // ... other fields
      });
      
      showMessage('Welcome back!');
      redirectToDashboard();
    } else {
      showError('Authentication failed');
    }
  }
}
```

### Option 3: Simple Unified Login (Recommended for MVP)

```typescript
async function handleGoogleAuth(credentialResponse: any) {
  try {
    const response = await axios.post('/api/StudentAuth/google-login', {
      idToken: credentialResponse.credential,
      deviceId: getDeviceId(),
      // ... other fields
    });
    
    if (response.data.isNewUser) {
      showWelcomeMessage();
      redirectToOnboarding();
    } else {
      showMessage('Welcome back!');
      redirectToDashboard();
    }
    
  } catch (error) {
    showError('Authentication failed');
  }
}
```

## Error Handling

### SignUp Errors

| Status | Error | Meaning | Action |
|--------|-------|---------|--------|
| 400 | Bad Request | Invalid data (validation failed) | Fix request data |
| 401 | Unauthorized | Invalid Google token | Get new token |
| 409 | Conflict | User already exists | Use login instead |
| 500 | Server Error | Internal error | Retry later |

### Login Errors

| Status | Error | Meaning | Action |
|--------|-------|---------|--------|
| 400 | Bad Request | Invalid data (validation failed) | Fix request data |
| 401 | Unauthorized | Invalid Google token | Get new token |
| 500 | Server Error | Internal error | Retry later |

## Database Behavior

### SignUp Operation
```
1. Validate Google token
2. Check if user exists by email
3. If exists → Throw error (409)
4. If not exists → Create User + Student/Instructor
5. Return success
```

### Login Operation
```
1. Validate Google token
2. Check if user exists by email
3. If exists → Update profile picture, return user
4. If not exists → Create User + Student/Instructor
5. Return success
```

## Best Practices

### For Mobile Apps
✅ **Use Login** - Simpler UX, users don't need to understand the difference

### For Web Apps with Marketing
✅ **Use Separate SignUp/Login** - Better analytics, clearer user journey

### For MVP/Prototype
✅ **Use Login** - Faster development, fewer edge cases

### For Enterprise Apps
✅ **Use Separate SignUp/Login** - Better control, audit trail, compliance

## Security Considerations

Both endpoints:
- ✅ Validate Google ID tokens
- ✅ Check email verification
- ✅ Apply FluentValidation rules
- ✅ Log authentication attempts
- ✅ Handle errors gracefully

SignUp provides additional security by:
- ✅ Preventing accidental duplicate accounts
- ✅ Ensuring explicit user consent
- ✅ Better tracking of new user acquisition

## Complete Endpoint List

### Student Endpoints
- `POST /api/StudentAuth/google-signup` - Create new student account
- `POST /api/StudentAuth/google-login` - Login or auto-create student
- `POST /api/StudentAuth/logout` - Logout student

### Instructor Endpoints
- `POST /api/InstructorAuth/google-signup` - Create new instructor account
- `POST /api/InstructorAuth/google-login` - Login or auto-create instructor
- `POST /api/InstructorAuth/logout` - Logout instructor

## Testing with Swagger

1. Start the application: `dotnet run`
2. Navigate to: `http://localhost:5000/`
3. Find `StudentAuth` or `InstructorAuth` sections
4. Test `google-signup` with a new Google account
5. Test `google-login` with the same account
6. Observe `IsNewUser` flag in responses

## Summary

- **SignUp**: Strict, only creates new accounts, fails for existing users
- **Login**: Flexible, works for both new and existing users
- Both use the same request format
- Both return the same response format
- Choose based on your application's UX requirements
