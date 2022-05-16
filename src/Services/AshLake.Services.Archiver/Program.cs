var builder = WebApplication.CreateBuilder(args);

builder.AddCustomProblemDetails();
builder.AddCustomControllers();
builder.AddCustomSwagger();
builder.AddCustomDatabase();
builder.AddCustomHttpClient();
builder.AddCustomRepositories();
builder.AddCustomBackgroundJobs();
builder.AddCustomAddHangfire();

builder.Services.AddMediatR(typeof(Program));

var app = builder.Build();

app.UseProblemDetails();
app.UseCustomSwagger();
app.MapControllers();
app.UseHangfireDashboard();

app.Run();