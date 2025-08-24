# 📊 Dashboard API - Frontend Integration Guide

## 🎯 Tổng quan cho Frontend Team

Dashboard API cung cấp **đầy đủ dữ liệu** để xây dựng một dashboard admin hoàn chỉnh với các tính năng:
- 📈 Thống kê tổng quan (Users, Products, Services, News, Notifications, **Contacts**)
- 👥 Phân tích người dùng (growth trends, role distribution)
- 📝 Thống kê nội dung (category distribution, monthly trends)
- 📞 **Thống kê liên hệ từ khách hàng** (NEW - response rates, trends)
- 🖥️ Monitoring hệ thống (uptime, memory, storage)
- ⚡ Dữ liệu real-time
- 📊 Charts và visualization data

---

## 🚀 Quick Start - Implementation Strategy

### 1. **Initial Dashboard Load** (Khuyến nghị)
```javascript
// ✅ BEST PRACTICE - Single request for all dashboard data
const loadDashboard = async () => {
  try {
    const response = await fetch('/api/dashboard/comprehensive', {
      headers: { 'Authorization': `Bearer ${token}` }
    });
    const result = await response.json();
    
    if (result.success) {
      const { overview, userStats, contentStats, contactStats, recentActivities } = result.data;
      
      // Set all dashboard data at once
      setOverview(overview);
      setUserStats(userStats);
      setContentStats(contentStats);
      setContactStats(contactStats);  // ⭐ NEW - Contact data
      setActivities(recentActivities);
    }
  } catch (error) {
    handleError(error);
  }
};
```

### 2. **Real-time Updates** (Optional)
```javascript
// ✅ For live metrics - call every 30 seconds
const updateRealtime = async () => {
  const response = await fetch('/api/dashboard/realtime');
  const result = await response.json();
  
  if (result.success) {
    setRealtimeData(result.data);
  }
};

// Auto-refresh every 30 seconds
useEffect(() => {
  const interval = setInterval(updateRealtime, 30000);
  return () => clearInterval(interval);
}, []);
```

---

## 📡 API Endpoints Chi Tiết

### 🏠 **Core Data Endpoints**

#### 1. **Dashboard Overview** - `/api/dashboard/overview`
```typescript
interface DashboardOverview {
  totalUsers: number;           // Tổng số người dùng
  totalProducts: number;        // Tổng số sản phẩm
  totalServices: number;        // Tổng số dịch vụ  
  totalNews: number;           // Tổng số tin tức
  totalNotifications: number;  // Tổng số thông báo
  totalContacts: number;       // 🆕 Tổng số liên hệ từ khách hàng
  activeUsers: number;         // Người dùng hoạt động (30 ngày)
  lastUpdated: string;         // Thời gian cập nhật
}
```

**Frontend Usage:**
```jsx
const OverviewCards = ({ data }) => (
  <div className="grid grid-cols-2 lg:grid-cols-3 xl:grid-cols-6 gap-4">
    <StatsCard 
      title="Người dùng" 
      value={data.totalUsers} 
      icon="👥"
      color="blue" 
    />
    <StatsCard 
      title="Sản phẩm" 
      value={data.totalProducts} 
      icon="📦"
      color="green" 
    />
    <StatsCard 
      title="Dịch vụ" 
      value={data.totalServices} 
      icon="⚙️"
      color="purple" 
    />
    <StatsCard 
      title="Tin tức" 
      value={data.totalNews} 
      icon="📰"
      color="yellow" 
    />
    <StatsCard 
      title="Thông báo" 
      value={data.totalNotifications} 
      icon="🔔"
      color="red" 
    />
    {/* 🆕 NEW - Contact card */}
    <StatsCard 
      title="Liên hệ" 
      value={data.totalContacts} 
      icon="📞"
      color="orange" 
    />
  </div>
);
```

#### 2. **User Statistics** - `/api/dashboard/users`
```typescript
interface UserStatistics {
  newUsersToday: number;
  newUsersThisWeek: number;
  newUsersThisMonth: number;
  activeUsersToday: number;
  totalUsers: number;
  usersByRole: Array<{
    role: string;
    count: number;
    percentage: number;
  }>;
  growthTrend: {
    monthlyGrowth: number;
    growthRate: number;           // Phần trăm tăng trưởng
    trendDirection: 'up' | 'down' | 'stable';
  };
}
```

