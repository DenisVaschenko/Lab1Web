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
            builder.Property(e => e.Phone).IsRequired();
            builder.Property(e => e.Id).ValueGeneratedOnAdd().IsRequired().HasColumnType("INTEGER PRIMARY KEY");
            builder.Property(e => e.AverageScore).HasDefaultValue(75);
        }
    }
}
