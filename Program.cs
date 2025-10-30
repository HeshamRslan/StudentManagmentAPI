using FastEndpoints;
using FastEndpoints.Swagger;
using StudentManagementAPI.SeedData;
using StudentManagementAPI.Services;
using StudentManagementAPI.Services.Interfaces;
using StudentManagementAPI.Services.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ============ REPOSITORIES (Singleton - Stateful in-memory stores) ============
builder.Services.AddSingleton<IClassRepository, ClassRepository>();
builder.Services.AddSingleton<IStudentRepository, StudentRepository>();
builder.Services.AddSingleton<IEnrollmentRepository, EnrollmentRepository>();
builder.Services.AddSingleton<IMarkRepository, MarkRepository>();

// ============ SERVICES (Scoped - Better for future DB integration) ============
// Note: Using Scoped instead of Singleton for better lifecycle management
builder.Services.AddScoped<IClassService, ClassService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<IMarkService, MarkService>();
builder.Services.AddScoped<IArchiveService, ArchiveService>();

// ============ BACKGROUND SERVICES ============
builder.Services.AddHostedService<ClassArchiveBackgroundService>();

// ============ INFRASTRUCTURE ============
builder.Services.AddMemoryCache();
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