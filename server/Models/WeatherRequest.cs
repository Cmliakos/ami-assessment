namespace server.Models;

// Represents the location submitted by the frontend
public record WeatherRequest(
    string City,
    string State,
    string Zip
);