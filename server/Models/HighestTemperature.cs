namespace server.Models;

public record HighestTemperature(
    List<double> Rolling12MonthTemps,
    string City,
    string State,
    string Zip
);