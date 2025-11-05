# Domain-Driven Design Analysis & Implementation Roadmap

**Project:** Orbit ERP System
**Analysis Date:** 2025-11-05
**Last Updated:** 2025-11-05
**Status:** ✅ Phase 1 & 2 Complete - Location-based access control and role-based query filtering fully implemented

---

## Executive Summary

The Orbit ERP system has a **solid technical foundation** with well-designed entities, value objects, and domain events. However, it is **missing critical business entities** required for a multi-location chain business ERP system.

### Business Model: Single-Tenant with Location-Based Access Control

**Key Characteristics:**
- ✅ **Single organization** (no multi-tenancy needed)
- ✅ **Multiple locations** with varying levels of independence
- ✅ **Shared resources** (products, customers) with location-specific variations
- ✅ **Location-scoped data** (inventory, sales) with aggregated visibility
- ✅ **Store ownership hierarchy** (HQ → Store Owner → Store Manager → Employee)
- ✅ **Context-based access** (users switch between locations)

### Key Findings
- ✅ **9 entities** currently implemented with good DDD practices
- ✅ **12 value objects** with proper validation
- ❌ **No Order/Sales entities** - Cannot track revenue or sales attribution
- ❌ **No Supplier/Purchasing entities** - Cannot manage procurement
- ❌ **No User-Location relationships** - Cannot assign staff to stores or track ownership
- ❌ **No ProductLocation entity** - Cannot support location-specific pricing
- ❌ **No InventoryTransfer entity** - Cannot track inter-location transfers

---

## Current Domain Model

### Implemented Entities (10)

| Entity | Bounded Context | Status | Notes |
|--------|----------------|--------|-------|
| User | Users | ✅ Complete | Authentication, authorization, location context |
| Role | Role | ✅ Complete | Permission grouping |
| Permission | Permission | ✅ Complete | Resource:action pattern with scope |
| Session | Session | ✅ Complete | Session tracking |
| PasswordHistory | Users | ✅ Complete | Password reuse prevention |
| Customer | Customers | ✅ Complete | Customer management |
| Product | Products | ✅ Complete | Product catalog |
| Location | Locations | ✅ Complete | Location management with ownership |
| Inventory | Inventory | ✅ Complete | Stock management with reservations |
| UserLocationAssignment | Users | ✅ Complete | User-location relationships |

### Implemented Value Objects (12)

- Email, FullName, Address, PhoneNumber (Shared)
- Money, Currency, ProductName, ProductDescription, Sku (Products)
- LocationName (Locations)
- Password, PasswordHash (Users)

### Current Relationships

```
User → Role (one-to-one)
Role → Permissions (many-to-many)
User → Sessions (one-to-many)
User → PasswordHistory (one-to-many)
User ↔ Location (many-to-many via UserLocationAssignment)
Location → User (many-to-one for Owner)
Location → User (many-to-one for Manager)
Inventory → Product (many-to-one)
Inventory → Location (many-to-one)
```

---

## User Role Hierarchy

### Business-Aligned Roles (Seeded in Database)

The system includes 5 pre-defined roles that align with typical chain business structures:

#### 1. **HQ Admin** (Corporate/Headquarters)
- **Permission Scope:** Global (sees ALL locations)
- **Permissions:** All 24 permissions (full system access)
- **Use Case:** Corporate administrators, IT staff, executives
- **Key Features:**
  - Manage users, roles, and permissions across all locations
  - View and manage all locations
  - Access all products, inventory, customers, and sales data
  - No context switching needed (always sees everything)

#### 2. **Store Owner** (Multi-Location Owner)
- **Permission Scope:** Owned (sees only owned locations)
- **Permissions:** 18 permissions (excludes user/role/permission management)
- **Use Case:** Franchise owners, multi-store operators
- **Key Features:**
  - Own multiple locations
  - View and manage data for owned locations only
  - Can switch context between owned stores
  - Cannot manage users, roles, or permissions (HQ-only)

#### 3. **Store Manager** (Single Location Manager)
- **Permission Scope:** Managed (sees only managed location)
- **Permissions:** 14 permissions (location-specific operations)
- **Use Case:** Store managers, location supervisors
- **Key Features:**
  - Manage one location
  - View and manage data for managed location only
  - Cannot create/delete locations
  - Cannot manage users or roles

#### 4. **Employee** (Store Staff)
- **Permission Scope:** Assigned (sees only assigned locations)
- **Permissions:** 8 permissions (basic operations)
- **Use Case:** Sales staff, cashiers, stock clerks
- **Key Features:**
  - Assigned to one or more locations
  - View and create data in current context
  - Read-only access to products and customers
  - Cannot manage inventory or locations

#### 5. **Read-Only User** (Auditor/Viewer)
- **Permission Scope:** Assigned (sees only assigned locations)
- **Permissions:** 6 permissions (read-only)
- **Use Case:** Auditors, accountants, analysts
- **Key Features:**
  - View-only access to assigned locations
  - Cannot create, update, or delete any data
  - Useful for reporting and analysis

### Role Hierarchy Diagram

