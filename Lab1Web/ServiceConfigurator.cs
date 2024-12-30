using Lab1Web.Configuration;
using Microsoft.Extensions.Options;
using FluentValidation;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
namespace Lab1Web
{
    public static class ServiceConfigurator
    {
        public static void Configure(IServiceCollection Services)
        {
            Services.AddDbContext<DataModelContext>(
                contextOptions => contextOptions.UseSqlite("Data Source = MyDatabase.db"));
            Services.AddScoped(typeof(IValidateOptions<>), typeof(ApiModeOptionsValidator<>));
            Services.AddScoped(typeof(IValidateOptions<>), typeof(MaxAttachOptionsValidator<>));
            Services.AddOptions<DataBaseConfiguration>()
                .BindConfiguration("DataBaseConfiguration");
            Services.AddOptions<StudentConfiguration>()
                .BindConfiguration("DataBaseConfiguration:StudentConfiguration");
            Services.AddOptions<CourseConfiguration>()
                .BindConfiguration("DataBaseConfiguration:CourseConfiguration");
            Services.AddOptions<InstructorConfiguration>()
                .BindConfiguration("DataBaseConfiguration:InstructorConfiguration");
            Services.AddControllers();
            Services.AddControllers();
            Services.AddOutputCache(opt =>
            {
                opt.AddBasePolicy(b => b.Expire(TimeSpan.FromSeconds(60)).SetVaryByQuery("*"));
            });
            Services.AddCors(options =>
                options.AddPolicy("AllowAll", builder =>
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
            Services.AddAutoMapper(Assembly.GetExecutingAssembly());
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            Services.AddEndpointsApiExplorer();
            Services.AddSwaggerGen(options =>
            {
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                // Підключити XML-коментарі
                options.IncludeXmlComments(xmlPath);

                // Опціонально: налаштування Swagger-документа
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "My API",
                    Version = "v1",
                    Description = "API для роботи з навчальним закладом."
                });
            });
            Services.AddValidatorsFromAssemblyContaining<CourseValidator>();
            Services.AddValidatorsFromAssemblyContaining<StudentValidator>();
            Services.AddValidatorsFromAssemblyContaining<InstructorValidator>();
            Services.AddRepositories();
        }
    }
}
