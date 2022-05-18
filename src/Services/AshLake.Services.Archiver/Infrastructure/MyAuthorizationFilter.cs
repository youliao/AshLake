using Hangfire.Dashboard;

namespace AshLake.Services.Archiver.Infrastructure;

public class MyAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        return true;
    }
}
