using Microsoft.EntityFrameworkCore;

namespace Easy.Net.Starter.EntityFramework
{
    public class BaseDbContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }

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
    }
}
