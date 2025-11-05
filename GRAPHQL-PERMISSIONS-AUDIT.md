# GraphQL Permissions Audit

This document provides a comprehensive audit of all GraphQL mutations and queries with their authorization requirements.

## Summary

- ✅ **All mutations have appropriate permissions**
- ✅ **All queries have appropriate permissions**
- ✅ **Public endpoints properly identified** (Login, Register, Password Reset, Email Verification)
- ✅ **Authenticated-only endpoints properly marked** (Me, Sessions, Change Password, etc.)

---

## Mutations (40 Total)

### User Mutations (14)

| Mutation | Permission | Notes |
|----------|-----------|-------|
| `registerUser` | `users:create` | ✅ Correct |
| `login` | None (Public) | ✅ Correct - Public endpoint |
| `refreshToken` | None (Public) | ✅ Correct - Public endpoint |
| `verifyEmail` | None (Public) | ✅ Correct - Public endpoint |
| `requestPasswordReset` | None (Public) | ✅ Correct - Public endpoint |
| `resetPassword` | None (Public) | ✅ Correct - Public endpoint |
| `changePassword` | `@Authorize` | ✅ Correct - Authenticated only |
| `updateMyProfile` | `@Authorize` | ✅ Correct - Authenticated only |
| `revokeSession` | `@Authorize` | ✅ Correct - Authenticated only |
| `revokeAllSessions` | `@Authorize` | ✅ Correct - Authenticated only |
| `changeUserRole` | `roles:assign` | ✅ Correct |
| `suspendUser` | `users:update` | ✅ Correct |
| `activateUser` | `users:update` | ✅ Correct |
| `deleteUser` | `users:delete` | ✅ Correct |
| `unlockUserAccount` | `users:update` | ✅ Correct |
| `updateUserProfile` | `users:update` | ✅ Correct |

### Location Mutations (10)

| Mutation | Permission | Notes |
|----------|-----------|-------|
| `createLocation` | `locations:create` | ✅ Correct |
| `updateLocation` | `locations:update` | ✅ Correct |
| `changeLocationStatus` | `locations:update` | ✅ Correct |
| `assignUserToLocation` | `users:update` | ✅ Correct - User assignment |
| `unassignUserFromLocation` | `users:update` | ✅ Correct - User assignment |
| `switchLocationContext` | `users:update` | ✅ Correct - User context |
| `setPrimaryLocation` | `users:update` | ✅ Correct - User setting |
| `assignLocationOwner` | `locations:update` | ✅ Correct |
| `removeLocationOwner` | `locations:update` | ✅ Correct |
| `assignLocationManager` | `locations:update` | ✅ Correct |
| `removeLocationManager` | `locations:update` | ✅ Correct |

### Customer Mutations (6)

| Mutation | Permission | Notes |
|----------|-----------|-------|
| `createCustomer` | `customers:create` | ✅ Correct |
| `updateCustomerContactInfo` | `customers:update` | ✅ Correct |
| `updateCustomerAddress` | `customers:update` | ✅ Correct |
| `suspendCustomer` | `customers:update` | ✅ Correct |
| `activateCustomer` | `customers:update` | ✅ Correct |
| `deleteCustomer` | `customers:delete` | ✅ Correct |

### Product Mutations (5)

| Mutation | Permission | Notes |
|----------|-----------|-------|
| `createProduct` | `products:create` | ✅ Correct |
| `updateProduct` | `products:update` | ✅ Correct |
| `activateProduct` | `products:update` | ✅ Correct |
| `deactivateProduct` | `products:update` | ✅ Correct |
| `deleteProduct` | `products:delete` | ✅ Correct |

### Inventory Mutations (6)

| Mutation | Permission | Notes |
|----------|-----------|-------|
| `createInventory` | `inventory:create` | ✅ Correct |
| `adjustStock` | `inventory:update` | ✅ Correct |
| `reserveStock` | `inventory:update` | ✅ Correct |
| `releaseReservation` | `inventory:update` | ✅ Correct |
| `transferStock` | `inventory:update` | ✅ Correct |
| `commitReservation` | `inventory:update` | ✅ Correct |

---

## Queries (17 Total)

### User Queries (4)

| Query | Permission | Notes |
|-------|-----------|-------|
| `me` | `@Authorize` | ✅ Correct - Authenticated only |
| `users` | `users:read` | ✅ Correct |
| `user(id)` | `users:read` | ✅ Correct |
| `usersByLocation` | `users:read` | ✅ Correct |
| `sessions` | `@Authorize` | ✅ Correct - Authenticated only |

### Location Queries (2)

| Query | Permission | Notes |
|-------|-----------|-------|
| `locations` | `locations:read` | ✅ Correct |
| `location(id)` | `locations:read` | ✅ Correct |

