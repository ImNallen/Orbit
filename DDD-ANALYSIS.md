# Domain-Driven Design Analysis & Implementation Roadmap

**Project:** Orbit ERP System
**Analysis Date:** 2025-11-05
**Last Updated:** 2025-11-05
**Status:** 🔴 Critical gaps identified - Foundation incomplete for chain-based ERP

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

### Implemented Entities (9)

| Entity | Bounded Context | Status | Notes |
|--------|----------------|--------|-------|
| User | Users | ✅ Complete | Authentication & authorization |
| Role | Role | ✅ Complete | Permission grouping |
| Permission | Permission | ✅ Complete | Resource:action pattern |
| Session | Session | ✅ Complete | Session tracking |
| PasswordHistory | Users | ✅ Complete | Password reuse prevention |
| Customer | Customers | ✅ Complete | Customer management |
| Product | Products | ✅ Complete | Product catalog |
| Location | Locations | ⚠️ Incomplete | Missing OrganizationId |
| Inventory | Inventory | ✅ Complete | Stock management with reservations |

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
Inventory → Product (many-to-one)
Inventory → Location (many-to-one)
```

---

## User Role Hierarchy

```
┌─────────────────────────────────────────┐
│ HQ Admin / Corporate                    │
│ - Sees ALL locations                    │
│ - Global permissions                    │
│ - No context switching needed           │
└─────────────────────────────────────────┘
              ↓
┌─────────────────────────────────────────┐
│ Store Owner                             │
│ - Owns multiple locations               │
│ - Sees data from owned locations        │
│ - Can switch context between owned stores│
└─────────────────────────────────────────┘
              ↓
┌─────────────────────────────────────────┐
│ Store Manager                           │
│ - Manages ONE location                  │
│ - Sees only their location's data       │
│ - No context switching (single location)│
└─────────────────────────────────────────┘
              ↓
┌─────────────────────────────────────────┐
│ Employee                                │
│ - Assigned to one or more locations     │
│ - Must switch context to active location│
│ - Sees only current context location    │
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

### 🚨 Priority 1: Location-Based Access Control (BLOCKING)

#### 1. User-Location Relationships
**Status:** ❌ Not Implemented
**Impact:** Cannot assign users to locations, no context switching, no ownership tracking
**Blocks:** All location-scoped features

**Required Components:**
- [ ] UserLocationAssignment entity
- [ ] Add CurrentLocationContextId to User entity
- [ ] Add OwnerId to Location entity (store ownership)
- [ ] Add ManagerId to Location entity (store management)
- [ ] LocationOwnership entity (optional - track ownership history)
- [ ] AssignmentStatus enum
- [ ] PermissionScope enum (Global, Owned, Managed, Assigned, Context)

**Relationships:**
- User ↔ Location (many-to-many via UserLocationAssignment)
- Location → User (many-to-one for Owner)
- Location → User (many-to-one for Manager)

**Key Features:**
- Users can be assigned to multiple locations
- Users can switch context between assigned locations
- Store owners can own multiple locations
- Store managers manage one location
- Location-scoped data access based on role and assignments

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
  - [ ] Write unit tests for User-Location logic

- [x] **Week 2: Location Ownership & Permission System**
  - [x] Add OwnerId to Location entity (nullable Guid)
  - [x] Add ManagerId to Location entity (nullable Guid)
  - [x] Add AssignOwner/AssignManager methods to Location
  - [x] Create PermissionScope enum (Global, Owned, Managed, Assigned, Context)
  - [x] Create ILocationAccessService interface for application layer
  - [x] Implement location-aware authorization helpers (LocationAccessHelper)
  - [ ] Create authorization policies for each role type (Application layer)
  - [ ] Update all queries to filter by user's accessible locations (Application layer)
  - [ ] Write integration tests for access control

**Deliverables:**
- ✅ Users can be assigned to multiple locations
- ✅ Context switching functionality working
- ✅ Store ownership and management tracking
- ✅ Location-based access control domain logic implemented
- ⏳ Authorization policies for all role types (Application layer - next step)

---

### Phase 2: Product Location Pricing (Week 3) 🚨 CRITICAL

**Goal:** Support location-specific products and pricing

- [ ] **Week 3: ProductLocation Entity**
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

### Phase 3: Order & Sales (Weeks 4-5) 🚨 CRITICAL

**Goal:** Track sales with location attribution and aggregated reporting

- [ ] **Week 4: Order Aggregate**
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

- [ ] **Week 5: Sales Reporting & Location-Scoped Queries**
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

### Phase 4: Procurement (Week 6) ⚠️ HIGH

