using Microsoft.EntityFrameworkCore;
using Lab1Web.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lab1Web.EntityConfigurations
{
    public class InstructorConfiguration : IEntityTypeConfiguration<Instructor>
    {
        public void Configure(EntityTypeBuilder<Instructor> builder)
        {
            builder.Property(e => e.Age).HasDefaultValue(30);
            builder.Property(e => e.Phone).IsRequired();
            builder.Property(e => e.Id).ValueGeneratedOnAdd().IsRequired().HasColumnType("INTEGER PRIMARY KEY");
            builder.Property(e => e.Degree).IsRequired();
        }
    }
}
