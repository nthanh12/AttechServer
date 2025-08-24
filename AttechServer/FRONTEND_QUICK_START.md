# 🚀 Frontend Quick Start - Dashboard API

## 📋 TL;DR - Cần làm gì?

### **Bước 1: Hiểu API endpoints**
### **Bước 2: Test API với Postman/Thunder Client**  
### **Bước 3: Code frontend theo examples**
### **Bước 4: Implement từng phần một**

---

## 🎯 1. API Endpoints bạn cần biết

### **🏠 Tổng quan Dashboard** 
```bash
GET /api/dashboard/comprehensive
```
**→ Trả về TẤT CẢ dữ liệu dashboard trong 1 request**

### **⚡ Dữ liệu thời gian thực**
```bash
GET /api/dashboard/realtime  
```
**→ Dữ liệu live cập nhật liên tục**

### **📞 Thống kê liên hệ (MỚI)**
```bash
GET /api/dashboard/contacts
```
**→ Thống kê liên hệ từ khách hàng**

### **📊 Charts**
```bash
GET /api/dashboard/charts/usergrowth?days=7      # User growth
GET /api/dashboard/charts/contacttrend?days=30   # Contact trend  
GET /api/dashboard/charts/contentdistribution    # Content distribution
```

---

## 🧪 2. Test API trước khi code

### **A. Test với Postman**
```bash
# 1. Test comprehensive endpoint
GET http://localhost:7276/api/dashboard/comprehensive
Headers: Authorization: Bearer YOUR_JWT_TOKEN

# 2. Test contact stats  
GET http://localhost:7276/api/dashboard/contacts
Headers: Authorization: Bearer YOUR_JWT_TOKEN

# 3. Test realtime
GET http://localhost:7276/api/dashboard/realtime
Headers: Authorization: Bearer YOUR_JWT_TOKEN
```

### **B. Response bạn sẽ nhận được**

**Comprehensive API response:**
```json
{
  "status": 1,
  "data": {
    "overview": {
      "totalUsers": 1245,
      "totalProducts": 89,
      "totalServices": 34, 
      "totalNews": 167,
      "totalNotifications": 23,
      "totalContacts": 442,        // 🆕 NEW - Contact count
      "activeUsers": 89
    },
    "userStats": { /* user statistics */ },
    "contentStats": { /* content statistics */ },
    "contactStats": {              // 🆕 NEW - Contact data
      "totalContacts": 442,
      "unreadContacts": 23,
      "readContacts": 419,
      "responseRate": 94.8,
      "trends": { "growthPercentage": 28.8 }
    },
    "recentActivities": [ /* activities */ ]
  },
  "code": 200,
  "message": "Comprehensive dashboard data retrieved successfully"
}
```

---

## 💻 3. Code Frontend Implementation

### **A. Fetch Dashboard Data (React example)**

```jsx
// hooks/useDashboard.js
import { useState, useEffect } from 'react';

export const useDashboard = () => {
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const fetchDashboard = async () => {
    try {
      setLoading(true);
      
      const response = await fetch('/api/dashboard/comprehensive', {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`,
          'Content-Type': 'application/json'
        }
      });
      
      const result = await response.json();
      
      if (result.status === 1) { // Success
        setData(result.data);
        setError(null);
      } else {
        throw new Error(result.message);
      }
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchDashboard();
  }, []);

  return { data, loading, error, refresh: fetchDashboard };
};
```

### **B. Dashboard Component**

```jsx
// components/Dashboard.jsx
import React from 'react';
import { useDashboard } from '../hooks/useDashboard';

const Dashboard = () => {
  const { data, loading, error } = useDashboard();

  if (loading) return <div>Loading dashboard...</div>;
  if (error) return <div>Error: {error}</div>;
  if (!data) return <div>No data available</div>;

  const { overview, userStats, contentStats, contactStats, recentActivities } = data;

  return (
    <div className="dashboard">
      <h1>📊 Dashboard</h1>
      
      {/* Overview Cards */}
      <div className="overview-grid">
        <StatCard title="👥 Users" value={overview.totalUsers} />
        <StatCard title="📦 Products" value={overview.totalProducts} />  
        <StatCard title="⚙️ Services" value={overview.totalServices} />
        <StatCard title="📰 News" value={overview.totalNews} />
        <StatCard title="🔔 Notifications" value={overview.totalNotifications} />
        {/* 🆕 NEW - Contact card */}
        <StatCard 
          title="📞 Contacts" 
          value={overview.totalContacts}
          subtitle={`${contactStats.unreadContacts} unread`}
          urgent={contactStats.unreadContacts > 0}
        />
      </div>

      {/* Contact Statistics Section */}
      <ContactStatsSection data={contactStats} />
      
      {/* Other sections */}
      <UserStatsSection data={userStats} />
      <ContentStatsSection data={contentStats} />
      <RecentActivities data={recentActivities} />
    </div>
  );
};

