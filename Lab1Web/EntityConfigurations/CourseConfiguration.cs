using Microsoft.EntityFrameworkCore;
using Lab1Web.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lab1Web.EntityConfigurations
{
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.Property(e => e.Difficulty).HasDefaultValue("medium");
            builder.Property(e => e.Id).ValueGeneratedOnAdd().IsRequired();

            builder.HasMany(e => e.Students).WithMany(e => e.Courses);
            builder.HasMany(e => e.Instructors).WithMany(e => e.Courses);
        }
    }
}
