# JWT Authentication Guide

## Overview

This guide explains how JWT (JSON Web Token) authentication works in the Educational Platform after Google OAuth signup/login.

## Authentication Flow

```
┌─────────────┐         ┌──────────────┐         ┌─────────────┐
│   Frontend  │         │    Google    │         │  Your API   │
└──────┬──────┘         └──────┬───────┘         └──────┬──────┘
       │                       │                        │
       │ 1. "Sign up with      │                        │
       │    Google"            │                        │
       ├──────────────────────>│                        │
       │                       │                        │
       │ 2. Google ID Token    │                        │
       │<──────────────────────┤                        │
       │                       │                        │
       │ 3. POST /api/StudentAuth/google-signup         │
       │    { idToken: "...", ... }                     │
       ├───────────────────────────────────────────────>│
       │                       │                        │
       │                       │ 4. Validate token      │
       │                       │<───────────────────────┤
       │                       │                        │
       │                       │ 5. Token valid         │
       │                       ├───────────────────────>│
       │                       │                        │
       │ 6. JWT Token + User Info                       │
       │<───────────────────────────────────────────────┤
       │                       │                        │
       │ 7. Store JWT token in localStorage/cookies     │
       │                       │                        │
       │ 8. Future API calls with JWT token             │
       │    Authorization: Bearer {jwt_token}           │
       ├───────────────────────────────────────────────>│
       │                       │                        │
       │ 9. Protected resource response                 │
       │<───────────────────────────────────────────────┤
```

## Step-by-Step Process

### 1. User Signup/Login with Google

**Request:**
```http
POST /api/StudentAuth/google-signup
Content-Type: application/json

{
  "idToken": "google_id_token_from_google_oauth",
  "deviceId": "unique_device_id",
  "ssn": "12345678",
  "phoneNumber": "+1234567890",
  "dateOfBirth": "2005-01-15",
  "gender": "Male",
  "educationYear": "High School Senior"
}
```

**Response:**
```json
{
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "fullName": "John Doe",
  "email": "john.doe@gmail.com",
  "profilePictureUrl": "https://lh3.googleusercontent.com/...",
  "userRole": "Student",
  "isNewUser": true,
  "authenticatedAt": "2025-10-04T16:32:30Z",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "tokenExpiresAt": "2025-10-05T16:32:30Z"
}
```

### 2. Store the JWT Token

The frontend must store the JWT token securely:

```javascript
// Store in localStorage (simpler but less secure)
localStorage.setItem('jwt_token', response.token);

// OR store in httpOnly cookie (more secure - recommended)
// This should be done server-side by setting the cookie in the response
```

### 3. Use JWT Token for Subsequent API Calls

For all protected endpoints, include the JWT token in the Authorization header:

```javascript
// Example fetch request
const response = await fetch('/api/Profile/me', {
  method: 'GET',
  headers: {
    'Authorization': `Bearer ${localStorage.getItem('jwt_token')}`,
    'Content-Type': 'application/json'
  }
});
```

```http
GET /api/Profile/me
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## Protected Endpoints

### Example 1: Any Authenticated User

```csharp
[HttpGet("me")]
[Authorize] // Any authenticated user (Student or Instructor)
public IActionResult GetCurrentUser()
{
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    var email = User.FindFirst(ClaimTypes.Email)?.Value;
    // ... access user info from JWT claims
}
```

### Example 2: Student-Only Endpoint

```csharp
[HttpGet("student-only")]
[Authorize(Roles = "Student")] // Only students
public IActionResult StudentOnly()
{
    // Only users with "Student" role can access
}
```

### Example 3: Instructor-Only Endpoint

```csharp
[HttpGet("instructor-only")]
[Authorize(Roles = "Instructor")] // Only instructors
public IActionResult InstructorOnly()
{
    // Only users with "Instructor" role can access
}
```

### Example 4: Multiple Roles

```csharp
[HttpGet("dashboard")]
[Authorize(Roles = "Student,Instructor")] // Both can access
public IActionResult Dashboard()
{
    // Both students and instructors can access
}
```

## JWT Token Structure

The JWT token contains the following claims:

```json
{
  "sub": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "email": "john.doe@gmail.com",
  "role": "Student",
  "name": "John Doe",
  "nameid": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "jti": "unique-token-id",
  "exp": 1728151950,
  "iss": "EducationalPlatform",
  "aud": "EducationalPlatformUsers"
}
```

You can decode and view the token at [jwt.io](https://jwt.io/).

## Accessing User Information in Controllers

```csharp
using System.Security.Claims;

