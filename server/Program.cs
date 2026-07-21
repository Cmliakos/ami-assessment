using server.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapPost("/api/weather", (WeatherRequest request) =>
{
    return Results.Ok(new
    {
        city = request.City,
        state = request.State,
        zip = request.Zip
    });
});

app.Run();