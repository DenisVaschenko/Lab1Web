using Microsoft.Extensions.Options;

namespace Lab1Web.Configuration
{
    public class DataBaseConfiguration
    {
        public CourseConfiguration CourseConfiguration { get; set; } = new();
        public StudentConfiguration StudentConfiguration { get; set; } = new();
        public InstructorConfiguration InstructorConfiguration { get; set; } = new();
        public string ApiMode { get; set; }
        public int MaxAttaching { get; set; }
    }
    public abstract class GenericConfiguration
    {
        public string ApiMode { get; set; }
        public int MaxAttaching { get; set; }
    }
    public class ApiModeOptionsValidator<T> : IValidateOptions<T> where T : GenericConfiguration
    {
        private readonly string parentApiMode;
        public ApiModeOptionsValidator(IOptionsSnapshot<DataBaseConfiguration> options)
        {
            parentApiMode = options.Value.ApiMode; 
        }
        public ValidateOptionsResult Validate(string? s, T obj)
        {
            string childApiMode = obj.ApiMode;
            ValidateOptionsResult success = ValidateOptionsResult.Success;
            ValidateOptionsResult fail = ValidateOptionsResult.Fail(nameof(T) + "Error: api mode is incorrect");
            switch (parentApiMode)
            {
                case "full_access":
                    return childApiMode == "full_access" || childApiMode != "read_only" || childApiMode != "write_only"? success : fail;
                case "read_only":
                    return parentApiMode == childApiMode? success : fail;
                case "write_only":
                    return parentApiMode == childApiMode ? success : fail;
            }
            return fail;
        }
    }
    public class MaxAttachOptionsValidator<T> : IValidateOptions<T> where T : GenericConfiguration
    {
        private readonly int parentMaxAttaching;
        public MaxAttachOptionsValidator(IOptionsSnapshot<DataBaseConfiguration> options)
        {
            parentMaxAttaching = options.Value.MaxAttaching;
        }
        public ValidateOptionsResult Validate(string? s, T obj)
        {
            int childMaxAttaching = obj.MaxAttaching;
            ValidateOptionsResult success = ValidateOptionsResult.Success;
            ValidateOptionsResult fail = ValidateOptionsResult.Fail(nameof(T) + "Error: max attachments parameter is incorrect");
            return childMaxAttaching <= parentMaxAttaching? success : fail;
        }
    }

}