public class YourController : ControllerBase
{
    [HttpGet("example")]
    [Authorize]
    public IActionResult Example()
    {
        // Get user ID
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userIdGuid = Guid.Parse(userId);
        
        // Get email
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        
        // Get role
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        
        // Get full name
        var fullName = User.FindFirst(ClaimTypes.Name)?.Value;
        
        // Check if user has specific role
        bool isStudent = User.IsInRole("Student");
        bool isInstructor = User.IsInRole("Instructor");
        
        return Ok(new { userId, email, role, fullName });
    }
}
```

## Configuration

### appsettings.json

```json
{
  "Jwt": {
    "SecretKey": "YOUR_SUPER_SECRET_KEY_MUST_BE_AT_LEAST_32_CHARACTERS_LONG",
    "Issuer": "EducationalPlatform",
    "Audience": "EducationalPlatformUsers",
    "ExpirationMinutes": "1440"
  }
}
```

**Important:** Change the `SecretKey` to a strong, random string in production!

### Production Best Practices

1. **Never commit JWT secrets to source control**
2. **Use environment variables or Azure Key Vault for production**
3. **Use strong, random secret keys (at least 32 characters)**
4. **Set appropriate token expiration times**
5. **Use HTTPS in production**
6. **Consider implementing refresh tokens for long-lived sessions**

## Testing with Swagger

1. **Start the application**
2. **Navigate to Swagger UI** (usually at `http://localhost:5000`)
3. **Call the signup/login endpoint** to get a JWT token
4. **Copy the token from the response**
5. **Click the "Authorize" button** at the top of Swagger UI
6. **Enter:** `Bearer {your_token_here}` (include the word "Bearer" with a space)
7. **Click "Authorize"**
8. **Now you can test protected endpoints**

## Frontend Integration Example

### React/JavaScript Example

```javascript
// auth.js - Authentication service
class AuthService {
  async signUpWithGoogle(googleIdToken, additionalData) {
    const response = await fetch('/api/StudentAuth/google-signup', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        idToken: googleIdToken,
        ...additionalData
      })
    });
    
    const data = await response.json();
    
    // Store JWT token
    localStorage.setItem('jwt_token', data.token);
    localStorage.setItem('user_info', JSON.stringify({
      userId: data.userId,
      fullName: data.fullName,
      email: data.email,
      role: data.userRole
    }));
    
    return data;
  }
  
  getToken() {
    return localStorage.getItem('jwt_token');
  }
  
  isAuthenticated() {
    const token = this.getToken();
    if (!token) return false;
    
    // Check if token is expired
    const payload = JSON.parse(atob(token.split('.')[1]));
    return payload.exp * 1000 > Date.now();
  }
  
  logout() {
    localStorage.removeItem('jwt_token');
    localStorage.removeItem('user_info');
  }
}

// api.js - API service with JWT
class ApiService {
  async fetchProtectedResource(endpoint) {
    const token = localStorage.getItem('jwt_token');
    
    const response = await fetch(endpoint, {
      method: 'GET',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    });
    
    if (response.status === 401) {
      // Token expired or invalid - redirect to login
      window.location.href = '/login';
      return;
    }
    
    return await response.json();
  }
}
```

## Error Responses

### 401 Unauthorized (No Token or Invalid Token)
```json
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401
}
```

### 403 Forbidden (Valid Token but Insufficient Permissions)
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.3",
  "title": "Forbidden",
  "status": 403
}
```

## Token Expiration

- **Default expiration:** 24 hours (1440 minutes)
- **Configurable in:** `appsettings.json` → `Jwt:ExpirationMinutes`
- **Frontend should:** Handle token expiration gracefully
- **Recommendation:** Implement token refresh mechanism or re-authenticate with Google

## Security Considerations

1. **HTTPS Only:** Always use HTTPS in production
2. **Secure Storage:** Use httpOnly cookies when possible
3. **CORS:** Configure CORS appropriately
4. **Token Rotation:** Consider implementing refresh tokens
5. **Logout:** Clear tokens on logout
6. **XSS Protection:** Sanitize all user inputs
7. **CSRF Protection:** Use anti-CSRF tokens for state-changing operations

## FAQ

**Q: Where does the idToken come from?**
A: It comes from Google OAuth when the user signs in with Google on your frontend.

**Q: Do I need both Google's idToken AND your JWT?**
A: Google's idToken is only used during signup/login. Your JWT is used for all subsequent API calls.

**Q: How long is the JWT valid?**
A: By default 24 hours, configurable in `appsettings.json`.

**Q: What happens when the JWT expires?**
A: The API will return 401 Unauthorized. The user needs to login again with Google.

**Q: Can I refresh the JWT without re-authenticating?**
A: Not currently implemented. You can add a refresh token mechanism for this.

**Q: How do I protect a controller or action?**
A: Add `[Authorize]` attribute to the controller or action method.

## Next Steps

1. **Implement token refresh mechanism** (optional but recommended)
2. **Add proper error handling** for expired tokens
3. **Implement logout functionality** that invalidates tokens
4. **Set up proper CORS** for your frontend domain
5. **Use environment variables** for secrets in production
6. **Consider adding rate limiting** to prevent abuse
7. **Implement audit logging** for authentication events

## Support

For questions about JWT implementation, refer to:
- [JWT.io](https://jwt.io/)
- [Microsoft JWT Bearer Authentication](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/)
- [OWASP JWT Security Best Practices](https://cheatsheetseries.owasp.org/cheatsheets/JSON_Web_Token_for_Java_Cheat_Sheet.html)
