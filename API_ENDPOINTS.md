# AttechServer API Endpoints Documentation

## 🔧 Backend Configuration

**Base URL**: `https://localhost:7276`  
**HTTP URL**: `http://localhost:5232`  
**Swagger UI**: `https://localhost:7276/swagger/index.html`

## 🔐 Authentication

### Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "your_password"
}

Response:
{
  "status": 1,
  "data": {
    "token": "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9..."
  },
  "code": 200,
  "message": "Ok"
}
```

### Register
```http
POST /api/auth/register
Content-Type: application/json
```

## 📰 News Management

### Get All News (Public)
```http
GET /api/news/find-all?pageIndex=1&pageSize=10
Response: {"status":1,"data":{"items":[],"totalItems":0},"code":200,"message":"Ok"}
```

### Get News by Category (Public)
```http
GET /api/news/category/{slug}?pageIndex=1&pageSize=10
```

### Get News Detail (Public)
```http
GET /api/news/find-by-id/{id}
GET /api/news/detail/{slug}
```

### Create News (Auth Required)
```http
POST /api/news/create
Authorization: Bearer {token}
Content-Type: application/json
```

### Update News (Auth Required)
```http
PUT /api/news/update
Authorization: Bearer {token}
```

### Delete News (Auth Required)
```http
DELETE /api/news/delete/{id}
Authorization: Bearer {token}
```

### Update News Status (Auth Required)
```http
PUT /api/news/update-post-status
Authorization: Bearer {token}
Content-Type: application/json

{
  "id": 1,
  "status": 1
}
```

## 📂 News Categories

### Get All News Categories (Public)
```http
GET /api/news-categories/find-all?pageIndex=1&pageSize=10
Response: {"status":1,"data":{"items":[],"totalItems":0},"code":200,"message":"Ok"}
```

### Get News Category Detail (Public)
```http
GET /api/news-categories/find-by-id/{id}
GET /api/news-categories/detail/{slug}
```

### Create News Category (Auth Required)
```http
POST /api/news-categories/create
Authorization: Bearer {token}
```

### Update News Category (Auth Required)
```http
PUT /api/news-categories/update
Authorization: Bearer {token}
```

### Delete News Category (Auth Required)
```http
DELETE /api/news-categories/delete/{id}
Authorization: Bearer {token}
```

## 🛍️ Products

### Get All Products (Public)
```http
GET /api/product/find-all?pageIndex=1&pageSize=10
Response: {"status":1,"data":{"items":[],"totalItems":0},"code":200,"message":"Ok"}
```

### Get Products by Category (Public)
```http
GET /api/product/category/{slug}?pageIndex=1&pageSize=10
```

### Get Product Detail (Public)  
```http
GET /api/product/find-by-id/{id}
GET /api/product/detail/{slug}
```

### Create Product (Auth Required)
```http
POST /api/product/create
Authorization: Bearer {token}
```

### Update Product (Auth Required)
```http
PUT /api/product/update
Authorization: Bearer {token}
```

### Delete Product (Auth Required)
```http
DELETE /api/product/delete/{id}
Authorization: Bearer {token}
```

### Update Product Status (Auth Required)
```http
PUT /api/product/update-status
Authorization: Bearer {token}
```

## 📂 Product Categories

### Get All Product Categories (Public)
```http
GET /api/product-categories/find-all?pageIndex=1&pageSize=10
Response: {"status":1,"data":{"items":[],"totalItems":0},"code":200,"message":"Ok"}
```

### Get Product Category Detail (Public)
```http
GET /api/product-categories/find-by-id/{id}
GET /api/product-categories/detail/{slug}
```

## 🔔 Notifications

### Get All Notifications (Public)
```http
GET /api/notification/find-all?pageIndex=1&pageSize=10
Response: {"status":1,"data":{"items":[],"totalItems":0},"code":200,"message":"Ok"}
```

### Get Notifications by Category (Public)
```http
GET /api/notification/category/{slug}?pageIndex=1&pageSize=10
```

## 📂 Notification Categories

### Get All Notification Categories (Public)
```http
GET /api/notification-categories/find-all?pageIndex=1&pageSize=10
Response: {"status":1,"data":{"items":[],"totalItems":0},"code":200,"message":"Ok"}
```

## 🛠️ Services

### Get All Services (Public)
```http
GET /api/service/find-all?pageIndex=1&pageSize=10
Response: {"status":1,"data":{"items":[],"totalItems":0},"code":200,"message":"Ok"}
```

### Get Service Detail (Public)
```http
GET /api/service/find-by-id/{id}
GET /api/service/detail/{slug}
```

## 📁 File Upload (TinyMCE Ready)

### Upload Image (Auth Required)
```http
POST /api/upload/image
Authorization: Bearer {token}
Content-Type: multipart/form-data