**Frontend Usage:**
```jsx
const UserStatsSection = ({ data }) => (
  <div className="bg-white p-6 rounded-lg shadow">
    <h3 className="text-lg font-semibold mb-4">📊 Thống kê người dùng</h3>
    
    <div className="grid grid-cols-2 lg:grid-cols-4 gap-4 mb-6">
      <MetricCard label="Hôm nay" value={data.newUsersToday} />
      <MetricCard label="Tuần này" value={data.newUsersThisWeek} />
      <MetricCard label="Tháng này" value={data.newUsersThisMonth} />
      <MetricCard label="Đang hoạt động" value={data.activeUsersToday} />
    </div>
    
    {/* Growth trend indicator */}
    <TrendIndicator 
      direction={data.growthTrend.trendDirection}
      rate={data.growthTrend.growthRate}
      value={data.growthTrend.monthlyGrowth}
    />
    
    {/* Role distribution pie chart */}
    <RoleDistributionChart data={data.usersByRole} />
  </div>
);
```

#### 3. **🆕 Contact Statistics** - `/api/dashboard/contacts`
```typescript
interface ContactStatistics {
  totalContacts: number;        // Tổng số liên hệ
  unreadContacts: number;       // Liên hệ chưa đọc
  readContacts: number;         // Liên hệ đã đọc  
  contactsToday: number;        // Liên hệ hôm nay
  contactsThisWeek: number;     // Liên hệ tuần này
  contactsThisMonth: number;    // Liên hệ tháng này
  responseRate: number;         // Tỷ lệ phản hồi (%)
  trends: {
    totalThisMonth: number;
    totalLastMonth: number;
    growthPercentage: number;   // % tăng trưởng so với tháng trước
    trendDirection: 'up' | 'down' | 'stable';
    averagePerDay: number;      // Trung bình liên hệ/ngày
  };
}
```

**Frontend Usage:**
```jsx
const ContactStatsSection = ({ data }) => (
  <div className="bg-white p-6 rounded-lg shadow">
    <h3 className="text-lg font-semibold mb-4">📞 Thống kê liên hệ khách hàng</h3>
    
    {/* Contact overview */}
    <div className="grid grid-cols-2 lg:grid-cols-4 gap-4 mb-6">
      <MetricCard 
        label="Tổng liên hệ" 
        value={data.totalContacts}
        color="blue"
      />
      <MetricCard 
        label="Chưa đọc" 
        value={data.unreadContacts}
        color="red"
        urgent={data.unreadContacts > 0}
      />
      <MetricCard 
        label="Đã xử lý" 
        value={data.readContacts}
        color="green"
      />
      <MetricCard 
        label="Tỷ lệ phản hồi" 
        value={`${data.responseRate}%`}
        color="purple"
      />
    </div>
    
    {/* Time-based stats */}
    <div className="grid grid-cols-3 gap-4 mb-4">
      <div className="text-center">
        <div className="text-2xl font-bold text-blue-600">{data.contactsToday}</div>
        <div className="text-sm text-gray-500">Hôm nay</div>
      </div>
      <div className="text-center">
        <div className="text-2xl font-bold text-green-600">{data.contactsThisWeek}</div>
        <div className="text-sm text-gray-500">Tuần này</div>
      </div>
      <div className="text-center">
        <div className="text-2xl font-bold text-purple-600">{data.contactsThisMonth}</div>
        <div className="text-sm text-gray-500">Tháng này</div>
      </div>
    </div>
    
    {/* Trend analysis */}
    <div className="bg-gray-50 p-4 rounded-lg">
      <div className="flex items-center justify-between">
        <span className="text-sm text-gray-600">Xu hướng tháng này</span>
        <TrendBadge 
          direction={data.trends.trendDirection}
          percentage={data.trends.growthPercentage}
        />
      </div>
      <div className="text-xs text-gray-500 mt-2">
        Trung bình {data.trends.averagePerDay} liên hệ/ngày
      </div>
    </div>
  </div>
);
```

#### 4. **Content Statistics** - `/api/dashboard/content`
```typescript
interface ContentStatistics {
  newsPublishedThisMonth: number;
  notificationsThisMonth: number;
  productsAddedThisMonth: number;
  servicesAddedThisMonth: number;
  categoryDistribution: Array<{
    category: string;
    count: number;
    type: 'news' | 'products';
    percentage: number;
  }>;
  trends: {
    totalContentThisMonth: number;
    totalContentLastMonth: number;
    growthPercentage: number;
    mostActiveCategory: string;
  };
}
```

