using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace LSC.OnlineCourse.API.Common
{
    public static class HealthCheckResponseWriter
    {
        public static async Task WriteJsonResponse(HttpContext context, HealthReport report)
        {
            context.Response.ContentType = "application/json";
            var json = new
            {
                status = report.Status.ToString(),
                results = report.Entries.ToDictionary(
                    kvp => kvp.Key,
                    kvp => new
                    {
                        status = kvp.Value.Status.ToString(),
                        description = kvp.Value.Description,
                        exception = kvp.Value.Exception?.Message,
                        duration = kvp.Value.Duration.ToString()
                    })
            };
            await context.Response.WriteAsJsonAsync(json);
        }
    }


}
