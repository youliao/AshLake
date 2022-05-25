using Hellang.Middleware.ProblemDetails;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDaprClient();
builder.Services.AddMediatR(typeof(Program));

builder.AddCustomProblemDetails();
builder.AddCustomControllers();
builder.AddCustomSwagger();
builder.AddCustomRepositories();
builder.AddCustomGrabberServices();
builder.AddCustomBackgroundJobs();
builder.AddCustomAddHangfire();
builder.AddCustomApplicationServices();

var app = builder.Build();

app.UseProblemDetails();
app.UseCustomSwagger();
app.UseCloudEvents();
app.MapControllers();
app.MapSubscribeHandler();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new AshLake.Services.Archiver.Infrastructure.MyAuthorizationFilter() }
});

app.Run();