### ⚡ **Real-time Data** - `/api/dashboard/realtime`
```typescript
interface RealtimeData {
  currentActiveUsers: number;    // Người dùng online hiện tại
  systemLoad: number;           // System load (GB memory)
  memoryUsage: number;          // Memory usage (GB)  
  storageUsed: number;          // Storage used (MB)
  lastUpdated: string;          // Timestamp
  activeAlerts: Array<{         // 🚨 System alerts
    type: string;
    message: string;
    severity: 'warning' | 'critical';
    createdAt: string;
  }>;
}
```

**Frontend Usage:**
```jsx
const RealtimePanel = ({ data }) => (
  <div className="bg-gradient-to-r from-blue-500 to-purple-600 p-6 rounded-lg text-white">
    <h3 className="text-lg font-semibold mb-4">⚡ Dữ liệu thời gian thực</h3>
    
    <div className="grid grid-cols-2 lg:grid-cols-4 gap-4">
      <div className="text-center">
        <div className="text-2xl font-bold">{data.currentActiveUsers}</div>
        <div className="text-sm opacity-80">👥 Online ngay</div>
      </div>
      <div className="text-center">
        <div className="text-2xl font-bold">{data.memoryUsage}GB</div>
        <div className="text-sm opacity-80">🧠 Bộ nhớ</div>
      </div>
      <div className="text-center">
        <div className="text-2xl font-bold">{data.storageUsed}MB</div>
        <div className="text-sm opacity-80">💾 Dung lượng</div>
      </div>
      <div className="text-center">
        <div className="text-2xl font-bold">{data.activeAlerts.length}</div>
        <div className="text-sm opacity-80">🚨 Cảnh báo</div>
      </div>
    </div>
    
    {/* Active alerts */}
    {data.activeAlerts.length > 0 && (
      <div className="mt-4 space-y-2">
        {data.activeAlerts.map((alert, index) => (
          <AlertBadge key={index} alert={alert} />
        ))}
      </div>
    )}
    
    <div className="text-xs opacity-70 mt-4">
      Cập nhật: {formatTimestamp(data.lastUpdated)}
    </div>
  </div>
);
```

---

## 📊 Charts & Visualization

### **Chart Data Endpoints**

#### 1. **User Growth Chart** - `/api/dashboard/charts/usergrowth?days=7`
```typescript
interface UserGrowthChart {
  labels: string[];              // ['20/12', '21/12', ...]
  datasets: [{
    label: "Người dùng mới";
    data: number[];              // [5, 8, 12, 3, ...]
    borderColor: "#3b82f6";
    backgroundColor: "rgba(59, 130, 246, 0.1)";
  }, {
    label: "Người dùng hoạt động";
    data: number[];
    borderColor: "#10b981";
    backgroundColor: "rgba(16, 185, 129, 0.1)";
  }];
  startDate: string;
  endDate: string;
  totalNewUsers: number;
  totalActiveUsers: number;
}
```

#### 2. **🆕 Contact Trend Chart** - `/api/dashboard/charts/contacttrend?days=30`
```typescript
interface ContactChart {
  labels: string[];              // Date labels
  datasets: [{
    label: "Liên hệ mới";
    data: number[];              // Daily contact counts
    borderColor: "#8b5cf6";      // Purple color
    backgroundColor: "rgba(139, 92, 246, 0.1)";
    fill: true;
  }];
  startDate: string;
  endDate: string;
  totalContacts: number;
  averagePerDay: number;
}
```

#### 3. **Content Distribution** - `/api/dashboard/charts/contentdistribution`
```typescript
interface ContentDistributionChart {
  labels: ["News", "Notifications", "Products", "Services"];
  datasets: [{
    label: "Phân bố nội dung";
    data: number[];              // [45, 23, 67, 12]
    backgroundColor: [
      "#3b82f6",                 // Blue for News
      "#10b981",                 // Green for Notifications  
      "#f59e0b",                 // Yellow for Products
      "#ef4444"                  // Red for Services
    ];
  }];
  totalContent: number;
  mostPopularType: string;
}
```