Form Data:
- file: [image file]
- entityType: (optional)
- entityId: (optional)

Response:
{
  "status": 1,
  "data": {
    "location": "https://localhost:7276/api/upload/file/images/2025/07/23/filename.jpg"
  },
  "code": 200,
  "message": "Ok"
}
```

### Upload Multiple Files (Auth Required)
```http
POST /api/upload/multi-upload
Authorization: Bearer {token}
Content-Type: multipart/form-data

Form Data:
- files: [multiple files]
- entityType: (optional)
- entityId: (optional)

Response:
{
  "status": 1,
  "data": {
    "locations": [
      "https://localhost:7276/api/upload/file/images/2025/07/23/file1.jpg",
      "https://localhost:7276/api/upload/file/images/2025/07/23/file2.jpg"
    ]
  },
  "code": 200,
  "message": "Ok"
}
```

### Upload Video (Auth Required)
```http
POST /api/upload/video
Authorization: Bearer {token}
```

### Upload Document (Auth Required)
```http
POST /api/upload/document
Authorization: Bearer {token}
```

### Access Files (Public)
```http
GET /api/upload/file/{subFolder}/{year}/{month}/{day}/{fileName}
GET /api/upload/file/{subFolder}/{fileName}
```

## 📋 Media Management

### Get All Media Files (Auth Required)
```http
GET /api/media/find-all?pageIndex=1&pageSize=10
Authorization: Bearer {token}
```

### Get Media by Entity (Auth Required)
```http
GET /api/media/find-by-entity/{entityType}/{entityId}?pageIndex=1&pageSize=10
Authorization: Bearer {token}
```

### Delete Media (Auth Required)
```http
DELETE /api/media/delete/{id}
Authorization: Bearer {token}
```

## 🔧 Frontend Configuration

### React/Vite Proxy Setup
```javascript
// vite.config.js
export default defineConfig({
  server: {
    proxy: {
      '/api': {
        target: 'https://localhost:7276',
        changeOrigin: true,
        secure: false
      }
    }
  }
})
```

### Axios Configuration
```javascript
// api.js
import axios from 'axios';

const API_BASE_URL = 'https://localhost:7276';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Add token to requests
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export default api;
```

### TinyMCE Upload Configuration  
```javascript
tinymce.init({
  images_upload_url: '/api/upload/image',
  images_upload_handler: function (blobInfo, success, failure) {
    const formData = new FormData();
    formData.append('file', blobInfo.blob(), blobInfo.filename());
    
    fetch('/api/upload/image', {
      method: 'POST',
      headers: { 
        'Authorization': 'Bearer ' + localStorage.getItem('token')
      },
      body: formData
    })
    .then(response => response.json())
    .then(result => {
      if (result.status === 1) {
        success(result.data.location);
      } else {
        failure(result.message);
      }
    })
    .catch(error => failure(error.message));
  }
});
```

## 🌐 CORS Configuration

Backend đã cấu hình CORS cho phép tất cả origins:
```csharp
app.UseCors("AllowAllOrigins");
```

## 📊 Response Format

Tất cả API response theo format chuẩn:
```json
{
  "status": 1,          // 1 = success, 0 = error
  "data": {...},        // Response data
  "code": 200,          // HTTP status code  
  "message": "Ok"       // Status message
}
```

## ⚡ Performance Features

- ✅ Response Caching (public endpoints)
- ✅ Request Timing Headers
- ✅ Global Exception Handling
- ✅ Security Headers
- ✅ File Upload Validation

## 🔒 Security Features

- ✅ JWT Authentication  
- ✅ Permission-based Authorization
- ✅ File Upload Security
- ✅ SQL Injection Protection
- ✅ XSS Protection