const StatCard = ({ title, value, subtitle, urgent }) => (
  <div className={`stat-card ${urgent ? 'urgent' : ''}`}>
    <h3>{title}</h3>
    <div className="value">{value.toLocaleString()}</div>
    {subtitle && <div className="subtitle">{subtitle}</div>}
  </div>
);
```

### **C. Contact Statistics Component (MỚI)**

```jsx
// components/ContactStatsSection.jsx
const ContactStatsSection = ({ data }) => {
  const { 
    totalContacts, 
    unreadContacts, 
    readContacts, 
    responseRate, 
    trends 
  } = data;

  return (
    <div className="contact-stats-section">
      <h2>📞 Thống kê liên hệ khách hàng</h2>
      
      <div className="stats-grid">
        <div className="stat-item">
          <span className="label">Tổng liên hệ</span>
          <span className="value">{totalContacts}</span>
        </div>
        
        <div className="stat-item urgent">
          <span className="label">Chưa đọc</span>
          <span className="value">{unreadContacts}</span>
          {unreadContacts > 0 && <span className="alert">!</span>}
        </div>
        
        <div className="stat-item">
          <span className="label">Đã xử lý</span>
          <span className="value">{readContacts}</span>
        </div>
        
        <div className="stat-item">
          <span className="label">Tỷ lệ phản hồi</span>
          <span className="value">{responseRate}%</span>
        </div>
      </div>

      {/* Growth trend */}
      <div className="trend-indicator">
        <span>Xu hướng tháng này: </span>
        <span className={`trend ${trends.trendDirection}`}>
          {trends.trendDirection === 'up' ? '📈' : 
           trends.trendDirection === 'down' ? '📉' : '➡️'}
          {trends.growthPercentage}%
        </span>
      </div>
    </div>
  );
};
```

### **D. Real-time Updates**

```jsx
// hooks/useRealtime.js
import { useState, useEffect } from 'react';

export const useRealtime = () => {
  const [realtimeData, setRealtimeData] = useState(null);

  useEffect(() => {
    const fetchRealtime = async () => {
      try {
        const response = await fetch('/api/dashboard/realtime', {
          headers: { 'Authorization': `Bearer ${localStorage.getItem('token')}` }
        });
        const result = await response.json();
        if (result.status === 1) {
          setRealtimeData(result.data);
        }
      } catch (error) {
        console.error('Realtime fetch error:', error);
      }
    };

    // Fetch immediately
    fetchRealtime();
    
    // Then fetch every 30 seconds
    const interval = setInterval(fetchRealtime, 30000);
    
    return () => clearInterval(interval);
  }, []);

  return realtimeData;
};

// Usage in component
const RealtimePanel = () => {
  const realtime = useRealtime();
  
  if (!realtime) return null;

  return (
    <div className="realtime-panel">
      <h3>⚡ Live Data</h3>
      <div>👥 Online: {realtime.currentActiveUsers}</div>
      <div>🧠 Memory: {realtime.memoryUsage}GB</div>
      <div>💾 Storage: {realtime.storageUsed}MB</div>
      <div>🚨 Alerts: {realtime.activeAlerts.length}</div>
    </div>
  );
};
```

---

## 🎨 4. CSS Styling Guide

```css
/* Dashboard Grid Layout */
.overview-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 1rem;
  margin-bottom: 2rem;
}

.stat-card {
  background: white;
  padding: 1.5rem;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
  text-align: center;
}

.stat-card.urgent {
  border-left: 4px solid #ef4444;
  background: #fef2f2;
}

.stat-card h3 {
  margin: 0 0 1rem 0;
  color: #6b7280;
  font-size: 0.9rem;
}