**Frontend Chart Implementation:**
```jsx
import { Line, Pie, Bar } from 'react-chartjs-2';

const ChartsSection = () => {
  const [userGrowth, setUserGrowth] = useState(null);
  const [contactTrend, setContactTrend] = useState(null);
  const [contentDist, setContentDist] = useState(null);
  
  useEffect(() => {
    // Load chart data
    Promise.all([
      fetch('/api/dashboard/charts/usergrowth?days=7').then(r => r.json()),
      fetch('/api/dashboard/charts/contacttrend?days=30').then(r => r.json()),
      fetch('/api/dashboard/charts/contentdistribution').then(r => r.json())
    ]).then(([userRes, contactRes, contentRes]) => {
      if (userRes.success) setUserGrowth(userRes.data);
      if (contactRes.success) setContactTrend(contactRes.data);
      if (contentRes.success) setContentDist(contentRes.data);
    });
  }, []);
  
  return (
    <div className="grid grid-cols-1 lg:grid-cols-2 xl:grid-cols-3 gap-6">
      {/* User Growth Line Chart */}
      <div className="bg-white p-6 rounded-lg shadow">
        <h4 className="font-semibold mb-4">📈 Tăng trưởng người dùng (7 ngày)</h4>
        {userGrowth && <Line data={userGrowth} options={chartOptions} />}
      </div>
      
      {/* 🆕 Contact Trend Chart */}
      <div className="bg-white p-6 rounded-lg shadow">
        <h4 className="font-semibold mb-4">📞 Xu hướng liên hệ (30 ngày)</h4>
        {contactTrend && <Line data={contactTrend} options={chartOptions} />}
        {contactTrend && (
          <div className="mt-2 text-sm text-gray-600">
            Trung bình: {contactTrend.averagePerDay} liên hệ/ngày
          </div>
        )}
      </div>
      
      {/* Content Distribution Pie Chart */}
      <div className="bg-white p-6 rounded-lg shadow">
        <h4 className="font-semibold mb-4">🥧 Phân bố nội dung</h4>
        {contentDist && <Pie data={contentDist} options={pieOptions} />}
      </div>
    </div>
  );
};
```

---

## 🔄 Complete Dashboard Component

```jsx
import React, { useState, useEffect } from 'react';

const Dashboard = () => {
  const [dashboardData, setDashboardData] = useState(null);
  const [realtimeData, setRealtimeData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  // 📡 Load initial dashboard data
  const loadDashboard = async () => {
    try {
      setLoading(true);
      const response = await fetch('/api/dashboard/comprehensive');
      const result = await response.json();
      
      if (result.success) {
        setDashboardData(result.data);
      } else {
        setError(result.message);
      }
    } catch (err) {
      setError('Failed to load dashboard data');
    } finally {
      setLoading(false);
    }
  };

  // ⚡ Update realtime data
  const updateRealtime = async () => {
    try {
      const response = await fetch('/api/dashboard/realtime');
      const result = await response.json();
      
      if (result.success) {
        setRealtimeData(result.data);
      }
    } catch (err) {
      console.error('Failed to update realtime data:', err);
    }
  };

  useEffect(() => {
    loadDashboard();
    
    // Auto-refresh realtime data every 30 seconds
    const interval = setInterval(updateRealtime, 30000);
    
    return () => clearInterval(interval);
  }, []);

  if (loading) return <DashboardSkeleton />;
  if (error) return <ErrorMessage error={error} onRetry={loadDashboard} />;
  if (!dashboardData) return <EmptyState />;

  const { overview, userStats, contentStats, contactStats, recentActivities } = dashboardData;

  return (
    <div className="space-y-6 p-6">
      {/* Page Header */}
      <div className="flex justify-between items-center">
        <h1 className="text-3xl font-bold text-gray-900">📊 Dashboard</h1>
        <div className="flex items-center space-x-4">
          {realtimeData && (
            <div className="flex items-center text-sm text-gray-500">
              <div className="w-2 h-2 bg-green-400 rounded-full mr-2 animate-pulse"></div>
              Live • {realtimeData.currentActiveUsers} online
            </div>
          )}
          <button 
            onClick={loadDashboard}
            className="px-4 py-2 bg-blue-500 text-white rounded-lg hover:bg-blue-600"
          >
            🔄 Refresh
          </button>
        </div>
      </div>

      {/* Overview Cards */}
      <OverviewCards data={overview} />

      {/* Realtime Panel */}
      {realtimeData && <RealtimePanel data={realtimeData} />}

      {/* Main Content Grid */}
      <div className="grid grid-cols-1 xl:grid-cols-3 gap-6">
        {/* Left Column - Statistics */}
        <div className="xl:col-span-2 space-y-6">
          <UserStatsSection data={userStats} />
          <ContentStatsSection data={contentStats} />
          {/* 🆕 Contact Statistics */}
          <ContactStatsSection data={contactStats} />
        </div>

        {/* Right Column - Activities */}
        <div className="space-y-6">
          <RecentActivitiesPanel data={recentActivities} />
          <SystemHealthPanel data={realtimeData} />
        </div>
      </div>

      {/* Charts Section */}
      <ChartsSection />
    </div>
  );
};

export default Dashboard;
```