```
┌─────────────────────────────────────────┐
│ HQ Admin / Corporate                    │
│ - Sees ALL locations                    │
│ - Global permissions (24)               │
│ - No context switching needed           │
└─────────────────────────────────────────┘
              ↓
┌─────────────────────────────────────────┐
│ Store Owner                             │
│ - Owns multiple locations               │
│ - Sees data from owned locations        │
│ - Can switch context between owned stores│
│ - 18 permissions                        │
└─────────────────────────────────────────┘
              ↓
┌─────────────────────────────────────────┐
│ Store Manager                           │
│ - Manages ONE location                  │
│ - Sees only their location's data       │
│ - No context switching (single location)│
│ - 14 permissions                        │
└─────────────────────────────────────────┘
              ↓
┌─────────────────────────────────────────┐
│ Employee                                │
│ - Assigned to one or more locations     │
│ - Must switch context to active location│
│ - Sees only current context location    │
│ - 8 permissions                         │
└─────────────────────────────────────────┘
              ↓
┌─────────────────────────────────────────┐
│ Read-Only User                          │
│ - Assigned to one or more locations     │
│ - View-only access                      │
│ - 6 permissions                         │
└─────────────────────────────────────────┘
```

## Data Visibility Matrix

| Data Type | HQ Admin | Store Owner | Store Manager | Employee |
|-----------|----------|-------------|---------------|----------|
| **Products (Catalog)** | All | All | All | All |
| **ProductLocation (Pricing)** | All | Owned locations | Managed location | Current context |
| **Customers** | All | All | All | All |
| **Inventory** | All locations | Owned locations | Managed location | Current context |
| **Orders** | All locations | Owned locations | Managed location | Current context |
| **Sales Reports** | All + aggregated | Owned + aggregated | Own + aggregated | Context + aggregated |
| **Transfers** | All | Owned locations | Can request only | Can request only |

## Critical Gaps

### ✅ Priority 1: Location-Based Access Control (COMPLETE)

#### 1. User-Location Relationships
**Status:** ✅ Implemented
**Impact:** Users can be assigned to locations, context switching works, ownership tracking enabled
**Completion Date:** 2025-11-05

**Implemented Components:**
- [x] UserLocationAssignment entity
- [x] Add CurrentLocationContextId to User entity
- [x] Add OwnerId to Location entity (store ownership)
- [x] Add ManagerId to Location entity (store management)
- [x] AssignmentStatus enum
- [x] PermissionScope enum (Global, Owned, Managed, Assigned, Context)
- [x] ILocationAccessService interface
- [x] LocationAccessService implementation
- [x] CurrentUserService for query handlers
- [x] Database migrations applied
- [x] GraphQL mutations for assignments and ownership
- [x] End-to-end testing completed

**Relationships:**
- User ↔ Location (many-to-many via UserLocationAssignment)
- Location → User (many-to-one for Owner)
- Location → User (many-to-one for Manager)

**Key Features:**
- ✅ Users can be assigned to multiple locations
- ✅ Users can switch context between assigned locations
- ✅ Store owners can own multiple locations
- ✅ Store managers manage one location
- ✅ Location-scoped data access based on role and assignments
- ✅ Permission scopes (Global, Owned, Managed, Assigned, Context)

---

### 🚨 Priority 2: Product Location Pricing (CRITICAL)

#### 2. ProductLocation Entity
**Status:** ❌ Not Implemented
**Impact:** Cannot support location-specific pricing or store-specific products
**Blocks:** Multi-location product management

**Required Components:**
- [ ] ProductLocation entity
- [ ] Update Product entity (rename Price to BasePrice)
- [ ] Add IsGloballyAvailable to Product
- [ ] LocationSku value object (optional)
- [ ] IProductLocationRepository interface

**Relationships:**
- ProductLocation → Product (many-to-one)
- ProductLocation → Location (many-to-one)
- Unique constraint on (ProductId, LocationId)

**Key Features:**
- Products have a base price (global default)
- Locations can override price for specific products
- Products can be available/unavailable at specific locations
- Support for store-specific products (only available at one location)
- Optional location-specific SKUs

---

### 🚨 Priority 3: Order/Sales Aggregate (CRITICAL)

#### 3. Order/Sales Aggregate
**Status:** ❌ Not Implemented
**Impact:** Cannot track sales, revenue, or sales attribution
**Blocks:** Core business functionality

**Required Components:**
- [ ] Order entity (aggregate root)
- [ ] OrderLine entity
- [ ] OrderNumber value object
- [ ] OrderStatus enum (Pending, Confirmed, Fulfilled, Cancelled, etc.)
- [ ] IOrderRepository interface
- [ ] Sales attribution reporting

**Relationships:**
- Order → Customer (many-to-one) - shared customer
- Order → Location (many-to-one) - where order was placed (SALES ATTRIBUTION)
- Order → User (many-to-one) - salesperson
- Order → OrderLines (one-to-many)
- OrderLine → Product (many-to-one)

**Key Features:**
- Sales attribution by location
- Location-scoped order visibility
- Aggregated sales totals (company-wide)
- Integration with Inventory (reserve stock)

