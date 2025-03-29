using Microsoft.EntityFrameworkCore;

namespace Easy.Net.Starter.EntityFramework
{
    public class BaseDbContext : DbContext
    {
        public BaseDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<EmailValidation> EmailValidations { get; set; }
        public virtual DbSet<TwoFactorCode> TwoFactorCodes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            ConfigureUser(modelBuilder);
            ConfigureEmailValidation(modelBuilder);
            ConfigureTwoFactorCode(modelBuilder);
        }

        protected virtual void ConfigureUser(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("User_pkey");

                entity.ToTable("User");

                entity.HasIndex(e => e.Email, "User_email_key").IsUnique();

                entity.Property(e => e.Id)
                    .HasDefaultValueSql("gen_random_uuid()")
                    .HasColumnName("id");

                entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .HasColumnName("email");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(50)
                    .HasColumnName("first_name");

                entity.Property(e => e.IsAdmin)
                    .HasDefaultValue(false)
                    .HasColumnName("is_admin");

                entity.Property(e => e.IsLocked)
                    .HasDefaultValue(false)
                    .HasColumnName("is_locked");

                entity.Property(e => e.LastName)
                    .HasMaxLength(50)
                    .HasColumnName("last_name");

                entity.Property(e => e.Password)
                    .HasMaxLength(255)
                    .HasColumnName("password");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(20)
                    .HasColumnName("phone_number");

                entity.Property(e => e.RegistrationDate)
                    .HasDefaultValueSql("now()")
                    .HasColumnName("registration_date");
            });
        }

        protected virtual void ConfigureEmailValidation(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EmailValidation>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("EmailValidation_pkey");

                entity.ToTable("EmailValidation");

                entity.Property(e => e.Id)
                      .HasDefaultValueSql("gen_random_uuid()")
                      .HasColumnName("id");

                entity.Property(e => e.UserId)
                      .HasColumnName("user_id");

                entity.Property(e => e.Token)
                      .IsRequired()
                      .HasMaxLength(500)
                      .HasColumnName("token");

                entity.Property(e => e.ExpiresAt)
                      .HasColumnName("expires_at");

                entity.Property(e => e.Used)
                      .HasColumnName("used");

                entity.Property(e => e.CreatedAt)
                      .HasDefaultValueSql("now()") // si vous voulez que la DB gère la valeur
                      .HasColumnName("created_at");

                entity.Property(e => e.VerifiedAt)
                      .HasColumnName("verified_at");

                entity.HasOne(e => e.User)
                      .WithMany(u => u.EmailValidations)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .HasConstraintName("EmailValidation_UserId_fkey");
            });
        }

        protected virtual void ConfigureTwoFactorCode(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresEnum<TwoFactorCodeType>();
            modelBuilder.Entity<TwoFactorCode>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("TwoFactorCode_pkey");

                entity.ToTable("TwoFactorCode");

                entity.Property(e => e.Id)
                      .HasDefaultValueSql("gen_random_uuid()")
                      .HasColumnName("id");

                entity.Property(e => e.UserId)
                      .HasColumnName("user_id");

                entity.Property(e => e.Code)
                      .IsRequired()
                      .HasMaxLength(6)
                      .HasColumnName("code");

                entity.Property(e => e.ExpiresAt)
                      .HasColumnName("expires_at");

                entity.Property(e => e.Used)
                      .HasColumnName("used");

                entity.Property(e => e.CreatedAt)
                      .HasDefaultValueSql("now()")
                      .HasColumnName("created_at");

                entity.Property(e => e.Type)
                      .HasConversion<string>()
                      .HasColumnName("type");

                entity.HasOne(e => e.User)
                      .WithMany(u => u.TwoFactorCodes)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .HasConstraintName("TwoFactorCode_UserId_fkey");
            });
        }
    }
}