---

## 🎨 Styling Guidelines

### **Color Scheme**
```css
/* Primary colors for different data types */
.users-color { color: #3b82f6; }      /* Blue */
.products-color { color: #10b981; }   /* Green */
.services-color { color: #8b5cf6; }   /* Purple */
.news-color { color: #f59e0b; }       /* Yellow */
.notifications-color { color: #ef4444; } /* Red */
.contacts-color { color: #f97316; }   /* Orange - NEW */

/* Status colors */
.success { color: #10b981; }          /* Green */
.warning { color: #f59e0b; }          /* Yellow */  
.danger { color: #ef4444; }           /* Red */
.info { color: #3b82f6; }             /* Blue */
```

### **Icons Mapping**
```javascript
const icons = {
  users: '👥',
  products: '📦',
  services: '⚙️',
  news: '📰',
  notifications: '🔔',
  contacts: '📞',          // NEW
  growth: '📈',
  decline: '📉',
  stable: '➡️',
  alert: '🚨',
  online: '🟢',
  offline: '🔴',
  email: '✉️',
  phone: '📱'
};
```

---

## 🚨 Error Handling

```javascript
const handleApiError = (error, response) => {
  if (!response.success) {
    switch (response.statusCode) {
      case 401:
        // Redirect to login
        window.location.href = '/login';
        break;
      case 403:
        showToast('Bạn không có quyền truy cập', 'error');
        break;
      case 500:
        showToast('Lỗi server, vui lòng thử lại', 'error');
        break;
      default:
        showToast(response.message || 'Có lỗi xảy ra', 'error');
    }
  }
};
```

---

## 📱 Responsive Design

```jsx
// Mobile-first responsive grid
<div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-6 gap-4">
  {/* Overview cards */}
</div>

// Charts responsive
<div className="grid grid-cols-1 lg:grid-cols-2 xl:grid-cols-3 gap-6">
  {/* Charts */}
</div>

// Mobile navigation
const MobileDashboard = () => (
  <div className="lg:hidden">
    <TabNavigation 
      tabs={['Overview', 'Users', 'Content', 'Contacts', 'System']}
      activeTab={activeTab}
      onChange={setActiveTab}
    />
    <div className="p-4">
      {renderActiveTab()}
    </div>
  </div>
);
```

---

## 🔧 Performance Tips

### **1. Lazy Loading**
```javascript
const ChartComponent = lazy(() => import('./ChartComponent'));

// Use with Suspense
<Suspense fallback={<ChartSkeleton />}>
  <ChartComponent data={chartData} />
</Suspense>
```

### **2. Memoization**
```javascript
const MemoizedChart = React.memo(({ data }) => {
  return <Line data={data} options={chartOptions} />;
});

const memoizedStats = useMemo(() => {
  return processStatistics(rawData);
}, [rawData]);
```

### **3. Virtual Scrolling** (for large activity lists)
```javascript
import { FixedSizeList } from 'react-window';

const ActivityList = ({ activities }) => (
  <FixedSizeList
    height={400}
    itemCount={activities.length}
    itemSize={60}
  >
    {({ index, style }) => (
      <div style={style}>
        <ActivityItem activity={activities[index]} />
      </div>
    )}
  </FixedSizeList>
);
```

---

## 🎯 Next Steps for Frontend Team

### **Phase 1: Basic Implementation**
1. ✅ Implement overview cards with contact data
2. ✅ Create user statistics section  
3. ✅ Add content statistics display
4. ✅ **NEW: Implement contact statistics section**
5. ✅ Build recent activities panel

### **Phase 2: Advanced Features**
1. ✅ Add real-time data updates
2. ✅ Implement chart visualizations
3. ✅ **NEW: Contact trend charts**
4. ✅ Add responsive design
5. ✅ Error handling & loading states

### **Phase 3: Polish & Optimization**
1. ✅ Performance optimization
2. ✅ Accessibility improvements
3. ✅ Mobile optimization
4. ✅ Advanced filtering
5. ✅ Export functionality

---

## 📞 Support & Questions

Nếu Frontend team có thắc mắc về:
- **API responses structure**
- **Data relationships**  
- **Performance optimization**
- **Error handling**
- **New contact features**

Please contact Backend team hoặc check API documentation tại `/swagger` endpoint.

---

Dashboard API hiện đã **hoàn thiện** với đầy đủ tính năng cho một admin dashboard professional! 🎉