**Goal:** Enable purchasing and inventory replenishment

- [ ] **Week 6: Supplier & PurchaseOrder Aggregates**
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

### Phase 5: Inventory Collaboration (Week 7) ⚠️ HIGH

**Goal:** Enable inter-location inventory transfers with blind request workflow

- [ ] **Week 7: InventoryTransfer Aggregate**
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

### Phase 6: Supporting Features (Weeks 8-10) 📋 MEDIUM

- [ ] **Week 8: Payment & Invoice**
  - [ ] Implement Payment entity
  - [ ] Implement Invoice entity
  - [ ] Link to Orders

- [ ] **Week 9: Shipment & Returns**
  - [ ] Implement Shipment entity
  - [ ] Implement Return entity
  - [ ] Link to Orders

- [ ] **Week 10: Additional Features**
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

### Overall Progress: 17% Complete (Phase 1 Domain Logic Complete)

- [x] Phase 1: Location-Based Access Control (2/2 weeks - Domain layer complete)
- [ ] Phase 2: Product Location Pricing (0/1 week)
- [ ] Phase 3: Order & Sales (0/2 weeks)
- [ ] Phase 4: Procurement (0/1 week)
- [ ] Phase 5: Inventory Collaboration (0/1 week)
- [ ] Phase 6: Supporting Features (0/3 weeks)

### Entity Implementation Status

| Entity | Status | Phase | Priority |
|--------|--------|-------|----------|
| UserLocationAssignment | ✅ Complete | 1 | 🚨 Critical |
| ProductLocation | ❌ Not Started | 2 | 🚨 Critical |
| Order | ❌ Not Started | 3 | 🚨 Critical |
| OrderLine | ❌ Not Started | 3 | 🚨 Critical |
| Supplier | ❌ Not Started | 4 | 🚨 Critical |
| PurchaseOrder | ❌ Not Started | 4 | 🚨 Critical |
| PurchaseOrderLine | ❌ Not Started | 4 | 🚨 Critical |
| InventoryTransfer | ❌ Not Started | 5 | ⚠️ High |
| TransferLine | ❌ Not Started | 5 | ⚠️ High |
| Payment | ❌ Not Started | 6 | 📋 Medium |
| Invoice | ❌ Not Started | 6 | 📋 Medium |
| Shipment | ❌ Not Started | 6 | 📋 Medium |
| Return | ❌ Not Started | 6 | 📋 Medium |

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
**Next Review:** After Phase 2 completion

---

## Phase 1 Completion Summary

### ✅ Completed (2025-11-05)

**Domain Layer Implementation:**
1. **UserLocationAssignment Entity** - Full lifecycle management for user-location relationships
2. **AssignmentStatus Enum** - Active, Inactive, Terminated states
3. **User Entity Updates** - Added CurrentLocationContextId and location assignment methods
4. **Location Entity Updates** - Added OwnerId and ManagerId with assignment methods
5. **PermissionScope Enum** - Global, Owned, Managed, Assigned, Context scopes
6. **LocationAccessHelper** - Pure domain logic for access control
7. **ILocationAccessService** - Interface for application layer implementation
8. **Domain Events** - 7 new events for user-location and ownership operations

**Files Created:**
- Domain/Users/Enums/AssignmentStatus.cs
- Domain/Users/UserLocationAssignment.cs
- Domain/Users/UserLocationAssignmentErrors.cs
- Domain/Users/Events/UserAssignedToLocationEvent.cs
- Domain/Users/Events/UserUnassignedFromLocationEvent.cs
- Domain/Users/Events/LocationContextSwitchedEvent.cs
- Domain/Locations/Events/LocationOwnerAssignedEvent.cs
- Domain/Locations/Events/LocationOwnerRemovedEvent.cs
- Domain/Locations/Events/LocationManagerAssignedEvent.cs
- Domain/Locations/Events/LocationManagerRemovedEvent.cs
- Domain/Permission/Enums/PermissionScope.cs
- Domain/Abstractions/ILocationAccessService.cs
- Domain/Abstractions/LocationAccessHelper.cs

**Files Modified:**
- Domain/Users/User.cs - Added location context and assignment methods
- Domain/Locations/Location.cs - Added ownership and management

### ⏳ Remaining Work (Application Layer)
- Database migrations for new properties and entities
- EF Core configurations for UserLocationAssignment
- Application layer implementation of ILocationAccessService
- GraphQL mutations for location assignments and context switching
- Authorization policies using PermissionScope
- Query filters for location-based access control
- Unit tests for domain logic
- Integration tests for access control

