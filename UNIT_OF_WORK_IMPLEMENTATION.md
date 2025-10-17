# Unit of Work Pattern Implementation

## Overview
Successfully implemented the Unit of Work pattern to manage transactions across multiple repositories, ensuring data consistency and atomic operations.

## What Was Changed

### 1. **Created IUnitOfWork Interface** (`Application/Interfaces/IUnitOfWork.cs`)
```csharp
public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IRefreshTokenRepository RefreshTokens { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
```

### 2. **Implemented UnitOfWork Class** (`Infrastructure/Repositories/UnitOfWork.cs`)
- Manages DbContext lifecycle
- Provides lazy-loaded repository instances
- Handles transaction management
- Single point for SaveChanges

### 3. **Updated RefreshTokenRepository**
**Before:**
```csharp
public async Task AddRefreshTokenAsync(string refreshToken, Guid UserId, CancellationToken cancellationToken)
{
    var tokenEntity = new RefreshToken { /* ... */ };
    await AddAsync(tokenEntity, cancellationToken);
    await SaveChangesAsync(cancellationToken);  // ❌ Saved immediately
}
```

**After:**
```csharp
public async Task AddRefreshTokenAsync(string refreshToken, Guid UserId, CancellationToken cancellationToken)
{
    var tokenEntity = new RefreshToken { /* ... */ };
    await AddAsync(tokenEntity, cancellationToken);
    // ✅ No SaveChanges - deferred to UnitOfWork
}
```

### 4. **Updated JwtTokenService**
**Before:**
```csharp
public async Task<string> GenerateRefreshToken(Guid UserId, CancellationToken cancellationToken)
{
    var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
    await _refreshTokenRepository.AddRefreshTokenAsync(token, UserId, cancellationToken);  // ❌ Saved internally
    return token;
}
```

**After:**
```csharp
public string GenerateRefreshToken()
{
    return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
    // ✅ Just generates token - no repository interaction
}
```

### 5. **Updated Command Handlers**

#### StudentGoogleLoginCommandHandler
**Before:**
```csharp
await _userRepository.AddAsync(user, cancellationToken);
await _userRepository.SaveChangesAsync(cancellationToken);  // First transaction
var refreshToken = await _jwtTokenService.GenerateRefreshToken(user.Id, cancellationToken);  // Second transaction ❌
```

**After:**
```csharp
await _unitOfWork.Users.AddAsync(user, cancellationToken);

var refreshToken = _jwtTokenService.GenerateRefreshToken();
await _unitOfWork.RefreshTokens.AddRefreshTokenAsync(refreshToken, user.Id, cancellationToken);

await _unitOfWork.SaveChangesAsync(cancellationToken);  // ✅ Single transaction for all operations
```

#### InstructorGoogleLoginCommandHandler
- Same pattern as StudentGoogleLoginCommandHandler

#### LoginWithRefreshTokenHandler
**Before:**
```csharp
await _refreshTokenRepository.DeleteRefreshTokenAsync(refreshToken, cancellationToken);  // Saved immediately ❌
var newToken = await _jwtTokenService.GenerateRefreshToken(userId, cancellationToken);  // Another save ❌
```

**After:**
```csharp
await _unitOfWork.RefreshTokens.DeleteRefreshTokenAsync(refreshToken, cancellationToken);
var newToken = _jwtTokenService.GenerateRefreshToken();
await _unitOfWork.RefreshTokens.AddRefreshTokenAsync(newToken, userId, cancellationToken);

await _unitOfWork.SaveChangesAsync(cancellationToken);  // ✅ Single transaction
```

### 6. **Updated Dependency Injection** (`Edu_Base/Program.cs`)
```csharp
// Added Unit of Work registration
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Individual repositories still available (accessed through UnitOfWork)
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
```

## Benefits Achieved

### 1. **Transactional Integrity**
- All repository operations in a business transaction commit together
- If any operation fails, everything rolls back
- No partial saves or inconsistent state

### 2. **Better Performance**
- Reduced number of database round-trips
- Single SaveChanges call per business operation
- Batch operations within single transaction

