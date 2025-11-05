# Permissions and Roles - Orbit ERP System

This document describes the complete permission and role structure for the Orbit ERP system.

## Overview

The Orbit ERP system uses a **role-based access control (RBAC)** system combined with **location-based access control**. Users are assigned roles that grant them specific permissions, and their access to data is further scoped by their location assignments.

---

## Permissions (24 Total)

Permissions follow the format: `resource:action`

### User Permissions (4)
- `users:create` - Create new users
- `users:read` - Read user information
- `users:update` - Update user information
- `users:delete` - Delete users

### Role Permissions (6)
- `roles:create` - Create new roles
- `roles:read` - Read role information
- `roles:update` - Update role information
- `roles:delete` - Delete roles
- `roles:assign` - Assign roles to users
- `roles:remove` - Remove roles from users

### Permission Permissions (1)
- `permissions:read` - Read permission information

### Session Permissions (2)
- `sessions:read` - Read session information
- `sessions:revoke` - Revoke sessions

### Location Permissions (4)
- `locations:create` - Create new locations
- `locations:read` - Read location information
- `locations:update` - Update location information
- `locations:delete` - Delete locations

### Inventory Permissions (4)
- `inventory:create` - Create inventory records
- `inventory:read` - Read inventory information
- `inventory:update` - Update inventory information
- `inventory:delete` - Delete inventory records

### Customer Permissions (4)
- `customers:create` - Create new customers
- `customers:read` - Read customer information
- `customers:update` - Update customer information
- `customers:delete` - Delete customers

### Product Permissions (4)
- `products:create` - Create new products
- `products:read` - Read product information
- `products:update` - Update product information
- `products:delete` - Delete products

---

## Roles (5 Total)

### 1. HQ Admin
**Description:** Headquarters administrator with full system access across all locations

**Access Scope:** Global (sees and manages everything)

**Permissions:** ALL 24 permissions

**Use Cases:**
- System administrators
- Headquarters management
- IT staff
- Full system configuration and management

**Default User:**
- Email: `admin@example.com`
- Password: `Admin123!`

---

### 2. Store Owner
**Description:** Owner of one or more store locations with management capabilities

**Access Scope:** Owned (sees and manages owned locations)

**Permissions (13):**
- `locations:read`
- `locations:update`
- `users:read`
- `users:create`
- `users:update`
- `inventory:read`
- `inventory:create`
- `inventory:update`
- `products:read`
- `customers:read`
- `customers:create`
- `customers:update`
- `sessions:read`

**Use Cases:**
- Franchise owners
- Multi-store owners
- Regional managers who own locations

**Capabilities:**
- View and update their owned locations
- Hire and manage staff for their stores
- Manage inventory for their stores
- View centralized product catalog
- Manage customers
- Cannot delete locations, users, or inventory
- Cannot create or modify products (centralized)

---

### 3. Store Manager
**Description:** Manager of a single store location with operational capabilities

**Access Scope:** Managed (sees and manages their assigned location)

**Permissions (11):**
- `locations:read`
- `users:read`
- `inventory:read`
- `inventory:create`
- `inventory:update`
- `products:read`
- `customers:read`
- `customers:create`
- `customers:update`
- `sessions:read`

**Use Cases:**
- Store managers
- Location supervisors
- Department managers

**Capabilities:**
- View location information
- View staff information (cannot create/modify users)
- Manage inventory for their store
- View product catalog
- Manage customers
- Cannot modify location settings
- Cannot manage users
- Cannot delete inventory

---

### 4. Employee
**Description:** Store employee with basic operational access to assigned locations

**Access Scope:** Assigned/Context (sees data for assigned locations, works in current context)

**Permissions (8):**
- `locations:read`
- `users:read`
- `inventory:read`
- `inventory:update`
- `products:read`
- `customers:read`
- `customers:create`
- `sessions:read`

**Use Cases:**
- Sales associates
- Cashiers
- Stock clerks
- Part-time staff

**Capabilities:**
- View location information
- View basic user information
- View and update inventory (stock counts, etc.)
- View product catalog
- View and create customers
- Can switch between assigned locations
- Cannot create inventory records
- Cannot modify users or locations

---

