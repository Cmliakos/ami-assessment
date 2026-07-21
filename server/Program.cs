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

        if (weatherData == null)
        {
            return Results.Problem("Operation failed: weather data was not returned.");
        }

        var historicalResponse = await httpClient.PostAsJsonAsync(
            "https://ami-interviewassessment.azurewebsites.net/WeatherData/ByLocation/HighestTemps",
            apiRequest
        );

        historicalResponse.EnsureSuccessStatusCode();

        var historicalData = await historicalResponse.Content.ReadFromJsonAsync<List<HighestTemperature>>();
        
        if (historicalData == null || historicalData.Count == 0 || historicalData[0].Rolling12MonthTemps == null || historicalData[0].Rolling12MonthTemps.Count == 0)
        {
            return Results.Problem("Operation failed: historical weather data was not returned.");
        }

        var monthlyTemperatures = historicalData[0].Rolling12MonthTemps;

        double totalTemperature = 0;
        for (int i = 0; i < monthlyTemperatures.Count; i++)
        {
            totalTemperature += monthlyTemperatures[i];
        }

        double averageTemperature = totalTemperature / monthlyTemperatures.Count;
        var averageHigh = Math.Round(averageTemperature, 1);

        return Results.Ok(new
        {
            current = weatherData,
            averageHigh
        });
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