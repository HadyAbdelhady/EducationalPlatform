# Google Authentication Consolidation Summary

## Overview
Google login and signup functionality has been consolidated to eliminate duplicate code for both Student and Instructor roles.

## Changes Made

### 1. Student Role Changes

#### Handler Consolidation
- **`StudentGoogleLoginCommandHandler`** now handles both signup and login
  - Creates new user if doesn't exist (signup flow)
  - Updates existing user if exists (login flow)
  - **NEW**: Added deviceID validation for existing students

#### DeviceID Validation
When an existing student attempts to login:
- System checks if the provided `deviceID` matches the stored `deviceID`
- If different: throws `UnauthorizedAccessException` with message:
  ```
  "Login attempt detected from a different device. Please use your registered device to access your account."
  ```
- If `deviceID` was null/empty (backward compatibility): updates it with the new one

#### Controller Updates
- `StudentAuthController.GoogleSignUp()` now internally uses `StudentGoogleLoginCommand`
- Both endpoints (`google-signup` and `google-login`) use the same consolidated handler
- Added `Ssn` field to `StudentGoogleLoginRequest` DTO for new user creation

### 2. Instructor Role Changes

#### Handler Consolidation
- **`InstructorGoogleLoginCommandHandler`** now handles both signup and login
  - Creates new user if doesn't exist (signup flow)
  - Updates existing user if exists (login flow)
  - Uses `Ssn` from request when creating new users

#### Controller Updates
- `InstructorAuthController.GoogleSignUp()` now internally uses `InstructorGoogleLoginCommand`
- Both endpoints (`google-signup` and `google-login`) use the same consolidated handler
- Added `Ssn` field to `InstructorGoogleLoginCommand` and `InstructorGoogleLoginRequest` DTO

### 3. Deprecated Components
The following components are marked as `[Obsolete]` but kept for backward compatibility:
- `StudentGoogleSignUpCommand`
- `StudentGoogleSignUpCommandHandler`
- `InstructorGoogleSignUpCommand`
- `InstructorGoogleSignUpCommandHandler`

These classes should not be used in new code.

## API Behavior

### Student Authentication
Both `/api/StudentAuth/google-signup` and `/api/StudentAuth/google-login` now:
1. Validate Google token
2. Check if user exists by email
3. **If new user**: Create account with provided details
4. **If existing user**: 
   - Validate deviceID matches stored value
   - Throw error if deviceID mismatch
   - Update user information
5. Generate JWT tokens
6. Return `AuthenticationResponse` with `IsNewUser` flag

### Instructor Authentication
Both `/api/InstructorAuth/google-signup` and `/api/InstructorAuth/google-login` now:
1. Validate Google token
2. Check if user exists by email
3. **If new user**: Create account with provided details
4. **If existing user**: Update user information
5. Generate JWT tokens
6. Return `AuthenticationResponse` with `IsNewUser` flag

## Benefits

1. **Eliminated Duplication**: Removed duplicate code between signup and login handlers
2. **Consistent Behavior**: Both endpoints follow the same logic flow
3. **Security Enhancement**: Added deviceID validation for students to prevent unauthorized device access
4. **Backward Compatibility**: Existing API endpoints remain functional
5. **Simplified Maintenance**: Single codebase to maintain for authentication logic

## Breaking Changes

### None for API consumers
- All existing endpoints continue to work
- Request/Response formats remain the same
- Added optional `Ssn` field to login requests (backward compatible)

### For Developers
- Direct usage of `*GoogleSignUpCommand` classes will trigger obsolete warnings
- Recommended to use `*GoogleLoginCommand` for all authentication scenarios

## Security Notes

### Student DeviceID Validation
- Prevents students from logging in from unauthorized devices
- First-time login establishes the device binding
- Subsequent logins must use the same device
- Empty/null deviceIDs are updated automatically (backward compatibility)

### Instructor Access
- No deviceID restriction (instructors can login from any device)
- Standard Google token validation applies

## Migration Guide

If you have direct references to SignUp commands:

**Before:**
```csharp
var command = new StudentGoogleSignUpCommand { ... };
```

**After:**
```csharp
var command = new StudentGoogleLoginCommand { ... };
```

The handler will automatically detect if it's a new user and create the account accordingly.

## Date
Changes implemented: 2025-10-12
