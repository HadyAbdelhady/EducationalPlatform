# Education Year API Documentation

This document provides comprehensive examples for using the Education Year CRUD API endpoints.

## Table of Contents
- [Get All Education Years](#get-all-education-years)
- [Get Education Year by ID](#get-education-year-by-id)
- [Create Education Year](#create-education-year)
- [Update Education Year](#update-education-year)
- [Delete Education Year](#delete-education-year)
- [Error Responses](#error-responses)
- [Postman Collection](#postman-collection)

---

## Get All Education Years

**Endpoint:** `GET /api/EducationYear`

**Headers:**
```
Content-Type: application/json
```

**Success Response (200 OK):**
```json
{
  "isSuccess": true,
  "data": [
    {
      "id": "a7f2c8e4-1234-5678-9abc-def012345678",
      "educationYearName": "Grade 10"
    },
    {
      "id": "b8e3d9f5-5678-1234-abcd-ef0123456789",
      "educationYearName": "Grade 11"
    },
    {
      "id": "c9e4a0g6-6789-2345-bcde-f01234567890",
      "educationYearName": "Grade 12"
    }
  ]
}
```

---

## Get Education Year by ID

**Endpoint:** `GET /api/EducationYear/{id}`

**Headers:**
```
Content-Type: application/json
```

**Success Response (200 OK):**
```json
{
  "isSuccess": true,
  "data": {
    "id": "a7f2c8e4-1234-5678-9abc-def012345678",
    "educationYearName": "Grade 10",
    "createdAt": "2025-10-04T11:29:51Z",
    "updatedAt": null
  }
}
```

**Error Response (404 Not Found):**
```json
{
  "isSuccess": false,
  "error": "Education year not found."
}
```

---

## Create Education Year

**Endpoint:** `POST /api/EducationYear`

**Headers:**
```
Content-Type: application/json
```

**Request Body:**
```json
{
  "educationYearName": "Grade 9"
}
```

**Success Response (200 OK):**
```json
{
  "isSuccess": true,
  "data": {
    "id": "d0f5b1h7-7890-3456-cdef-012345678901",
    "educationYearName": "Grade 9",
    "createdAt": "2025-10-04T12:15:30Z",
    "updatedAt": null
  }
}
```

**Error Responses:**

**400 Bad Request (Validation Error):**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "EducationYear.EducationYearName": [
      "Education year name is required."
    ]
  }
}
```

**409 Conflict (Duplicate Name):**
```json
{
  "isSuccess": false,
  "error": "An education year with this name already exists."
}
```

---

## Update Education Year

**Endpoint:** `PUT /api/EducationYear/{id}`

**Headers:**
```
Content-Type: application/json
```

**Request Body:**
```json
{
  "educationYearName": "Grade 9 - Updated"
}
```

**Success Response (200 OK):**
```json
{
  "isSuccess": true,
  "data": {
    "id": "d0f5b1h7-7890-3456-cdef-012345678901",
    "educationYearName": "Grade 9 - Updated",
    "createdAt": "2025-10-04T12:15:30Z",
    "updatedAt": "2025-10-04T13:20:45Z"
  }
}
```

**Error Responses:**

**404 Not Found:**
```json
{
  "isSuccess": false,
  "error": "Education year not found."
}
```

**409 Conflict (Duplicate Name):**
```json
{
  "isSuccess": false,
  "error": "Another education year with this name already exists."
}
```

---

## Delete Education Year

**Endpoint:** `DELETE /api/EducationYear/{id}`

**Headers:**
```
Content-Type: application/json
```

**Success Response (200 OK):**
```json
{
  "isSuccess": true,
  "data": true
}
```

**Error Responses:**

**404 Not Found:**
```json
{
  "isSuccess": false,
  "error": "Education year not found."
}
```

**409 Conflict (Has Dependencies):**
```json
{
  "isSuccess": false,
  "error": "Cannot delete education year. It has associated students or courses."
}
```

---

## Error Responses

### Common Error Types

**400 Bad Request:** Validation errors in request body
**404 Not Found:** Education year not found
**409 Conflict:** Duplicate name or dependency conflicts

### Error Response Format
```json
{
  "isSuccess": false,
  "error": "Error message description"
}
```

---

## Postman Collection

```json
{
  "info": {
    "name": "Educational Platform - Education Years",
    "schema": "https://schema.getpostman.com/json/collection/v2.1.0/collection.json"
  },
  "item": [
    {
      "name": "Get All Education Years",
      "request": {
        "method": "GET",
        "header": [
          {
            "key": "Content-Type",
            "value": "application/json"
          }
        ],
        "url": {
          "raw": "{{BASE_URL}}/api/EducationYear",
          "host": ["{{BASE_URL}}"],
          "path": ["api", "EducationYear"]
        }
      }
    },
    {
      "name": "Get Education Year by ID",
      "request": {
        "method": "GET",
        "header": [
          {
            "key": "Content-Type",
            "value": "application/json"
          }
        ],
        "url": {
          "raw": "{{BASE_URL}}/api/EducationYear/{{EDUCATION_YEAR_ID}}",
          "host": ["{{BASE_URL}}"],
          "path": ["api", "EducationYear", "{{EDUCATION_YEAR_ID}}"]
        }
      }
    },
    {
      "name": "Create Education Year",
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
          "raw": "{\n  \"educationYearName\": \"Grade 8\"\n}"
        },
        "url": {
          "raw": "{{BASE_URL}}/api/EducationYear",
          "host": ["{{BASE_URL}}"],
          "path": ["api", "EducationYear"]
        }
      }
    },
    {
      "name": "Update Education Year",
      "request": {
        "method": "PUT",
        "header": [
          {
            "key": "Content-Type",
            "value": "application/json"
          }
        ],
        "body": {
          "mode": "raw",
          "raw": "{\n  \"educationYearName\": \"Grade 8 - Updated\"\n}"
        },
        "url": {
          "raw": "{{BASE_URL}}/api/EducationYear/{{EDUCATION_YEAR_ID}}",
          "host": ["{{BASE_URL}}"],
          "path": ["api", "EducationYear", "{{EDUCATION_YEAR_ID}}"]
        }
      }
    },
    {
      "name": "Delete Education Year",
      "request": {
        "method": "DELETE",
        "header": [
          {
            "key": "Content-Type",
            "value": "application/json"
          }
        ],
        "url": {
          "raw": "{{BASE_URL}}/api/EducationYear/{{EDUCATION_YEAR_ID}}",
          "host": ["{{BASE_URL}}"],
          "path": ["api", "EducationYear", "{{EDUCATION_YEAR_ID}}"]
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
      "key": "EDUCATION_YEAR_ID",
      "value": ""
    }
  ]
}
```

---

## cURL Examples

### Get All Education Years
```bash
curl -X GET "http://localhost:5000/api/EducationYear" \
  -H "Content-Type: application/json"
```

### Get Education Year by ID
```bash
curl -X GET "http://localhost:5000/api/EducationYear/a7f2c8e4-1234-5678-9abc-def012345678" \
  -H "Content-Type: application/json"
```

### Create Education Year
```bash
curl -X POST "http://localhost:5000/api/EducationYear" \
  -H "Content-Type: application/json" \
  -d '{
    "educationYearName": "Grade 9"
  }'
```

### Update Education Year
```bash
curl -X PUT "http://localhost:5000/api/EducationYear/a7f2c8e4-1234-5678-9abc-def012345678" \
  -H "Content-Type: application/json" \
  -d '{
    "educationYearName": "Grade 9 - Updated"
  }'
```

### Delete Education Year
```bash
curl -X DELETE "http://localhost:5000/api/EducationYear/a7f2c8e4-1234-5678-9abc-def012345678" \
  -H "Content-Type: application/json"
```

---

## Client-Side Integration Examples

### JavaScript/TypeScript

```typescript
// Education Year Service
class EducationYearService {
  private baseUrl = 'http://localhost:5000/api/EducationYear';

  async getAllEducationYears(): Promise<EducationYearDto[]> {
    const response = await fetch(`${this.baseUrl}`);
    const result = await response.json();
    return result.data;
  }

  async getEducationYearById(id: string): Promise<EducationYearResponse> {
    const response = await fetch(`${this.baseUrl}/${id}`);
    const result = await response.json();
    return result.data;
  }

  async createEducationYear(educationYear: CreateEducationYearRequest): Promise<EducationYearResponse> {
    const response = await fetch(`${this.baseUrl}`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(educationYear)
    });
    const result = await response.json();
    return result.data;
  }

  async updateEducationYear(id: string, educationYear: UpdateEducationYearRequest): Promise<EducationYearResponse> {
    const response = await fetch(`${this.baseUrl}/${id}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(educationYear)
    });
    const result = await response.json();
    return result.data;
  }

  async deleteEducationYear(id: string): Promise<boolean> {
    const response = await fetch(`${this.baseUrl}/${id}`, {
      method: 'DELETE',
      headers: { 'Content-Type': 'application/json' }
    });
    const result = await response.json();
    return result.data;
  }
}

// Type definitions
interface EducationYearDto {
  id: string;
  educationYearName: string;
}

interface EducationYearResponse extends EducationYearDto {
  createdAt: string;
  updatedAt?: string;
}

interface CreateEducationYearRequest {
  educationYearName: string;
}

interface UpdateEducationYearRequest {
  educationYearName: string;
}
```

---

## Validation Rules

### Education Year Name
- **Required:** Yes
- **Min Length:** 2 characters
- **Max Length:** 100 characters
- **Allowed Characters:** Letters, numbers, spaces, and hyphens
- **Case Insensitive:** Duplicate names are not allowed

### Business Rules
- Education years can only be soft deleted (not permanently removed)
- Cannot delete education years with associated students or courses
- All operations respect soft delete pattern
- CreatedAt and UpdatedAt timestamps are automatically managed

---

## Best Practices

1. **Error Handling:** Always check the `isSuccess` property in responses
2. **Validation:** Client-side validation should match server-side rules
3. **Concurrency:** Use optimistic locking for updates in high-concurrency scenarios
4. **Caching:** Consider caching GET requests for better performance
5. **Security:** Implement proper authorization for write operations
6. **Testing:** Use the provided Postman collection for API testing