### 5. Read-Only User
**Description:** User with read-only access to basic information

**Access Scope:** Varies (depends on location assignments)

**Permissions (11):** All read permissions
- `users:read`
- `roles:read`
- `permissions:read`
- `sessions:read`
- `locations:read`
- `inventory:read`
- `customers:read`
- `products:read`

**Use Cases:**
- Reporting users
- Analytics staff
- Auditors
- External consultants

**Capabilities:**
- View all information within their location scope
- Cannot create, update, or delete anything
- Useful for reporting and analytics

---

## Location-Based Access Control

In addition to role-based permissions, users have location-based access control through the `PermissionScope` enum:

### Permission Scopes

1. **Global** - Access to all locations
   - Typically for HQ Admin role
   - Sees all data across all locations

2. **Owned** - Access to owned locations
   - Typically for Store Owner role
   - Sees data for locations they own (via `Location.OwnerId`)

3. **Managed** - Access to managed location
   - Typically for Store Manager role
   - Sees data for the location they manage (via `Location.ManagerId`)

4. **Assigned** - Access to assigned locations
   - Typically for Employee role
   - Sees data for locations they're assigned to (via `UserLocationAssignment`)

5. **Context** - Access to current location context only
   - Typically for Employee role
   - Sees data only for their current active location (via `User.CurrentLocationContextId`)

---

## How It Works Together

### Example 1: HQ Admin
- **Role:** HQ Admin
- **Permissions:** All 24 permissions
- **Scope:** Global
- **Result:** Can see and manage everything across all locations

### Example 2: Store Owner
- **Role:** Store Owner
- **Permissions:** 13 permissions (no delete, no product creation)
- **Scope:** Owned
- **Owned Locations:** Store A, Store B, Store C
- **Result:** Can manage users, inventory, and customers for Stores A, B, and C. Can view but not modify products.

### Example 3: Store Manager
- **Role:** Store Manager
- **Permissions:** 11 permissions (read-heavy, limited updates)
- **Scope:** Managed
- **Managed Location:** Store A
- **Result:** Can manage inventory and customers for Store A. Can view users but not modify them.

### Example 4: Employee
- **Role:** Employee
- **Permissions:** 8 permissions (basic operational)
- **Scope:** Assigned or Context
- **Assigned Locations:** Store A, Store B
- **Current Context:** Store A
- **Result:** Can view and update inventory, create customers for Store A. Can switch context to Store B.

---

## Database Seeding

The system automatically seeds the following on first run:

1. **24 Permissions** - All permissions listed above
2. **5 Roles** - All roles with their respective permissions
3. **1 Default Admin User:**
   - Email: `admin@example.com`
   - Password: `Admin123!`
   - Role: HQ Admin
   - Email verified: Yes

**Note:** Seeding only occurs if the database is empty. Existing data is never overwritten.

---

## GraphQL Authorization

All GraphQL mutations and queries are protected with the `[Authorize(Policy = "permission:action")]` attribute.

Example:
```csharp
[Authorize(Policy = "locations:update")]
public async Task<AssignLocationOwnerPayload> AssignLocationOwnerAsync(...)
```

This ensures that only users with the required permission can execute the operation.

---

## Future Enhancements (Phase 2+)

- **Custom Roles:** Allow HQ Admin to create custom roles with specific permission combinations
- **Location-Specific Roles:** Allow different roles at different locations (e.g., Manager at Store A, Employee at Store B)
- **Permission Scopes on Permissions:** Add scope metadata to permissions themselves
- **Audit Logging:** Track all permission checks and access attempts
- **Time-Based Permissions:** Temporary permission grants
- **IP-Based Restrictions:** Restrict certain permissions to specific IP ranges

---

## Summary

The Orbit ERP permission system provides:
- ✅ **24 granular permissions** covering all resources
- ✅ **5 predefined roles** matching business hierarchy
- ✅ **Location-based access control** for multi-location operations
- ✅ **Flexible scope system** (Global, Owned, Managed, Assigned, Context)
- ✅ **Automatic database seeding** for quick setup
- ✅ **GraphQL-level authorization** for API security

This system ensures that users only see and can modify data appropriate to their role and location assignments.

