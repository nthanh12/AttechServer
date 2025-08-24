# 📊 Dashboard API - Ví dụ thực tế và Test Cases

## 🎯 Sample API Responses

### 1. **GET /api/dashboard/comprehensive** - Complete Dashboard Data

**Request:**
```bash
curl -X GET "https://api.yoursite.com/api/dashboard/comprehensive" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json"
```

**Response:**
```json
{
  "success": true,
  "message": "Comprehensive dashboard data retrieved successfully",
  "data": {
    "overview": {
      "totalUsers": 1245,
      "totalProducts": 89,
      "totalServices": 34,
      "totalNews": 167,
      "totalNotifications": 23,
      "totalContacts": 442,
      "activeUsers": 89,
      "lastUpdated": "2024-12-20T10:30:00Z"
    },
    "userStats": {
      "newUsersToday": 5,
      "newUsersThisWeek": 23,
      "newUsersThisMonth": 78,
      "activeUsersToday": 12,
      "totalUsers": 1245,
      "usersByRole": [
        {
          "role": "Admin",
          "count": 3,
          "percentage": 0.2
        },
        {
          "role": "Editor",
          "count": 15,
          "percentage": 1.2
        },
        {
          "role": "User",
          "count": 1227,
          "percentage": 98.6
        }
      ],
      "growthTrend": {
        "monthlyGrowth": 78,
        "growthRate": 15.2,
        "trendDirection": "up"
      }
    },
    "contentStats": {
      "newsPublishedThisMonth": 34,
      "notificationsThisMonth": 8,
      "productsAddedThisMonth": 12,
      "servicesAddedThisMonth": 3,
      "categoryDistribution": [
        {
          "category": "Công nghệ",
          "count": 45,
          "type": "news",
          "percentage": 26.9
        },
        {
          "category": "Sản phẩm A",
          "count": 23,
          "type": "products", 
          "percentage": 25.8
        }
      ],
      "trends": {
        "totalContentThisMonth": 57,
        "totalContentLastMonth": 43,
        "growthPercentage": 32.6,
        "mostActiveCategory": "Công nghệ"
      }
    },
    "contactStats": {
      "totalContacts": 442,
      "unreadContacts": 23,
      "readContacts": 419,
      "contactsToday": 3,
      "contactsThisWeek": 18,
      "contactsThisMonth": 67,
      "responseRate": 94.8,
      "trends": {
        "totalThisMonth": 67,
        "totalLastMonth": 52,
        "growthPercentage": 28.8,
        "trendDirection": "up",
        "averagePerDay": 2.3
      }
    },
    "recentActivities": [
      {
        "id": 1,
        "type": "user_created",
        "message": "Người dùng mới đăng ký: john.doe@email.com",
        "timestamp": "2024-12-20T10:25:00Z",
        "severity": "Info",
        "icon": "bi-person-plus",
        "timeAgo": "5 phút trước"
      },
      {
        "id": 2,
        "type": "post_created",
        "message": "Bài viết mới được tạo: 'Xu hướng công nghệ 2024'",
        "timestamp": "2024-12-20T09:45:00Z",
        "severity": "Info", 
        "icon": "bi-newspaper",
        "timeAgo": "45 phút trước"
      }
    ]
  },
  "timestamp": "2024-12-20T10:30:00Z"
}
```

### 2. **GET /api/dashboard/contacts** - Contact Statistics

