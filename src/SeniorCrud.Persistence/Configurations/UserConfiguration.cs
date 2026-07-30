using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SeniorCrud.Domain.Constants;
using SeniorCrud.Domain.Entities;

namespace SeniorCrud.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(user => user.Id);

        builder.Property(user => user.Id)
            .ValueGeneratedNever();

        builder.Property(user => user.Name)
            .HasMaxLength(DomainConstraints.MaxUserNameLength)
            .IsRequired();

        builder.Property(user => user.BirthDate)
            .HasColumnType("date")
            .IsRequired(false);

        builder.Property(user => user.Role)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(user => user.IsActive)
            .IsRequired();

        builder.Property(user => user.CreatedAt)
            .IsRequired();

        builder.Property(user => user.UpdatedAt)
            .IsRequired();

        builder.Property<byte[]>("RowVersion")
            .IsRowVersion()
            .IsConcurrencyToken();

        builder.OwnsOne(user => user.Email, email =>
        {
            email.Property(value => value.Value)
                .HasColumnName("Email")
                .HasMaxLength(DomainConstraints.MaxEmailLength)
                .IsRequired();

            email.HasIndex(value => value.Value)
                .IsUnique();
        });

        builder.Navigation(user => user.Email)
            .IsRequired();

        builder.OwnsOne(user => user.Cpf, cpf =>
        {
            cpf.Property(value => value.Value)
                .HasColumnName("Cpf")
                .HasMaxLength(DomainConstraints.CpfLength)
                .IsUnicode(false)
                .IsRequired(false);

            cpf.HasIndex(value => value.Value)
                .IsUnique()
                .HasFilter("[Cpf] IS NOT NULL");
        });

        builder.Navigation(user => user.Cpf)
            .IsRequired(false);

        builder.OwnsOne(user => user.PasswordHash, passwordHash =>
        {
            passwordHash.Property(value => value.Value)
                .HasColumnName("PasswordHash")
                .HasMaxLength(DomainConstraints.MaxPasswordHashLength)
                .IsRequired();
        });

        builder.Navigation(user => user.PasswordHash)
            .IsRequired();

        builder.HasMany(user => user.Addresses)
            .WithOne()
            .HasForeignKey(address => address.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        var addressesNavigation = builder.Metadata.FindNavigation(nameof(User.Addresses));
        addressesNavigation?.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
