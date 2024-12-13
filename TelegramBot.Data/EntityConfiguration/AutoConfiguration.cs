using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TelegramBot.Domain.Models;

namespace TelegramBot.Data.EntityConfiguration;

public class AutoConfiguration : IEntityTypeConfiguration<Auto>
{
    public void Configure(EntityTypeBuilder<Auto> builder)
    {
        builder.ToTable("Auto");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id);
        builder.Property(x => x.Brand).IsRequired();
        builder.Property(x => x.YearofIssue).IsRequired();
        builder.Property(x => x.Body).IsRequired();
        builder.Property(x => x.SeatInTheCabin).IsRequired();
        builder.Property(x => x.FuelType).IsRequired();
        builder.Property(x => x.EngineSize).IsRequired();
        builder.Property(x => x.Transmission).IsRequired();
        builder.Property(x => x.Drive).IsRequired();
        builder.Property(x => x.Mileage).IsRequired();
        builder.Property(x => x.Registration).IsRequired();
        
        builder.HasOne<Person>()
            .WithMany(c => c.Autos)
            .HasForeignKey(a => a.PersonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}