**Request:**
```bash
curl -X GET "https://api.yoursite.com/api/dashboard/contacts" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

**Response:**
```json
{
  "success": true,
  "message": "Contact statistics retrieved successfully",
  "data": {
    "totalContacts": 442,
    "unreadContacts": 23,
    "readContacts": 419,
    "contactsToday": 3,
    "contactsThisWeek": 18,
    "contactsThisMonth": 67,
    "responseRate": 94.8,
    "trends": {
      "totalThisMonth": 67,
      "totalLastMonth": 52,
      "growthPercentage": 28.8,
      "trendDirection": "up",
      "averagePerDay": 2.3
    },
    "contactSources": [
      {
        "source": "Website Form",
        "count": 387,
        "percentage": 87.6
      },
      {
        "source": "Email Direct",
        "count": 34,
        "percentage": 7.7
      },
      {
        "source": "Phone",
        "count": 21,
        "percentage": 4.7
      }
    ]
  },
  "timestamp": "2024-12-20T10:30:00Z"
}
```

### 3. **GET /api/dashboard/realtime** - Real-time Data

**Response:**
```json
{
  "success": true,
  "message": "Realtime data retrieved successfully",
  "data": {
    "currentActiveUsers": 12,
    "systemLoad": 2.45,
    "memoryUsage": 2.45,
    "storageUsed": 1247,
    "lastUpdated": "2024-12-20T10:30:15Z",
    "activeAlerts": [
      {
        "type": "storage",
        "message": "Storage usage above 80%",
        "severity": "warning",
        "createdAt": "2024-12-20T10:15:00Z"
      }
    ]
  },
  "timestamp": "2024-12-20T10:30:15Z"
}
```

### 4. **GET /api/dashboard/charts/contacttrend?days=7** - Contact Trend Chart

**Response:**
```json
{
  "success": true,
  "message": "Chart data for contacttrend retrieved successfully",
  "data": {
    "labels": ["14/12", "15/12", "16/12", "17/12", "18/12", "19/12", "20/12"],
    "datasets": [
      {
        "label": "Liên hệ mới",
        "data": [2, 5, 3, 8, 4, 6, 3],
        "borderColor": "#8b5cf6",
        "backgroundColor": "rgba(139, 92, 246, 0.1)",
        "tension": 0.4,
        "fill": true
      }
    ],
    "options": {
      "responsive": true,
      "maintainAspectRatio": false
    },
    "startDate": "2024-12-14T00:00:00Z",
    "endDate": "2024-12-20T00:00:00Z", 
    "totalContacts": 31,
    "averagePerDay": 4.4
  },
  "timestamp": "2024-12-20T10:30:00Z"
}
```

---

## 🧪 Frontend Integration Examples

### React Hook cho Dashboard Data

```javascript
// hooks/useDashboard.js
import { useState, useEffect, useCallback } from 'react';

