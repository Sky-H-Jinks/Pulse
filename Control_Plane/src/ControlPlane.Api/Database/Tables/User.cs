using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControlPlane.Api.Database.Tables;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = null!; // Used for login
    public string PasswordHash { get; set; } = null!; 
    public string Email { get; set; } = null!; // Used for username recovery and email verification
    public bool EmailConfirmed { get; set; } = false;
}

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users", t => t.HasCheckConstraint(
            "ck_users_email_format",
            "\"Email\" ~* '^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$'"));

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .ValueGeneratedOnAdd();
        
        builder.Property(u => u.Username)
            .HasMaxLength(50)
            .HasColumnType("citext")
            .IsRequired();
        
        builder.Property(u => u.PasswordHash)
            .HasMaxLength(256)
            .IsRequired();
        
        builder.Property(u => u.Email)
            .HasMaxLength(254)
            .HasColumnType("citext")
            .IsRequired();

        builder.Property(u => u.EmailConfirmed)
            .HasDefaultValue(false)
            .IsRequired();

        builder.HasIndex(u => u.Username).IsUnique();
        builder.HasIndex(u => u.Email).IsUnique();
    }
}