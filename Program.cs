using FastEndpoints;
using FastEndpoints.Swagger;
using StudentManagementAPI.SeedData;
using StudentManagementAPI.Services;
using StudentManagementAPI.Services.Interfaces;
using StudentManagementAPI.Services.Repositories;
using StudentManagementAPI.Services.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ============ CACHE CONFIGURATION ============
var cacheConfig = new CacheConfiguration
{
    EnableCaching = builder.Configuration.GetValue<bool>("CacheSettings:EnableCaching", true),
    StudentsCacheExpiration = TimeSpan.FromMinutes(builder.Configuration.GetValue<int>("CacheSettings:StudentsExpirationMinutes", 5)),
    ClassesCacheExpiration = TimeSpan.FromMinutes(builder.Configuration.GetValue<int>("CacheSettings:ClassesExpirationMinutes", 10)),
    EnrollmentsCacheExpiration = TimeSpan.FromMinutes(builder.Configuration.GetValue<int>("CacheSettings:EnrollmentsExpirationMinutes", 3)),
    MarksCacheExpiration = TimeSpan.FromMinutes(builder.Configuration.GetValue<int>("CacheSettings:MarksExpirationMinutes", 2)),
    ReportsCacheExpiration = TimeSpan.FromSeconds(builder.Configuration.GetValue<int>("CacheSettings:ReportsExpirationSeconds", 30))
};
builder.Services.AddSingleton(cacheConfig);

// ============ REPOSITORIES (Singleton - Stateful in-memory stores) ============
builder.Services.AddSingleton<IClassRepository, ClassRepository>();
builder.Services.AddSingleton<IStudentRepository, StudentRepository>();
builder.Services.AddSingleton<IEnrollmentRepository, EnrollmentRepository>();
builder.Services.AddSingleton<IMarkRepository, MarkRepository>();

// ============ INFRASTRUCTURE SERVICES ============
builder.Services.AddMemoryCache(options =>
{
    options.SizeLimit = 1024; // Limit cache size
    options.CompactionPercentage = 0.25; // Compact 25% when limit reached
});
builder.Services.AddSingleton<ICacheService, CacheService>();

// ============ SERVICES (Scoped - Better for future DB integration) ============
builder.Services.AddScoped<IClassService, ClassService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<IMarkService, MarkService>();
builder.Services.AddScoped<IArchiveService, ArchiveService>();

// ============ BACKGROUND SERVICES ============
builder.Services.AddHostedService<ClassArchiveBackgroundService>();

// ============ AUTHENTICATION & AUTHORIZATION ============
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

// ============ FAST ENDPOINTS ============
builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument(options =>
{
    options.DocumentSettings = s =>
    {
        s.Title = "Student Management API";
        s.Version = "v1";
        s.Description = "A comprehensive student management system with classes, enrollments, and marks tracking";
    };
});

var app = builder.Build();

// ============ SEED DATA ============
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var studentService = services.GetRequiredService<IStudentService>();
    var classService = services.GetRequiredService<IClassService>();
    var enrollmentService = services.GetRequiredService<IEnrollmentService>();
    var markService = services.GetRequiredService<IMarkService>();

    DataSeeder.Seed(
        (StudentService)studentService,
        classService,
        (EnrollmentService)enrollmentService,
        (MarkService)markService
    );
}

// ============ MIDDLEWARE PIPELINE ============
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseFastEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
}

app.Run();