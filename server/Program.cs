using server.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddHttpClient();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapPost("/api/weather", async (
    WeatherRequest request,
    HttpClient httpClient
    ) =>
{
    try
    {
        var tokenResponse = await httpClient.GetFromJsonAsync<TokenResponse>(
            "https://ami-interviewassessment.azurewebsites.net/Auth/AccessToken"
        );

    return Results.Ok(new
    {
        city = request.City,
        state = request.State,
        zip = request.Zip,
        token = tokenResponse?.AccessToken
    });
}
    catch (Exception ex)
    {
        return Results.Problem($"Error fetching token: {ex.Message}");
    }
});

app.Run();