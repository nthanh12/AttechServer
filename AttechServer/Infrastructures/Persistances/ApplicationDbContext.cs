using AttechServer.Domains.Entities;
using AttechServer.Domains.Entities.Main;
using AttechServer.Domains.EntityBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace AttechServer.Infrastructures.Persistances
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly int? UserId;
        private readonly ILogger<ApplicationDbContext> _logger;

        #region User
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<KeyPermission> KeyPermissions { get; set; }
        public DbSet<ApiEndpoint> ApiEndpoints { get; set; }
        public DbSet<PermissionForApiEndpoint> PermissionForApiEndpoints { get; set; }
        #endregion

        #region Main
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostCategory> PostCategories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<FileUpload> FileUploads { get; set; }
        #endregion

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ApplicationDbContext> logger)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;

            // Lấy UserId từ HttpContext nếu có
            if (_httpContextAccessor.HttpContext != null)
            {
                var claims = _httpContextAccessor.HttpContext.User?.Identity as ClaimsIdentity;
                var claim = claims?.FindFirst("user_id");
                if (claim != null && int.TryParse(claim.Value, out int userId))
                {
                    UserId = userId;
                }
            }

            if (!UserId.HasValue)
            {
                _logger.LogWarning("UserId is null during DbContext initialization. No user context available.");
            }
        }

        private void CheckAudit()
        {
            ChangeTracker.DetectChanges();

            // Xử lý các entity mới thêm
            var added = ChangeTracker.Entries()
                .Where(t => t.State == EntityState.Added)
                .Select(t => t.Entity);

            foreach (var entity in added)
            {
                if (entity is ICreatedBy createdEntity && !createdEntity.CreatedBy.HasValue)
                {
                    createdEntity.CreatedDate = DateTime.UtcNow;
                    createdEntity.CreatedBy = UserId;
                }
            }

            // Xử lý các entity được sửa
            var modified = ChangeTracker.Entries()
                .Where(t => t.State == EntityState.Modified)
                .Select(t => t.Entity);

            foreach (var entity in modified)
            {
                if (entity is IModifiedBy modifiedEntity && !modifiedEntity.ModifiedBy.HasValue)
                {
                    modifiedEntity.ModifiedDate = DateTime.UtcNow;
                    modifiedEntity.ModifiedBy = UserId;
                }
            }
        }

        public override int SaveChanges()
        {
            CheckAudit();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            CheckAudit();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            CheckAudit();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            CheckAudit();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Áp dụng cấu hình cho tất cả entity implement ICreatedBy
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                if (entity.ClrType.IsAssignableTo(typeof(ICreatedBy)))
                {
                    // Có thể thêm cấu hình chung nếu cần
                }
            }

            #region User
            modelBuilder.Entity<User>(e =>
            {
                e.ToTable("Users");
                e.HasKey(i => i.Id);
                e.HasMany(u => u.UserRoles)
                    .WithOne(c => c.User)
                    .HasForeignKey(u => u.UserId)
                    .OnDelete(DeleteBehavior.Restrict); // Đổi sang Restrict để tránh xóa cascade
                e.Property(u => u.Deleted).HasDefaultValue(false);
            });

            modelBuilder.Entity<UserRole>(e =>
            {
                e.ToTable("UserRoles");
                e.HasKey(ur => ur.Id);
                e.Property(ur => ur.Deleted).HasDefaultValue(false);
            });

            modelBuilder.Entity<Role>(e =>
            {
                e.ToTable("Roles");
                e.HasKey(r => r.Id);
                e.HasMany(r => r.UserRoles)
                    .WithOne(ur => ur.Role)
                    .HasForeignKey(ur => ur.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);
                e.HasMany(r => r.RolePermissions)
                    .WithOne(rp => rp.Role)
                    .HasForeignKey(rp => rp.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);
                e.Property(r => r.Deleted).HasDefaultValue(false);
            });

            modelBuilder.Entity<RolePermission>(e =>
            {
                e.ToTable("RolePermissions");
                e.HasKey(rp => rp.Id);
                e.Property(rp => rp.Deleted).HasDefaultValue(false);
            });

            modelBuilder.Entity<KeyPermission>(e =>
            {
                e.ToTable("KeyPermissions");
                e.HasKey(c => c.Id);
                e.Property(c => c.Deleted).HasDefaultValue(false);
            });

            modelBuilder.Entity<ApiEndpoint>(e =>
            {
                e.ToTable("ApiEndpoints");
                e.HasKey(a => a.Id);
            });

            modelBuilder.Entity<PermissionForApiEndpoint>(e =>
            {
                e.ToTable("PermissionForApiEndpoints");
                e.HasKey(p => p.Id);
            });
            #endregion

            #region Main Configuration
            modelBuilder.Entity<Post>(entity =>
            {
                entity.ToTable("Posts");
                entity.HasIndex(e => e.Slug).IsUnique();
                entity.HasIndex(e => e.PostCategoryId);
                entity.HasIndex(e => e.Type); // Thêm index cho Type
                entity.HasIndex(e => new { e.Id, e.Deleted }).HasDatabaseName("IX_Post");
                entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Slug).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(160).IsRequired();
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.TimePosted).IsRequired();
                entity.Property(e => e.Type).HasColumnType("int").IsRequired();
                entity.Property(e => e.Deleted).HasDefaultValue(false);
                entity.HasOne(p => p.PostCategory)
                    .WithMany(c => c.Posts)
                    .HasForeignKey(p => p.PostCategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PostCategory>(entity =>
            {
                entity.ToTable("PostCategory");
                entity.HasIndex(e => e.Slug).IsUnique();
                entity.HasIndex(e => e.Type); // Thêm index cho Type
                entity.HasIndex(e => new { e.Id, e.Deleted }).HasDatabaseName("IX_PostCategory");
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Slug).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(160);
                entity.Property(e => e.Type).HasColumnType("int").IsRequired();
                entity.Property(e => e.Deleted).HasDefaultValue(false);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");
                entity.HasIndex(e => e.Slug).IsUnique();
                entity.HasIndex(e => e.ProductCategoryId);
                entity.HasIndex(e => new { e.Id, e.Deleted }).HasDatabaseName("IX_Product");
                entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Slug).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(160).IsRequired();
                entity.Property(e => e.Deleted).HasDefaultValue(false);
                entity.HasOne(p => p.ProductCategory)
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.ProductCategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ProductCategory>(entity =>
            {
                entity.ToTable("ProductCategories");
                entity.HasIndex(e => e.Slug).IsUnique();
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Slug).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(160);
                entity.Property(e => e.Deleted).HasDefaultValue(false);
            });

            modelBuilder.Entity<Service>(entity =>
            {
                entity.ToTable("Services");
                entity.HasIndex(e => e.Slug).IsUnique();
                entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Slug).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(160).IsRequired();
                entity.Property(e => e.Deleted).HasDefaultValue(false);
            });

            modelBuilder.Entity<FileUpload>(entity =>
            {
                entity.ToTable("FileUploads");
                entity.HasKey(e => e.Id);
            });
            #endregion

            base.OnModelCreating(modelBuilder);
        }
    }
}