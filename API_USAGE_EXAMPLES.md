# API Usage Examples

This document provides practical examples for using the Google Authentication API endpoints.

## Table of Contents
- [Student Authentication](#student-authentication)
- [Instructor Authentication](#instructor-authentication)
- [Client-Side Integration](#client-side-integration)
- [Postman Collection](#postman-collection)

## Student Authentication

### Student Google Login

**Endpoint:** `POST /api/StudentAuth/google-login`

**Headers:**
```
Content-Type: application/json
```

**Request Body:**
```json
{
  "idToken": "eyJhbGciOiJSUzI1NiIsImtpZCI6IjE4MmU0MjRhZGI3MjRmMzU4MjgyODA5MmI5NTk3MTFkOGNmOTUyMGQiLCJ0eXAiOiJKV1QifQ...",
  "deviceId": "mobile-device-12345",
  "phoneNumber": "+201234567890",
  "dateOfBirth": "2005-03-15",
  "gender": "Male",
  "educationYear": "Grade 10",
  "locationMaps": "https://maps.google.com/?q=30.0444,31.2357"
}
```

**Success Response (200 OK):**
```json
{
  "userId": "a7f2c8e4-1234-5678-9abc-def012345678",
  "fullName": "Ahmed Mohamed",
  "email": "ahmed.mohamed@gmail.com",
  "profilePictureUrl": "https://lh3.googleusercontent.com/a/default-user=s96-c",
  "userRole": "Student",
  "isNewUser": true,
  "authenticatedAt": "2025-10-04T11:29:51Z"
}
```

**Error Responses:**

**401 Unauthorized:**
```json
{
  "message": "Invalid Google token or email not verified."
}
```

**400 Bad Request (Validation Error):**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "DeviceId": [
      "Device ID is required for student accounts."
    ],
    "DateOfBirth": [
      "Student must be at least 10 years old."
    ]
  }
}
```

### Student Logout

**Endpoint:** `POST /api/StudentAuth/logout`

**Headers:**
```
Content-Type: application/json
```

**Request Body:**
```json
"a7f2c8e4-1234-5678-9abc-def012345678"
```

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Logged out successfully"
}
```

**Error Response (400 Bad Request):**
```json
{
  "message": "Invalid user ID"
}
```

## Instructor Authentication

### Instructor Google Login

**Endpoint:** `POST /api/InstructorAuth/google-login`

**Headers:**
```
Content-Type: application/json
```

**Request Body:**
```json
{
  "idToken": "eyJhbGciOiJSUzI1NiIsImtpZCI6IjE4MmU0MjRhZGI3MjRmMzU4MjgyODA5MmI5NTk3MTFkOGNmOTUyMGQiLCJ0eXAiOiJKV1QifQ...",
  "phoneNumber": "+201234567890",
  "dateOfBirth": "1985-07-20",
  "gender": "Female",
  "educationYear": "Master's Degree in Computer Science",
  "locationMaps": null
}
```

**Success Response (200 OK):**
```json
{
  "userId": "b8e3d9f5-5678-1234-abcd-ef0123456789",
  "fullName": "Dr. Sarah Ahmed",
  "email": "sarah.ahmed@gmail.com",
  "profilePictureUrl": "https://lh3.googleusercontent.com/a/default-user=s96-c",
  "userRole": "Instructor",
  "isNewUser": false,
  "authenticatedAt": "2025-10-04T11:29:51Z"
}
```

### Instructor Logout

**Endpoint:** `POST /api/InstructorAuth/logout`

**Headers:**
```
Content-Type: application/json
```

**Request Body:**
```json
"b8e3d9f5-5678-1234-abcd-ef0123456789"
```

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Logged out successfully"
}
```

## Client-Side Integration

### JavaScript/TypeScript (React/Vue/Angular)

#### Installation
```bash
npm install @react-oauth/google
# or
npm install vue3-google-oauth2
```

#### React Example

```typescript
import { GoogleOAuthProvider, GoogleLogin } from '@react-oauth/google';
import axios from 'axios';

const GOOGLE_CLIENT_ID = 'YOUR_CLIENT_ID.apps.googleusercontent.com';
const API_BASE_URL = 'http://localhost:5000/api';

function StudentLoginComponent() {
  const handleGoogleSuccess = async (credentialResponse: any) => {
    try {
      const response = await axios.post(
        `${API_BASE_URL}/StudentAuth/google-login`,
        {
          idToken: credentialResponse.credential,
          deviceId: getDeviceFingerprint(), // Implement device fingerprinting
          phoneNumber: '+201234567890', // Collect from user
          dateOfBirth: '2005-03-15', // Collect from user
          gender: 'Male', // Collect from user
          educationYear: 'Grade 10', // Collect from user
          locationMaps: null
        }
      );

      const authData = response.data;
      
      // Store authentication data
      localStorage.setItem('userId', authData.userId);
      localStorage.setItem('userRole', authData.userRole);
      localStorage.setItem('userName', authData.fullName);
      
      if (authData.isNewUser) {
        // Redirect to onboarding
        window.location.href = '/onboarding';
      } else {
        // Redirect to dashboard
        window.location.href = '/dashboard';
      }
      
    } catch (error) {
      console.error('Login failed:', error);
      alert('Login failed. Please try again.');
    }
  };

  const handleGoogleError = () => {
    console.error('Google Sign-In failed');
    alert('Google Sign-In failed');
  };

  return (
    <GoogleOAuthProvider clientId={GOOGLE_CLIENT_ID}>
      <div className="login-container">
        <h1>Student Login</h1>
        <GoogleLogin
          onSuccess={handleGoogleSuccess}
          onError={handleGoogleError}
          useOneTap
        />
      </div>
    </GoogleOAuthProvider>
  );
}

// Device fingerprinting helper
function getDeviceFingerprint(): string {
  // Simple implementation - use a library like fingerprintjs2 for production
  const nav = window.navigator;
  const screen = window.screen;
  
  const fingerprint = [
    nav.userAgent,
    nav.language,
    screen.width,
    screen.height,
    screen.colorDepth,
    new Date().getTimezoneOffset()
  ].join('|');
  
  return btoa(fingerprint).substring(0, 50);
}

// Logout function
async function handleLogout() {
  const userId = localStorage.getItem('userId');
  
  try {
    await axios.post(
      `${API_BASE_URL}/StudentAuth/logout`,
      JSON.stringify(userId),
      {
        headers: { 'Content-Type': 'application/json' }
      }
    );
    
    localStorage.clear();
    window.location.href = '/login';
  } catch (error) {
    console.error('Logout failed:', error);
  }
}

export { StudentLoginComponent, handleLogout };
```

#### Vue 3 Example

```typescript
<template>
  <div class="login-container">
    <h1>Instructor Login</h1>
    <div id="g_id_onload"
         :data-client_id="googleClientId"
         data-callback="handleCredentialResponse">
    </div>
    <div class="g_id_signin" data-type="standard"></div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import axios from 'axios';

const googleClientId = 'YOUR_CLIENT_ID.apps.googleusercontent.com';
const apiBaseUrl = 'http://localhost:5000/api';

// Attach to window for Google callback
(window as any).handleCredentialResponse = async (response: any) => {
  try {
    const result = await axios.post(
      `${apiBaseUrl}/InstructorAuth/google-login`,
      {
        idToken: response.credential,
        phoneNumber: '+201234567890', // Collect from user
        dateOfBirth: '1985-07-20', // Collect from user
        gender: 'Female', // Collect from user
        educationYear: 'Ph.D. in Education', // Collect from user
        locationMaps: null
      }
    );

    const authData = result.data;
    
    localStorage.setItem('userId', authData.userId);
    localStorage.setItem('userRole', authData.userRole);
    localStorage.setItem('userName', authData.fullName);
    
    window.location.href = '/instructor/dashboard';
    
  } catch (error) {
    console.error('Login failed:', error);
    alert('Login failed. Please try again.');
  }
};

onMounted(() => {
  // Load Google Sign-In script
  const script = document.createElement('script');
  script.src = 'https://accounts.google.com/gsi/client';
  script.async = true;
  script.defer = true;
  document.head.appendChild(script);
});
</script>
```

### Mobile Integration (Flutter)

```dart
import 'package:google_sign_in/google_sign_in.dart';
import 'package:http/http.dart' as http;
import 'dart:convert';

class GoogleAuthService {
  final GoogleSignIn _googleSignIn = GoogleSignIn(
    scopes: ['email', 'profile'],
  );
  
  final String apiBaseUrl = 'http://localhost:5000/api';

  Future<Map<String, dynamic>?> signInStudent({
    required String deviceId,
    required String phoneNumber,
    required String dateOfBirth,
    required String gender,
    required String educationYear,
  }) async {
    try {
      // Sign in with Google
      final GoogleSignInAccount? googleUser = await _googleSignIn.signIn();
      
      if (googleUser == null) {
        return null; // User cancelled
      }

      // Get authentication details
      final GoogleSignInAuthentication googleAuth = 
          await googleUser.authentication;
      
      final String? idToken = googleAuth.idToken;
      
      if (idToken == null) {
        throw Exception('Failed to get ID token');
      }

      // Call backend API
      final response = await http.post(
        Uri.parse('$apiBaseUrl/StudentAuth/google-login'),
        headers: {'Content-Type': 'application/json'},
        body: jsonEncode({
          'idToken': idToken,
          'deviceId': deviceId,
          'phoneNumber': phoneNumber,
          'dateOfBirth': dateOfBirth,
          'gender': gender,
          'educationYear': educationYear,
          'locationMaps': null,
        }),
      );

      if (response.statusCode == 200) {
        return jsonDecode(response.body);
      } else {
        throw Exception('Login failed: ${response.body}');
      }
      
    } catch (error) {
      print('Google Sign-In error: $error');
      rethrow;
    }
  }

  Future<void> signOut(String userId) async {
    try {
      // Sign out from Google
      await _googleSignIn.signOut();
      
      // Call backend logout
      await http.post(
        Uri.parse('$apiBaseUrl/StudentAuth/logout'),
        headers: {'Content-Type': 'application/json'},
        body: jsonEncode(userId),
      );
      
    } catch (error) {
      print('Sign out error: $error');
    }
  }
}
```

## Postman Collection

### Student Google Login Request

```json
{
  "info": {
    "name": "Educational Platform - Google Auth",
    "schema": "https://schema.getpostman.com/json/collection/v2.1.0/collection.json"
  },
  "item": [
    {
      "name": "Student Google Login",
      "request": {
        "method": "POST",
        "header": [
          {
            "key": "Content-Type",
            "value": "application/json"
          }
        ],
        "body": {
          "mode": "raw",
          "raw": "{\n  \"idToken\": \"{{GOOGLE_ID_TOKEN}}\",\n  \"deviceId\": \"test-device-123\",\n  \"phoneNumber\": \"+201234567890\",\n  \"dateOfBirth\": \"2005-03-15\",\n  \"gender\": \"Male\",\n  \"educationYear\": \"Grade 10\",\n  \"locationMaps\": null\n}"
        },
        "url": {
          "raw": "{{BASE_URL}}/api/StudentAuth/google-login",
          "host": ["{{BASE_URL}}"],
          "path": ["api", "StudentAuth", "google-login"]
        }
      }
    },
    {
      "name": "Student Logout",
      "request": {
        "method": "POST",
        "header": [
          {
            "key": "Content-Type",
            "value": "application/json"
          }
        ],
        "body": {
          "mode": "raw",
          "raw": "\"{{USER_ID}}\""
        },
        "url": {
          "raw": "{{BASE_URL}}/api/StudentAuth/logout",
          "host": ["{{BASE_URL}}"],
          "path": ["api", "StudentAuth", "logout"]
        }
      }
    },
    {
      "name": "Instructor Google Login",
      "request": {
        "method": "POST",
        "header": [
          {
            "key": "Content-Type",
            "value": "application/json"
          }
        ],
        "body": {
          "mode": "raw",
          "raw": "{\n  \"idToken\": \"{{GOOGLE_ID_TOKEN}}\",\n  \"phoneNumber\": \"+201234567890\",\n  \"dateOfBirth\": \"1985-07-20\",\n  \"gender\": \"Female\",\n  \"educationYear\": \"Ph.D. in Computer Science\",\n  \"locationMaps\": null\n}"
        },
        "url": {
          "raw": "{{BASE_URL}}/api/InstructorAuth/google-login",
          "host": ["{{BASE_URL}}"],
          "path": ["api", "InstructorAuth", "google-login"]
        }
      }
    },
    {
      "name": "Instructor Logout",
      "request": {
        "method": "POST",
        "header": [
          {
            "key": "Content-Type",
            "value": "application/json"
          }
        ],
        "body": {
          "mode": "raw",
          "raw": "\"{{USER_ID}}\""
        },
        "url": {
          "raw": "{{BASE_URL}}/api/InstructorAuth/logout",
          "host": ["{{BASE_URL}}"],
          "path": ["api", "InstructorAuth", "logout"]
        }
      }
    }
  ],
  "variable": [
    {
      "key": "BASE_URL",
      "value": "http://localhost:5000"
    },
    {
      "key": "GOOGLE_ID_TOKEN",
      "value": ""
    },
    {
      "key": "USER_ID",
      "value": ""
    }
  ]
}
```

## cURL Examples

### Student Login
```bash
curl -X POST "http://localhost:5000/api/StudentAuth/google-login" \
  -H "Content-Type: application/json" \
  -d '{
    "idToken": "YOUR_GOOGLE_ID_TOKEN",
    "deviceId": "test-device-123",
    "phoneNumber": "+201234567890",
    "dateOfBirth": "2005-03-15",
    "gender": "Male",
    "educationYear": "Grade 10",
    "locationMaps": null
  }'
