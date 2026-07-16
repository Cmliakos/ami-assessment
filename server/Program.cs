var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/api/test", () =>
{
    return Results.Ok(new
    {
        message = "Hello World!"
    });
});

app.Run();