export const useDashboard = () => {
  const [data, setData] = useState(null);
  const [realtime, setRealtime] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const fetchDashboard = useCallback(async () => {
    try {
      setLoading(true);
      
      const response = await fetch('/api/dashboard/comprehensive', {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`,
          'Content-Type': 'application/json'
        }
      });
      
      const result = await response.json();
      
      if (result.success) {
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
  }, []);

  const fetchRealtime = useCallback(async () => {
    try {
      const response = await fetch('/api/dashboard/realtime', {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        }
      });
      
      const result = await response.json();
      
      if (result.success) {
        setRealtime(result.data);
      }
    } catch (err) {
      console.error('Realtime update failed:', err);
    }
  }, []);

  useEffect(() => {
    fetchDashboard();
    
    // Auto-refresh realtime data every 30 seconds
    const interval = setInterval(fetchRealtime, 30000);
    
    return () => clearInterval(interval);
  }, [fetchDashboard, fetchRealtime]);

  return {
    data,
    realtime, 
    loading,
    error,
    refresh: fetchDashboard,
    refreshRealtime: fetchRealtime
  };
};
```

### Vue.js Composition API Example

```javascript
// composables/useDashboard.js
import { ref, onMounted, onUnmounted } from 'vue';

export function useDashboard() {
  const dashboardData = ref(null);
  const realtimeData = ref(null);
  const loading = ref(true);
  const error = ref(null);
  
  let realtimeInterval = null;

  const loadDashboard = async () => {
    try {
      loading.value = true;
      
      const response = await $fetch('/api/dashboard/comprehensive');
      
      dashboardData.value = response.data;
      error.value = null;
      
    } catch (err) {
      error.value = err.message;
    } finally {
      loading.value = false;
    }
  };

  const updateRealtime = async () => {
    try {
      const response = await $fetch('/api/dashboard/realtime');
      realtimeData.value = response.data;
    } catch (err) {
      console.error('Realtime update failed:', err);
    }
  };

  onMounted(() => {
    loadDashboard();
    realtimeInterval = setInterval(updateRealtime, 30000);
  });

  onUnmounted(() => {
    if (realtimeInterval) {
      clearInterval(realtimeInterval);
    }
  });

  return {
    dashboardData,
    realtimeData,
    loading,
    error,
    loadDashboard,
    updateRealtime
  };
}
```

### Angular Service Example

```typescript
// services/dashboard.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, interval, Observable } from 'rxjs';
import { switchMap, catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private dashboardData$ = new BehaviorSubject(null);
  private realtimeData$ = new BehaviorSubject(null);
  private loading$ = new BehaviorSubject(true);

  constructor(private http: HttpClient) {
    this.initializeRealtime();
  }

  loadDashboard(): Observable<any> {
    this.loading$.next(true);
    
    return this.http.get('/api/dashboard/comprehensive').pipe(
      switchMap((response: any) => {
        if (response.success) {
          this.dashboardData$.next(response.data);
          this.loading$.next(false);
          return response.data;
        }
        throw new Error(response.message);
      }),
      catchError(error => {
        this.loading$.next(false);
        throw error;
      })
    );
  }

  private initializeRealtime(): void {
    interval(30000).pipe(
      switchMap(() => this.http.get('/api/dashboard/realtime')),
      catchError(error => {
        console.error('Realtime update failed:', error);
        return [];
      })
    ).subscribe((response: any) => {
      if (response.success) {
        this.realtimeData$.next(response.data);
      }
    });
  }

  getDashboardData(): Observable<any> {
    return this.dashboardData$.asObservable();
  }

  getRealtimeData(): Observable<any> {
    return this.realtimeData$.asObservable();
  }

  isLoading(): Observable<boolean> {
    return this.loading$.asObservable();
  }
}
```

---

## 📊 Chart Integration Examples

### Chart.js with React

```jsx
import { Line, Pie, Bar } from 'react-chartjs-2';
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend,
  ArcElement,
  BarElement
} from 'chart.js';

// Register components
ChartJS.register(
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend,
  ArcElement,
  BarElement
);

const ContactTrendChart = () => {
  const [chartData, setChartData] = useState(null);
  
  useEffect(() => {
    fetch('/api/dashboard/charts/contacttrend?days=30')
      .then(response => response.json())
      .then(result => {
        if (result.success) {
          setChartData(result.data);
        }
      });
  }, []);

  const options = {
    responsive: true,
    plugins: {
      legend: {
        position: 'top',
      },
      title: {
        display: true,
        text: 'Xu hướng liên hệ khách hàng (30 ngày)',
      },
    },
    scales: {
      y: {
        beginAtZero: true,
        ticks: {
          stepSize: 1
        }
      }
    }
  };

  if (!chartData) return <div>Loading chart...</div>;

  return (
    <div className="bg-white p-6 rounded-lg shadow">
      <Line data={chartData} options={options} />
      <div className="mt-4 text-sm text-gray-600">
        <div className="flex justify-between">
          <span>Tổng liên hệ: {chartData.totalContacts}</span>
          <span>TB/ngày: {chartData.averagePerDay}</span>
        </div>
      </div>
    </div>
  );
};
```

### ApexCharts Example

```jsx
import ReactApexChart from 'react-apexcharts';

const UserGrowthApexChart = ({ data }) => {
  const series = [
    {
      name: 'Người dùng mới',
      data: data.datasets[0].data
    },
    {
      name: 'Người dùng hoạt động', 
      data: data.datasets[1].data
    }
  ];

  const options = {
    chart: {
      type: 'line',
      height: 350,
      toolbar: {
        show: false
      }
    },
    colors: ['#3b82f6', '#10b981'],
    dataLabels: {
      enabled: false
    },
    stroke: {
      curve: 'smooth',
      width: 3
    },
    xaxis: {
      categories: data.labels
    },
    yaxis: {
      title: {
        text: 'Số lượng người dùng'
      }
    },
    tooltip: {
      y: {
        formatter: (val) => `${val} người dùng`
      }
    },
    legend: {
      position: 'top',
      horizontalAlign: 'right'
    }
  };

  return (
    <div className="bg-white p-6 rounded-lg shadow">
      <h4 className="text-lg font-semibold mb-4">📈 Tăng trưởng người dùng</h4>
      <ReactApexChart 
        options={options} 
        series={series} 
        type="line" 
        height={350} 
      />
    </div>
  );
};
```

---

## 🎨 UI Components Examples

### Contact Statistics Card

```jsx
const ContactStatsCard = ({ data }) => {
  const { totalContacts, unreadContacts, readContacts, responseRate, trends } = data;
  
  return (
    <div className="bg-white rounded-lg shadow-md p-6">
      <div className="flex items-center justify-between mb-4">
        <h3 className="text-lg font-semibold text-gray-900">📞 Thống kê liên hệ</h3>
        <span className="bg-orange-100 text-orange-800 text-xs px-2 py-1 rounded-full">
          Live
        </span>
      </div>
      
      <div className="grid grid-cols-2 gap-4 mb-6">
        <div className="text-center">
          <div className="text-3xl font-bold text-orange-600">{totalContacts}</div>
          <div className="text-sm text-gray-500">Tổng liên hệ</div>
        </div>
        
        <div className="text-center">
          <div className="text-3xl font-bold text-red-600">{unreadContacts}</div>
          <div className="text-sm text-gray-500">Chưa đọc</div>
          {unreadContacts > 0 && (
            <div className="inline-flex items-center mt-1">
              <span className="w-2 h-2 bg-red-400 rounded-full mr-1 animate-pulse"></span>
              <span className="text-xs text-red-500">Cần xử lý</span>
            </div>
          )}
        </div>
        
        <div className="text-center">
          <div className="text-3xl font-bold text-green-600">{readContacts}</div>
          <div className="text-sm text-gray-500">Đã xử lý</div>
        </div>
        
        <div className="text-center">
          <div className="text-3xl font-bold text-blue-600">{responseRate}%</div>
          <div className="text-sm text-gray-500">Tỷ lệ phản hồi</div>
        </div>
      </div>
      
      <div className="border-t pt-4">
        <div className="flex items-center justify-between">
          <span className="text-sm text-gray-600">Xu hướng tháng này</span>
          <div className={`flex items-center text-sm ${
            trends.trendDirection === 'up' ? 'text-green-600' : 
            trends.trendDirection === 'down' ? 'text-red-600' : 'text-gray-600'
          }`}>
            {trends.trendDirection === 'up' && '📈'}
            {trends.trendDirection === 'down' && '📉'}
            {trends.trendDirection === 'stable' && '➡️'}
            <span className="ml-1">{trends.growthPercentage}%</span>
          </div>
        </div>
        <div className="text-xs text-gray-500 mt-1">
          Trung bình {trends.averagePerDay} liên hệ/ngày
        </div>
      </div>
    </div>
  );
};
```

### Real-time Status Panel

```jsx
const RealtimeStatusPanel = ({ data }) => {
  const { currentActiveUsers, systemLoad, memoryUsage, storageUsed, activeAlerts } = data;
  
  const getStatusColor = (value, threshold) => {
    if (value > threshold * 0.8) return 'text-red-500';
    if (value > threshold * 0.6) return 'text-yellow-500';
    return 'text-green-500';
  };

  return (
    <div className="bg-gradient-to-r from-blue-500 to-purple-600 text-white rounded-lg p-6">
      <div className="flex items-center justify-between mb-4">
        <h3 className="text-lg font-semibold">⚡ Trạng thái thời gian thực</h3>
        <div className="flex items-center text-sm">
          <div className="w-2 h-2 bg-green-400 rounded-full mr-2 animate-pulse"></div>
          Live
        </div>
      </div>
      
      <div className="grid grid-cols-2 lg:grid-cols-4 gap-4 mb-4">
        <div className="text-center">
          <div className="text-2xl font-bold">{currentActiveUsers}</div>
          <div className="text-sm opacity-80">👥 Online</div>
        </div>
        
        <div className="text-center">
          <div className={`text-2xl font-bold ${getStatusColor(memoryUsage, 8)}`}>
            {memoryUsage}GB
          </div>
          <div className="text-sm opacity-80">🧠 RAM</div>
        </div>
        
        <div className="text-center">
          <div className={`text-2xl font-bold ${getStatusColor(storageUsed, 10000)}`}>
            {(storageUsed / 1024).toFixed(1)}GB
          </div>
          <div className="text-sm opacity-80">💾 Storage</div>
        </div>
        
        <div className="text-center">
          <div className={`text-2xl font-bold ${activeAlerts.length > 0 ? 'text-red-400' : 'text-green-400'}`}>
            {activeAlerts.length}
          </div>
          <div className="text-sm opacity-80">🚨 Alerts</div>
        </div>
      </div>
      
      {activeAlerts.length > 0 && (
        <div className="bg-white bg-opacity-10 rounded-lg p-3">
          <div className="text-sm font-medium mb-2">⚠️ Active Alerts</div>
          {activeAlerts.slice(0, 3).map((alert, index) => (
            <div key={index} className="text-xs opacity-90 mb-1">
              {alert.message}
            </div>
          ))}
        </div>
      )}
      
      <div className="text-xs opacity-70 mt-4">
        Cập nhật: {new Date(data.lastUpdated).toLocaleTimeString('vi-VN')}
      </div>
    </div>
  );
};
```

---

## 🔄 Loading States & Skeletons

```jsx
const DashboardSkeleton = () => (
  <div className="space-y-6 p-6 animate-pulse">
    {/* Header skeleton */}
    <div className="h-8 bg-gray-200 rounded-lg w-1/4"></div>
    
    {/* Overview cards skeleton */}
    <div className="grid grid-cols-2 lg:grid-cols-6 gap-4">
      {[...Array(6)].map((_, i) => (
        <div key={i} className="bg-white p-6 rounded-lg shadow">
          <div className="h-4 bg-gray-200 rounded mb-2"></div>
          <div className="h-8 bg-gray-200 rounded"></div>
        </div>
      ))}
    </div>
    
    {/* Main content skeleton */}
    <div className="grid grid-cols-1 xl:grid-cols-3 gap-6">
      <div className="xl:col-span-2 space-y-6">
        {[...Array(3)].map((_, i) => (
          <div key={i} className="bg-white p-6 rounded-lg shadow">
            <div className="h-6 bg-gray-200 rounded w-1/3 mb-4"></div>
            <div className="space-y-3">
              <div className="h-4 bg-gray-200 rounded"></div>
              <div className="h-4 bg-gray-200 rounded w-5/6"></div>
              <div className="h-4 bg-gray-200 rounded w-4/6"></div>
            </div>
          </div>
        ))}
      </div>
      
      <div className="space-y-6">
        <div className="bg-white p-6 rounded-lg shadow">
          <div className="h-6 bg-gray-200 rounded w-1/2 mb-4"></div>
          {[...Array(5)].map((_, i) => (
            <div key={i} className="flex items-center space-x-3 mb-3">
              <div className="w-8 h-8 bg-gray-200 rounded-full"></div>
              <div className="flex-1 space-y-2">
                <div className="h-3 bg-gray-200 rounded"></div>
                <div className="h-3 bg-gray-200 rounded w-3/4"></div>
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  </div>
);
```

---

## 🚨 Error Handling Examples

```jsx
const ErrorBoundary = ({ children }) => {
  const [hasError, setHasError] = useState(false);
  const [error, setError] = useState(null);

  const handleError = (error, errorInfo) => {
    setHasError(true);
    setError(error);
    console.error('Dashboard Error:', error, errorInfo);
    
    // Log to external service
    // logErrorToService(error, errorInfo);
  };

  const retry = () => {
    setHasError(false);
    setError(null);
    window.location.reload();
  };

  if (hasError) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-50">
        <div className="text-center">
          <div className="text-6xl mb-4">😵</div>
          <h2 className="text-2xl font-bold text-gray-900 mb-2">
            Oops! Có lỗi xảy ra
          </h2>
          <p className="text-gray-600 mb-6">
            Dashboard gặp sự cố không mong muốn. Vui lòng thử lại.
          </p>
          <button 
            onClick={retry}
            className="bg-blue-500 hover:bg-blue-600 text-white px-6 py-2 rounded-lg"
          >
            🔄 Thử lại
          </button>
        </div>
      </div>
    );
  }

  return children;
};

// Usage
const App = () => (
  <ErrorBoundary>
    <Dashboard />
  </ErrorBoundary>
);
```

---

## 📱 Mobile-First Responsive Example

```jsx
const MobileDashboard = () => {
  const [activeTab, setActiveTab] = useState('overview');
  const { data, realtime, loading } = useDashboard();

  const tabs = [
    { id: 'overview', label: '📊 Tổng quan', icon: '📊' },
    { id: 'users', label: '👥 Users', icon: '👥' },
    { id: 'contacts', label: '📞 Contacts', icon: '📞' },
    { id: 'system', label: '🖥️ System', icon: '🖥️' }
  ];

  if (loading) return <DashboardSkeleton />;

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Mobile header */}
      <div className="bg-white shadow-sm p-4 lg:hidden">
        <h1 className="text-xl font-bold">📊 Dashboard</h1>
        {realtime && (
          <div className="text-sm text-gray-500 flex items-center mt-1">
            <div className="w-2 h-2 bg-green-400 rounded-full mr-2 animate-pulse"></div>
            {realtime.currentActiveUsers} online
          </div>
        )}
      </div>

      {/* Mobile tabs */}
      <div className="lg:hidden bg-white border-b overflow-x-auto">
        <div className="flex space-x-1 p-2">
          {tabs.map(tab => (
            <button
              key={tab.id}
              onClick={() => setActiveTab(tab.id)}
              className={`flex-shrink-0 px-4 py-2 text-sm rounded-lg transition-colors ${
                activeTab === tab.id
                  ? 'bg-blue-500 text-white'
                  : 'text-gray-600 hover:bg-gray-100'
              }`}
            >
              <span className="mr-1">{tab.icon}</span>
              {tab.label}
            </button>
          ))}
        </div>
      </div>

      {/* Mobile content */}
      <div className="p-4 lg:hidden">
        {activeTab === 'overview' && <OverviewMobile data={data?.overview} />}
        {activeTab === 'users' && <UserStatsMobile data={data?.userStats} />}
        {activeTab === 'contacts' && <ContactStatsMobile data={data?.contactStats} />}
        {activeTab === 'system' && <SystemStatsMobile data={realtime} />}
      </div>

      {/* Desktop layout */}
      <div className="hidden lg:block">
        <Dashboard />
      </div>
    </div>
  );
};
```

---

Dashboard API đã được document chi tiết với đầy đủ examples thực tế! Frontend team có thể reference và implement ngay lập tức. 🚀