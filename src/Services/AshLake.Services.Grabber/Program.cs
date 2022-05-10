var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.AddCustomSwagger();
builder.AddCustomHttpClient();
builder.AddCustomDatabase();
builder.AddCustomRepositories();

builder.Services.AddMediatR(typeof(Program));

var app = builder.Build();

if (true)
{
    app.UseExceptionHandler(exceptionHandlerApp =>
    {
        exceptionHandlerApp.Run(async context =>
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            context.Response.ContentType = MediaTypeNames.Application.Json;

            var exceptionHandlerPathFeature =
                context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();

            var prob = new ProblemDetails()
            {
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
                Status = StatusCodes.Status500InternalServerError,
                Detail = exceptionHandlerPathFeature?.Error.Message,
                Title = exceptionHandlerPathFeature?.Error.GetType().ToString(),
                Instance = context.Request.Path,
            };
            await context.Response.WriteAsJsonAsync(prob);
        });
    });

    app.UseHsts();
}

app.UseCustomSwagger();
app.MapControllers();

app.Run();