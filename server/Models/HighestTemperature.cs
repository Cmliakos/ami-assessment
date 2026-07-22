namespace server.Models;

// Represents historical monthly highest temperature data for a specific location
public record HighestTemperature(
    List<double> Rolling12MonthTemps,
    string City,
    string State,
    string Zip
);