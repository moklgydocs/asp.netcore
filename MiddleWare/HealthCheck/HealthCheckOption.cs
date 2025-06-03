namespace MiddleWare.HealthCheck
{
    public class HealthCheckOption
    {
        public string HealthEndpointPath { get; set; } = "/healthcheck";

        public List<Func<HttpContext, Task<HealthCheckResult>>> HealthChecks { get; } = new List<Func<HttpContext, Task<HealthCheckResult>>>();
    }
}
