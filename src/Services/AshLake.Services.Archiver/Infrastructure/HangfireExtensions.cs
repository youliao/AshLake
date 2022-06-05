using Hangfire.Dashboard;

namespace AshLake.Services.Archiver.Infrastructure;

public class HangfireExtensions : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        return true;
    }
}
