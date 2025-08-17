using AttechServer.Infrastructures.Persistances;
using AttechServer.Shared.ApplicationBase.Common;
using Microsoft.EntityFrameworkCore;

namespace AttechServer.Services
{
    /// <summary>
    /// Background service để dọn dẹp file tạm thời và record Attachment quá hạn (Logic 3)
    /// - Xóa file vật lý cũ hơn 24 giờ
    /// - Xóa record temporary trong database
    /// - Dọn dẹp orphaned files không có record
    /// </summary>
    public class TempFileCleanupBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TempFileCleanupBackgroundService> _logger;
        private readonly TimeSpan _period = TimeSpan.FromHours(24); // Chạy hàng ngày
        private readonly int _tempFileExpiryHours = 24; // File temp quá 24 giờ sẽ bị xóa

        public TempFileCleanupBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<TempFileCleanupBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Đợi 5 giây sau khi start để tránh conflict với initialization
            await Task.Delay(5000, stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await DoCleanupWork();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during temporary file cleanup");
                }

                // Tính toán thời gian đến lần chạy tiếp theo (2:00 AM hôm sau)
                var now = DateTime.UtcNow;
                var nextRun = now.Date.AddDays(1).AddHours(2); // 2:00 AM UTC tomorrow
                if (now.Hour < 2) // Nếu chưa đến 2:00 AM hôm nay
                {
                    nextRun = now.Date.AddHours(2); // 2:00 AM hôm nay
                }

                var delay = nextRun - now;
                _logger.LogInformation($"Next temp file cleanup scheduled at: {nextRun:yyyy-MM-dd HH:mm:ss} UTC (in {delay.TotalHours:F1} hours)");

                await Task.Delay(delay, stoppingToken);
            }
        }

        private async Task DoCleanupWork()
        {
            _logger.LogInformation("Starting temporary file cleanup process");

            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var cutoffDate = DateTime.UtcNow.AddHours(-_tempFileExpiryHours);
            var filesDeleted = 0;
            var recordsDeleted = 0;

            try
            {
                // 1. Tìm tất cả file tạm thời quá hạn
                var expiredTempFiles = await dbContext.Attachments
                    .Where(f => f.IsTemporary && 
                               f.CreatedDate.HasValue && 
                               f.CreatedDate.Value < cutoffDate &&
                               !f.Deleted)
                    .ToListAsync();

                _logger.LogInformation($"Found {expiredTempFiles.Count} expired temporary files for cleanup");

                // 2. Xóa file vật lý và cập nhật record
                foreach (var file in expiredTempFiles)
                {
                    try
                    {
                        // Xóa file vật lý
                        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "AttechServer", file.FilePath.TrimStart('/'));
                        if (File.Exists(fullPath))
                        {
                            File.Delete(fullPath);
                            filesDeleted++;
                            _logger.LogDebug($"Deleted physical file: {file.FilePath}");
                        }

                        // Đánh dấu record là deleted (soft delete)
                        file.Deleted = true;
                        file.ModifiedDate = DateTime.UtcNow;
                        recordsDeleted++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, $"Failed to delete temporary file: {file.FilePath}");
                    }
                }

                // 3. Dọn dẹp thêm file trong thư mục temp không có record
                await CleanupOrphanedTempFiles();

                // 4. Dọn dẹp record TinyMCE temp quá hạn (không link với entity nào)
                var orphanedTinyMceFiles = await dbContext.Attachments
                    .Where(f => f.ObjectType == ObjectType.TinyMCE &&
                               f.ObjectId == null &&
                               f.IsTemporary &&
                               f.CreatedDate.HasValue &&
                               f.CreatedDate.Value < cutoffDate.AddDays(-1) && // TinyMCE files có thể dọn sớm hơn
                               !f.Deleted)
                    .ToListAsync();

                foreach (var file in orphanedTinyMceFiles)
                {
                    try
                    {
                        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "AttechServer", file.FilePath.TrimStart('/'));
                        if (File.Exists(fullPath))
                        {
                            File.Delete(fullPath);
                            filesDeleted++;
                        }
                        file.Deleted = true;
                        file.ModifiedDate = DateTime.UtcNow;
                        recordsDeleted++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, $"Failed to delete orphaned TinyMCE file: {file.FilePath}");
                    }
                }

                await dbContext.SaveChangesAsync();

                // 5. Cleanup orphaned entity files (files whose entities were deleted)
                // IFileCleanupService không còn tồn tại - skip cleanup orphaned files
                var orphanedFilesDeleted = 0;

                _logger.LogInformation($"Temporary file cleanup completed. Files deleted: {filesDeleted}, Records marked as deleted: {recordsDeleted}, Orphaned files cleaned: {orphanedFilesDeleted}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during temporary file cleanup process");
                throw;
            }
        }

        /// <summary>
        /// Dọn dẹp file trong thư mục temp không có record trong DB
        /// </summary>
        private async Task CleanupOrphanedTempFiles()
        {
            try
            {
                var tempDir = Path.Combine(Directory.GetCurrentDirectory(), "AttechServer", "Uploads", "temp");
                if (!Directory.Exists(tempDir))
                    return;

                var tempFiles = Directory.GetFiles(tempDir, "*", SearchOption.AllDirectories);
                var orphanedCount = 0;

                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                foreach (var tempFile in tempFiles)
                {
                    try
                    {
                        var fileInfo = new FileInfo(tempFile);
                        
                        // Nếu file quá cũ (7 ngày) 
                        if (fileInfo.CreationTimeUtc < DateTime.UtcNow.AddDays(-7))
                        {
                            var relativePath = Path.GetRelativePath(
                                Path.Combine(Directory.GetCurrentDirectory(), "AttechServer"), 
                                tempFile
                            ).Replace('\\', '/');

                            // Kiểm tra xem có record trong DB không
                            var hasRecord = await dbContext.Attachments
                                .AnyAsync(f => f.FilePath.Contains(Path.GetFileName(tempFile)) && !f.Deleted);

                            if (!hasRecord)
                            {
                                File.Delete(tempFile);
                                orphanedCount++;
                                _logger.LogDebug($"Deleted orphaned temp file: {relativePath}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, $"Failed to process temp file: {tempFile}");
                    }
                }

                if (orphanedCount > 0)
                {
                    _logger.LogInformation($"Cleaned up {orphanedCount} orphaned temporary files");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during orphaned temp file cleanup");
            }
        }
    }
}
