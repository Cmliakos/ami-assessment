namespace server.Models;

public record WeatherRequest (
    string City,
    string State,
    string Zip
);