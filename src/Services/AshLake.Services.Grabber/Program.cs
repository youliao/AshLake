var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.AddCustomSwagger();
builder.AddCustomHttpClient();
builder.AddCustomDatabase();
builder.AddCustomRepositories();
builder.Services.AddMediatR(typeof(Program));
var app = builder.Build();

app.UseCustomSwagger();
app.MapControllers();
app.Run();