#### 4. Supplier Aggregate
**Status:** ❌ Not Implemented
**Impact:** Cannot manage vendors or procurement

**Required Components:**
- [ ] Supplier entity (aggregate root)
- [ ] SupplierName value object
- [ ] SupplierStatus enum
- [ ] PaymentTerms value object
- [ ] ISupplierRepository interface

#### 5. PurchaseOrder Aggregate
**Status:** ❌ Not Implemented
**Impact:** Cannot track purchases from suppliers

**Required Components:**
- [ ] PurchaseOrder entity (aggregate root)
- [ ] PurchaseOrderLine entity
- [ ] PurchaseOrderNumber value object
- [ ] PurchaseOrderStatus enum
- [ ] IPurchaseOrderRepository interface

**Relationships:**
- PurchaseOrder → Supplier (many-to-one)
- PurchaseOrder → Location (many-to-one) - receiving location
- PurchaseOrder → User (many-to-one) - created by
- PurchaseOrder → PurchaseOrderLines (one-to-many)
- PurchaseOrderLine → Product (many-to-one)

---

### ⚠️ Priority 4: Inventory Collaboration (HIGH)

#### 6. InventoryTransfer Aggregate
**Status:** ❌ Not Implemented
**Impact:** Cannot track inventory movements between locations

**Required Components:**
- [ ] InventoryTransfer entity (aggregate root)
- [ ] TransferLine entity
- [ ] TransferNumber value object
- [ ] TransferStatus enum (Requested, Approved, Shipped, Received, Cancelled)
- [ ] IInventoryTransferRepository interface

**Relationships:**
- InventoryTransfer → Location (many-to-one) - source
- InventoryTransfer → Location (many-to-one) - destination
- InventoryTransfer → User (many-to-one) - requested by
- InventoryTransfer → User (many-to-one) - approved by
- InventoryTransfer → TransferLines (one-to-many)
- TransferLine → Product (many-to-one)

**Key Features:**
- "Blind request" workflow (stores can't see other stores' inventory)
- HQ approval workflow for transfers
- Integration with Inventory aggregate
- Transfer tracking and audit trail

---

### 📋 Priority 5: Supporting Features (MEDIUM)

#### 7. Payment Entity
**Status:** ❌ Not Implemented  
- [ ] Payment entity
- [ ] PaymentMethod value object
- [ ] PaymentStatus enum

#### 8. Invoice Entity
**Status:** ❌ Not Implemented  
- [ ] Invoice entity
- [ ] InvoiceNumber value object
- [ ] InvoiceStatus enum

#### 9. Shipment Entity
**Status:** ❌ Not Implemented  
- [ ] Shipment entity
- [ ] TrackingNumber value object
- [ ] ShipmentStatus enum

#### 10. Return Entity
**Status:** ❌ Not Implemented  
- [ ] Return entity
- [ ] ReturnReason value object
- [ ] ReturnStatus enum

---

## Implementation Roadmap

### Phase 1: Location-Based Access Control (Weeks 1-2) ✅ COMPLETE

**Goal:** Enable multi-location access control, context switching, and store ownership

- [x] **Week 1: User-Location Relationships**
  - [x] Create Domain/Users/UserLocationAssignment.cs entity
  - [x] Create AssignmentStatus enum
  - [x] Add CurrentLocationContextId to User entity
  - [x] Add SwitchLocationContext method to User
  - [x] Add CanAccessLocation method to User
  - [x] Create UserLocationAssignmentErrors
  - [x] Create domain events (UserAssignedToLocation, ContextSwitched, etc.)
  - [x] Create EF Core configuration for UserLocationAssignment
  - [x] Create database migration
  - [x] Create GraphQL mutations (AssignUserToLocation, UnassignUserFromLocation, SwitchLocationContext, SetPrimaryLocation)
  - [ ] Write unit tests for User-Location logic (deferred)

- [x] **Week 2: Location Ownership & Permission System**
  - [x] Add OwnerId to Location entity (nullable Guid)
  - [x] Add ManagerId to Location entity (nullable Guid)
  - [x] Add AssignOwner/AssignManager methods to Location
  - [x] Create PermissionScope enum (Global, Owned, Managed, Assigned, Context)
  - [x] Add Scope property to Permission entity
  - [x] Create database migration for Permission.Scope
  - [x] Update DatabaseSeeder with permission scopes
  - [x] Create ILocationAccessService interface
  - [x] Implement LocationAccessService in Infrastructure layer
  - [x] Create ICurrentUserService interface
  - [x] Implement CurrentUserService in Infrastructure layer
  - [x] Create GraphQL mutations for ownership (AssignLocationOwner, RemoveLocationOwner, AssignLocationManager, RemoveLocationManager)
  - [x] Test all mutations end-to-end
  - [x] Update all queries to filter by user's accessible locations (Phase 2)
  - [ ] Write unit tests for domain logic (deferred)

