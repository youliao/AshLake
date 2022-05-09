const string appName = "Grabber API";

var builder = WebApplication.CreateBuilder(args);


builder.AddCustomSwagger();
builder.AddCustomHttpClient();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCarter();
builder.Services.AddMediatR(typeof(Program));
var app = builder.Build();

app.UseCustomSwagger();

app.MapCarter();

app.Run();