var builder = WebApplication.CreateBuilder(args);

builder.AddCustomSwagger();
builder.AddCustomControllers();
builder.AddCustomDatabase();
builder.AddCustomRepositories();
builder.AddCustomEasyCaching();

var app = builder.Build();

app.UseCustomSwagger();
app.MapControllers();

app.Run();