using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.Contact;
using AttechServer.Domains.Entities.Main;
using AttechServer.Infrastructures.Persistances;
using AttechServer.Shared.ApplicationBase.Common;
using AttechServer.Shared.Consts.Exceptions;
using AttechServer.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace AttechServer.Applications.UserModules.Implements
{
    public class ContactService : IContactService
    {
        private readonly ILogger<ContactService> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IActivityLogService _activityLogService;
        private readonly IContactEmailService _contactEmailService;
        private readonly IContactNotificationService _contactNotificationService;
        private readonly IServiceProvider _serviceProvider;

        public ContactService(
            ApplicationDbContext dbContext, 
            ILogger<ContactService> logger, 
            IHttpContextAccessor httpContextAccessor,
            IActivityLogService activityLogService,
            IContactEmailService contactEmailService,
            IContactNotificationService contactNotificationService,
            IServiceProvider serviceProvider)
        {
            _dbContext = dbContext;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _activityLogService = activityLogService;
            _contactEmailService = contactEmailService;
            _contactNotificationService = contactNotificationService;
            _serviceProvider = serviceProvider;
        }

        public async Task<ContactDto> Submit(CreateContactDto input, string? ipAddress = null, string? userAgent = null)
        {
            _logger.LogInformation($"{nameof(Submit)}: Submitting contact form from {input.Email}");

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var contact = new Contact
                {
                    Name = input.Name.Trim(),
                    Email = input.Email.Trim().ToLower(),
                    PhoneNumber = string.IsNullOrWhiteSpace(input.PhoneNumber) ? null : input.PhoneNumber.Trim(),
                    Subject = input.Subject.Trim(),
                    Message = input.Message.Trim(),
                    Status = 0, // Unread
                    SubmittedAt = input.SubmittedAt,
                    IpAddress = ipAddress,
                    UserAgent = userAgent,
                    CreatedDate = DateTime.Now,
                    Deleted = false
                };

                _dbContext.Contacts.Add(contact);
                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();

                // Log activity
                await _activityLogService.LogAsync("CONTACT_SUBMIT", "Liên hệ mới", 
                    $"Email: {input.Email}, Subject: {input.Subject}", "Info");

                _logger.LogInformation($"Contact submitted successfully. ID: {contact.Id}, Email: {contact.Email}");

                var contactDto = new ContactDto
                {
                    Id = contact.Id,
                    Name = contact.Name,
                    Email = contact.Email,
                    PhoneNumber = contact.PhoneNumber,
                    Subject = contact.Subject,
                    Message = contact.Message,
                    Status = contact.Status,
                    SubmittedAt = contact.SubmittedAt,
                    IpAddress = contact.IpAddress,
                    CreatedDate = contact.CreatedDate ?? DateTime.Now,
                    ModifiedDate = contact.ModifiedDate
                };

                // Send notifications and emails asynchronously - capture current count before background task
                var currentUnreadCount = await _dbContext.Contacts
                    .Where(c => !c.Deleted && c.Status == 0)
                    .CountAsync();
                    
                _ = Task.Run(async () =>
                {
                    try
                    {
                        // Send confirmation email to customer
                        await _contactEmailService.SendCustomerConfirmationAsync(contactDto);
                        
                        // Send notification to admin
                        await _contactEmailService.SendAdminNotificationAsync(contactDto);
                        
                        // Send real-time notification
                        await _contactNotificationService.NotifyAdminNewContactAsync(contactDto);
                        
                        // Check if urgent and send urgent notification
                        if (IsUrgentContact(contactDto))
                        {
                            await _contactNotificationService.NotifyAdminUrgentContactAsync(contactDto);
                        }
                        
                        // Update unread count - use captured count
                        await _contactNotificationService.UpdateUnreadContactCountAsync(currentUnreadCount);
                        
                        // Send immediate service department notification
                        var recentContacts = new List<ContactDto> { contactDto };
                        var reportPeriod = $"Liên hệ mới - {DateTime.Now:dd/MM/yyyy HH:mm}";
                        await _contactEmailService.SendServiceDepartmentNotificationAsync(recentContacts, reportPeriod);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error in background contact processing for contact {contact.Id}");
                    }
                });

                return contactDto;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Error submitting contact form from {input.Email}");
                throw;
            }
        }

        public async Task<PagingResult<ContactDto>> FindAll(PagingRequestBaseDto input)
        {
            _logger.LogInformation($"{nameof(FindAll)}: input = {JsonSerializer.Serialize(input)}");

            var baseQuery = _dbContext.Contacts.AsNoTracking()
                .Where(c => !c.Deleted);

            // Filter by status
            if (input.Status.HasValue)
            {
                baseQuery = baseQuery.Where(c => c.Status == input.Status.Value);
            }

            // Filter by date range
            if (input.DateFrom.HasValue)
            {
                baseQuery = baseQuery.Where(c => c.SubmittedAt >= input.DateFrom.Value);
            }
            if (input.DateTo.HasValue)
            {
                baseQuery = baseQuery.Where(c => c.SubmittedAt <= input.DateTo.Value);
            }

            // Search in multiple fields
            if (!string.IsNullOrEmpty(input.Keyword))
            {
                baseQuery = baseQuery.Where(c =>
                    c.Name.Contains(input.Keyword) ||
                    c.Email.Contains(input.Keyword) ||
                    (!string.IsNullOrEmpty(c.PhoneNumber) && c.PhoneNumber.Contains(input.Keyword)) ||
                    c.Subject.Contains(input.Keyword) ||
                    c.Message.Contains(input.Keyword));
            }

            var totalItems = await baseQuery.CountAsync();

            // Apply sorting
            var query = ApplySorting(baseQuery, input);

            var pagedItems = await query
                .Skip(input.GetSkip())
                .Take(input.PageSize)
                .Select(c => new ContactDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Email = c.Email,
                    PhoneNumber = c.PhoneNumber,
                    Subject = c.Subject,
                    Message = c.Message,
                    Status = c.Status,
                    SubmittedAt = c.SubmittedAt,
                    IpAddress = c.IpAddress,
                    CreatedDate = c.CreatedDate ?? DateTime.Now,
                    ModifiedDate = c.ModifiedDate
                })
                .ToListAsync();

            return new PagingResult<ContactDto>
            {
                TotalItems = totalItems,
                Items = pagedItems
            };
        }

        public async Task<DetailContactDto> FindById(int id)
        {
            _logger.LogInformation($"{nameof(FindById)}: id = {id}");

            var contact = await _dbContext.Contacts
                .Where(c => c.Id == id && !c.Deleted)
                .Select(c => new DetailContactDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Email = c.Email,
                    PhoneNumber = c.PhoneNumber,
                    Subject = c.Subject,
                    Message = c.Message,
                    Status = c.Status,
                    SubmittedAt = c.SubmittedAt,
                    IpAddress = c.IpAddress,
                    UserAgent = c.UserAgent,
                    CreatedDate = c.CreatedDate ?? DateTime.Now,
                    ModifiedDate = c.ModifiedDate,
                    CreatedBy = c.CreatedBy,
                    ModifiedBy = c.ModifiedBy
                })
                .FirstOrDefaultAsync();

            if (contact == null)
                throw new UserFriendlyException(ErrorCode.ContactNotFound);

            return contact;
        }

        public async Task<ContactDto> UpdateStatus(int id, UpdateContactStatusDto input)
        {
            _logger.LogInformation($"{nameof(UpdateStatus)}: id = {id}, status = {input.Status}");

            var contact = await _dbContext.Contacts
                .FirstOrDefaultAsync(c => c.Id == id && !c.Deleted)
                ?? throw new UserFriendlyException(ErrorCode.ContactNotFound);

            var oldStatus = contact.Status;
            contact.Status = input.Status;
            contact.ModifiedDate = DateTime.Now;
            contact.ModifiedBy = GetCurrentUserId();

            await _dbContext.SaveChangesAsync();

            // Log activity
            await _activityLogService.LogAsync("CONTACT_STATUS_UPDATE", "Cập nhật trạng thái liên hệ",
                $"Contact ID: {id}, Status: {oldStatus} → {input.Status}", "Info");

            var contactDto = new ContactDto
            {
                Id = contact.Id,
                Name = contact.Name,
                Email = contact.Email,
                PhoneNumber = contact.PhoneNumber,
                Subject = contact.Subject,
                Message = contact.Message,
                Status = contact.Status,
                SubmittedAt = contact.SubmittedAt,
                IpAddress = contact.IpAddress,
                CreatedDate = contact.CreatedDate ?? DateTime.Now,
                ModifiedDate = contact.ModifiedDate
            };

            // Send real-time status update notification
            _ = Task.Run(async () =>
            {
                try
                {
                    await _contactNotificationService.NotifyAdminContactStatusChangedAsync(contactDto, oldStatus, input.Status);
                    
                    // Update unread count if status changed to/from unread
                    if (oldStatus == 0 || input.Status == 0)
                    {
                        var unreadCount = await GetUnreadCount();
                        await _contactNotificationService.UpdateUnreadContactCountAsync(unreadCount);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error sending status update notification for contact {id}");
                }
            });

            return contactDto;
        }

        public async Task Delete(int id)
        {
            _logger.LogInformation($"{nameof(Delete)}: id = {id}");

            var contact = await _dbContext.Contacts
                .FirstOrDefaultAsync(c => c.Id == id && !c.Deleted)
                ?? throw new UserFriendlyException(ErrorCode.ContactNotFound);

            contact.Deleted = true;
            contact.ModifiedDate = DateTime.Now;
            contact.ModifiedBy = GetCurrentUserId();

            await _dbContext.SaveChangesAsync();

            // Log activity
            await _activityLogService.LogAsync("CONTACT_DELETE", "Xóa liên hệ",
                $"Contact ID: {id}, Email: {contact.Email}", "Warning");

            _logger.LogInformation($"Contact deleted successfully. ID: {id}");
        }

        public async Task MarkAsRead(int id)
        {
            await UpdateStatus(id, new UpdateContactStatusDto { Status = 1 });
        }

        public async Task MarkAsUnread(int id)
        {
            await UpdateStatus(id, new UpdateContactStatusDto { Status = 0 });
        }

        public async Task<int> GetUnreadCount()
        {
            return await _dbContext.Contacts
                .Where(c => !c.Deleted && c.Status == 0)
                .CountAsync();
        }

        private IQueryable<Contact> ApplySorting(IQueryable<Contact> query, PagingRequestBaseDto input)
        {
            if (!string.IsNullOrEmpty(input.SortBy))
            {
                switch (input.SortBy.ToLower())
                {
                    case "id":
                        return input.IsAscending ? query.OrderBy(x => x.Id) : query.OrderByDescending(x => x.Id);
                    case "name":
                        return input.IsAscending ? query.OrderBy(x => x.Name) : query.OrderByDescending(x => x.Name);
                    case "email":
                        return input.IsAscending ? query.OrderBy(x => x.Email) : query.OrderByDescending(x => x.Email);
                    case "subject":
                        return input.IsAscending ? query.OrderBy(x => x.Subject) : query.OrderByDescending(x => x.Subject);
                    case "submittedat":
                        return input.IsAscending ? query.OrderBy(x => x.SubmittedAt) : query.OrderByDescending(x => x.SubmittedAt);
                    case "status":
                        return input.IsAscending ? query.OrderBy(x => x.Status) : query.OrderByDescending(x => x.Status);
                    default:
                        return query.OrderByDescending(x => x.SubmittedAt); // default sort
                }
            }
            else
            {
                return query.OrderByDescending(x => x.SubmittedAt); // default sort
            }
        }

        private int GetCurrentUserId()
        {
            return int.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirst("user_id")?.Value, out var id) ? id : 0;
        }

        private bool IsUrgentContact(ContactDto contact)
        {
            var urgentKeywords = new[] { "khẩn cấp", "urgent", "gấp", "emergency", "asap", "ngay lập tức" };
            var content = $"{contact.Subject} {contact.Message}".ToLower();
            return urgentKeywords.Any(keyword => content.Contains(keyword));
        }
    }
}