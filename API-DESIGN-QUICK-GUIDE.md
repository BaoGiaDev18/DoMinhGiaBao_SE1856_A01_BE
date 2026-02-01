# API Design Guideline 📝

- Tài liệu tham khảo cho việc thiết kế API trong FU News Management System

---

## 🎯 Quy Tắc Đặt Tên URL

```
✅ ĐÚNG:   /api/categories
✅ ĐÚNG:   /api/news-articles
❌ SAI:    /api/Categories
❌ SAI:    /api/newsArticles
```

**Quy tắc:**

- Chữ thường (lowercase)
- Dấu gạch ngang cho tên nhiều từ (kebab-case)
- Danh từ số nhiều (plural nouns)
- Đơn giản & nhất quán

---

## 📚 GET Collection - `/api/{resource}`

### Tham Số Bắt Buộc

```
?page=1              // Mặc định: 1, tối thiểu: 1
?pageSize=10         // Mặc định: 10, TỐI ĐA: 100 ⚠️
```

### Tham Số Tùy Chọn

```
?sortBy=categoryName       // Tên thuộc tính (camelCase)
?sortOrder=desc           // "asc" hoặc "desc" (mặc định: asc)
?search=technology        // Tìm kiếm toàn văn (tối đa 200 ký tự)
?fields=id,name,date      // Chọn trường trả về (tối đa 200 ký tự)
```

### Bộ Lọc Theo Resource

**Single values:**

```
?status=active            // Chuỗi: "active"/"inactive"
?categoryId=3            // Số: ID đơn
?createdById=10          // Số: ID đơn
?startDate=2024-01-01    // Ngày: ISO 8601
?endDate=2024-12-31
```

**Nhiều giá trị (Tương lai):**

```
?categoryId=1,2,3        // Chưa triển khai
?tagId=5,6,7
```

### Dùng Enum hay String?

**Dùng STRING khi:**

- Giá trị 2 trạng thái đơn giản
- Tự mô tả
- Ví dụ: `status` ("active"/"inactive")

### Cấu Trúc Response

```json
{
  "success": true,
  "message": "Đã lấy 10 trong số 45 danh mục",
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

### Ví Dụ Query

```
Đơn giản:
GET /api/categories?page=1&pageSize=10

Có sắp xếp:
GET /api/categories?page=1&pageSize=20&sortBy=categoryName&sortOrder=desc

Có tìm kiếm:
GET /api/news-articles?page=1&sortBy=createdDate&sortOrder=desc&search=AI

Phức tạp:
GET /api/news-articles?page=1&pageSize=20&sortBy=createdDate&sortOrder=desc
    &status=active&categoryId=3&startDate=2024-01-01&endDate=2024-12-31&tagId=5

Với field selection:
GET /api/categories?fields=categoryId,categoryName&status=active
```

---

## 🔍 GET By ID - `/api/{resource}/{id}`

```
GET /api/categories/5
GET /api/news-articles/NA001
```

### Response

```json
{
  "success": true,
  "message": "Category retrieved successfully",
  "data": {
    "categoryId": 5,
    "categoryName": "Technology",
    "categoryDescription": "Tech news",
    "isActive": true,
    "parentCategoryId": null,
    "createdDate": "2024-01-15T10:30:00Z",
    "newsArticleCount": 25 // ← Include aggregated/related data
  },
  "errors": []
}
```

**Key Points:**

- Return **full detail** (vs minimal in list)
- Include **related data** (Category, Tags, CreatedBy)
- Return **404** if not found
- Use `Include()` to avoid N+1 queries

---

## ✨ POST Create - `/api/{resource}`

```http
POST /api/categories
Content-Type: application/json