```

### Instructor Login
```bash
curl -X POST "http://localhost:5000/api/InstructorAuth/google-login" \
  -H "Content-Type: application/json" \
  -d '{
    "idToken": "YOUR_GOOGLE_ID_TOKEN",
    "phoneNumber": "+201234567890",
    "dateOfBirth": "1985-07-20",
    "gender": "Female",
    "educationYear": "Ph.D. in Computer Science",
    "locationMaps": null
  }'
```

### Logout
```bash
curl -X POST "http://localhost:5000/api/StudentAuth/logout" \
  -H "Content-Type: application/json" \
  -d '"a7f2c8e4-1234-5678-9abc-def012345678"'
```

## Tips & Best Practices

1. **ID Token Expiration**: Google ID tokens expire after 1 hour. Always get a fresh token for each login attempt.

2. **Device Fingerprinting**: Use a reliable library like `fingerprintjs2` for production device identification.

3. **Error Handling**: Always handle network errors and validation errors gracefully in your UI.

4. **Secure Storage**: Store user data securely:
   - Web: Use `httpOnly` cookies or secure localStorage
   - Mobile: Use secure storage (Keychain on iOS, Keystore on Android)

5. **User Experience**: Show loading states during authentication and provide clear error messages.

6. **Testing**: Use Google OAuth Playground to get test ID tokens during development.
