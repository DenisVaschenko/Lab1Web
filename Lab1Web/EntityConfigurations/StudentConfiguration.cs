using Microsoft.EntityFrameworkCore;
using Lab1Web.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lab1Web.EntityConfigurations
{
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.Property(e => e.Age).HasDefaultValue(18);
            builder.Property(e => e.Phone).IsRequired().HasColumnType("UNIQUE");
            builder.Property(e => e.Id).ValueGeneratedOnAdd().IsRequired();
            builder.Property(e => e.AverageScore).HasDefaultValue(75);
        }
    }
}