{
  "categoryName": "Artificial Intelligence",
  "categoryDescription": "AI and ML news",
  "parentCategoryId": 1
}
```

### Response (201 Created)

```json
{
  "success": true,
  "message": "Category created successfully",
  "data": {
    "categoryId": 15, // ← Return created resource
    "categoryName": "Artificial Intelligence",
    "categoryDescription": "AI and ML news",
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
Location: /api/categories/15  ← Points to new resource
```

**Remember:**

- Use **201 Created**, not 200
- Return **created resource** (no need for another GET)
- Include **Location header**
- Set **default values** (createdDate, isActive = true)

---

## 🔄 PUT Update - `/api/{resource}/{id}`

```http
PUT /api/categories/15
Content-Type: application/json

{
  "categoryName": "AI & Machine Learning",
  "categoryDescription": "Updated description",
  "parentCategoryId": 1,
  "isActive": true
}
```

### Response (200 OK)

```json
{
  "success": true,
  "message": "Category updated successfully",
  "data": {
    "categoryId": 15,
    "categoryName": "AI & Machine Learning",
    "categoryDescription": "Updated description",
    "isActive": true,
    "parentCategoryId": 1,
    "createdDate": "2024-01-20T14:30:00Z",
    "modifiedDate": "2024-01-21T09:15:00Z" // ← Updated
  },
  "errors": []
}
```

**Remember:**

- Use **200 OK**
- Return **updated resource**
- Return **404** if not found
- **PUT = full update** (all fields)

---

## 🗑️ DELETE - `/api/{resource}/{id}`

```http
DELETE /api/categories/15
```

### Basic Delete

```json
{
  "success": true,
  "message": "Category deleted successfully",
  "errors": []
}
```

### Pre-Delete Validation

```http
DELETE /api/categories/15?validate=true  ← Check only
DELETE /api/categories/15                ← Actually delete
```

**Soft Delete vs Hard Delete:**

- **Soft:** Set `IsActive = false` or `IsDeleted = true` ✅ Recommended
- **Hard:** Actually remove from DB ⚠️ Only if necessary

**Remember:**

- Check **foreign key constraints**
- Return **404** if not found
- Return **400** for constraint violations

---

## 📦 Response Structure Cheat Sheet

### Single Resource

```csharp
ApiResponse<T>
{
    bool Success
    string Message
    T? Data
    List<string> Errors
}
```

### Collection (Paginated)

```csharp
PaginatedApiResponse<T>
{
    bool Success
    string Message
    IEnumerable<T>? Data
    PaginationMetadata? Pagination
    List<string> Errors
}
```

---

## ⚠️ HTTP Status Codes

| Code | When                      | Example                            |
| ---- | ------------------------- | ---------------------------------- |
| 200  | GET/PUT/DELETE success    | Resource retrieved/updated/deleted |
| 201  | POST success              | Resource created                   |
| 400  | Validation/business error | Invalid input, duplicate name      |
| 401  | Not authenticated         | Missing/invalid token              |
| 403  | Not authorized            | User lacks permission              |
| 404  | Not found                 | GET/PUT/DELETE non-existent        |
| 500  | Server error              | Unhandled exception                |

---

## ✅ Validation Quick Reference

### Request Model

```csharp
public class CreateCategoryRequest
{
    [Required(ErrorMessage = "Category name is required")]
    [MaxLength(100, ErrorMessage = "Max 100 characters")]
    [MinLength(3, ErrorMessage = "Min 3 characters")]
    public string CategoryName { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? CategoryDescription { get; set; }

    [Range(1, short.MaxValue)]
    public short? ParentCategoryId { get; set; }
}
```

### Query Parameters

```csharp
public class QueryParameters
{
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(1, 100)]  // MAX: 100 ⚠️
    public int PageSize { get; set; } = 10;

    [MaxLength(50)]
    public string? SortBy { get; set; }

    [RegularExpression("^(asc|desc)$")]
    public string SortOrder { get; set; } = "asc";

    [MaxLength(200)]
    public string? Search { get; set; }

    [MaxLength(200)]
    public string? Fields { get; set; }
}
```

### Common Attributes

```csharp
[Required]              // Must have value
[MaxLength(n)]          // Max string length
[MinLength(n)]          // Min string length
[Range(min, max)]       // Numeric range
[RegularExpression]     // Pattern matching
[EmailAddress]          // Valid email
[Url]                   // Valid URL
[Phone]                 // Valid phone
```

---

## 🎯 Error Handling Pattern

### Validation Errors (400)

```csharp
if (!ModelState.IsValid)
{
    var errors = ModelState.Values
        .SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
        .ToList();
    return BadRequest(ApiResponse<T>.ErrorResponse(
        "Validation failed",
        errors
    ));
}
```

### Business Logic Errors (400)

```csharp
try
{
    var result = await _service.CreateAsync(dto);
}
catch (InvalidOperationException ex)
{
    return BadRequest(ApiResponse<T>.ErrorResponse(
        "Failed to create",
        ex.Message
    ));
}
```

### Not Found (404)

```csharp
if (dto == null)
{
    return NotFound(ApiResponse<T>.ErrorResponse(
        "Resource not found",
        $"No resource found with ID: {id}"
    ));
}
```

---

## 📊 Implementation Checklist

### GET Collection

- [ ] Inherit from `QueryParameters`
- [ ] Add resource-specific filters
- [ ] Support pagination (max 100)
- [ ] Support sorting, search, fields
- [ ] Return `PaginatedApiResponse<T>`

### GET By ID

- [ ] Accept ID in route
- [ ] Return 404 if not found
- [ ] Return full detail + related data
- [ ] Return `ApiResponse<T>`

### POST Create

- [ ] Use `[FromBody]`
- [ ] Validate input
- [ ] Return **201 Created**
- [ ] Include **Location header**
- [ ] Return created resource

### PUT Update

- [ ] Accept ID in route + `[FromBody]`
- [ ] Validate input
- [ ] Return 404 if not found
- [ ] Return updated resource

### DELETE

- [ ] Accept ID in route
- [ ] Check constraints
- [ ] Return 404 if not found
- [ ] Consider soft delete

---

## 🔢 Key Numbers

| What              | Value      |
| ----------------- | ---------- |
| Max page size     | **100** ⚠️ |
| Default page size | **10**     |
| Max search length | **200**    |
| Max fields param  | **200**    |
| Max sortBy length | **50**     |

---

## 💡 Quick Tips

1. **Always paginate** collections (max 100)
2. **Use 201** for POST, not 200
3. **Include Location header** on create
4. **Return created/updated resource** (save client a GET)
5. **Soft delete** when possible
6. **Validate early** (ModelState first)
7. **Use meaningful errors** (not "Invalid input")
8. **List vs Detail responses** (minimal vs full)
9. **Field selection** for bandwidth optimization
10. **Consider N+1** queries (use Include/ThenInclude)

---

## 📝 Example: Complete CRUD

```http
# 1. Get all (paginated, filtered, sorted)
GET /api/categories?page=1&pageSize=20&sortBy=categoryName&sortOrder=asc&status=active&search=tech

# 2. Get by ID
GET /api/categories/5

# 3. Create
POST /api/categories
{
  "categoryName": "Blockchain",
  "categoryDescription": "Crypto news",
  "parentCategoryId": 1
}

# 4. Update
PUT /api/categories/5
{
  "categoryName": "Blockchain & Crypto",
  "categoryDescription": "Updated",
  "parentCategoryId": 1,
  "isActive": true
}

# 5. Delete (with validation)
DELETE /api/categories/5?validate=true  # Check first
DELETE /api/categories/5                # Actually delete
```

---

## 🎓 Controller Template

```csharp
[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    // GET /api/categories
    [HttpGet]
    public async Task<ActionResult<PaginatedApiResponse<T>>> GetAll(
        [FromQuery] QueryParameters query) { }

    // GET /api/categories/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<T>>> GetById(short id) { }

    // POST /api/categories
    [HttpPost]
    public async Task<ActionResult<ApiResponse<T>>> Create(
        [FromBody] CreateRequest request)
    {
        // Return CreatedAtAction with 201
    }

    // PUT /api/categories/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<T>>> Update(
        short id, [FromBody] UpdateRequest request) { }

    // DELETE /api/categories/{id}
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> Delete(
        short id, [FromQuery] bool validate = false) { }
}
```

---

**Version:** 1.0  
**Last Updated:** 2024-01-21  
**Note:** This is a quick reference. See full guideline for detailed explanations.