**Deliverables:**
- ✅ Users can be assigned to multiple locations
- ✅ Context switching functionality working
- ✅ Store ownership and management tracking
- ✅ Location-based access control domain logic implemented
- ✅ Permission scopes defined and seeded
- ✅ CurrentUserService for query handlers
- ✅ All GraphQL mutations tested
- ✅ Location-based query filtering (Phase 2 - complete for inventory)
- ⏳ Unit tests for domain logic (deferred)

---

### Phase 2: Location-Based Query Filtering (Week 3) ✅ COMPLETE

**Goal:** Implement location-based data filtering for queries with role-based scope resolution

- [x] **Foundation (Completed 2025-11-05)**
  - [x] Add Scope property to Permission entity
  - [x] Create database migration for Permission.Scope
  - [x] Update DatabaseSeeder with permission scopes
  - [x] Create ICurrentUserService interface
  - [x] Implement CurrentUserService in Infrastructure layer
  - [x] Register CurrentUserService in DI container

- [x] **Role-Based Scope Resolution (Completed 2025-11-05)**
  - [x] Implement GetEffectiveScopeAsync() in CurrentUserService
  - [x] Map roles to effective scopes (HQ Admin → Global, Store Owner → Owned, etc.)
  - [x] Update GetAccessibleLocationIdsAsync() to use effective scope
  - [x] Add comprehensive logging for debugging scope resolution

- [x] **Repository Updates (Inventory Only)**
  - [x] Add GetByLocationIdsAsync to IInventoryRepository
  - [x] Implement GetByLocationIdsAsync in InventoryRepository
  - [x] Fix UserRepository.UpdateAsync to persist UserLocationAssignment entities
  - [ ] Add GetByLocationIdsAsync to ICustomerRepository (not needed - customers are Global scope)
  - [ ] Add GetByLocationIdsAsync to IProductRepository (deferred to Phase 3 - location-specific products)

- [x] **Query Handler Updates (Inventory Only)**
  - [x] Update GetInventoryByProductQueryHandler with location filtering
  - [x] Update GetInventoryByLocationQueryHandler with access check
  - [x] Update GetInventoryByIdQueryHandler with access check
  - [ ] Update CustomersQueryHandler with location filtering (not needed - customers are Global scope)
  - [ ] Update ProductsQueryHandler with location filtering (deferred to Phase 3)

- [x] **Database Fixes (Completed 2025-11-05)**
  - [x] Create SetInitialPermissionScopes migration to populate permission scopes
  - [x] Set Global scope for customer, role, and permission management permissions
  - [x] Set Assigned scope for location-based permissions (inventory, products, users, locations)

- [x] **Testing (All Roles)**
  - [x] Create automated test suite (test-phase2.js)
  - [x] Test HQ Admin sees all data (Global scope)
  - [x] Test multiple location aggregation
  - [x] Test location-specific queries
  - [x] Create test users with different roles
  - [x] Test Store Owner sees only owned stores (Owned scope)
  - [x] Test Store Manager sees only managed store (Managed scope)
  - [x] Test Employee sees only assigned locations (Assigned scope)
  - [x] Test access denied for unauthorized locations

**Deliverables:**
- ✅ Permission scopes defined and implemented
- ✅ Role-based scope resolution working for all roles
- ✅ CurrentUserService for accessing user context
- ✅ Location-filtered queries for inventory (complete)
- ✅ Automated testing with all roles (100% pass rate)
- ✅ UserLocationAssignment persistence fixed
- ⏳ Location-filtered queries for products (deferred to Phase 3)

---

### Phase 3: Product Location Pricing (Week 4) 🚨 CRITICAL

**Goal:** Support location-specific products and pricing

- [ ] **Week 4: ProductLocation Entity**
  - [ ] Create Domain/Products/ProductLocation.cs entity
  - [ ] Add LocationPrice (Money, nullable) property
  - [ ] Add IsAvailable (bool) property
  - [ ] Add LocationSku (string, nullable) property
  - [ ] Add MinimumStock (int, nullable) property
  - [ ] Create ProductLocationErrors
  - [ ] Create domain events (ProductPriceChangedAtLocation, etc.)
  - [ ] Update Product entity: rename Price to BasePrice
  - [ ] Add IsGloballyAvailable to Product entity
  - [ ] Add GetPriceForLocation method to Product
  - [ ] Create IProductLocationRepository interface
  - [ ] Add unique constraint on (ProductId, LocationId)
  - [ ] Update product queries to include location pricing
  - [ ] Write unit tests

**Deliverables:**
- Products can have different prices per location
- Products can be available/unavailable at specific locations
- Store-specific products supported
- Location-aware product queries

---

### Phase 4: Order & Sales (Weeks 5-6) 🚨 CRITICAL

**Goal:** Track sales with location attribution and aggregated reporting

- [ ] **Week 5: Order Aggregate**
  - [ ] Create Domain/Orders folder structure
  - [ ] Implement Order entity (aggregate root)
  - [ ] Implement OrderLine entity
  - [ ] Implement OrderNumber value object
  - [ ] Create OrderStatus enum
  - [ ] Add LocationId (sales attribution)
  - [ ] Add SalesPersonId (User who made the sale)
  - [ ] Add CustomerId (shared customer)
  - [ ] Create IOrderRepository interface
  - [ ] Implement order business logic (add line, calculate totals, etc.)
  - [ ] Create domain events (OrderCreated, OrderSubmitted, etc.)
  - [ ] Integrate with Inventory (reserve stock on order)
  - [ ] Write unit tests

