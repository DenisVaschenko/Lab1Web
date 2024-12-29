using Lab1Web;
using Lab1Web.Entities;
using Microsoft.EntityFrameworkCore.Design;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using FluentValidation;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Lab1Web.Configuration;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DataModelContext>(
    contextOptions => contextOptions.UseSqlite("Data Source = MyDatabase.db"));
builder.Services.AddScoped(typeof(IValidateOptions<>), typeof(ApiModeOptionsValidator<>));
builder.Services.AddScoped(typeof(IValidateOptions<>), typeof(MaxAttachOptionsValidator<>));
builder.Services.AddOptions<DataBaseConfiguration>()
    .BindConfiguration("DataBaseConfiguration");
builder.Services.AddOptions<StudentConfiguration>()
    .BindConfiguration("DataBaseConfiguration:StudentConfiguration");
builder.Services.AddOptions<CourseConfiguration>()
    .BindConfiguration("DataBaseConfiguration:CourseConfiguration");
builder.Services.AddOptions<InstructorConfiguration>()
    .BindConfiguration("DataBaseConfiguration:InstructorConfiguration");
builder.Services.AddControllers();
builder.Services.AddOutputCache(opt =>
{
    opt.AddBasePolicy(b => b.Expire(TimeSpan.FromSeconds(60)).SetVaryByQuery("*"));
});
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    // ϳ�������� XML-��������
    options.IncludeXmlComments(xmlPath);

    // �����������: ������������ Swagger-���������
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1",
        Description = "API ��� ������ � ���������� ��������."
    });
});
builder.Services.AddValidatorsFromAssemblyContaining<CourseValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<StudentValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<InstructorValidator>();
builder.Services.AddRepositories();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseOutputCache();
app.MapControllers();

app.Run();
