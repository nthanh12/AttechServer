using AttechServer.Domains.Entities.Main;

namespace AttechServer.Shared.SampleData
{
    public static class ContactSampleData
    {
        public static List<Contact> GetSampleContacts()
        {
            return new List<Contact>
            {
                new Contact
                {
                    Name = "Nguyễn Văn An",
                    Email = "vanan@gmail.com",
                    PhoneNumber = "0987654321",
                    Subject = "Hỏi về sản phẩm A",
                    Message = "Tôi muốn hỏi về tình trạng sản phẩm A hiện tại có còn hàng không? Và giá bán là bao nhiêu?",
                    Status = 0, // Chưa đọc
                    SubmittedAt = DateTime.Now.AddHours(-2),
                    IpAddress = "192.168.1.100",
                    UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36",
                    CreatedDate = DateTime.Now.AddHours(-2),
                    Deleted = false
                },
                
                new Contact
                {
                    Name = "Trần Thị Bình",
                    Email = "tranbinh@yahoo.com",
                    PhoneNumber = "0912345678",
                    Subject = "KHẨN CẤP - Lỗi đơn hàng #12345",
                    Message = "Tôi đã đặt đơn hàng #12345 từ 3 ngày trước nhưng chưa nhận được hàng. Đây là đơn hàng gấp, mong quý công ty xử lý sớm.",
                    Status = 1, // Đã đọc
                    SubmittedAt = DateTime.Now.AddDays(-1),
                    IpAddress = "10.0.0.5",
                    UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 15_0 like Mac OS X)",
                    CreatedDate = DateTime.Now.AddDays(-1),
                    ModifiedDate = DateTime.Now.AddHours(-1),
                    Deleted = false
                },
                
                new Contact
                {
                    Name = "Lê Minh Cường",
                    Email = "cuonglm@company.vn",
                    PhoneNumber = null, // Không có số điện thoại
                    Subject = "Hợp tác kinh doanh",
                    Message = "Công ty chúng tôi muốn tìm hiểu về cơ hội hợp tác kinh doanh. Xin vui lòng liên hệ lại để trao đổi chi tiết.",
                    Status = 0, // Chưa đọc
                    SubmittedAt = DateTime.Now.AddMinutes(-30),
                    IpAddress = "172.16.0.10",
                    UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/605.1.15",
                    CreatedDate = DateTime.Now.AddMinutes(-30),
                    Deleted = false
                },
                
                new Contact
                {
                    Name = "Phạm Thu Hà",
                    Email = "thuha.dev@outlook.com", 
                    PhoneNumber = "0903456789",
                    Subject = "Góp ý cải thiện website",
                    Message = "Website của công ty rất đẹp nhưng tôi nghĩ nên thêm tính năng tìm kiếm để người dùng dễ tìm sản phẩm hơn. Cảm ơn!",
                    Status = 1, // Đã đọc
                    SubmittedAt = DateTime.Now.AddDays(-2),
                    IpAddress = "203.113.131.15",
                    UserAgent = "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36",
                    CreatedDate = DateTime.Now.AddDays(-2),
                    ModifiedDate = DateTime.Now.AddDays(-1),
                    Deleted = false
                },
                
                new Contact
                {
                    Name = "Vũ Đình Nam",
                    Email = "namvd@gmail.com",
                    PhoneNumber = "0976543210",
                    Subject = "Urgent - Cần hỗ trợ kỹ thuật gấp",
                    Message = "Hệ thống của chúng tôi đang gặp sự cố nghiêm trọng và cần hỗ trợ kỹ thuật ngay lập tức. Vui lòng liên hệ số hotline để được hỗ trợ.",
                    Status = 0, // Chưa đọc
                    SubmittedAt = DateTime.Now.AddMinutes(-10),
                    IpAddress = "192.168.100.50",
                    UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36",
                    CreatedDate = DateTime.Now.AddMinutes(-10),
                    Deleted = false
                }
            };
        }
    }
}