- [ ] **Week 6: Sales Reporting & Location-Scoped Queries**
  - [ ] Implement location-scoped order queries
  - [ ] Implement aggregated sales totals (company-wide)
  - [ ] Create sales attribution reports
  - [ ] Implement "accessible locations" query helper
  - [ ] Update GraphQL queries to respect location access
  - [ ] Create sales dashboard queries
  - [ ] Write integration tests

**Deliverables:**
- Complete order management system
- Sales attribution by location
- Location-scoped order visibility
- Aggregated reporting (company-wide totals)
- Integration with inventory management

---

### Phase 5: Procurement (Week 7) ⚠️ HIGH

**Goal:** Enable purchasing and inventory replenishment

- [ ] **Week 7: Supplier & PurchaseOrder Aggregates**
  - [ ] Create Domain/Suppliers folder structure
  - [ ] Implement Supplier entity
  - [ ] Implement SupplierName value object
  - [ ] Implement PaymentTerms value object
  - [ ] Create SupplierStatus enum
  - [ ] Create ISupplierRepository interface
  - [ ] Create Domain/PurchaseOrders folder structure
  - [ ] Implement PurchaseOrder entity
  - [ ] Implement PurchaseOrderLine entity
  - [ ] Implement PurchaseOrderNumber value object
  - [ ] Create PurchaseOrderStatus enum
  - [ ] Create IPurchaseOrderRepository interface
  - [ ] Implement PO business logic
  - [ ] Create domain events
  - [ ] Integrate with Inventory (receive stock)
  - [ ] Write unit tests

**Deliverables:**
- Supplier management
- Purchase order tracking
- Inventory replenishment workflow

---

### Phase 6: Inventory Collaboration (Week 8) ⚠️ HIGH

**Goal:** Enable inter-location inventory transfers with blind request workflow