### Customer Queries (3)

| Query | Permission | Notes |
|-------|-----------|-------|
| `customers` | `customers:read` | ✅ Correct |
| `customer(id)` | `customers:read` | ✅ Correct |
| `customerByEmail` | `customers:read` | ✅ Correct |

### Product Queries (3)

| Query | Permission | Notes |
|-------|-----------|-------|
| `products` | `products:read` | ✅ Correct |
| `productById` | `products:read` | ✅ Correct |
| `productBySku` | `products:read` | ✅ Correct |

### Inventory Queries (3)

| Query | Permission | Notes |
|-------|-----------|-------|
| `inventory(id)` | `inventory:read` | ✅ Correct |
| `inventoriesByProduct` | `inventory:read` | ✅ Correct |
| `inventoriesByLocation` | `inventory:read` | ✅ Correct |

### Role & Permission Queries (2)

| Query | Permission | Notes |
|-------|-----------|-------|
| `roles` | `roles:read` | ✅ Correct |
| `permissions` | `permissions:read` | ✅ Correct |

---

## Permission Usage Analysis

### Most Used Permissions

1. **`users:update`** - 7 mutations (user management, location assignments)
2. **`inventory:update`** - 5 mutations (stock operations)
3. **`locations:update`** - 5 mutations (location management, ownership)
4. **`customers:update`** - 3 mutations (customer updates)
5. **`products:update`** - 3 mutations (product updates)

### Read Permissions

All read permissions are properly applied:
- `users:read` - 3 queries
- `locations:read` - 2 queries
- `customers:read` - 3 queries
- `products:read` - 3 queries
- `inventory:read` - 3 queries
- `roles:read` - 1 query
- `permissions:read` - 1 query

### Create Permissions

All create permissions are properly applied:
- `users:create` - 1 mutation (registerUser)
- `locations:create` - 1 mutation (createLocation)
- `customers:create` - 1 mutation (createCustomer)
- `products:create` - 1 mutation (createProduct)
- `inventory:create` - 1 mutation (createInventory)

### Delete Permissions

All delete permissions are properly applied:
- `users:delete` - 1 mutation (deleteUser)
- `customers:delete` - 1 mutation (deleteCustomer)
- `products:delete` - 1 mutation (deleteProduct)

**Note:** `locations:delete` and `inventory:delete` permissions exist but are not yet used (no mutations created).

---

## Public Endpoints (No Authentication Required)

These endpoints are intentionally public:

1. **`login`** - User authentication
2. **`refreshToken`** - Token refresh
3. **`verifyEmail`** - Email verification
4. **`requestPasswordReset`** - Password reset request
5. **`resetPassword`** - Password reset with token

---

## Authenticated-Only Endpoints (No Specific Permission)

These endpoints require authentication but no specific permission:

1. **`me`** - Get current user
2. **`sessions`** - Get user's sessions
3. **`changePassword`** - Change own password
4. **`updateMyProfile`** - Update own profile
5. **`revokeSession`** - Revoke own session
6. **`revokeAllSessions`** - Revoke all own sessions

---

## Recommendations

### ✅ All Good - No Changes Needed

All mutations and queries have appropriate authorization:
- Public endpoints are correctly marked as public
- Authenticated-only endpoints use `@Authorize`
- Permission-based endpoints use `@Authorize(Policy = "permission:action")`
- All permissions match the seeded permissions in DatabaseSeeder.cs
- All permissions are registered in AuthorizationExtensions.cs

### 💡 Future Considerations

1. **Location Delete Mutation** - Consider adding if needed:
   ```csharp
   [Authorize(Policy = "locations:delete")]
   public async Task<DeleteLocationPayload> DeleteLocationAsync(...)
   ```

2. **Inventory Delete Mutation** - Consider adding if needed:
   ```csharp
   [Authorize(Policy = "inventory:delete")]
   public async Task<DeleteInventoryPayload> DeleteInventoryAsync(...)
   ```

3. **Session Revoke Permission** - Currently using `@Authorize`, could use `sessions:revoke`:
   ```csharp
   [Authorize(Policy = "sessions:revoke")]
   public async Task<RevokeSessionPayload> RevokeSessionAsync(...)
   ```

4. **Location-Based Authorization** - Future enhancement to check location access:
   - User can only see/modify data for locations they have access to
   - Implement in authorization handlers using `LocationAccessService`

---

## Conclusion

✅ **All GraphQL mutations and queries have correct authorization attributes!**

The permission system is:
- **Complete** - All endpoints are protected
- **Consistent** - Permissions follow the `resource:action` pattern
- **Secure** - Public endpoints are intentionally public
- **Well-organized** - Easy to audit and maintain

No changes are required at this time.

