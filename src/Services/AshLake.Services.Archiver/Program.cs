using Hellang.Middleware.ProblemDetails;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDaprClient();
builder.Services.AddMediatR(typeof(Program));

builder.AddCustomProblemDetails();
builder.AddCustomControllers();
builder.AddCustomSwagger();
builder.AddCustomDatabase();
builder.AddCustomGrabberServices();
builder.AddCustomRepositories();
builder.AddCustomBackgroundJobs();
builder.AddCustomAddHangfire();

var app = builder.Build();

app.UseProblemDetails();
app.UseCustomSwagger();
app.MapControllers();
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new AshLake.Services.Archiver.Infrastructure.MyAuthorizationFilter() }
});

app.Run();