.stat-card .value {
  font-size: 2rem;
  font-weight: bold;
  color: #111827;
}

.stat-card .subtitle {
  font-size: 0.8rem;
  color: #6b7280;
  margin-top: 0.5rem;
}

/* Contact Stats */
.contact-stats-section {
  background: white;
  padding: 1.5rem;
  border-radius: 8px;
  margin-bottom: 2rem;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
  gap: 1rem;
  margin: 1rem 0;
}

.stat-item {
  text-align: center;
  padding: 1rem;
  border-radius: 4px;
  background: #f9fafb;
}

.stat-item.urgent {
  background: #fef2f2;
  border: 1px solid #fecaca;
}

.trend {
  font-weight: bold;
}

.trend.up { color: #10b981; }
.trend.down { color: #ef4444; }
.trend.stable { color: #6b7280; }

/* Realtime Panel */
.realtime-panel {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 1.5rem;
  border-radius: 8px;
}
```

---

## 🔧 5. Error Handling

```jsx
// utils/apiErrorHandler.js
export const handleApiError = (response, result) => {
  if (result.status !== 1) {
    switch (result.code) {
      case 401:
        // Redirect to login
        window.location.href = '/login';
        break;
      case 403:
        alert('Bạn không có quyền truy cập');
        break;
      case 500:
        alert('Lỗi server, vui lòng thử lại');
        break;
      default:
        alert(result.message || 'Có lỗi xảy ra');
    }
  }
};
```

---

## 📱 6. Mobile Responsive

```jsx
// components/MobileDashboard.jsx
const MobileDashboard = () => {
  const [activeTab, setActiveTab] = useState('overview');
  const { data } = useDashboard();

  const tabs = [
    { id: 'overview', label: '📊 Tổng quan', icon: '📊' },
    { id: 'contacts', label: '📞 Liên hệ', icon: '📞' },
    { id: 'users', label: '👥 Users', icon: '👥' },
    { id: 'system', label: '🖥️ System', icon: '🖥️' }
  ];

  return (
    <div className="mobile-dashboard">
      {/* Tab Navigation */}
      <div className="tab-nav">
        {tabs.map(tab => (
          <button 
            key={tab.id}
            className={`tab-button ${activeTab === tab.id ? 'active' : ''}`}
            onClick={() => setActiveTab(tab.id)}
          >
            {tab.icon} {tab.label}
          </button>
        ))}
      </div>

      {/* Tab Content */}
      <div className="tab-content">
        {activeTab === 'overview' && <OverviewTab data={data?.overview} />}
        {activeTab === 'contacts' && <ContactsTab data={data?.contactStats} />}
        {activeTab === 'users' && <UsersTab data={data?.userStats} />}
        {activeTab === 'system' && <SystemTab />}
      </div>
    </div>
  );
};
```

---

## ✅ 7. Checklist cho Frontend

### **Phase 1: Basic Setup**
- [ ] Test API endpoints với Postman
- [ ] Setup base components (Dashboard, StatCard)  
- [ ] Implement useDashboard hook
- [ ] Display overview cards (including contacts)
- [ ] Add loading và error states

### **Phase 2: Contact Features** 
- [ ] Implement ContactStatsSection component
- [ ] Add contact trend indicators
- [ ] Handle unread contact alerts
- [ ] Test contact chart endpoint

### **Phase 3: Advanced Features**
- [ ] Add real-time updates (useRealtime hook)
- [ ] Implement chart components  
- [ ] Add mobile responsive design
- [ ] Performance optimization

### **Phase 4: Polish**
- [ ] Add animations và transitions
- [ ] Implement error boundaries
- [ ] Add accessibility features
- [ ] Final testing và debugging

---

## 🆘 Need Help?

### **Common Issues:**
1. **401 Unauthorized** → Check JWT token trong localStorage
2. **CORS Error** → Kiểm tra backend CORS config
3. **Empty data** → Check API response structure
4. **Chart not showing** → Verify chart library installation

### **Debug Tips:**
```javascript
// Log API responses để debug
console.log('Dashboard data:', data);
console.log('Contact stats:', data?.contactStats);
console.log('Realtime data:', realtimeData);
```

**Dashboard API đã sẵn sàng cho Frontend implement! Bắt đầu với Phase 1 và test từng phần một.** 🚀