### 3. **Cleaner Code**
- Clear separation of concerns
- Services don't save data - handlers do
- Repositories focus on data access only

### 4. **Easier Testing**
- Mock IUnitOfWork instead of multiple repositories
- Better control over transaction behavior in tests
- Simplified test setup

## Before vs After Comparison

### Transaction Flow Before:
```
Handler
  ├─> UserRepository.AddAsync()
  ├─> UserRepository.SaveChangesAsync()  ❌ Transaction 1
  ├─> JwtTokenService.GenerateRefreshToken()
  │     └─> RefreshTokenRepository.SaveChangesAsync()  ❌ Transaction 2
  └─> Return
  
Problem: If Transaction 2 fails, Transaction 1 is already committed!
```

### Transaction Flow After:
```
Handler
  ├─> UnitOfWork.Users.AddAsync()
  ├─> UnitOfWork.RefreshTokens.AddRefreshTokenAsync()
  ├─> UnitOfWork.SaveChangesAsync()  ✅ Single Transaction
  └─> Return
  
Success: All operations commit together or fail together!
```

## Usage Example

```csharp
public class SomeCommandHandler
{
    private readonly IUnitOfWork _unitOfWork;
    
    public async Task Handle(SomeCommand request, CancellationToken cancellationToken)
    {
        // Work with multiple repositories
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
        user.UpdatedAt = DateTimeOffset.UtcNow;
        _unitOfWork.Users.Update(user);
        
        // Add related data
        await _unitOfWork.RefreshTokens.AddRefreshTokenAsync("token", user.Id, cancellationToken);
        
        // Save all changes in ONE transaction
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        // If an exception occurs before SaveChangesAsync, nothing is committed ✅
    }
}
```

## Advanced Transaction Control

For complex scenarios requiring explicit transaction management:

```csharp
await _unitOfWork.BeginTransactionAsync(cancellationToken);
try
{
    // Multiple operations
    await _unitOfWork.Users.AddAsync(user, cancellationToken);
    await _unitOfWork.RefreshTokens.AddRefreshTokenAsync(token, userId, cancellationToken);
    
    // Commit transaction
    await _unitOfWork.CommitTransactionAsync(cancellationToken);
}
catch
{
    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
    throw;
}
```

## Files Modified

### New Files:
1. `Application/Interfaces/IUnitOfWork.cs`
2. `Infrastructure/Repositories/UnitOfWork.cs`

### Modified Files:
1. `Application/Interfaces/IJwtTokenService.cs`
2. `Application/Interfaces/IRefreshTokenRepository.cs`
3. `Infrastructure/Services/JwtTokenService.cs`
4. `Infrastructure/Repositories/RefreshTokenRepository.cs`
5. `Application/Features/Auth/Commands/StudentGoogleLogin/StudentGoogleLoginCommandHandler.cs`
6. `Application/Features/Auth/Commands/InstructorGoogleLogin/InstructorGoogleLoginCommandHandler.cs`
7. `Application/Features/Auth/Commands/UserLoginWithRefreshToken/LoginWithRefreshToken.cs`
8. `Edu_Base/Program.cs`

## Next Steps (Optional Enhancements)

1. **Add More Repositories to UnitOfWork** as needed:
   ```csharp
   ICourseRepository Courses { get; }
   ISectionRepository Sections { get; }
   ```

2. **Implement Audit Logging** in SaveChangesAsync:
   ```csharp
   public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
   {
       // Auto-set CreatedAt/UpdatedAt
       // Log changes
       return await base.SaveChangesAsync(cancellationToken);
   }
   ```

3. **Add Unit Tests** for transactional behavior

## Summary

✅ **Transactional integrity** - All operations succeed or fail together  
✅ **No data inconsistencies** - Eliminated split transactions  
✅ **Cleaner architecture** - Separation of concerns  
✅ **Better performance** - Single database round-trip  
✅ **Easier maintenance** - Centralized transaction management  

The Unit of Work pattern is now fully integrated into your Educational Platform!
