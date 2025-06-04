namespace MiddleWare.HealthCheck
{
    public class HealthCheckResult
    {
        public string Name { get; set; }

        public HealthStatus HealthStatus { get; set; }

        public string Description { get; set; }

        public TimeSpan Duration { get; set; }
    }
}
