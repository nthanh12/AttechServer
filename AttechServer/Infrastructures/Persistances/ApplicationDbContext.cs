using AttechServer.Domains.Entities;
using AttechServer.Domains.Entities.Main;
using AttechServer.Domains.EntityBase;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AttechServer.Infrastructures.Persistances
{
    public class ApplicationDbContext : DbContext
    {
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly int? UserId = null;

        #region User
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<KeyPermission> KeyPermission { get; set; }
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

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
            var claims = _httpContextAccessor.HttpContext?.User?.Identity as ClaimsIdentity;
            var claim = claims?.FindFirst("user_id");
            if (claim != null && int.TryParse(claim.Value, out int userId))
            {
                UserId = userId;
            }
        }

        private void CheckAudit()
        {
            ChangeTracker.DetectChanges();
            var added = ChangeTracker.Entries()
                .Where(t => t.State == EntityState.Added)
                .Select(t => t.Entity)
                .AsParallel();

            added.ForAll(entity =>
            {
                if (entity is ICreatedBy createdEntity && createdEntity.CreatedBy == null)
                {
                    createdEntity.CreatedDate = DateTime.Now;
                    createdEntity.CreatedBy = UserId;
                }
            });

            var modified = ChangeTracker.Entries()
                        .Where(t => t.State == EntityState.Modified)
                        .Select(t => t.Entity)
                        .AsParallel();
            modified.ForAll(entity =>
            {
                if (entity is IModifiedBy modifiedEntity && modifiedEntity.ModifiedBy == null)
                {
                    modifiedEntity.ModifiedDate = DateTime.Now;
                    modifiedEntity.ModifiedBy = UserId;
                }
            });
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
            var entityTypes = modelBuilder.Model.GetEntityTypes();
            foreach (var entity in entityTypes)
            {
                if (entity.ClrType.IsAssignableTo(typeof(ICreatedBy)))
                {
                }
            }

            #region User
            modelBuilder.Entity<User>(e =>
            {
                e.HasKey(i => i.Id);
                e.HasMany(u => u.UserRoles).WithOne(c => c.User).HasForeignKey(u => u.UserId).OnDelete(DeleteBehavior.Cascade); ;

            });
            #endregion

            #region UserRole
            modelBuilder.Entity<UserRole>(e =>
            {
                e.HasKey(ur => ur.Id);
                e.Property(ur => ur.Deleted).HasDefaultValue(false);
            });
            #endregion

            #region  Role
            modelBuilder.Entity<Role>(e =>
            {
                e.HasKey(r => r.Id);
                e.HasMany(r => r.UserRoles)
                .WithOne(ur => ur.Role)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

                e.HasMany(r => r.RolePermissions)
                .WithOne(rp => rp.Role)
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

                e.Property(r => r.Deleted).HasDefaultValue(false);
            });
            #endregion

            #region RolePermission
            modelBuilder.Entity<RolePermission>(e =>
            {
                e.HasKey(rp => rp.Id);

                e.Property(rp => rp.Deleted).HasDefaultValue(false);
            });
            #endregion

            modelBuilder.Entity<KeyPermission>(e =>
            {
                e.HasKey(c => c.Id);
            });

            #region Main Configuration
            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasIndex(e => e.Slug).IsUnique();
                entity.HasIndex(e => e.PostCategoryId);
                entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Slug).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(160).IsRequired();
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.TimePosted).IsRequired();
                entity.HasIndex(e => e.Slug).IsUnique();
                entity.HasIndex(e => e.PostCategoryId);
                entity.HasIndex(e => new { e.Id, e.Deleted }).HasDatabaseName("IX_Post");
                entity.HasOne(p => p.PostCategory)
                .WithMany(c => c.Posts)
                .HasForeignKey(p => p.PostCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PostCategory>(entity =>
            {
                entity.HasIndex(e => e.Slug).IsUnique();
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Slug).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(160);
                entity.HasIndex(e => new { e.Id, e.Deleted }).HasDatabaseName("IX_PostCategory");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasIndex(e => e.Slug).IsUnique();
                entity.HasIndex(e => e.ProductCategoryId);
                entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Slug).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(160).IsRequired();
                entity.HasIndex(e => e.Slug).IsUnique();
                entity.HasIndex(e => e.ProductCategoryId);
                entity.HasIndex(e => new { e.Id, e.Deleted }).HasDatabaseName("IX_Product");
                entity.HasOne(p => p.ProductCategory)
                      .WithMany(c => c.Products)
                      .HasForeignKey(p => p.ProductCategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ProductCategory>(entity =>
            {
                entity.HasIndex(e => e.Slug).IsUnique();
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Slug).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(160);

            });

            modelBuilder.Entity<Service>(entity =>
            {
                entity.HasIndex(e => e.Slug).IsUnique();
                entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Slug).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(160).IsRequired();
                entity.HasIndex(e => e.Slug).IsUnique();
            });

            modelBuilder.Entity<FileUpload>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            #endregion

            base.OnModelCreating(modelBuilder);
        }
    }
}
