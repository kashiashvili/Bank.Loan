using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Bank.Loan.Persistence.Configurations;

public class LoanConfiguration : IEntityTypeConfiguration<Domain.Loan>
{
    public void Configure(EntityTypeBuilder<Domain.Loan> builder)
    {
        builder.Property(e => e.Type)
            .HasMaxLength(256)
            .HasConversion(new EnumToStringConverter<Domain.Enums.LoanType>());
        builder.Property(e => e.Amount).HasColumnType("decimal(18,2)");
        builder.Property(e => e.Ccy).HasColumnType("varchar(3)");
        builder.Property(e => e.Status)
            .HasMaxLength(256)
            .HasConversion(new EnumToStringConverter<Domain.Enums.LoanStatus>());
    }
}