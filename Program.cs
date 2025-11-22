using FastEndpoints;
using FastEndpoints.Swagger;
using StudentManagementAPI.SeedData;
using StudentManagementAPI.Services;
using StudentManagementAPI.Services.Interfaces;
using StudentManagementAPI.Services.Repositories;
using StudentManagementAPI.Services.Infrastructure;
using StudentManagementAPI.Services.Infrastructure.Logging;
using Serilog;

// ============ CONFIGURE SERILOG EARLY ============
Log.Logger = LoggingConfiguration.CreateLogger(
    new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
        .Build()
);

try
{
    Log.Information("Starting Student Management API...");

    var builder = WebApplication.CreateBuilder(args);

    // ============ REPLACE DEFAULT LOGGING WITH SERILOG ============
    builder.Host.UseSerilog();

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
        options.SizeLimit = 1024;
        options.CompactionPercentage = 0.25;
    });
    builder.Services.AddSingleton<ICacheService, CacheService>();
    builder.Services.AddSingleton<IMetricsService, MetricsService>();

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

    // Add Serilog as a singleton service
    builder.Services.AddSingleton<Serilog.ILogger>(Log.Logger);

    var app = builder.Build();

    // ============ MIDDLEWARE PIPELINE ============
    // IMPORTANT: Order matters!

    // 1. Exception handling (must be first)
    app.UseGlobalExceptionHandler();

    // 2. Serilog request logging with enrichment
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms";
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
            diagnosticContext.Set("RemoteIP", httpContext.Connection.RemoteIpAddress?.ToString());
        };
    });

    // 3. Performance logging
    app.UsePerformanceLogging();

    // 4. Standard middleware
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();

    // 5. FastEndpoints
    app.UseFastEndpoints();

    // 6. Swagger (development only)
    if (app.Environment.IsDevelopment())
    {
        app.UseOpenApi();
        app.UseSwaggerUi();
    }

    // ============ SEED DATA ============
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            Log.Information("Seeding database...");

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

            Log.Information("Database seeded successfully");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while seeding database");
            throw;
        }
    }

    Log.Information("Student Management API started successfully");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application startup failed");
    throw;
}
finally
{
    Log.CloseAndFlush();
}