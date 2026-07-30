using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SeniorCrud.Domain.Constants;
using SeniorCrud.Domain.Entities;

namespace SeniorCrud.Persistence.Configurations;

public sealed class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.ToTable("Addresses");

        builder.HasKey(address => address.Id);

        builder.Property(address => address.Id)
            .ValueGeneratedNever();

        builder.Property(address => address.UserId)
            .IsRequired();

        builder.Property(address => address.Street)
            .HasMaxLength(DomainConstraints.MaxStreetLength)
            .IsRequired();

        builder.Property(address => address.Complement)
            .HasMaxLength(DomainConstraints.MaxComplementLength)
            .IsRequired(false);

        builder.Property(address => address.Neighborhood)
            .HasMaxLength(DomainConstraints.MaxNeighborhoodLength)
            .IsRequired();

        builder.Property(address => address.City)
            .HasMaxLength(DomainConstraints.MaxCityLength)
            .IsRequired();

        builder.Property(address => address.State)
            .HasMaxLength(DomainConstraints.StateLength)
            .IsFixedLength()
            .IsUnicode(false)
            .IsRequired();

        builder.Property(address => address.IsPrimary)
            .IsRequired();

        builder.Property(address => address.CreatedAt)
            .IsRequired();

        builder.Property(address => address.UpdatedAt)
            .IsRequired();

        builder.Property<byte[]>("RowVersion")
            .IsRowVersion()
            .IsConcurrencyToken();

        builder.OwnsOne(address => address.Cep, cep =>
        {
            cep.Property(value => value.Value)
                .HasColumnName("Cep")
                .HasMaxLength(DomainConstraints.CepLength)
                .IsUnicode(false)
                .IsRequired();

            cep.HasIndex(value => value.Value);
        });

        builder.Navigation(address => address.Cep)
            .IsRequired();

        builder.OwnsOne(address => address.Number, number =>
        {
            number.Property(value => value.Value)
                .HasColumnName("Number")
                .HasMaxLength(DomainConstraints.MaxAddressNumberLength)
                .IsRequired();
        });

        builder.Navigation(address => address.Number)
            .IsRequired();

        builder.HasIndex(address => address.UserId);

        builder.HasIndex(address => new { address.UserId, address.IsPrimary })
            .HasFilter("[IsPrimary] = 1")
            .IsUnique();
    }
}
