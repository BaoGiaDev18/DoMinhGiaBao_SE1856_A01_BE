# API Design Guidelines - FU News Management System

## ?? Table of Contents
1. [General Principles](#general-principles)
2. [URL Structure & Naming](#url-structure--naming)
3. [HTTP Methods](#http-methods)
4. [Get Collection (List) Endpoints](#get-collection-list-endpoints)
5. [Get By ID Endpoints](#get-by-id-endpoints)
6. [Create Endpoints](#create-endpoints)
7. [Update Endpoints](#update-endpoints)
8. [Delete Endpoints](#delete-endpoints)
9. [Response Structure](#response-structure)
10. [Error Handling](#error-handling)
11. [Validation](#validation)
12. [Examples](#examples)

---

## ?? General Principles

### RESTful Design
- Follow REST architectural constraints
- Use standard HTTP methods (GET, POST, PUT, DELETE)
- Design resources around business entities (not database tables)
- Use plural nouns for resource names

### Consistency
- All endpoints must follow the same patterns
- Use consistent naming conventions across the API
- Standardize response structures

### API Versioning
- Not implemented yet, but consider URL versioning for future: `/api/v1/categories`

---

## ?? URL Structure & Naming

### Base Pattern
```
/api/{resource-name}/{id}
```

### Rules

#### 1. Use Lowercase
? **GOOD**: `/api/categories`  
? **BAD**: `/api/Categories`

#### 2. Use Kebab-Case for Multi-Word Resources
? **GOOD**: `/api/news-articles`  
? **BAD**: `/api/newsArticles`, `/api/NewsArticles`

#### 3. Use Plural Nouns
? **GOOD**: `/api/categories`, `/api/news-articles`  
? **BAD**: `/api/category`, `/api/news-article`

#### 4. Route Examples
```
Categories:        /api/categories
News Articles:     /api/news-articles
Tags:              /api/tags
System Accounts:   /api/system-accounts
Authentication:    /api/auth
```

---

## ?? HTTP Methods

| Method | Purpose | Success Response | Idempotent |
|--------|---------|------------------|------------|
| GET | Retrieve resource(s) | 200 OK | ? Yes |
| POST | Create new resource | 201 Created | ? No |
| PUT | Update existing resource | 200 OK | ? Yes |
| DELETE | Delete resource | 200 OK | ? Yes |

---

## ?? Get Collection (List) Endpoints

### URL Pattern
```
GET /api/{resource}
```

### Query Parameters

All list endpoints **MUST** inherit from `QueryParameters` base class and support the following:

#### 1. Pagination (Required)

**Parameters:**
- `page` (int, default: 1, min: 1)
  - 1-based page number
  - Validation: `[Range(1, int.MaxValue)]`
  
- `pageSize` (int, default: 10, min: 1, **max: 100**)
  - Number of items per page
  - Automatically capped at 100 even if client requests more
  - Validation: `[Range(1, 100)]`

**Example:**
```
GET /api/categories?page=1&pageSize=20
GET /api/news-articles?page=2&pageSize=50
```

**Max Page Size Policy:**
- Hard limit: **100 items per page**
- Default: **10 items**
- If client requests more than 100, automatically return 100

#### 2. Sorting (Optional)

**Parameters:**
- `sortBy` (string, max: 50 chars)
  - Property name to sort by
  - Must match response model property names (camelCase)
  - Examples: `"categoryName"`, `"createdDate"`, `"newsTitle"`
  
- `sortOrder` (string, values: `"asc"` or `"desc"`, default: `"asc"`)
  - Sort direction
  - Validation: `[RegularExpression("^(asc|desc)$")]`

**Examples:**
```
GET /api/categories?sortBy=categoryName&sortOrder=asc
GET /api/news-articles?sortBy=createdDate&sortOrder=desc
```

#### 3. Search (Optional)

**Parameter:**
- `search` (string, max: 200 chars)
  - Full-text search across relevant fields
  - Usually searches: name, title, description fields
  - Case-insensitive

**Examples:**
```
GET /api/categories?search=technology
GET /api/news-articles?search=AI
```

**Implementation Notes:**
- Use `Contains()` for substring matching
- Search multiple fields (e.g., title + content for news articles)
- Combine with other filters using AND logic

#### 4. Field Selection (Optional)

**Parameter:**
- `fields` (string, max: 200 chars)
  - Comma-separated list of fields to include in response
  - Reduces payload size for bandwidth optimization
  - Must match response model property names (camelCase)

**Examples:**
```
GET /api/categories?fields=categoryId,categoryName
GET /api/news-articles?fields=newsArticleId,newsTitle,categoryName,createdDate
```

**Returns:** Dynamic object with only requested fields

#### 5. Resource-Specific Filters

##### When to Use Enum vs String

**Use ENUM when:**
- Fixed set of values defined in code
- Type safety is important
- Examples: `NewsStatus`, `AccountRole`

**Implementation:**
```csharp
public enum NewsStatus
{
    Draft = 0,
    Published = 1,
    Archived = 2
}

// In controller
public async Task<IActionResult> GetAll([FromQuery] NewsStatus? status)
```

**Use STRING when:**
- Simple two-state values
- Values are self-descriptive
- Don't need type-safety overhead

**Example:**
```csharp
public class CategoryQueryParameters : QueryParameters
{
    public string? Status { get; set; }  // "active" or "inactive"
}
```

**Usage:**
```
GET /api/categories?status=active
GET /api/categories?status=inactive
```

##### Single Value Filters

**Format:** `paramName=value`

**Examples:**
```
GET /api/categories?parentCategoryId=5
GET /api/news-articles?categoryId=3
GET /api/news-articles?createdById=10
GET /api/news-articles?tagId=7
```

**Best Practices:**
- Use singular parameter names
- Use appropriate data types (int, short, DateTime)
- Allow nullable types for optional filters

##### Multiple Value Filters

**?? Current Implementation:** Single value only

**Future Enhancement:** Support comma-separated values
```
GET /api/news-articles?categoryId=1,2,3  // Not yet implemented
GET /api/news-articles?tagId=5,6,7       // Not yet implemented
```

**Recommendation for Implementation:**
```csharp
public class NewsArticleQueryParameters : QueryParameters
{
    // Current: Single value
    public short? CategoryId { get; set; }
    
    // Future: Multiple values
    // Option 1: Parse comma-separated string
    public string? CategoryIds { get; set; }  // "1,2,3"
    
    // Option 2: Use array binding (ASP.NET Core supports this)
    public short[]? CategoryIds { get; set; }  // ?categoryIds=1&categoryIds=2&categoryIds=3
}
```

##### Date Range Filters

**Parameters:**
- `startDate` (DateTime, nullable)
- `endDate` (DateTime, nullable)

**Examples:**
```
GET /api/news-articles?startDate=2024-01-01&endDate=2024-12-31
GET /api/news-articles?startDate=2024-06-01
```

**Best Practices:**
- Use ISO 8601 format: `YYYY-MM-DD` or `YYYY-MM-DDTHH:mm:ss`
- Support time zones if needed
- Validate startDate < endDate

#### 6. Complete Query Examples

**Simple pagination:**
```
GET /api/categories?page=1&pageSize=10
```

**Pagination + sorting:**
```
GET /api/categories?page=2&pageSize=20&sortBy=categoryName&sortOrder=desc
```

**Pagination + sorting + search:**
```
GET /api/news-articles?page=1&pageSize=15&sortBy=createdDate&sortOrder=desc&search=technology
```

**Complex filtering:**
```
GET /api/news-articles?page=1&pageSize=20&sortBy=createdDate&sortOrder=desc&status=active&categoryId=3&startDate=2024-01-01&endDate=2024-12-31&tagId=5
```

**With field selection:**
```
GET /api/news-articles?page=1&pageSize=10&fields=newsArticleId,newsTitle,categoryName&status=active
```

### Response Structure (Paginated)

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Retrieved 10 of 45 categories",
  "data": [
    {
      "categoryId": 1,
      "categoryName": "Technology",
      "categoryDescription": "Tech news",
      "isActive": true,
      "parentCategoryId": null
    }
  ],
  "pagination": {
    "page": 1,
    "pageSize": 10,
    "totalCount": 45,
    "totalPages": 5,
    "hasPreviousPage": false,
    "hasNextPage": true
  },
  "errors": []
}
```

**Error Response (400 Bad Request):**
```json
{
  "success": false,
  "message": "Invalid query parameters",
  "data": null,
  "pagination": null,
  "errors": [
    "Page must be greater than 0",
    "PageSize must be between 1 and 100"
  ]
}
```

### Implementation Checklist

When creating a new list endpoint:

- [ ] Inherit query parameters from `QueryParameters`
- [ ] Add resource-specific filters as needed
- [ ] Validate all query parameters with data annotations
- [ ] Use `[FromQuery]` attribute in controller
- [ ] Call service layer with all parameters
- [ ] Return `PaginatedApiResponse<T>`
- [ ] Include `PaginationMetadata`
- [ ] Support field selection via `SelectFields()` extension
- [ ] Handle validation errors properly

---

## ?? Get By ID Endpoints

### URL Pattern
```
GET /api/{resource}/{id}
```

### Guidelines

#### Single ID
**Most common pattern** - retrieve one resource by its primary key

**Examples:**
```
GET /api/categories/5
GET /api/news-articles/NA001
GET /api/tags/12
```

**Controller Implementation:**
```csharp
[HttpGet("{id}")]
public async Task<ActionResult<ApiResponse<CategoryResponse>>> GetById(short id)
{
    var dto = await _categoryService.GetCategoryByIdAsync(id);
    if (dto == null)
    {
        return NotFound(ApiResponse<CategoryResponse>.ErrorResponse(
            "Category not found",
            $"No category found with ID: {id}"
        ));
    }
    
    var response = dto.ToResponse();
    return Ok(ApiResponse<CategoryResponse>.SuccessResponse(
        response,
        "Category retrieved successfully"
    ));
}
```

#### Multiple IDs (Future Enhancement)

**Not currently implemented**, but consider for bulk retrieval:

**Option 1: Query parameter**
```
GET /api/categories?ids=1,2,3,4,5
```

**Option 2: POST with body (for many IDs)**
```
POST /api/categories/batch
{
  "ids": [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]
}
```

### Response Structure

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Category retrieved successfully",
  "data": {
    "categoryId": 5,
    "categoryName": "Technology",
    "categoryDescription": "Technology related news",
    "isActive": true,
    "parentCategoryId": null,
    "createdDate": "2024-01-15T10:30:00Z",
    "newsArticleCount": 25
  },
  "errors": []
}
```

**Not Found Response (404 Not Found):**
```json
{
  "success": false,
  "message": "Category not found",
  "data": null,
  "errors": [
    "No category found with ID: 5"
  ]
}
```

### What to Return

#### Detail vs List Response
- **List endpoints** return `ListResponse` (minimal fields)
- **Detail endpoints** return full `Response` (all fields including related data)

**Example:**
```csharp
// List Response (minimal)
public class CategoryListResponse
{
    public short CategoryId { get; set; }
    public string CategoryName { get; set; }
    public string? CategoryDescription { get; set; }
    public bool IsActive { get; set; }
}

// Detail Response (complete)
public class CategoryResponse : CategoryListResponse
{
    public short? ParentCategoryId { get; set; }
    public DateTime CreatedDate { get; set; }
    public int NewsArticleCount { get; set; }  // Aggregated data
    // ... other detailed fields
}
```

#### Include Related Data
- Load related entities when appropriate
- Use DTOs to shape response
- Consider N+1 query problems (use Include/ThenInclude)

**Example:**
```csharp
// News article detail includes category and tags
public class NewsArticleResponse
{
    public string NewsArticleId { get; set; }
    public string NewsTitle { get; set; }
    public string NewsContent { get; set; }
    
    // Related data
    public CategoryResponse Category { get; set; }
    public List<TagResponse> Tags { get; set; }
    public SystemAccountResponse CreatedBy { get; set; }
}
```

---

## ? Create Endpoints

### URL Pattern
```
POST /api/{resource}
```

### Request Structure

#### Request Model Requirements
- Use `[FromBody]` attribute
- Create dedicated `CreateXxxRequest` classes
- Include all required fields
- Add validation attributes

**Example:**
```csharp
public class CreateCategoryRequest
{
    [Required(ErrorMessage = "Category name is required")]
    [MaxLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
    public string CategoryName { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? CategoryDescription { get; set; }

    public short? ParentCategoryId { get; set; }
}
```

#### Request Body Example
```json
{
  "categoryName": "Artificial Intelligence",
  "categoryDescription": "AI and machine learning news",
  "parentCategoryId": 1
}
```

### Controller Implementation

```csharp
[HttpPost]
public async Task<ActionResult<ApiResponse<CategoryResponse>>> Create(
    [FromBody] CreateCategoryRequest request)
{
    // 1. Validate model state
    if (!ModelState.IsValid)
    {
        var errors = ModelState.Values
            .SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
            .ToList();
        return BadRequest(ApiResponse<CategoryResponse>.ErrorResponse(
            "Validation failed",
            errors
        ));
    }

    try
    {
        // 2. Map request to DTO
        var createDto = request.ToCreateDto();
        
        // 3. Call service
        var dto = await _categoryService.CreateCategoryAsync(createDto);
        
        // 4. Map to response
        var response = dto.ToResponse();
        
        // 5. Return 201 Created with Location header
        return CreatedAtAction(
            nameof(GetById), 
            new { id = response.CategoryId }, 
            ApiResponse<CategoryResponse>.SuccessResponse(
                response,
                "Category created successfully"
            )
        );
    }
    catch (InvalidOperationException ex)
    {
        return BadRequest(ApiResponse<CategoryResponse>.ErrorResponse(
            "Failed to create category",
            ex.Message
        ));
    }
}
```

### Response Structure

**Success Response (201 Created):**
```json
{
  "success": true,
  "message": "Category created successfully",
  "data": {
    "categoryId": 15,
    "categoryName": "Artificial Intelligence",
    "categoryDescription": "AI and machine learning news",
    "isActive": true,
    "parentCategoryId": 1,
    "createdDate": "2024-01-20T14:30:00Z"
  },
  "errors": []
}
```

**Headers:**
```
HTTP/1.1 201 Created
Location: /api/categories/15
Content-Type: application/json
```

**Validation Error (400 Bad Request):**
```json
{
  "success": false,
  "message": "Validation failed",
  "data": null,
  "errors": [
    "Category name is required",
    "Category name cannot exceed 100 characters"
  ]
}
```

**Business Logic Error (400 Bad Request):**
```json
{
  "success": false,
  "message": "Failed to create category",
  "data": null,
  "errors": [
    "A category with this name already exists"
  ]
}
```

### Best Practices

1. **Always return the created resource**
   - Client should not need to make another GET request
   
2. **Use 201 Created status code**
   - Not 200 OK
   
3. **Include Location header**
   - Points to the newly created resource
   - Use `CreatedAtAction()` helper
   
4. **Validate before processing**
   - Check ModelState first
   - Return 400 for validation errors
   
5. **Handle duplicates**
   - Check for unique constraints
   - Return meaningful error messages
   
6. **Set default values**
   - CreatedDate, CreatedBy automatically
   - IsActive = true by default
   
7. **Use transactions**
   - For multi-step operations
   - Rollback on any failure

---

## ?? Update Endpoints

### URL Pattern
```
PUT /api/{resource}/{id}
```

### Request Structure

#### Request Model Requirements
- Use `[FromBody]` attribute
- Create dedicated `UpdateXxxRequest` classes
- Include only updatable fields
- Add validation attributes

**Example:**
```csharp
public class UpdateCategoryRequest
{
    [Required(ErrorMessage = "Category name is required")]
    [MaxLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
    public string CategoryName { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? CategoryDescription { get; set; }

    public short? ParentCategoryId { get; set; }
    
    public bool IsActive { get; set; }
}
```

#### Request Body Example
```json
{
  "categoryName": "Artificial Intelligence & ML",
  "categoryDescription": "Updated description",
  "parentCategoryId": 1,
  "isActive": true
}
```

### Controller Implementation

```csharp
[HttpPut("{id}")]
public async Task<ActionResult<ApiResponse<CategoryResponse>>> Update(
    short id, 
    [FromBody] UpdateCategoryRequest request)
{
    // 1. Validate model state
    if (!ModelState.IsValid)
    {
        var errors = ModelState.Values
            .SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
            .ToList();
        return BadRequest(ApiResponse<CategoryResponse>.ErrorResponse(
            "Validation failed",
            errors
        ));
    }

    try
    {
        // 2. Map request to DTO
        var updateDto = request.ToUpdateDto();
        
        // 3. Call service
        var dto = await _categoryService.UpdateCategoryAsync(id, updateDto);
        
        // 4. Check if resource exists
        if (dto == null)
        {
            return NotFound(ApiResponse<CategoryResponse>.ErrorResponse(
                "Category not found",
                $"No category found with ID: {id}"
            ));
        }
        
        // 5. Map to response
        var response = dto.ToResponse();
        
        // 6. Return 200 OK
        return Ok(ApiResponse<CategoryResponse>.SuccessResponse(
            response,
            "Category updated successfully"
        ));
    }
    catch (InvalidOperationException ex)
    {
        return BadRequest(ApiResponse<CategoryResponse>.ErrorResponse(
            "Failed to update category",
            ex.Message
        ));
    }
}
```

### Response Structure

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Category updated successfully",
  "data": {
    "categoryId": 15,
    "categoryName": "Artificial Intelligence & ML",
    "categoryDescription": "Updated description",
    "isActive": true,
    "parentCategoryId": 1,
    "createdDate": "2024-01-20T14:30:00Z",
    "modifiedDate": "2024-01-21T09:15:00Z"
  },
  "errors": []
}
```

**Not Found (404):**
```json
{
  "success": false,
  "message": "Category not found",
  "data": null,
  "errors": [
    "No category found with ID: 15"
  ]
}
```

### PUT vs PATCH

**Current Implementation: PUT (Full Update)**
- Replace entire resource
- All fields must be provided
- Idempotent

**Future Enhancement: PATCH (Partial Update)**
```
PATCH /api/categories/15
{
  "categoryName": "New Name"  // Only update name
}
```

**Recommendation:** Implement PATCH using JSON Patch standard (RFC 6902)

---

## ??? Delete Endpoints

### URL Pattern
```
DELETE /api/{resource}/{id}
```

### Basic Delete

**Controller Implementation:**
```csharp
[HttpDelete("{id}")]
public async Task<ActionResult<ApiResponse>> Delete(short id)
{
    try
    {
        var result = await _categoryService.DeleteCategoryAsync(id);
        
        if (!result)
        {
            return NotFound(ApiResponse.ErrorResponse(
                "Category not found",
                $"No category found with ID: {id}"
            ));
        }
        
        return Ok(ApiResponse.SuccessResponse(
            "Category deleted successfully"
        ));
    }
    catch (InvalidOperationException ex)
    {
        return BadRequest(ApiResponse.ErrorResponse(
            "Failed to delete category",
            ex.Message
        ));
    }
}
```

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Category deleted successfully",
  "errors": []
}
```

**Not Found (404):**
```json
{
  "success": false,
  "message": "Category not found",
  "errors": [
    "No category found with ID: 15"
  ]
}
```

**Constraint Violation (400 Bad Request):**
```json
{
  "success": false,
  "message": "Failed to delete category",
  "errors": [
    "Cannot delete category that has news articles"
  ]
}
```

### Soft Delete vs Hard Delete

**Recommended: Soft Delete**
- Set `IsActive = false` or `IsDeleted = true`
- Preserve data for audit trail
- Allow restoration

**Hard Delete:**
- Physically remove from database
- Only when absolutely necessary
- Check foreign key constraints

### Pre-Delete Validation

**Pattern:** Validation query parameter

```csharp
[HttpDelete("{id}")]
public async Task<ActionResult<ApiResponse>> Delete(
    short id, 
    [FromQuery] bool validate = false)
{
    // Validation mode: Just check if deletion is possible
    if (validate)
    {
        var canDelete = await _categoryService.CanDeleteCategoryAsync(id);
        return Ok(new 
        { 
            canDelete, 
            message = canDelete 
                ? "Category can be deleted" 
                : "Cannot delete category that has news articles" 
        });
    }

    // Actual deletion
    try
    {
        var result = await _categoryService.DeleteCategoryAsync(id);
        if (!result)
        {
            return NotFound(ApiResponse.ErrorResponse(
                "Category not found",
                $"No category found with ID: {id}"
            ));
        }
        
        return Ok(ApiResponse.SuccessResponse(
            "Category deleted successfully"
        ));
    }
    catch (InvalidOperationException ex)
    {
        return BadRequest(ApiResponse.ErrorResponse(
            "Failed to delete category",
            ex.Message
        ));
    }
}
```

**Usage:**
```
DELETE /api/categories/15?validate=true   // Check only
DELETE /api/categories/15                 // Actually delete
```

---

## ?? Response Structure

### Standard Response Wrapper

#### Single Resource Response
```csharp
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
}
```

**Example:**
```json
{
  "success": true,
  "message": "Category retrieved successfully",
  "data": {
    "categoryId": 5,
    "categoryName": "Technology"
  },
  "errors": []
}
```

#### Collection Response (Paginated)
```csharp
public class PaginatedApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public IEnumerable<T>? Data { get; set; }
    public PaginationMetadata? Pagination { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
}
```

**Example:**
```json
{
  "success": true,
  "message": "Retrieved 10 of 45 categories",
  "data": [...],
  "pagination": {
    "page": 1,
    "pageSize": 10,
    "totalCount": 45,
    "totalPages": 5,
    "hasPreviousPage": false,
    "hasNextPage": true
  },
  "errors": []
}
```

#### Pagination Metadata
```csharp
public class PaginationMetadata
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
}
```

### HTTP Status Codes

| Status Code | Meaning | When to Use |
|-------------|---------|-------------|
| 200 OK | Success | GET, PUT, DELETE successful |
| 201 Created | Resource created | POST successful |
| 400 Bad Request | Client error | Validation failed, business rule violation |
| 401 Unauthorized | Not authenticated | Missing/invalid token |
| 403 Forbidden | Not authorized | User lacks permission |
| 404 Not Found | Resource not found | GET/PUT/DELETE non-existent resource |
| 500 Internal Server Error | Server error | Unhandled exceptions (via middleware) |

---

## ?? Error Handling

### Validation Errors

**Pattern:** Collect all ModelState errors

```csharp
if (!ModelState.IsValid)
{
    var errors = ModelState.Values
        .SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
        .ToList();
    return BadRequest(ApiResponse<CategoryResponse>.ErrorResponse(
        "Validation failed",
        errors
    ));
}
```

**Response:**
```json
{
  "success": false,
  "message": "Validation failed",
  "data": null,
  "errors": [
    "Category name is required",
    "Description cannot exceed 500 characters"
  ]
}
```

### Business Logic Errors

**Pattern:** Use exceptions with meaningful messages

```csharp
try
{
    var dto = await _categoryService.CreateCategoryAsync(createDto);
    // ...
}
catch (InvalidOperationException ex)
{
    return BadRequest(ApiResponse<CategoryResponse>.ErrorResponse(
        "Failed to create category",
        ex.Message
    ));
}
```

**Response:**
```json
{
  "success": false,
  "message": "Failed to create category",
  "data": null,
  "errors": [
    "A category with this name already exists"
  ]
}
```

### Not Found

```csharp
if (dto == null)
{
    return NotFound(ApiResponse<CategoryResponse>.ErrorResponse(
        "Category not found",
        $"No category found with ID: {id}"
    ));
}
```

### Global Exception Handler

**All unhandled exceptions** are caught by `GlobalExceptionHandler` middleware:
- Returns 500 Internal Server Error
- Logs exception details
- Hides sensitive information from client

---

## ? Validation

### Request Model Validation

Use Data Annotations on request models:

```csharp
public class CreateCategoryRequest
{
    [Required(ErrorMessage = "Category name is required")]
    [MaxLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
    [MinLength(3, ErrorMessage = "Category name must be at least 3 characters")]
    public string CategoryName { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? CategoryDescription { get; set; }

    [Range(1, short.MaxValue, ErrorMessage = "Parent category ID must be positive")]
    public short? ParentCategoryId { get; set; }
}
```

### Query Parameter Validation

```csharp
public class QueryParameters
{
    [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0")]
    public int Page { get; set; } = 1;

    [Range(1, 100, ErrorMessage = "PageSize must be between 1 and 100")]
    public int PageSize { get; set; } = 10;

    [MaxLength(50, ErrorMessage = "SortBy cannot exceed 50 characters")]
    public string? SortBy { get; set; }

    [RegularExpression("^(asc|desc)$", ErrorMessage = "SortOrder must be 'asc' or 'desc'")]
    public string SortOrder { get; set; } = "asc";
}
```

### Common Validation Attributes

| Attribute | Purpose | Example |
|-----------|---------|---------|
| `[Required]` | Field must have value | Email, Name |
| `[MaxLength(n)]` | String max length | Name (100), Description (500) |
| `[MinLength(n)]` | String min length | Password (8) |
| `[Range(min, max)]` | Numeric range | Page (1, int.Max), Rating (1, 5) |
| `[RegularExpression]` | Pattern matching | Email, Phone, URL |
| `[EmailAddress]` | Valid email format | User email |
| `[Url]` | Valid URL format | Website link |
| `[Phone]` | Valid phone format | Contact number |

---

## ?? Examples

### Complete CRUD Examples

#### Categories API

**1. Get all categories (paginated, filtered, sorted)**
```http
GET /api/categories?page=1&pageSize=20&sortBy=categoryName&sortOrder=asc&status=active&search=tech
```

**2. Get category by ID**
```http
GET /api/categories/5
```

**3. Create category**
```http
POST /api/categories
Content-Type: application/json

{
  "categoryName": "Blockchain Technology",
  "categoryDescription": "Blockchain and cryptocurrency news",
  "parentCategoryId": 1
}
```

**4. Update category**
```http
PUT /api/categories/5
Content-Type: application/json

{
  "categoryName": "Blockchain & Crypto",
  "categoryDescription": "Updated description",
  "parentCategoryId": 1,
  "isActive": true
}
```

**5. Delete category (with validation)**
```http
DELETE /api/categories/5?validate=true   # Check first
DELETE /api/categories/5                 # Actually delete
```

#### News Articles API

**1. Get all articles (complex filtering)**
```http
GET /api/news-articles?page=1&pageSize=15&sortBy=createdDate&sortOrder=desc&status=active&categoryId=3&startDate=2024-01-01&endDate=2024-12-31&tagId=5&search=AI
```

**2. Get article by ID**
```http
GET /api/news-articles/NA001
```

**3. Create article**
```http
POST /api/news-articles
Content-Type: application/json

{
  "newsTitle": "Breaking News in AI",
  "newsContent": "Lorem ipsum dolor sit amet...",
  "categoryId": 3,
  "tagIds": [1, 2, 5],
  "createdById": 10,
  "newsSource": "TechCrunch",
  "newsStatus": 1
}
```

**4. Update article**
```http
PUT /api/news-articles/NA001
Content-Type: application/json

{
  "newsTitle": "Updated: Breaking News in AI",
  "newsContent": "Updated content...",
  "categoryId": 3,
  "tagIds": [1, 2, 5, 7],
  "updatedById": 10,
  "newsStatus": 1
}
```

**5. Delete article**
```http
DELETE /api/news-articles/NA001
```

### Field Selection Examples

**Get only IDs and names:**
```http
GET /api/categories?fields=categoryId,categoryName
```

**Response:**
```json
{
  "success": true,
  "message": "Retrieved 45 categories",
  "data": [
    {
      "categoryId": 1,
      "categoryName": "Technology"
    },
    {
      "categoryId": 2,
      "categoryName": "Sports"
    }
  ],
  "pagination": {...}
}
```

---

## ?? Summary & Quick Reference

### Checklist for New Endpoints

**GET Collection:**
- [ ] Inherit from `QueryParameters`
- [ ] Add resource-specific filters
- [ ] Support pagination (max 100)
- [ ] Support sorting
- [ ] Support search
- [ ] Support field selection
- [ ] Return `PaginatedApiResponse<T>`

**GET By ID:**
- [ ] Accept ID in route
- [ ] Return 404 if not found
- [ ] Return full detail response
- [ ] Include related data
- [ ] Return `ApiResponse<T>`

**POST Create:**
- [ ] Use `[FromBody]` attribute
- [ ] Create dedicated request model
- [ ] Validate input
- [ ] Return 201 Created
- [ ] Include Location header
- [ ] Return created resource

**PUT Update:**
- [ ] Accept ID in route
- [ ] Use `[FromBody]` for data
- [ ] Validate input
- [ ] Return 404 if not found
- [ ] Return updated resource

**DELETE:**
- [ ] Accept ID in route
- [ ] Check constraints
- [ ] Return 404 if not found
- [ ] Return 200 OK on success
- [ ] Consider soft delete

### Key Numbers to Remember

| Parameter | Value |
|-----------|-------|
| Max page size | **100** |
| Default page size | **10** |
| Default page | **1** |
| Max search length | **200 chars** |
| Max fields param | **200 chars** |
| Max sortBy length | **50 chars** |

### URL Naming Patterns

```
? CORRECT:
/api/categories
/api/news-articles
/api/system-accounts
/api/tags

? WRONG:
/api/Categories
/api/newsArticles
/api/systemAccounts
/api/tag
```

---

## ?? Additional Resources

- **ASP.NET Core Documentation**: https://docs.microsoft.com/aspnet/core/
- **REST API Best Practices**: https://restfulapi.net/
- **HTTP Status Codes**: https://httpstatuses.com/

---

**Document Version:** 1.0  
**Last Updated:** 2024-01-21  
**Maintained By:** Development Team
