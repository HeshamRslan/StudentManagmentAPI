using FastEndpoints;
using FastEndpoints.Swagger;
using StudentManagementAPI;
using StudentManagementAPI.SeedData;
using StudentManagementAPI.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument(options =>
{
    options.DocumentSettings = s =>
    {
        s.Title = "Student Management API";
        s.Version = "v1";
    };
});


builder.Services.AddSingleton<ClassService>();
builder.Services.AddSingleton<StudentService>();
builder.Services.AddSingleton<EnrollmentService>();
builder.Services.AddSingleton<MarkService>();
builder.Services.AddSingleton<ArchiveService>();


builder.Services.AddHostedService<ClassArchiveBackgroundService>();


builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var studentService = scope.ServiceProvider.GetRequiredService<StudentService>();
    var classService = scope.ServiceProvider.GetRequiredService<ClassService>();
    var enrollmentService = scope.ServiceProvider.GetRequiredService<EnrollmentService>();
    var markService = scope.ServiceProvider.GetRequiredService<MarkService>();

    DataSeeder.Seed(studentService, classService, enrollmentService, markService);
}
app.UseFastEndpoints();   

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();      
    app.UseSwaggerUI();   
}


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.Run();
