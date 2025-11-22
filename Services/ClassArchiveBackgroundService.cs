using StudentManagementAPI.Services;
using StudentManagementAPI.Services.Interfaces;

public class ClassArchiveBackgroundService : BackgroundService
{
    private readonly IServiceProvider _sp;

    public ClassArchiveBackgroundService(IServiceProvider sp)
    {
        _sp = sp;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _sp.CreateScope();

            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var archiveSettings = config.GetSection("ArchiveSettings");

            bool isEnabled = archiveSettings.GetValue<bool>("Enabled", true);
            int intervalMinutes = archiveSettings.GetValue<int>("IntervalMinutes", 10);
            int archiveAfterMonths = archiveSettings.GetValue<int>("ArchiveAfterMonths", 6);

            if (!isEnabled)
            {
                await Task.Delay(TimeSpan.FromMinutes(intervalMinutes), stoppingToken);
                continue;
            }
            var classService = scope.ServiceProvider.GetRequiredService<IClassService>(); // ✓ WORKS
            var enrollmentService = scope.ServiceProvider.GetRequiredService<IEnrollmentService>(); // ✓ WORKS
            var archiveService = scope.ServiceProvider.GetRequiredService<ArchiveService>();

            var cutoff = DateTime.UtcNow.AddMonths(-archiveAfterMonths);
            var oldClasses = classService.GetAll().Where(c => c.CreatedAt < cutoff).ToList();

            foreach (var cls in oldClasses)
            {
                var enrolled = enrollmentService.GetByClassId(cls.Id).ToList();
                if (enrolled.Any()) continue;

                if (archiveService.Archive(cls))
                {
                    classService.Delete(cls.Id);
                }
            }

            await Task.Delay(TimeSpan.FromMinutes(intervalMinutes), stoppingToken);
        }
    }
}
