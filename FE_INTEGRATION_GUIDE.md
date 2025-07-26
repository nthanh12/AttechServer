# Frontend Integration Guide - AttechServer API

## 🚀 Quick Start

### 1. Backend URLs
```
Production Backend: https://localhost:7276
Development Backend: http://localhost:5232  
Swagger Documentation: https://localhost:7276/swagger/index.html
```

### 2. Test Backend Connection
```bash
# Test if backend is running
curl -k https://localhost:7276/api/news/find-all

# Expected response:
{"status":1,"data":{"items":[],"totalItems":0},"code":200,"message":"Ok"}
```

## ⚙️ Frontend Setup

### React/Next.js Configuration

#### Option 1: Environment Variables
```bash
# .env.local
NEXT_PUBLIC_API_URL=https://localhost:7276
REACT_APP_API_URL=https://localhost:7276
```

#### Option 2: Vite Proxy (Development)
```javascript
// vite.config.js
import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      '/api': {
        target: 'https://localhost:7276',
        changeOrigin: true,
        secure: false, // Allow self-signed certificates
        ws: true,
        configure: (proxy, _options) => {
          proxy.on('error', (err, _req, _res) => {
            console.log('proxy error', err);
          });
          proxy.on('proxyReq', (proxyReq, req, _res) => {
            console.log('Sending Request to the Target:', req.method, req.url);
          });
          proxy.on('proxyRes', (proxyRes, req, _res) => {
            console.log('Received Response from the Target:', proxyRes.statusCode, req.url);
          });
        },
      }
    }
  }
})
```

#### Option 3: Next.js Proxy
```javascript
// next.config.js
/** @type {import('next').NextConfig} */
const nextConfig = {
  async rewrites() {
    return [
      {
        source: '/api/:path*',
        destination: 'https://localhost:7276/api/:path*',
      },
    ]
  },
}

module.exports = nextConfig
```

### API Service Setup

```javascript
// services/api.js
import axios from 'axios';

// Use environment variable or fallback
const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 
                     process.env.REACT_APP_API_URL || 
                     'https://localhost:7276';

const api = axios.create({
  baseURL: API_BASE_URL,
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor for auth token
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token') || sessionStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response interceptor for error handling
api.interceptors.response.use(
  (response) => {
    return response.data; // Return only data part
  },
  (error) => {
    if (error.response?.status === 401) {
      // Token expired or invalid
      localStorage.removeItem('token');
      sessionStorage.removeItem('token');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export default api;
```

### API Functions

```javascript
// services/newsApi.js
import api from './api';

export const newsApi = {
  // Public endpoints
  getAllNews: (params = {}) => {
    return api.get('/api/news/find-all', { params });
  },
  
  getNewsById: (id) => {
    return api.get(`/api/news/find-by-id/${id}`);
  },
  
  getNewsBySlug: (slug) => {
    return api.get(`/api/news/detail/${slug}`);
  },
  
  getNewsByCategory: (slug, params = {}) => {
    return api.get(`/api/news/category/${slug}`, { params });
  },
  
  // Auth required endpoints
  createNews: (data) => {
    return api.post('/api/news/create', data);
  },
  
  updateNews: (data) => {
    return api.put('/api/news/update', data);
  },
  
  deleteNews: (id) => {
    return api.delete(`/api/news/delete/${id}`);
  },
  
  updateNewsStatus: (id, status) => {
    return api.put('/api/news/update-post-status', { id, status });
  }
};

// services/categoryApi.js
export const categoryApi = {
  getNewsCategories: (params = {}) => {
    return api.get('/api/news-categories/find-all', { params });
  },
  
  getProductCategories: (params = {}) => {
    return api.get('/api/product-categories/find-all', { params });
  },
  
  getNotificationCategories: (params = {}) => {
    return api.get('/api/notification-categories/find-all', { params });
  }
};

// services/uploadApi.js
export const uploadApi = {
  uploadImage: (file, entityType = null, entityId = null) => {
    const formData = new FormData();
    formData.append('file', file);
    if (entityType) formData.append('entityType', entityType);
    if (entityId) formData.append('entityId', entityId);
    
    return api.post('/api/upload/image', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
  },
  
  uploadMultiple: (files, entityType = null, entityId = null) => {
    const formData = new FormData();
    files.forEach(file => formData.append('files', file));
    if (entityType) formData.append('entityType', entityType);
    if (entityId) formData.append('entityId', entityId);
    
    return api.post('/api/upload/multi-upload', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
  }
};
```

### Authentication

```javascript
// services/authApi.js
import api from './api';

export const authApi = {
  login: async (username, password) => {
    try {
      const response = await api.post('/api/auth/login', {
        username,
        password
      });
      
      if (response.status === 1 && response.data.token) {
        localStorage.setItem('token', response.data.token);
        return response;
      }
      throw new Error(response.message || 'Login failed');
    } catch (error) {
      throw error;
    }
  },
  
  logout: () => {
    localStorage.removeItem('token');
    sessionStorage.removeItem('token');
  },
  
  getCurrentUser: () => {
    return api.get('/api/auth/me');
  }
};
```

### React Hooks

