using server.Models;

var builder = WebApplication.CreateBuilder(args);

// Register OpenAPI documentation and HttpClient support
builder.Services.AddOpenApi();
builder.Services.AddHttpClient();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Receive a location from the frontend and return current and historical weather data
app.MapPost("/api/weather", async (
    WeatherRequest request,
    HttpClient httpClient
    ) =>
{
    try
    {
        // Request an access token before calling the weather endpoints
        var tokenResponse = await httpClient.GetFromJsonAsync<TokenResponse>(
            "https://ami-interviewassessment.azurewebsites.net/Auth/AccessToken"
        );

        if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken))
        {
            return Results.Problem("Operation failed: access token was not returned.");
        }

        // Attach the token response to following API requests
        httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

        // Build the location request in expected format
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

        // Request the current weather data
        var apiResponse = await httpClient.PostAsJsonAsync(
            "https://ami-interviewassessment.azurewebsites.net/WeatherData/ByLocation",
            apiRequest
        );

        apiResponse.EnsureSuccessStatusCode();

        var weatherData = await apiResponse.Content.ReadFromJsonAsync<object>();

        if (weatherData == null)
        {
            return Results.Problem("Operation failed: weather data was not returned.");
        }

        // Request the previous 12 months of high temperatures
        var historicalResponse = await httpClient.PostAsJsonAsync(
            "https://ami-interviewassessment.azurewebsites.net/WeatherData/ByLocation/HighestTemps",
            apiRequest
        );

        historicalResponse.EnsureSuccessStatusCode();

        var historicalData = await historicalResponse.Content.ReadFromJsonAsync<List<HighestTemperature>>();

        // Verify historical API returned usable values
        if (historicalData == null || historicalData.Count == 0 || historicalData[0].Rolling12MonthTemps == null || historicalData[0].Rolling12MonthTemps.Count == 0)
        {
            return Results.Problem("Operation failed: historical weather data was not returned.");
        }

        var monthlyTemperatures = historicalData[0].Rolling12MonthTemps;

        // Calculate average of returned monthly high temperatures
        double totalTemperature = 0;
        for (int i = 0; i < monthlyTemperatures.Count; i++)
        {
            totalTemperature += monthlyTemperatures[i];
        }

        double averageTemperature = totalTemperature / monthlyTemperatures.Count;
        var averageHigh = Math.Round(averageTemperature, 1);

        // Return both datasets to frontend in one response
        return Results.Ok(new
        {
            current = weatherData,
            averageHigh
        });
    }
    catch (Exception ex)
    {
        // Return error message for failed external API requests
        var message = ex.Message.StartsWith("Operation failed:")
            ? ex.Message
            : $"Operation failed: {ex.Message}";

        return Results.Problem(message);
    }
});

app.Run();