- [ ] **Week 8: InventoryTransfer Aggregate**
  - [ ] Create Domain/Transfers folder structure
  - [ ] Implement InventoryTransfer entity
  - [ ] Implement TransferLine entity
  - [ ] Implement TransferNumber value object
  - [ ] Create TransferStatus enum (Requested, Approved, Rejected, Shipped, Received, Cancelled)
  - [ ] Create IInventoryTransferRepository interface
  - [ ] Implement "blind request" workflow (stores can't see other inventory)
  - [ ] Implement HQ approval workflow
  - [ ] Implement ship/receive workflow
  - [ ] Create domain events
  - [ ] Integrate with Inventory aggregate
  - [ ] Write unit tests

**Deliverables:**
- Transfer request system (blind requests)
- HQ approval workflow
- Inventory movement tracking
- Integration with inventory management

---

### Phase 7: Supporting Features (Weeks 9-11) 📋 MEDIUM

- [ ] **Week 9: Payment & Invoice**
  - [ ] Implement Payment entity
  - [ ] Implement Invoice entity
  - [ ] Link to Orders

- [ ] **Week 10: Shipment & Returns**
  - [ ] Implement Shipment entity
  - [ ] Implement Return entity
  - [ ] Link to Orders

- [ ] **Week 11: Additional Features**
  - [ ] Product categories
  - [ ] Promotions/Discounts
  - [ ] Customer loyalty programs

---

## Required Entity Updates

### Location Entity Changes
```csharp
// ADD:
public Guid? OwnerId { get; private set; }     // User who owns this store
public Guid? ManagerId { get; private set; }   // User who manages this store

// ADD methods:
public void AssignOwner(Guid userId)
public void RemoveOwner()
public void AssignManager(Guid userId)
public void RemoveManager()
```

### User Entity Changes
```csharp
// ADD:
private readonly List<UserLocationAssignment> _locationAssignments = [];
public IReadOnlyCollection<UserLocationAssignment> LocationAssignments => _locationAssignments.AsReadOnly();
public Guid? CurrentLocationContextId { get; private set; }

// ADD methods:
public void AssignToLocation(Guid locationId, Guid? roleId, bool isPrimary)
public void UnassignFromLocation(Guid locationId)
public void SwitchLocationContext(Guid locationId)
public bool CanAccessLocation(Guid locationId)
public IEnumerable<Guid> GetAccessibleLocationIds()
```

### Product Entity Changes
```csharp
// RENAME:
public Money Price { get; private set; }  →  public Money BasePrice { get; private set; }

// ADD:
public bool IsGloballyAvailable { get; private set; }

// ADD method:
public Money GetPriceForLocation(Guid locationId, IReadOnlyList<ProductLocation> locationPrices)
```

### Customer Entity Changes
```csharp
// ADD (optional):
public Guid? PreferredLocationId { get; private set; }  // Customer's preferred/home location
```

### Inventory Entity Changes
```csharp
// ADD methods for transfers:
public Result<DomainError> TransferOut(int quantity, Guid transferId)
public Result<DomainError> TransferIn(int quantity, Guid transferId)
```

---

## Progress Tracking

### Overall Progress: 35% Complete (Phase 1 & 2 Fully Complete)

- [x] Phase 1: Location-Based Access Control (2/2 weeks - ✅ COMPLETE)
- [x] Phase 2: Location-Based Query Filtering (1/1 week - ✅ COMPLETE)
  - Note: Products query filtering deferred to Phase 3 (location-specific products)
- [ ] Phase 3: Product Location Pricing (0/1 week)
- [ ] Phase 4: Order & Sales (0/2 weeks)
- [ ] Phase 5: Procurement (0/1 week)
- [ ] Phase 6: Inventory Collaboration (0/1 week)
- [ ] Phase 7: Supporting Features (0/3 weeks)

### Entity Implementation Status

| Entity | Status | Phase | Priority |
|--------|--------|-------|----------|
| UserLocationAssignment | ✅ Complete | 1 | 🚨 Critical |
| ProductLocation | ❌ Not Started | 3 | 🚨 Critical |
| Order | ❌ Not Started | 4 | 🚨 Critical |
| OrderLine | ❌ Not Started | 4 | 🚨 Critical |
| Supplier | ❌ Not Started | 5 | 🚨 Critical |
| PurchaseOrder | ❌ Not Started | 5 | 🚨 Critical |
| PurchaseOrderLine | ❌ Not Started | 5 | 🚨 Critical |
| InventoryTransfer | ❌ Not Started | 6 | ⚠️ High |
| TransferLine | ❌ Not Started | 6 | ⚠️ High |
| Payment | ❌ Not Started | 7 | 📋 Medium |
| Invoice | ❌ Not Started | 7 | 📋 Medium |
| Shipment | ❌ Not Started | 7 | 📋 Medium |
| Return | ❌ Not Started | 7 | 📋 Medium |

---

## Notes & Decisions

### Design Decisions

**Single-Tenant Architecture:**
- ✅ Single organization (no multi-tenancy)
- ✅ No OrganizationId needed on entities
- ✅ Location-based access control instead of organization-based

**User-Location Relationship:**
- ✅ UserLocationAssignment entity (separate entity for flexibility)
- ✅ Users can be assigned to multiple locations
- ✅ Context switching model (users switch between assigned locations)
- ✅ Store ownership tracked via Location.OwnerId
- ✅ Store management tracked via Location.ManagerId

**Product Pricing Strategy:**
- ✅ Products have BasePrice (global default)
- ✅ ProductLocation entity for location-specific pricing
- ✅ Locations can override price for specific products
- ✅ Support for store-specific products (only available at one location)
- 🤔 **Decision Needed:** Can stores set their own prices, or only HQ?

**Customer Management:**
- ✅ Customers are shared across all locations
- ✅ No duplication of customer data
- 🤔 **Decision Needed:** Track "home location" or "preferred location" for customers?

**Inventory Transfer Workflow:**
- ✅ "Blind request" model (stores can't see other stores' inventory)
- ✅ HQ approval workflow for transfers
- ✅ Track transfer status (Requested → Approved → Shipped → Received)
- ✅ Update inventory at both locations atomically
- 🤔 **Decision Needed:** Automatic approval for small quantities?

**Permission System:**
- ✅ PermissionScope enum (Global, Owned, Managed, Assigned, Context)
- ✅ Location-aware authorization
- ✅ Role hierarchy: HQ Admin > Store Owner > Store Manager > Employee

### Open Questions

1. **Store-Specific Products:** If a store creates a product only available at their location, can other stores make it available too?
2. **Pricing Authority:** Can stores set their own prices, or only HQ? Are there min/max constraints?
3. **Customer Home Location:** Should we track a preferred/home location for customers?
4. **Transfer Approval:** Should transfers be auto-approved for small quantities, or always require HQ approval?
5. **Aggregated Data Timing:** Should aggregated sales data be real-time or delayed (e.g., yesterday's totals)?
6. **Product Categories:** Do we need product categories/hierarchies?
7. **Partial Fulfillment:** Should we support partial order fulfillment?

---

## References

- **DDD Patterns:** Aggregates, Entities, Value Objects, Domain Events
- **Architecture:** Clean Architecture, CQRS, Repository Pattern
- **Current Tech Stack:** .NET, Entity Framework Core, PostgreSQL, GraphQL

---

**Last Updated:** 2025-11-05
**Next Review:** After Phase 3 completion

---

## Phase 1 Completion Summary

### ✅ Completed (2025-11-05)

**Domain Layer Implementation:**
1. **UserLocationAssignment Entity** - Full lifecycle management for user-location relationships
2. **AssignmentStatus Enum** - Active, Inactive, Terminated states
3. **User Entity Updates** - Added CurrentLocationContextId and location assignment methods
4. **Location Entity Updates** - Added OwnerId and ManagerId with assignment methods
5. **PermissionScope Enum** - Global, Owned, Managed, Assigned, Context scopes
6. **Permission Entity Updates** - Added Scope property to Permission entity
7. **LocationAccessHelper** - Pure domain logic for access control
8. **ILocationAccessService** - Interface for application layer implementation
9. **Domain Events** - 7 new events for user-location and ownership operations

**Infrastructure Layer Implementation:**
1. **LocationAccessService** - Concrete implementation of ILocationAccessService
2. **CurrentUserService** - Service for accessing current user context in query handlers
3. **EF Core Configurations** - UserLocationAssignment, Permission.Scope
4. **Database Migrations** - 2 migrations (AddLocationBasedAccessControl, AddPermissionScope)
5. **DatabaseSeeder Updates** - Added permission scopes to all 24 permissions
6. **DI Registration** - Registered LocationAccessService and CurrentUserService

**Application Layer Implementation:**
1. **GraphQL Mutations** - 8 mutations for location assignments and ownership
   - AssignUserToLocation, UnassignUserFromLocation, SwitchLocationContext, SetPrimaryLocation
   - AssignLocationOwner, RemoveLocationOwner, AssignLocationManager, RemoveLocationManager
2. **Command Handlers** - 8 command handlers with validation
3. **Payloads** - GraphQL response types for all mutations

**Testing:**
1. **End-to-End Testing** - All 8 mutations tested successfully via GraphQL playground
2. **Authentication Testing** - Login and JWT token generation verified
3. **Authorization Testing** - Permission enforcement verified
4. **Database Testing** - Migrations applied and data seeded successfully

**Files Created:**
- Domain/Users/Enums/AssignmentStatus.cs
- Domain/Users/UserLocationAssignment.cs
- Domain/Users/UserLocationAssignmentErrors.cs
- Domain/Users/Events/UserAssignedToLocationEvent.cs
- Domain/Users/Events/UserUnassignedFromLocationEvent.cs
- Domain/Users/Events/LocationContextSwitchedEvent.cs
- Domain/Users/Events/PrimaryLocationSetEvent.cs
- Domain/Locations/Events/LocationOwnerAssignedEvent.cs
- Domain/Locations/Events/LocationOwnerRemovedEvent.cs
- Domain/Locations/Events/LocationManagerAssignedEvent.cs
- Domain/Locations/Events/LocationManagerRemovedEvent.cs
- Domain/Permission/Enums/PermissionScope.cs
- Domain/Abstractions/ILocationAccessService.cs
- Domain/Abstractions/LocationAccessHelper.cs
- Infrastructure/Services/LocationAccessService.cs
- Infrastructure/Services/CurrentUserService.cs
- Application/Services/ICurrentUserService.cs
- Application/Commands/Locations/* (8 command files)
- Application/Handlers/Locations/* (8 handler files)
- Api/GraphQL/Mutations/LocationMutations.cs (extended)
- Api/GraphQL/Payloads/* (8 payload files)
- Infrastructure/Database/Configurations/UserLocationAssignmentConfiguration.cs
- Infrastructure/Database/Migrations/20251105154922_AddLocationBasedAccessControl.cs
- Infrastructure/Database/Migrations/20251105192504_AddPermissionScope.cs

**Files Modified:**
- Domain/Users/User.cs - Added location context and assignment methods
- Domain/Locations/Location.cs - Added ownership and management
- Domain/Permission/Permission.cs - Added Scope property
- Infrastructure/Database/DatabaseSeeder.cs - Added permission scopes
- Infrastructure/Database/Configurations/PermissionConfiguration.cs - Added Scope column
- Infrastructure/DependencyInjection.cs - Registered new services

### ✅ Phase 1 Status: COMPLETE

All Phase 1 deliverables have been implemented, tested, and verified:
- ✅ User-location relationships working
- ✅ Context switching functional
- ✅ Store ownership and management tracking
- ✅ Permission scopes defined and seeded
- ✅ Location access service implemented
- ✅ Current user service for query handlers
- ✅ All GraphQL mutations tested end-to-end
- ✅ Database migrations applied successfully

### ✅ Phase 2 Status: COMPLETE

Phase 2 implemented location-based query filtering for inventory queries with full role-based scope resolution. All deliverables have been implemented, tested, and verified with 100% test pass rate across all user roles.

**Implementation Summary:**

**Domain Layer Updates:**
1. **InventoryErrors.AccessDenied** - New error for unauthorized location access

**Repository Layer Updates:**
1. **IInventoryRepository.GetByLocationIdsAsync()** - New method for filtering by multiple locations
2. **InventoryRepository.GetByLocationIdsAsync()** - Implementation using LINQ Contains
3. **UserRepository.UpdateAsync()** - Fixed to persist UserLocationAssignment entities
   - Manually handles location assignments collection (ignored in User EF configuration)
   - Compares existing vs current assignments
   - Adds new, updates modified, removes deleted assignments

**Application Layer Updates:**
1. **GetInventoryByProductQueryHandler** - Updated to filter by accessible locations
   - Injects ICurrentUserService
   - Calls GetAccessibleLocationIdsAsync("inventory:read")
   - Filters inventory results by accessible locations
   - Calculates totals only for accessible inventory
2. **GetInventoryByLocationQueryHandler** - Updated to check location access
   - Verifies user has access to requested location
   - Returns AccessDenied error if unauthorized
3. **GetInventoryByIdQueryHandler** - Updated to check location access
   - Verifies user has access to inventory's location
   - Returns AccessDenied error if unauthorized

**Infrastructure Layer Updates:**
1. **CurrentUserService.GetEffectiveScopeAsync()** - New method for role-based scope resolution
   - Maps HQ Admin → Global scope
   - Maps Store Owner → Owned scope
   - Maps Store Manager → Managed scope
   - Maps Employee → Assigned scope
   - Maps Read-Only User → Assigned scope
   - Preserves Global and Context scopes as-is
2. **CurrentUserService.GetAccessibleLocationIdsAsync()** - Updated to use effective scope
   - Gets base scope from permission
   - Determines effective scope based on user's role
   - Calls LocationAccessService with effective scope
   - Comprehensive logging for debugging
3. **DatabaseSeeder** - Updated customer permissions from Assigned to Global scope
   - Customers are shared across all locations (no LocationId on Customer entity)
   - All users can see all customers regardless of location assignments

**Database Migrations:**
1. **20251105194246_UpdateCustomerPermissionScopes.cs** - Migration to update customer permissions
2. **20251105202753_SetInitialPermissionScopes.cs** - Migration to populate permission scopes
   - Set Global scope for: customers:*, roles:*, permissions:read
   - Set Assigned scope for: locations:*, users:*, inventory:*, products:*, sessions:read

**Testing:**
1. **Automated Test Suite** - Created comprehensive test script (test-phase2.js)
   - Test 1: HQ Admin can query inventory by product (2 records) ✅
   - Test 2: HQ Admin sees inventory from multiple locations (2 locations) ✅
   - Test 3: Employee sees only assigned location inventory (1 record) ✅
   - Test 4: Store Manager sees only managed store inventory (1 record) ✅
   - Test 5: Store Owner sees only owned store inventory (1 record) ✅
   - Test 6: Access denied for unauthorized location ✅
   - **Pass Rate: 100% (6/6 tests)**
2. **Test Data** - Created inventory at 2 locations with total quantity 37
3. **Multi-Role Testing** - All user roles tested successfully

**Files Created:**
- test-phase2.js - Automated test script with GraphQL queries
- check-permission-scope.js - Database verification script
- check-employee-assignments.js - Assignment verification script
- Infrastructure/Database/Migrations/20251105202753_SetInitialPermissionScopes.cs

**Files Modified:**
- Domain/Inventory/InventoryErrors.cs - Added AccessDenied error
- Domain/Inventory/IInventoryRepository.cs - Added GetByLocationIdsAsync method
- Infrastructure/Database/Repositories/InventoryRepository.cs - Implemented GetByLocationIdsAsync
- Infrastructure/Database/Repositories/UserRepository.cs - Fixed UpdateAsync to persist location assignments
- Infrastructure/Services/CurrentUserService.cs - Added role-based scope resolution
- Application/Inventory/Queries/GetInventoryByProduct/GetInventoryByProductQueryHandler.cs - Added location filtering
- Application/Inventory/Queries/GetInventoryByLocation/GetInventoryByLocationQueryHandler.cs - Added access check
- Application/Inventory/Queries/GetInventoryById/GetInventoryByIdQueryHandler.cs - Added access check
- Infrastructure/Database/DatabaseSeeder.cs - Updated customer permission scopes
- Infrastructure/Database/Migrations/20251105194246_UpdateCustomerPermissionScopes.cs - Created migration

**Test Results (2025-11-05):**
- ✅ HQ Admin with Global scope sees all inventory across all locations (2 records)
- ✅ Store Owner with Owned scope sees only owned store inventory (1 record)
- ✅ Store Manager with Managed scope sees only managed store inventory (1 record)
- ✅ Employee with Assigned scope sees only assigned location inventory (1 record)
- ✅ Inventory from multiple locations correctly aggregated
- ✅ Location-specific queries return only data for that location
- ✅ Totals calculated only for accessible inventory
- ✅ No data leakage between locations
- ✅ Access denied for unauthorized location access

**Verified Functionality:**
- ✅ Query handlers filter by user's accessible locations
- ✅ ICurrentUserService.GetAccessibleLocationIdsAsync() integration working
- ✅ Role-based scope resolution working for all roles
- ✅ Permission scope evaluation (all scopes tested: Global, Owned, Managed, Assigned)
- ✅ Access control enforcement in place
- ✅ Multiple locations supported
- ✅ Data aggregation working correctly
- ✅ UserLocationAssignment persistence working
- ✅ Location ownership and management working
- ✅ Multi-role testing complete

**Critical Fixes Applied:**
1. **Empty Permission Scopes** - Created migration to populate all permission scopes in database
2. **UserLocationAssignment Not Persisting** - Fixed UserRepository.UpdateAsync to manually handle location assignments collection
3. **Role-Based Scope Resolution** - Implemented GetEffectiveScopeAsync to map roles to effective scopes

