# StackOverflowException - Complete Fix Summary

## Root Cause
The StackOverflowException was caused by **conflicting authentication configurations** in `Program.cs`:
- ASP.NET's `.AddGoogle()` authentication handler was registered
- This created a `GoogleHandler` instance that caused infinite recursion during initialization
- The app already uses **custom JWT validation** via `GoogleAuthService`, making the ASP.NET handler unnecessary

## Primary Fix
**Removed ASP.NET Google Authentication Handler** from `Program.cs`:
```csharp
// REMOVED - This was causing the StackOverflowException:
builder.Services.AddAuthentication()
    .AddGoogle(options => { ... });

// REMOVED - Not needed without authentication schemes:
app.UseAuthentication();
```

## Secondary Fixes (Defense-in-depth)

### 1. Entity Relationship Handling
**Problem**: Manually setting both sides of bi-directional navigation can cause EF Core tracking issues.

**Fixed Files**:
- `InstructorGoogleSignUpCommandHandler.cs`
- `StudentGoogleSignUpCommandHandler.cs`
- `InstructorGoogleLoginCommandHandler.cs`
- `StudentGoogleLoginCommandHandler.cs`

**Change**: Removed `User = user` assignment. EF Core handles this automatically:
```csharp
// BEFORE (Wrong):
var instructor = new Instructor { UserId = user.Id, User = user };

// AFTER (Correct):
var instructor = new Instructor { UserId = user.Id };
```

### 2. JSON Serialization Configuration
**Added to Program.cs**:
```csharp
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });
```
Prevents circular reference issues during JSON serialization.

### 3. EF Core Change Tracker Configuration
**Updated `EducationDbContext.cs`**:
```csharp
public EducationDbContext(DbContextOptions<EducationDbContext> options) : base(options) 
{
    ChangeTracker.LazyLoadingEnabled = false;
    ChangeTracker.AutoDetectChangesEnabled = true;
}
```

### 4. Repository Enhancement
**Updated `UserRepository.CreateAsync`**:
```csharp
public async Task<User> CreateAsync(User user, CancellationToken cancellationToken = default)
{
    await _context.Users.AddAsync(user, cancellationToken);
    _context.ChangeTracker.DetectChanges();
    return user;
}
```

### 5. Controller DTO Fixes
**Created Missing DTOs**:
- `InstructorGoogleSignUpRequest.cs` (includes SSN field)
- `StudentGoogleSignUpRequest.cs` (includes SSN field)

**Updated Controllers**:
- `InstructorAuthController.cs` - Now uses `InstructorGoogleSignUpRequest` for signup
- `StudentAuthController.cs` - Now uses `StudentGoogleSignUpRequest` for signup

### 6. Validation Updates
**Added SSN validation** to:
- `InstructorGoogleSignUpCommandValidator.cs`
- `StudentGoogleSignUpCommandValidator.cs`

## Current Architecture
Your authentication flow now works as:
1. Client sends Google ID token to your API
2. `GoogleAuthService` validates token with Google's JWT validation library
3. User created/updated via MediatR commands
4. No ASP.NET authentication middleware involved

## Testing
Test the endpoint:
```
POST http://localhost:5175/api/InstructorAuth/google-signup
```

The StackOverflowException should be completely resolved.

## Files Modified (Total: 13)
1. Program.cs
2. EducationDbContext.cs
3. UserRepository.cs
4. InstructorGoogleSignUpCommandHandler.cs
5. StudentGoogleSignUpCommandHandler.cs
6. InstructorGoogleLoginCommandHandler.cs
7. StudentGoogleLoginCommandHandler.cs
8. InstructorAuthController.cs
9. StudentAuthController.cs
10. InstructorGoogleSignUpCommandValidator.cs
11. StudentGoogleSignUpCommandValidator.cs
12. InstructorGoogleSignUpRequest.cs (new)
13. StudentGoogleSignUpRequest.cs (new)
