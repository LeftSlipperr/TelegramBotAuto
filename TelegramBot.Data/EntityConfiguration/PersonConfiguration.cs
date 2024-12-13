using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TelegramBot.Domain.Models;

namespace TelegramBot.Data.EntityConfiguration;

public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.ToTable("Person");
        builder.Property(c => c.Id);
        builder.Property(c => c.Name).HasMaxLength(100).IsRequired();
        builder.Property(c => c.SecondName).IsRequired();
        builder.Property(c => c.ThirdName);
        builder.Property(c => c.PhoneNumber).IsRequired();
        builder.Property(c => c.UserName).IsRequired();
        
        builder.HasKey(с => с.Id);
    }
}