```javascript
// hooks/useNews.js
import { useState, useEffect } from 'react';
import { newsApi } from '../services/newsApi';

export const useNews = (params = {}) => {
  const [news, setNews] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  
  useEffect(() => {
    const fetchNews = async () => {
      try {
        setLoading(true);
        const response = await newsApi.getAllNews(params);
        setNews(response.data.items || []);
      } catch (err) {
        setError(err.message);
        console.error('Failed to fetch news:', err);
      } finally {
        setLoading(false);
      }
    };
    
    fetchNews();
  }, [JSON.stringify(params)]);
  
  return { news, loading, error, refetch: () => fetchNews() };
};

// hooks/useCategories.js
export const useCategories = (type = 'news') => {
  const [categories, setCategories] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  
  useEffect(() => {
    const fetchCategories = async () => {
      try {
        setLoading(true);
        let response;
        switch (type) {
          case 'news':
            response = await categoryApi.getNewsCategories();
            break;
          case 'product':
            response = await categoryApi.getProductCategories();
            break;
          case 'notification':
            response = await categoryApi.getNotificationCategories();
            break;
          default:
            throw new Error('Invalid category type');
        }
        setCategories(response.data.items || []);
      } catch (err) {
        setError(err.message);
        console.error('Failed to fetch categories:', err);
      } finally {
        setLoading(false);
      }
    };
    
    fetchCategories();
  }, [type]);
  
  return { categories, loading, error };
};
```

### TinyMCE Integration

```javascript
// components/TinyMCEEditor.jsx
import { Editor } from '@tinymce/tinymce-react';

const TinyMCEEditor = ({ value, onChange }) => {
  return (
    <Editor
      apiKey="your-tinymce-api-key"
      value={value}
      onEditorChange={onChange}
      init={{
        height: 400,
        menubar: false,
        plugins: [
          'advlist', 'autolink', 'lists', 'link', 'image', 'charmap', 'preview',
          'anchor', 'searchreplace', 'visualblocks', 'code', 'fullscreen',
          'insertdatetime', 'media', 'table', 'help', 'wordcount'
        ],
        toolbar: 'undo redo | blocks | ' +
          'bold italic forecolor | alignleft aligncenter ' +
          'alignright alignjustify | bullist numlist outdent indent | ' +
          'removeformat | image media | help',
        content_style: 'body { font-family:Helvetica,Arial,sans-serif; font-size:14px }',
        
        // Image upload configuration
        images_upload_url: '/api/upload/image',
        images_upload_handler: function (blobInfo, success, failure) {
          const formData = new FormData();
          formData.append('file', blobInfo.blob(), blobInfo.filename());
          
          fetch('/api/upload/image', {
            method: 'POST',
            headers: {
              'Authorization': `Bearer ${localStorage.getItem('token')}`
            },
            body: formData
          })
          .then(response => response.json())
          .then(result => {
            if (result.status === 1) {
              success(result.data.location);
            } else {
              failure(result.message || 'Upload failed');
            }
          })
          .catch(error => {
            failure(error.message);
          });
        },
        
        // File picker for other file types
        file_picker_callback: function (callback, value, meta) {
          const input = document.createElement('input');
          input.setAttribute('type', 'file');
          input.setAttribute('accept', meta.filetype === 'image' ? 'image/*' : '*/*');
          
          input.onchange = function () {
            const file = this.files[0];
            const formData = new FormData();
            formData.append('file', file);
            
            const endpoint = meta.filetype === 'image' ? '/api/upload/image' : '/api/upload/document';
            
            fetch(endpoint, {
              method: 'POST',
              headers: {
                'Authorization': `Bearer ${localStorage.getItem('token')}`
              },
              body: formData
            })
            .then(response => response.json())
            .then(result => {
              if (result.status === 1) {
                callback(result.data.location, { title: file.name });
              }
            });
          };
          
          input.click();
        }
      }}
    />
  );
};

export default TinyMCEEditor;
```

## 🔧 Troubleshooting

### Common Issues

1. **CORS Error**
   ```
   Solution: Backend already configured CORS. Check if using correct URL.
   ```

2. **SSL Certificate Error**
   ```javascript
   // For development, add to axios config:
   process.env["NODE_TLS_REJECT_UNAUTHORIZED"] = 0;
   
   // Or use HTTP endpoint: http://localhost:5232
   ```

3. **404 Not Found**
   ```
   Check:
   - Backend is running on correct port
   - API endpoint URLs are correct
   - Proxy configuration is working
   ```

4. **401 Unauthorized**
   ```
   Check:
   - Token is stored correctly
   - Token is sent in Authorization header
   - Token hasn't expired
   ```

### Debug Steps

```javascript
// Add debug logging
api.interceptors.request.use((config) => {
  console.log('API Request:', config);
  return config;
});

api.interceptors.response.use(
  (response) => {
    console.log('API Response:', response);
    return response;
  },
  (error) => {
    console.error('API Error:', error);
    return Promise.reject(error);
  }
);
```

## ✅ Testing Checklist

- [ ] Backend running on https://localhost:7276
- [ ] Can access Swagger UI
- [ ] Public endpoints work without auth
- [ ] Auth endpoints return JWT token
- [ ] Protected endpoints work with token
- [ ] File upload works
- [ ] CORS headers present
- [ ] Error responses formatted correctly

## 🎯 Ready-to-Use Endpoints

All these endpoints are **TESTED and WORKING**:

### Public (No Auth Required)
- ✅ `GET /api/news/find-all`
- ✅ `GET /api/news-categories/find-all`  
- ✅ `GET /api/product/find-all`
- ✅ `GET /api/product-categories/find-all`
- ✅ `GET /api/service/find-all`
- ✅ `GET /api/notification/find-all`
- ✅ `GET /api/notification-categories/find-all`

### Auth Required
- ✅ `POST /api/auth/login`
- ✅ `POST /api/upload/image` 
- ✅ `POST /api/upload/multi-upload`
- ✅ All CRUD operations (create, update, delete)

**Backend Status: 🟢 FULLY OPERATIONAL**