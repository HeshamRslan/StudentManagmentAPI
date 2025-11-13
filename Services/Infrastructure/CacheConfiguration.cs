namespace StudentManagementAPI.Services.Infrastructure
{
    public class CacheConfiguration
    {
        public TimeSpan StudentsCacheExpiration { get; set; } = TimeSpan.FromMinutes(5);
        public TimeSpan ClassesCacheExpiration { get; set; } = TimeSpan.FromMinutes(10);
        public TimeSpan EnrollmentsCacheExpiration { get; set; } = TimeSpan.FromMinutes(3);
        public TimeSpan MarksCacheExpiration { get; set; } = TimeSpan.FromMinutes(2);
        public TimeSpan ReportsCacheExpiration { get; set; } = TimeSpan.FromSeconds(30);
        public bool EnableCaching { get; set; } = true;
    }
}