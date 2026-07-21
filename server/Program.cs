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

        if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken))
        {
            return Results.Problem("Operation failed: access token was not returned.");
        }

        httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

            var apiRequest = new
            {
                locations = new[]
                {
                    new
                    {
                        city = request.City,
                        state = request.State,
                        zip = request.Zip
                    }
                },
                unitOfMeasurement = "F"
            };

        var apiResponse = await httpClient.PostAsJsonAsync(
            "https://ami-interviewassessment.azurewebsites.net/WeatherData/ByLocation",
            apiRequest
        );

        apiResponse.EnsureSuccessStatusCode();

        var weatherData = await apiResponse.Content.ReadFromJsonAsync<object>();

        return Results.Ok(weatherData);
}
    catch (Exception ex)
    {
        var message = ex.Message.StartsWith("Operation failed:")
            ? ex.Message
            : $"Operation failed: {ex.Message}";

        return Results.Problem(message);
    }
});

app.Run();