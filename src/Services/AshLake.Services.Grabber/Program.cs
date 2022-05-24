var builder = WebApplication.CreateBuilder(args);

builder.AddCustomSwagger();
builder.AddCustomControllers();
builder.AddCustomProblemDetails();
builder.AddCustomDatabase();
builder.AddCustomRepositories();
builder.AddCustomEasyCaching();

var app = builder.Build();

app.UseCustomSwagger();
app.MapControllers();

app.Run();